using System;
using System.Net.Http;
using System.Threading.Tasks;
using ClientApp.Controllers;
using ClientApp.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace ClientApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;

        public AdminService(ILogger<AdminController> logger, HttpClient httpClient,
            IOptions<AppSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _remoteServiceBaseUrl = $"{_settings.Value.MessagingApiUrl}/api/v1/Admin";
        }

        public async Task<string> TestMessagingApiEndpoint()
        {
            var uri = Api.Messaging.AdminTestApiEndpoint(_remoteServiceBaseUrl);

            var content = await _httpClient.GetStringAsync(uri);

            if (string.IsNullOrEmpty(content))
            {
                _logger.LogError("Could not access Messaging API endpoint.");
                return string.Empty;
            }

            return JArray.Parse(content).ToString();
        }
    }
}
