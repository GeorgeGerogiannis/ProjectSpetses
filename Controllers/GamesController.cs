using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Data;
using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Controllers
{
    public class GamesController(ApplicationDbContext dbContext) : Controller
    {
        //get access to the database
        private readonly ApplicationDbContext _dbContext = dbContext;

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Quiz()
        {
            //This is adds the quiz items to the view

            //This is where you do DB stuff
            var quiz_items = await _dbContext.Quiz_items.ToListAsync();

            //Testing data
            //here i select 4 quizes from the DB since we dont have a selection method yet
            quiz_items = new List<Quiz_item> { quiz_items[0], quiz_items[1], quiz_items[2], quiz_items[3] };

            //this shuffles the answers on the quiz, in case it isn't already done in the DB
            for (int i = 0; i < quiz_items.Count; i++)
            {
                quiz_items[i].Answers = ShuffleList(quiz_items[i].Answers);
            }
            return View(quiz_items);
        }
        
        public async Task<IActionResult> SubmitQuiz()
        {
            //this is what we do with the quiz data
            //the quiz page doesn't save the data in any form yet (use asp-for= and a model)
            return View(nameof(Index));
        }
        public async Task<IActionResult> WordMatch()
        {
            //This is where you do DB stuff
            var Match_items = await _dbContext.Match_items.ToListAsync();

            //Testing data
            //here i select 4 quizes from the DB since we dont have a selection method yet
            Match_items = new List<Match_item> { Match_items[0], Match_items[1], Match_items[2], Match_items[3] };

            //get the drop list for the html
            List<String> dropList = new List<String>();
            foreach (var item in Match_items)
            {
                dropList.Add(item.Solution);
            }
            dropList = ShuffleList(dropList);

            return View((Match_items, dropList));
        }

        public async Task<IActionResult> SubmitWordMatch()
        {
            //this is what we do with the WordMatch data
            //the WordMatch page doesn't save the data in any form yet (use asp-for= and a model)
            return View(nameof(Index));
        }
        public async Task<IActionResult> FillBlank()
        {
            //This is where you do DB stuff
            var blank_items = await _dbContext.Blank_items.ToListAsync();

            //Testing data
            //here i select 4 quizes from the DB since we dont have a selection method yet
            //blank_items = new List<Quiz_item> { blank_items[0], blank_items[1], blank_items[2], blank_items[3] };

            //this shuffles the answers on the quiz, in case it isn't already done in the DB
            for (int i = 0; i < blank_items.Count; i++)
            {
                blank_items[i].Answers = ShuffleList(blank_items[i].Answers);
            }


            return View(blank_items);
        }
        
        public static List<T> ShuffleList<T>(List<T> list)
        {//this shuffles String lists to avoid obvious tests
            Random random = new Random();
            return list.OrderBy(_ => random.Next()).ToList();
        }
    }
}
