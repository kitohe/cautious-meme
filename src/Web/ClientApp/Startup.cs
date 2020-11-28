using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using StackExchange.Redis;

namespace ClientApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddCustomMvc(Configuration);
            services.AddCustomAuthentication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    static class ServiceCollectionExtensions
    {
        public static void AddCustomAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");
            var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = identityUrl;
                    options.SignedOutRedirectUri = callBackUrl;
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClientId = "cmessaging";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    options.Scope.Add("messaging.api");
                    options.Scope.Add("offline_access");
                    options.ClaimActions.Add(new JsonKeyClaimAction("role", null, "role"));
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        public static void AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppSettings>(configuration);
            services.AddSession();
            services.AddDistributedMemoryCache();

            services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "cmessaging.webclient";
                })
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(configuration["Redis"]), "DataProtection-Keys")
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });
        }
    }
}
