using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task Testing()
        {
            await StartGame(5, 
                new List<ushort> { 1, 2, 3 });
        }

        public async Task StartGame(int duration, List<ushort> sections, List<ushort>? requiredQuiz = null,
            List<ushort>? requiredBlank = null, List<ushort>? requiredMatch = null)
        {
            //this might need "await" on every call
            //Load all items from the database
            List<Blank_item> blank_items = await _dbContext.Blank_items.ToListAsync();
            List<Quiz_item> quiz_items = await _dbContext.Quiz_items.ToListAsync();
            List<Match_item> match_items = await _dbContext.Match_items.ToListAsync();

            //ensure a minimum duration of 3 games
            if (duration < 3) duration = 3;

            //variable to ensure enough Wordmatch games are selected
            int matchGames = 0;

            //find the games dedicated to the Required IDs 
            //duration / 3 (rounded up)
            int requiredDuration = duration / 3;
            if (duration % 3 != 0) requiredDuration++;

            //selected games
            var selectedGames = new List<object>();

            //select the required items from the IDs
            List<object> allRequired = new List<object>();
            if (requiredQuiz != null)
            {
                foreach (var id in requiredQuiz)
                {
                    Quiz_item item = quiz_items.FirstOrDefault(q => q.Id == id);
                    if (item != null)
                    {
                        allRequired.Add(item);
                    }
                }
            }
            if (requiredBlank != null)
            {
                foreach (var id in requiredBlank)
                {
                    Blank_item item = blank_items.FirstOrDefault(b => b.Id == id);
                    if (item != null)
                    {
                        allRequired.Add(item);
                    }
                }
            }
            if (requiredMatch != null)
            {
                foreach (var id in requiredMatch)
                {
                    Match_item item = match_items.FirstOrDefault(m => m.Id == id);
                    if (item != null)
                    {
                        allRequired.Add(item);
                    }
                }
            }

            //add some/all the required items to the final list
            //it's important that all lists are intitialized to avoid Exv=ceptions from .Count
            if (allRequired.Count > 0)
            {
                //if there are more required items than the required duration, select a random subset
                for (int i = 0; i < requiredDuration; i++)
                {
                    int randomIndex = new Random().Next(allRequired.Count);
                    //add a random game, remove it from the main list(to avoid replays)
                    //and empty the required list
                    if (allRequired[randomIndex] is Quiz_item quizItem)
                    {
                        selectedGames.Add(quizItem);
                        quiz_items.RemoveAll(q => q.Id == quizItem.Id);
                        allRequired.RemoveAt(randomIndex);
                    }
                    else if (allRequired[randomIndex] is Blank_item blankItem)
                    {
                        selectedGames.Add(blankItem);
                        blank_items.RemoveAll(b => b.Id == blankItem.Id);
                        allRequired.RemoveAt(randomIndex);
                    }
                    else if (allRequired[randomIndex] is Match_item matchItem)
                    {
                        selectedGames.Add(matchItem);
                        match_items.RemoveAll(m => m.Id == matchItem.Id);
                        allRequired.RemoveAt(randomIndex);
                        matchGames++;
                    }
                    //break if all required items are selected
                    if (allRequired.Count == 0) 
                        break; 
                }
            }
            //adjust the duration in case any required games where added
            duration -= selectedGames.Count; 

            //remove items that are not in the selected sections
            quiz_items = quiz_items.Where(q => sections.Contains(q.SectionId)).ToList();
            blank_items = blank_items.Where(b => sections.Contains(b.SectionId)).ToList();
            match_items = match_items.Where(m => sections.Contains(m.SectionId)).ToList();

            //Q for Quiz, B for Blank, M for Match
            List<char> gameNames = ['Q', 'B', 'M'];
            
            //select random games
            for (int i = 0; i < duration; i++)
            {
                //removes game types that have no items left
                if (quiz_items.Count == 0)
                    gameNames.Remove('Q');
                if (blank_items.Count == 0)
                    gameNames.Remove('B');
                if (match_items.Count == 0)
                    gameNames.Remove('M');
                //if no game types are left, break the loop
                if (gameNames.Count == 0) 
                    break; 

                //select a random game type
                int gameType = new Random().Next(gameNames.Count);
                //fill the rest of the selected Games
                if (gameType == 0)
                {
                    int randomIndex = new Random().Next(quiz_items.Count);
                    selectedGames.Add(quiz_items[randomIndex]);
                    quiz_items.RemoveAt(randomIndex);
                }
                else if (gameType == 1)
                {
                    int randomIndex = new Random().Next(blank_items.Count);
                    selectedGames.Add(blank_items[randomIndex]);
                    blank_items.RemoveAt(randomIndex);
                }
                else if (gameType == 2)
                {
                    int randomIndex = new Random().Next(match_items.Count);
                    selectedGames.Add(match_items[randomIndex]);
                    match_items.RemoveAt(randomIndex);
                    matchGames++;
                }
            }

            //ensures there are enough Match games to play
            for (int i = 0; i < 3 - matchGames; i++)
            {
                //if there are no Match games selected, don't force them in
                if (matchGames == 0)
                    break;
                //add a random Match game if there are any left
                if (match_items.Count > 0)
                {
                    //add a random Match game if there are any left
                    int randomIndex = new Random().Next(match_items.Count);
                    selectedGames.Add(match_items[randomIndex]);
                    match_items.RemoveAt(randomIndex);
                }
                else
                {
                    //if no Match games are left, make a placeholder Match game
                    if (quiz_items.Count > 0)
                    {
                        Match_item matchItem = new Match_item
                        {
                            Id = 0,
                            Image = "placeholder.png",
                            Solution = "Default solution",
                            SectionId = 0
                        };
                        selectedGames.Add(matchItem);
                    }
                }
            }
            //Start the games
            HttpContext.Session.SetString("SelectedGames", 
                    System.Text.Json.JsonSerializer.Serialize(selectedGames));
            HttpContext.Session.SetString("Test", "I work");
            NextGame();
        }

        public void NextGame()
        {
            string test = HttpContext.Session.GetString("Test");
            Debug.WriteLine($"Test variable value: {test}");
            //Debug.WriteLine("hello".GetType());

            string json = HttpContext.Session.GetString("SelectedGames");
            Debug.WriteLine($"{json}");
            List<object> games = System.Text.Json.JsonSerializer.Deserialize<List<object>>(json);
            Debug.WriteLine($"Behold!!!!{games}");
            if (games.Count == 0)
            {
                //FinishGames();
                int ttacer = 0;
            }
            else if (games[0] is Quiz_item quizItem)
            {
                games.RemoveAt(0);
                HttpContext.Session.SetString("SelectedGames",
                    System.Text.Json.JsonSerializer.Serialize(games));
                Quiz(quizItem);
            }
            else if (games[0] is Blank_item blankItem)
            {
                games.RemoveAt(0);
                HttpContext.Session.SetString("SelectedGames",
                    System.Text.Json.JsonSerializer.Serialize(games));
                FillBlank(blankItem);
            }
            else if (games[0] is Match_item matchItem)
            {
                //pointer problems? (renfrences lists)
                List<object> noMatchGames = games.ToList();
                List<Match_item> allMatches = new List<Match_item>();
                foreach (var item in games)
                {
                    if (item is Match_item match)
                    {
                        allMatches.Add(match);
                        noMatchGames.Remove((object)match);
                    }
                }
                WordMatch(allMatches);
            }
        }


        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Quiz(Quiz_item quiz_item)
        {
            //This is adds the quiz items to the view
            //this shuffles the answers on the quiz, in case it isn't already done in the DB
            quiz_item.Answers = ShuffleList(quiz_item.Answers);
            return View(quiz_item);
        }
        
        public async Task<IActionResult> SubmitQuiz()
        {
            //this is what we do with the quiz data
            //the quiz page doesn't save the data in any form yet (use asp-for= and a model)
            return View(nameof(Index));
        }
        public async Task<IActionResult> WordMatch(List<Match_item> match_items)
        {
            //get the drop list for the html
            List<String> dropList = new List<String>();
            foreach (var item in match_items)
            {
                dropList.Add(item.Solution);
            }
            dropList = ShuffleList(dropList);

            return View((match_items, dropList));
        }

        public async Task<IActionResult> SubmitWordMatch()
        {
            //this is what we do with the WordMatch data
            //the WordMatch page doesn't save the data in any form yet (use asp-for= and a model)
            return View(nameof(Index));
        }
        public async Task<IActionResult> FillBlank(Blank_item blank_item)
        {
            //This is adds the blank items to the view
            //this shuffles the answers on the quiz, in case it isn't already done in the DB
            blank_item.Answers = ShuffleList(blank_item.Answers);
            return View(blank_item);
        }
        
        public static List<T> ShuffleList<T>(List<T> list)
        {//this shuffles String lists to avoid obvious tests
            Random random = new Random();
            return list.OrderBy(_ => random.Next()).ToList();
        }
    }
}
