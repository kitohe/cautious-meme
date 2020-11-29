using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
