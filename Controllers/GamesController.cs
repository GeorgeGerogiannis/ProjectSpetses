using System.Collections.Generic;
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

        public async Task<IActionResult> StartGame(int duration, List<ushort> sections, List<ushort>? requiredQuiz = null,
            List<ushort>? requiredBlank = null, List<ushort>? requiredMatch = null)
        {
            //Load all items from the database
            List<Quiz_item> quiz_items = await _dbContext.Quiz_items.ToListAsync();
            List<Blank_item> blank_items = await _dbContext.Blank_items.ToListAsync();
            List<Match_item> match_items = await _dbContext.Match_items.ToListAsync();

            //ensure a minimum duration of 3 games
            if (duration < 3) duration = 3;

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
                }
            }
        }


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
