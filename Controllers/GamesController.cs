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
            return null;
        }
        public IActionResult WordMatch()
        {
            var (Match_items, dropList) = GetWordMatches();
            return View((Match_items, dropList));
        }
        public (List<Match_item>, List<string>) GetWordMatches() 
        {
            //This is where you do DB stuff
            //add "/images/" by code, take the name and extension from DB

            //Testing data
            List<Match_item> Match_items = new List<Match_item> {
                new Match_item
            {
                Section_Id = Guid.NewGuid(),
                Image = "/images/placeholder.png",
                Solution = "Solution 1"
            },
                new Match_item
            {
                Section_Id = Guid.NewGuid(),
                Image = "/images/placeholder.png",
                Solution = "Solution 2"
            },
                new Match_item
            {
                Section_Id = Guid.NewGuid(),
                Image = "/images/placeholder.png",
                Solution = "Solution 3"
            },
                new Match_item
            {
                Section_Id = Guid.NewGuid(),
                Image = "/images/placeholder.png",
                Solution = "Solution 4"
                }
            };
            //End of Testing data

            //this makes a shuffled list of the solutions to be used in the drop down
            List<String> dropList = new List<String>();
            foreach (var item in Match_items)
            {
                dropList.Add(item.Solution);
            }
            dropList = ShuffleList(dropList);
          
            return (Match_items, dropList);
        }
        public async Task<IActionResult> SubmitWordMatch()
        {
            //this is what we do with the WordMatch data
            //the WordMatch page doesn't save the data in any form yet (use asp-for= and a model)
            return null;
        }
        public IActionResult FillBlank()
        {
            var quiz_items = GetBlanks();
            return View(quiz_items);
        }
        private List<Blank_item> GetBlanks()
        {
            //This is where you do DB stuff
            //Blank items are near identical to quiz items, when DB is developed we should consider merging these

            //Testing data
            List<Blank_item> quiz_items = new List<Blank_item> {
                new Blank_item
            {
                Section_Id = Guid.NewGuid(),
                Description1 = "Oranges are a ",
                Description2 = "fruit?",
                Solution = "yummy",
                Answers = new List<string>() { "gross", "red", "blue", "unhealthy" }
            },
                new Blank_item
            {
                Section_Id = Guid.NewGuid(),
                Description1 = "Italians live in ",
                Description2 = " city?",
                Solution = "Rome",
                Answers = new List<string>() { "Athens", "Rome", "Napoli", "Heraklion" }
            },
                new Blank_item
            {
                Section_Id = Guid.NewGuid(),
                Description1 = "Oranges are a ",
                Description2 = "fruit?",
                Solution = "Cairo",
                Answers = new List<string>() { "Athens", "Nile", "Patras", "Cairo" }
            },
                new Blank_item
            {
                Section_Id = Guid.NewGuid(),
                Description1 = "Oranges are a ",
                Description2 = "fruit?",
                Solution = "London",
                Answers = new List<string>() { "London", "Dublin", "Patras", "Heraklion" }
            },
                new Blank_item
            {
                Section_Id = Guid.NewGuid(),
                Description1 = "Oranges are a ",
                Description2 = "fruit?",
                Solution = "London",
                Answers = new List<string>() { "London", "Dublin", "Patras", "Heraklion" }
            }
            };
            //End of Testing data

            //this shuffles the answers on the quiz, in case it isn't already done in the DB
            for (int i = 0; i < quiz_items.Count; i++)
            {
                quiz_items[i].Answers = ShuffleList(quiz_items[i].Answers);
            }

            return quiz_items;
        }
        public static List<T> ShuffleList<T>(List<T> list)
        {//this shuffles String lists to avoid obvious tests
            Random random = new Random();
            return list.OrderBy(_ => random.Next()).ToList();
        }
    }
}
