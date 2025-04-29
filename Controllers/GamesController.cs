using Microsoft.AspNetCore.Mvc;
using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Controllers
{
    public class GamesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Quiz()
        {
            //This is adds the quiz items to the view
            var quiz_items = GetQuizes();
            return View(quiz_items);
        }
        private List<Quiz_item> GetQuizes()
        {
            //This is where you do DB stuff
            //Testing data
            List<Quiz_item> quiz_items = new List<Quiz_item> {
            new Quiz_item
            {
                Section_Id = Guid.NewGuid(),
                Description = "What is the capital of Greece?",
                Solution = "Athens",
                Answers = new List<string>() { "Athens", "Thessaloniki", "Patras", "Heraklion" }
            },
            new Quiz_item
            {
                Section_Id = Guid.NewGuid(),
                Description = "What is the capital of italy?",
                Solution = "Rome",
                Answers = new List<string>() { "Athens", "Rome", "Napoli", "Heraklion" }
            },
            new Quiz_item
            {
                Section_Id = Guid.NewGuid(),
                Description = "What is the capital of Egypt?",
                Solution = "Cairo",
                Answers = new List<string>() { "Athens", "Nile", "Patras", "Cairo" }
            },
            new Quiz_item
            {
                Section_Id = Guid.NewGuid(),
                Description = "What is the capital of UK?",
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
        public static List<T> ShuffleList<T>(List<T> list)
        {//this shuffles String lists to avoid obvious tests
            Random random = new Random();
            return list.OrderBy(_ => random.Next()).ToList();
        }

    }
}
