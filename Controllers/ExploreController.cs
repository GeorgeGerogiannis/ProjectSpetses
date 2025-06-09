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

            //get the current user points
            var points = await _dbContext.Stats
                .Where(s => s.Id == GetCurrentUserId())
                .Select(s => s.TotalPoints)
                .FirstOrDefaultAsync();

            //get all section data
            var sections = await _dbContext.Sections.ToListAsync();

            var sectionCategories = new List<ushort>();
            var completedSections = new List<ushort>();
            
            foreach (var section in sections)
            {
                //get the number of categories in each section
                var categoryCount = await _dbContext.Categories
                    .Where(c => c.SectionId == section.Id)
                    .CountAsync();

                //get the number of completed categories for the current user
                var userStats = await _dbContext.Stats.FirstOrDefaultAsync(s => s.Id == GetCurrentUserId());
                var categoriesRead = userStats.CategoriesCompleted
                    .Where(entry => entry.StartsWith($"{section.Id}:"))
                    .ToList();

                //add both in lists to use them for showing user progression
                sectionCategories.Add((ushort)categoryCount);
                completedSections.Add((ushort)categoriesRead.Count);
            }

            //create the model
            var model = new ExploreViewModel
            {
                Sections = sections,
                CategoryCount = sectionCategories,
                CompletedCategories = completedSections,
                Points = points
            };

            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Section(ushort Id, ushort cId = 0)
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound("Something Went Wrong");
            }

            //get the section data
            var section = await _dbContext.Sections
                .FirstOrDefaultAsync(s => s.Id == Id);

            //check if the section exists
            if (section == null)
            {
                return NotFound("Section not found");
            }

            //get the current user stats
            var stats = await _dbContext.Stats
                .FirstOrDefaultAsync(s => s.Id == GetCurrentUserId());

            if (section.PointsRequired > stats.TotalPoints)
            {
                //if the user does not have enough points, redirect to the index page
                return RedirectToAction(nameof(Index));
            }

            //get the completed categories
            var categoriesRead = _dbContext.Update(stats).Entity.CategoriesCompleted;

            if (cId != 0)
            {
                //if the user just completed a category, add it to the completed categories
                var entry = $"{Id}:{cId}";
                
                if (!categoriesRead.Contains(entry))
                {
                    categoriesRead.Add(entry);
                    await _dbContext.SaveChangesAsync();
                }
            }

            //get all the section's categories
            var categories = await _dbContext.Categories
                .Where(c => c.SectionId == Id)
                .ToListAsync();

            var completed = new List<bool>();

            foreach (var category in categories)
            {
                var entry = $"{Id}:{category.SectionIndex}";

                //if the user has completed this category add 'true' to the list to show user progression
                if (categoriesRead.Contains(entry))
                {
                    completed.Add(true);
                }
                else
                {
                    completed.Add(false);
                }
            }

            var notify = false;

            if (!completed.Contains(false) && !stats.SectionsCompleted.Contains(Id))
            {
                //if the user has completed all categories in this section and has not been notified yet, give them a notification
                //and consider the section completed and the user ready to play the section's games
                var sectionsCompleted = _dbContext.Update(stats).Entity.SectionsCompleted;
                sectionsCompleted.Add(Id);
                await _dbContext.SaveChangesAsync();
                notify = true;
            }

            //create the model
            var model = new SectionViewModel
            {
                SectionId = Id,
                SectionName = section.Name,
                Categories = categories,
                Completed = completed,
                Notify = notify
            };

            return View(model);
        }

        [HttpGet]
        [Route("Explore/Section/{sectionId}/Category/{sectionIndex}/Page/{page}")]
        public async Task<IActionResult> Category(ushort sectionId, ushort sectionIndex, ushort page)
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound("Something Went Wrong");
            }

            //get the current user stats
            var stats = await _dbContext.Stats
                .FirstOrDefaultAsync(s => s.Id == GetCurrentUserId());

            var pointsRequired = await _dbContext.Sections
                .Where(s => s.Id == sectionId)
                .Select(s => s.PointsRequired)
                .FirstOrDefaultAsync();

            if (pointsRequired > stats.TotalPoints)
            {
                //if the user does not have enough points, redirect to the index page
                return RedirectToAction(nameof(Index));
            }

            //get category name
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.SectionIndex == sectionIndex && c.SectionId == sectionId);

            //check if the category exists
            if (category == null)
            {
                return NotFound("Category not found");
            }

            //get content from database
            var content = await _dbContext.Content
                .FirstOrDefaultAsync(c => c.CategoryId == category.Id && c.Page == page);

            //get the category page count
            var pageCount = (ushort) await _dbContext.Content
                .Where(c => c.CategoryId == category.Id)
                .CountAsync();

            //add the last read content to the user stats
            stats.LastRead = $"{sectionId}:{sectionIndex}:{page}";
            await _dbContext.SaveChangesAsync();

            //create the model
            var model = new CategoryViewModel
            {
                SectionId = sectionId,
                CategoryId = sectionIndex,
                CategoryName = category.Name,
                Content = content,
                PageCount = pageCount,
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
