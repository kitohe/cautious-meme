using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ClientApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> ClaimsSummary()
        {
            return View();
        }

        public async Task<IActionResult> ApiTest()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            _logger.LogInformation("Entered admin func");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("http://messaging.api/api/v1/test/TestApi");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View();
        }
    }
}
