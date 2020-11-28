using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ClientApp
{
    public class Program
    {
        private static readonly string Namespace = typeof(Program).Namespace;
        private static readonly string AppName = Namespace;

        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            Log.Information($"[{AppName}] Starting web host");
            CreateHostBuilder(args, configuration).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
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
