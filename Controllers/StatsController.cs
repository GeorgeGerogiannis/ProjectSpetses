using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Data;
using ProjectSpetses.Models;
using System.Threading.Tasks;

namespace ProjectSpetses.Controllers
{
    public class StatsController(ApplicationDbContext dbContext) : Controller
    {
        //get access to the database
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IActionResult> Index()
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound("Something Went Wrong");
            }

            //get the current username
            var username = await _dbContext.Users
                .Where(u => u.Id == GetCurrentUserId())
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            //get the user's stats
            var stats = await _dbContext.Stats
                .Where(s => s.Id == GetCurrentUserId())
                .Select(s => new
                {
                    s.TotalPoints,
                    s.CategoriesRead,
                })
                .FirstOrDefaultAsync();

            var model = new StatsViewModel
            {
                Username = username,
                TotalPoints = (uint)(stats?.TotalPoints),
                CategoriesRead = (uint)stats?.CategoriesRead.Count
            };

            return View(model);
        }

        private Guid GetCurrentUserId()
        {
            var currentUserId = User.FindFirst("User_id")?.Value;
            return Guid.TryParse(currentUserId, out Guid userId) ? userId : Guid.Empty;
        }
    }
}
