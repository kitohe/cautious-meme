using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.API.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly ILogger<RegisterService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterService(ILogger<RegisterService> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public async Task<IEnumerable<IdentityError>> RegisterUser(RegisterViewModel model)
        {
            var user = GetUserFromViewModel(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // for now only log confirmation code, later on send email containing this code
                _logger.LogInformation($"Registered new user, confirmation code: {confirmationCode}");    
            }
            else
            {
                _logger.LogWarning($"Could not register new user, message: {string.Join(", ", result.Errors)}");
            }

            return result.Errors;
        }

        private ApplicationUser GetUserFromViewModel(RegisterViewModel model)
        {
            return new()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Country = model.Country
            };
        }
    }
}
