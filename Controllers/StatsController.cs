using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Data;
using ProjectSpetses.Models;

namespace ProjectSpetses.Controllers
{
    public class StatsController(ApplicationDbContext dbContext) : Controller
    {
        //get access to the database
        private readonly ApplicationDbContext _dbContext = dbContext;

        [HttpGet]
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

            //get the user's needed stats
            var stats = await _dbContext.Stats
                .Where(s => s.Id == GetCurrentUserId())
                .Select(s => new
                {
                    s.TotalPoints,
                    s.CategoriesRead,
                    s.NotificationsGiven,
                    s.CreatedAt,
                    s.WrongAnswers,
                    s.CorrectAnswers
                })
                .FirstOrDefaultAsync();

            //create the model
            var model = new StatsViewModel
            {
                Username = username,
                TotalPoints = stats.TotalPoints,
                SectionsCompleted = (ushort)stats.NotificationsGiven.Count, // NotificationsGiven can be represented as a collection of sections completed
                CategoriesCompleted = (ushort)stats.CategoriesRead.Count,
                CreatedAt = stats.CreatedAt,
                WrongAnswers = (uint)stats.WrongAnswers.Count,
                CorrectAnswers = (uint)stats.CorrectAnswers.Count,
            };

            return View(model);
        }

        //gets the user's id from session
        private Guid GetCurrentUserId()
        {
            var currentUserId = User.FindFirst("User_id")?.Value;
            return Guid.TryParse(currentUserId, out Guid userId) ? userId : Guid.Empty;
        }
    }
}
