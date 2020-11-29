using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.API.Infrastructure;
using Identity.API.Models;
using Identity.API.Models.AccountViewModels;
using Identity.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers
{
    [SecurityHeaders]
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

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return BadRequest();

            _logger.LogInformation($"Trying to register new user with username: {model.UserName}");

            var errors = await _registerService.RegisterUser(model);

            var identityErrors = errors as IdentityError[] ?? errors.ToArray();
            if (identityErrors.Any())
            {
                AddErrorsToModel(identityErrors);
                return View(model);
            }

            if (returnUrl == null) return BadRequest();

            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                return Redirect(returnUrl);
            if (ModelState.IsValid)
                return RedirectToAction(nameof(Login), "Account", new { returnUrl });

            return View(model);

        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var redirectUrl = await _loginService.LoginAsync(model);

            if (string.IsNullOrEmpty(redirectUrl))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                ViewData["ReturnUrl"] = model.ReturnUrl;
                return View(new LoginViewModel
                {
                    Email = model.Email,
                    RememberMe = model.RememberMe,
                    ReturnUrl = model.ReturnUrl
                });
            }

            return Redirect(redirectUrl);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // invalidate cookies
            await HttpContext.SignOutAsync();

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _loginService.GetLogoutContextAsync(logoutId);

            return Redirect(logout?.PostLogoutRedirectUri);
        }

        [HttpGet]
        public IActionResult ForgotPassword(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        private void AddErrorsToModel(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
