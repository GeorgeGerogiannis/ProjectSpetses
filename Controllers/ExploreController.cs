using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Data;
using ProjectSpetses.Models;

namespace ProjectSpetses.Controllers
{
    public class ExploreController(ApplicationDbContext dbContext) : Controller
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

            var stats = await _dbContext.Stats
                .FirstOrDefaultAsync(s => s.Id == GetCurrentUserId());

            var sections = await _dbContext.Sections.ToListAsync();

            //get sections from database
            var model = new ExploreViewModel
            {
                Sections = sections,
                Points = stats.TotalPoints
            };

            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Section(ushort Id)
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound("Something Went Wrong");
            }

            //get section name
            var section = await _dbContext.Sections
                .FirstOrDefaultAsync(s => s.Id == Id);

            //get categories from database
            var categories = await _dbContext.Categories
                .Where(c => c.SectionId == Id)
                .ToListAsync();

            var model = new SectionViewModel
            {
                SectionId = Id,
                SectionName = section.Name,
                Categories = categories
            };

            return View(model);
        }

        [HttpGet]
        [Route("Explore/Section/{sectionId}/Category/{Id}/Page/{page}")]
        public async Task<IActionResult> Category(ushort sectionId, ushort Id, ushort page)
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound("Something Went Wrong");
            }

            //get category name
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == Id);

            //get content from database
            var content = await _dbContext.Content
                .FirstOrDefaultAsync(c => c.CategoryId == Id && c.Page == page);

            //get the category page count
            var pageCount = (ushort) await _dbContext.Content
                .Where(c => c.CategoryId == Id)
                .CountAsync();

            //get the current user stats
            var stats = await _dbContext.Stats
                .FirstOrDefaultAsync(s => s.Id == GetCurrentUserId());

            //add the last read content to the user stats
            //format: {sectionId}:{Id}:{page}
            stats.LastRead = $"{sectionId}:{Id}:{page}";
            await _dbContext.SaveChangesAsync();


            var model = new CategoryViewModel
            {
                SectionId = sectionId,
                CategoryId = Id,
                CategoryName = category.Name,
                Content = content,
                PageCount = pageCount,
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
