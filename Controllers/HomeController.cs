using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Data;
using ProjectSpetses.Models;

namespace ProjectSpetses.Controllers
{
    public class HomeController(ApplicationDbContext dbContext) : Controller
    {
        //get access to the database
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IActionResult> Index()
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            //get the user's last read content
            var lastRead = await _dbContext.Stats
                .Where(s => s.Id == GetCurrentUserId())
                .Select(s => s.LastRead)
                .FirstOrDefaultAsync();

            if (lastRead != null)
            {
                //parse the "{sectionId}:{categoryId}:{contentPage}" format string
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
                        .Select(c => new { c.SectionIndex, c.Name })
                        .FirstOrDefaultAsync(c => c.SectionIndex == categoryId);

                    if (section != null && category != null)
                    {
                        //create the model
                        var model = new HomeViewModel
                        {
                            SectionId = section.Id,
                            SectionName = section.Name,
                            CategoryId = category.SectionIndex,
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

        //gets the user's id from session
        private Guid GetCurrentUserId()
        {
            var currentUserId = User.FindFirst("User_id")?.Value;
            return Guid.TryParse(currentUserId, out Guid userId) ? userId : Guid.Empty;
        }
    }
}
