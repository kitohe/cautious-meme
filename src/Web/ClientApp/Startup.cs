using System;
using ClientApp.Infrastructure;
using ClientApp.Services;
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
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace ClientApp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddCustomMvc(_configuration);
            services.AddCustomAuthentication(_configuration);
            services.AddHttpClientServices(_configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.None }); // order matters

            app.UseRouting();

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
            var identityUrl = configuration.GetValue<string>("IDENTITY_URL");
            var callBackUrl = configuration.GetValue<string>("CALLBACK_URL");
            var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = identityUrl;
                    options.SignedOutRedirectUri = callBackUrl;
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClientId = "cmessaging";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.UsePkce = true;
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

            services.AddAccessTokenManagement();
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

        public static void AddHttpClientServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register delegating handlers
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<HttpClientRequestIdDelegatingHandler>();

            //set 5 min as the lifetime for each HttpMessageHandler int the pool
            services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddHttpClient<IAdminService, AdminService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
        }
    }
}
