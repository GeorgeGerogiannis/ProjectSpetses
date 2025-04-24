using Microsoft.AspNetCore.Mvc;

namespace ProjectSpetses.Controllers
{
    public class ExploreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
