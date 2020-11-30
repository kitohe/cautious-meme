using System;
using System.Threading.Tasks;
using ClientApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClientApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;

        public AdminController(ILogger<AdminController> logger, IAdminService adminService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
        }

        public async Task<IActionResult> ClaimsSummary()
        {
            return View();
        }

        public async Task<IActionResult> MessagingApiTest()
        {
            _logger.LogInformation("Executing ADMIN function");

            var result = await _adminService.TestMessagingApiEndpoint();

            ViewBag.Result = result;

            return View();
        }
    }
}
