using System;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRedirectService _redirectService;
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(IRedirectService redirectService, IIdentityServerInteractionService interaction)
        {
            _redirectService = redirectService ?? throw new ArgumentNullException(nameof(redirectService));
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
        }

        public IActionResult Index(string returnUrl)
        {
            return View();
        }

        public IActionResult ReturnToOriginalApplication(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                return BadRequest();

            return Redirect(_redirectService.ExtractRedirectUriFromReturnUrl(returnUrl));
        }
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}
