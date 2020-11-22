using System;
using System.Net;
using System.Threading.Tasks;
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
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IRegisterService _registerService;

        public AccountController(ILogger<AccountController> logger,
            IRegisterService registerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get(string test)
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Model passed was invalid");

            _logger.LogInformation($"Trying to register new user with username: {model.UserName}");

            await _registerService.RegisterUser(model);

            return Ok();
        }
    }
}
