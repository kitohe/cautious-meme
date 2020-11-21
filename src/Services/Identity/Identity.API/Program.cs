using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Identity.API
{
    public class Program
    {
        private static readonly string Namespace = typeof(Program).Namespace;
        private static readonly string AppName = Namespace.
            Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            var host = CreateHostBuilder(args, configuration).Build();

            var seedTrigger = Environment.GetEnvironmentVariable("SEED_DATABASE");
            if (!string.IsNullOrEmpty(seedTrigger) && seedTrigger == "true")
            {
                using var scope = host.Services.CreateScope();
                
                await SeedData.SeedUserRoles(scope.ServiceProvider, configuration);
                await SeedData.SeedAdminAccount(scope.ServiceProvider, configuration);
            }

            try
            {
                Log.Information($"[{AppName}] Starting web host");
                await host.RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, $"[{AppName}] Application has terminated unexpectedly");
            }

            Log.CloseAndFlush();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .CaptureStartupErrors(false)
                        .UseStartup<Startup>()
                        .UseConfiguration(configuration)
                        .UseSerilog();
                });

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
