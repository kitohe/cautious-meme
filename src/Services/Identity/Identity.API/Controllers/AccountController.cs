using System;
using System.Net;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Models.AccountModels;
using Identity.API.Models.AccountViewModels;
using Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IRegisterService _registerService;
        private readonly ILoginService<ApplicationUser> _loginService;

        public AccountController(ILogger<AccountController> logger,
            IRegisterService registerService,
            ILoginService<ApplicationUser> loginService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            _logger.LogInformation($"Trying to register new user with username: {model.UserName}");

            await _registerService.RegisterUser(model);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var redirect = await _loginService.SignInAsync(model);

            var url = string.IsNullOrEmpty(redirect) ? "~/" : redirect;

            return Redirect(url);
        }
    }
}
