using System;
using Identity.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRedirectService _redirectService;

        public HomeController(IRedirectService redirectService)
        {
            _redirectService = redirectService ?? throw new ArgumentNullException(nameof(redirectService));
        }

        public IActionResult ReturnToOriginalApplication(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                return BadRequest();

            return Redirect(_redirectService.ExtractRedirectUriFromReturnUrl(returnUrl));
        }
    }
}
