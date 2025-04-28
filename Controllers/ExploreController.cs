using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Data;
using ProjectSpetses.Models;
using System.Reflection.Metadata;

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

            //get sections from database
            var sections = await _dbContext.Sections.ToListAsync();

            return View(sections);
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
    }
}
