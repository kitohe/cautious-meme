using System;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Models.AccountViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.API.Services
{
    public class LoginService : ILoginService<ApplicationUser>
    {
        private readonly ILogger<LoginService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginService> logger,
            IConfiguration configuration,
            IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _interaction = interaction;
        }

        public async Task<string> LoginAsync(LoginViewModel viewModel)
        {
            var user = await _userManager.FindByEmailAsync(viewModel.Email);

            if (!await _userManager.CheckPasswordAsync(user, viewModel.Password))
                return string.Empty;

            var tokenLifetime = _configuration.GetValue("TokenLifetimeMinutes", 120);

            var props = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(tokenLifetime),
                AllowRefresh = true,
                RedirectUri = viewModel.ReturnUrl
            };

            if (viewModel.RememberMe)
            {
                var permanentTokenLifetime = _configuration.GetValue("PermanentTokenLifetimeDays", 365);

                props.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(permanentTokenLifetime);
                props.IsPersistent = true;
            };

            _logger.LogInformation($"Logging in user with username: {user.UserName}");
            await _signInManager.SignInAsync(user, props);

            return _interaction.IsValidReturnUrl(viewModel.ReturnUrl) ? viewModel.ReturnUrl : string.Empty;
        }

        public async Task<LogoutRequest> GetLogoutContextAsync(string logoutId)
        {
            return await _interaction.GetLogoutContextAsync(logoutId);
        }
    }
}
