using Microsoft.AspNetCore.Mvc;

namespace ProjectSpetses.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
