using Microsoft.AspNetCore.Mvc;

namespace ProjectSpetses.Controllers
{
    public class GamesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
