﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Models.AccountViewModels;
using Identity.API.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IRegisterService _registerService;
        private readonly ILoginService<ApplicationUser> _loginService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public AccountController(ILogger<AccountController> logger,
            IRegisterService registerService,
            ILoginService<ApplicationUser> loginService,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                ViewData["ReturnUrl"] = model.ReturnUrl;
                return View();
            }

            var redirect = await _loginService.LoginAsync(model);

            var url = string.IsNullOrEmpty(redirect) ? "~/" : redirect;

            return Redirect(url);
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
