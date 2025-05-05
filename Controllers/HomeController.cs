using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Data;
using ProjectSpetses.Models;

namespace ProjectSpetses.Controllers
{
    public class HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IActionResult> Index()
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            //get the user's last read stat
            var lastRead = await _dbContext.Stats
                .Where(s => s.Id == GetCurrentUserId())
                .Select(s => s.LastRead)
                .FirstOrDefaultAsync();

            if (lastRead != null)
            {
                //parse the "{sectionId}:{categoryId}:{page}" format string
                var parts = lastRead.Split(':');
                if (parts.Length == 3 &&
                    ushort.TryParse(parts[0], out ushort sectionId) &&
                    ushort.TryParse(parts[1], out ushort categoryId) &&
                    ushort.TryParse(parts[2], out ushort page))
                {
                    //get the section name
                    var section = await _dbContext.Sections
                        .Select(s => new { s.Id, s.Name })
                        .FirstOrDefaultAsync(s => s.Id == sectionId);

                    //get the category name
                    var category = await _dbContext.Categories
                        .Select(c => new { c.Id, c.Name })
                        .FirstOrDefaultAsync(c => c.Id == categoryId);

                    if (section != null && category != null)
                    {
                        //create the model and set it's properties
                        var model = new HomeViewModel
                        {
                            SectionId = section.Id,
                            SectionName = section.Name,
                            CategoryId = category.Id,
                            CategoryName = category.Name,
                            Page = page
                        };

                        return View(model);
                    }

                    
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Guid GetCurrentUserId()
        {
            var currentUserId = User.FindFirst("User_id")?.Value;
            return Guid.TryParse(currentUserId, out Guid userId) ? userId : Guid.Empty;
        }
    }
}
