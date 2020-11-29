using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Messaging.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("TestApi")]
        public object TestApi()
        {
            return User.Claims.Select(c =>
                new
                {
                    Type = c.Type,
                    Value = c.Value
                });
        }
    }
}
