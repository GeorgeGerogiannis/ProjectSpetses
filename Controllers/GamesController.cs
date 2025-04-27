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

            return quiz_items;
        }
        public async Task<IActionResult> SubmitQuiz()
        {
            //this is what we do with the quiz data
            //the quiz page doesn't save the data in any form yet (use asp-for= and a model)
            return View();
        }


    }
}
