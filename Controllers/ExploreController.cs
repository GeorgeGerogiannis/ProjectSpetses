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

            //get all sections from the database
            var sections = await _dbContext.Sections.ToListAsync();

            var completedSections = new List<bool>();

            foreach (var section in sections)
            {
                //get the number of categories in each section
                var categoryCount = await _dbContext.Categories
                    .Where(c => c.SectionId == section.Id)
                    .CountAsync();

                //get the number of completed categories for the current user
                var categoriesRead = _dbContext.Update(_dbContext.Stats.FirstOrDefault(s => s.Id == GetCurrentUserId())).Entity.CategoriesRead.Count;

                //check if the user has completed all categories in this section
                if (categoriesRead == categoryCount)
                {
                    completedSections.Add(true);
                }
                else
                {
                    completedSections.Add(false);
                }
            }

            var model = new ExploreViewModel
            {
                Sections = sections,
                Completed = completedSections,
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

            //get the section
            var section = await _dbContext.Sections
                .FirstOrDefaultAsync(s => s.Id == Id);

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
            var categoriesRead = _dbContext.Update(stats).Entity.CategoriesRead;

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

            //get the section's categories
            var categories = await _dbContext.Categories
                .Where(c => c.SectionId == Id)
                .ToListAsync();

            var completed = new List<bool>();

            foreach (var category in categories)
            {
                var entry = $"{Id}:{category.SectionIndex}";

                //check if the user has completed this category
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

            if (!completed.Contains(false) && !stats.NotificationsGiven.Contains(Id))
            {
                //if the user has completed all categories in this section and has not been notified yet, give them a notification
                var notifications = _dbContext.Update(stats).Entity.NotificationsGiven;
                notifications.Add(Id);
                await _dbContext.SaveChangesAsync();
                notify = true;
            }

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
                .FirstOrDefaultAsync(c => c.SectionIndex == Id && c.SectionId == sectionId);

            if (category == null)
            {
                return NotFound("Category not found");
            }

            //get content from database
            var content = await _dbContext.Content
                .FirstOrDefaultAsync(c => c.CategoryId == category.Id && c.Page == page);

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
