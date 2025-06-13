using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectSpetses.Data;
using ProjectSpetses.Migrations;
using ProjectSpetses.Models;
using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Controllers
{
    public class GamesController(ApplicationDbContext dbContext) : Controller
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private const string selectedGamesSession = "SelectedGames";
        private const string gameResultsSession = "GameResults";
        private const string gameDifficulty = "GameDifficulty";
        private const int pointsEasy = 1;
        private const int pointsHard = 2;

        public async Task<IActionResult> StartGameConverter(int duration, string sections, 
            string? difficulty, string? requiredQuiz, string? requiredBlank, string? requiredMatch)
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound("Something Went Wrong");
            }

            //this function converts the parameters from the html to their inteded types
            static List<ushort> ParseList(string? csv) =>
                string.IsNullOrWhiteSpace(csv)
                    ? new()
                    : csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => ushort.TryParse(s.Trim(), out var v) ? v : (ushort?)null)
                         .Where(v => v.HasValue)
                         .Select(v => v.Value)
                         .ToList();
            //parse the sections from the string to a List<ushort>
            var sectionList = ParseList(sections);
            //same but also makes them null if they are empty
            var quizList = string.IsNullOrWhiteSpace(requiredQuiz) ? null : ParseList(requiredQuiz);
            var blankList = string.IsNullOrWhiteSpace(requiredBlank) ? null : ParseList(requiredBlank);
            var matchList = string.IsNullOrWhiteSpace(requiredMatch) ? null : ParseList(requiredMatch);

            //check if the user has unlocked the games for the selected sections
            var unlockledSections = await _dbContext.Stats
                .Where(s => s.Id == GetCurrentUserId())
                .Select(s => s.SectionsCompleted)
                .FirstOrDefaultAsync();

            //if the user has not unlocked one of the sections, redirect to the games page
            if (unlockledSections == null || !sectionList.All(unlockledSections.Contains))
            {
                return RedirectToAction(nameof(Index), nameof(GamesController)[..nameof(GamesController).LastIndexOf("Controller")]);
            }

            return RedirectToAction(nameof(StartGame), new
            {
                duration,
                sections = sectionList,
                difficulty,
                requiredQuiz = quizList,
                requiredBlank = blankList,
                requiredMatch = matchList
            });

            //the following code is an example on how to call the StartGameConverter Method in HTML
            /*
                <a 
                    asp-controller="Games" 
                    asp-action="@nameof(GamesController.StartGameConverter)"
                    asp-route-duration="10"
	                   asp-route-sections="1,2,3">
                    Testing initiation
                </a>
             */
        }

        public async Task<IActionResult> StartGame(int duration, List<ushort> sections, string? difficulty = null, 
            List<ushort>? requiredQuiz = null, List<ushort>? requiredBlank = null, List<ushort>? requiredMatch = null)
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
            List<object> selectedGames = [];

            //select the required items from the IDs
            List<object> allRequired = [];
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
            //remove items that are not in the selected difficulty(if selected)
            if (difficulty != null)
            {
                quiz_items = quiz_items.Where(q => q.Difficulty == difficulty).ToList();
                blank_items = blank_items.Where(b => b.Difficulty == difficulty).ToList();
                match_items = match_items.Where(m => m.Difficulty == difficulty).ToList();
            }

            //Q for Quiz, B for Blank, M for Match
            List<char> gameNames = ['Q', 'B', 'M'];

            //select random games
            Random random = new Random();
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
                char gameType = gameNames[random.Next(gameNames.Count)];
                //fill the rest of the selected Games
                if (gameType == 'Q')
                {
                    int randomIndex = new Random().Next(quiz_items.Count);
                    selectedGames.Add(quiz_items[randomIndex]);
                    quiz_items.RemoveAt(randomIndex);
                }
                else if (gameType == 'B')
                {
                    int randomIndex = new Random().Next(blank_items.Count);
                    selectedGames.Add(blank_items[randomIndex]);
                    blank_items.RemoveAt(randomIndex);
                }
                else if (gameType == 'M')
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
                        var matchItem = new Match_item
                        {
                            Id = 0,
                            Difficulty = difficulty,
                            Image = "placeholder.png",
                            Solution = "Default solution",
                            SectionId = 0
                        };
                        selectedGames.Add(matchItem);
                    }
                }
            }
            //Start the games
            //wrap the games in a GameWrapper to help with deserialization
            var wrappedGames = selectedGames.Select(g => new GameWrapper
            {
                Type = g.GetType().Name,
                Data = g
            }).ToList();
            //save them in session
            HttpContext.Session.SetString(selectedGamesSession, JsonSerializer.Serialize(wrappedGames));
            HttpContext.Session.SetString(gameResultsSession, "");
            if (difficulty == null)
                difficulty = "none";
            //figure out what game we playing
            string pointType = "skip";
            if (difficulty == null || sections.Count != 1)
            { 
                pointType = "skip";//skip point giving 
            }
            else if (difficulty == "easy")
            {
                pointType = sections[0].ToString() + "easy";
            }
            else if (difficulty == "hard")
            {
                pointType = sections[0].ToString() + "hard";
            }
            HttpContext.Session.SetString(gameDifficulty, pointType);

            //HttpContext.Session.SetString("Test", "I work");
            return RedirectToAction(nameof(NextGame));
        }

        [HttpGet]
        public async Task<IActionResult> NextGame()
        {
            string json = HttpContext.Session.GetString(selectedGamesSession);
            var deserializedGames = JsonSerializer.Deserialize<List<GameWrapper>>(json);
            List<object> selectedGames = [];
            foreach (var game in deserializedGames)
            {
                switch (game.Type)
                {
                    case nameof(Quiz_item):
                        var quiz = JsonSerializer.Deserialize<Quiz_item>(game.Data.ToString());
                        // use quiz
                        selectedGames.Add(quiz);
                        break;
                    case nameof(Blank_item):
                        var blank = JsonSerializer.Deserialize<Blank_item>(game.Data.ToString());
                        // use blank
                        selectedGames.Add(blank);
                        break;
                    case nameof(Match_item):
                        var match = JsonSerializer.Deserialize<Match_item>(game.Data.ToString());
                        // use match
                        selectedGames.Add(match);
                        break;
                }
            }
            Debug.WriteLine($"%%%%%Behold!!!!{selectedGames.Count}");

            //redirect to the game pages
            if (selectedGames.Count == 0)
            {
                return RedirectToAction(nameof(FinishGames));
            }
            else if (selectedGames[0] is Quiz_item quizItem)
            {
                selectedGames.RemoveAt(0);
                //wrap the games in a GameWrapper to help with deserialization
                var wrappedGames = selectedGames.Select(g => new GameWrapper
                {
                    Type = g.GetType().Name,
                    Data = g
                }).ToList();
                //save them in session
                HttpContext.Session.SetString(selectedGamesSession, JsonSerializer.Serialize(wrappedGames));
                return RedirectToAction(nameof(Quiz), new { id = quizItem.Id });
            }
            else if (selectedGames[0] is Blank_item blankItem)
            {
                selectedGames.RemoveAt(0);
                //wrap the games in a GameWrapper to help with deserialization
                var wrappedGames = selectedGames.Select(g => new GameWrapper
                {
                    Type = g.GetType().Name,
                    Data = g
                }).ToList();
                //save them in session
                HttpContext.Session.SetString(selectedGamesSession, JsonSerializer.Serialize(wrappedGames));
                return RedirectToAction(nameof(FillBlank), new { id = blankItem.Id });
            }
            else if (selectedGames[0] is Match_item matchItem)
            {
                //pointer problems? (renfrences lists)
                List<object> noMatchGames = selectedGames.ToList();
                List<Match_item> allMatches = [];
                foreach (var item in selectedGames)
                {
                    if (item is Match_item match)
                    {
                        allMatches.Add(match);
                        noMatchGames.Remove((object)match);
                    }
                }
                //wrap the games in a GameWrapper to help with deserialization
                var wrappedGames = noMatchGames.Select(g => new GameWrapper
                {
                    Type = g.GetType().Name,
                    Data = g
                }).ToList();
                //save them in session
                HttpContext.Session.SetString(selectedGamesSession, JsonSerializer.Serialize(wrappedGames));
                return RedirectToAction(nameof(WordMatch), new { ids = allMatches.Select(m => m.Id).ToList() });
            }
            else
            {
                //This shoud NEVER happen
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound("Something Went Wrong");
            }

            //get the numer of sections in the database
            var sectionCount = await _dbContext.Sections.CountAsync();

            //get the section names
            var sectionNames = await _dbContext.Sections
                .Select(s => s.Name)
                .ToListAsync();

            //get the user's completed sections (can be represented by the NotificationsGiven column)
            var completedSections = await _dbContext.Stats
                .Where(s => s.Id == GetCurrentUserId())
                .Select(s => s.SectionsCompleted)
                .FirstOrDefaultAsync();

            //create the model
            var model = new GamesViewModel
            {
                SectionCount = (ushort)sectionCount,
                CompletedSectionIds = completedSections,
                SectionNames = sectionNames
            };

            return View(model);
        }
        public async Task<IActionResult> Quiz(ushort id)
        {
            //This is adds the quiz item(s) to the view
            //this shuffles the answers on the quiz, in case it isn't already done in the DB
            var quiz_item = await _dbContext.Quiz_items.FirstOrDefaultAsync(q => q.Id == id);
            quiz_item.Answers = ShuffleList(quiz_item.Answers);

            return View(quiz_item);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(GameSubmissionViewModel submission)
        {
            //save the quiz results in session
            List<GameAnswerViewModel> oldResults = [];

            string jsonResults = HttpContext.Session.GetString(gameResultsSession);
            if (!string.IsNullOrEmpty(jsonResults))
            {
                // make a list of the ansewrs or an empty list if the results is empty
                oldResults = JsonSerializer.Deserialize<List<GameAnswerViewModel>>(jsonResults) ?? [];
            }

            // Add new answers to the results
            if (submission.Answers != null)
            {
                oldResults.AddRange(submission.Answers);
            }

            // Save updated results back to session
            HttpContext.Session.SetString(gameResultsSession, JsonSerializer.Serialize(oldResults));

            return RedirectToAction(nameof(NextGame));
        }

        public async Task<IActionResult> WordMatch(List<ushort> ids)
        {
            var match_items = await _dbContext.Match_items
                .Where(m => ids.Contains(m.Id))
                .ToListAsync();
            //get the drop list for the html
            List<string> dropList = [];
            foreach (var item in match_items)
            {
                dropList.Add(item.Solution);
            }
            dropList = ShuffleList(dropList);
            return View((match_items, dropList));
        }

        public async Task<IActionResult> SubmitWordMatch(GameSubmissionViewModel submission)
        {
            //save the word Match results in session
            List<GameAnswerViewModel> oldResults = [];

            string jsonResults = HttpContext.Session.GetString(gameResultsSession);
            if (!string.IsNullOrEmpty(jsonResults))
            {
                // make a list of the ansewrs or an empty list if the results is empty
                oldResults = JsonSerializer.Deserialize<List<GameAnswerViewModel>>(jsonResults) ?? [];
            }

            // Add new answers to the results
            if (submission.Answers != null)
            {
                oldResults.AddRange(submission.Answers);
            }

            //testing
            Debug.WriteLine($"&&&&Answers: {JsonSerializer.Serialize(submission.Answers)}");

            // Save updated results back to session
            HttpContext.Session.SetString(gameResultsSession, JsonSerializer.Serialize(oldResults));

            return RedirectToAction(nameof(NextGame));
        }
        public async Task<IActionResult> FillBlank(ushort id)
        {
            //This is adds the blank items to the view
            //this shuffles the answers on the quiz, in case it isn't already done in the DB
            var blank_item = await _dbContext.Blank_items.FirstOrDefaultAsync(b => b.Id == id);
            blank_item.Answers = ShuffleList(blank_item.Answers);

            return View(blank_item);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFillBlank(GameSubmissionViewModel submission)
        {
            //You COULD just use SubmitQuiz, it's the same thing...
            //save the blank results in session
            List<GameAnswerViewModel> oldResults = [];

            string jsonResults = HttpContext.Session.GetString(gameResultsSession);
            if (!string.IsNullOrEmpty(jsonResults))
            {
                // make a list of the ansewrs or an empty list if the results is empty
                oldResults = JsonSerializer.Deserialize<List<GameAnswerViewModel>>(jsonResults) ?? [];
            }

            // Add new answers to the results
            if (submission.Answers != null)
            {
                oldResults.AddRange(submission.Answers);
            }

            // Save updated results back to session
            HttpContext.Session.SetString(gameResultsSession, JsonSerializer.Serialize(oldResults));

            return RedirectToAction(nameof(NextGame));
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {//this shuffles String lists to avoid obvious tests
            Random random = new();
            return list.OrderBy(_ => random.Next()).ToList();
        }

        public async Task<IActionResult> FinishGames()
        {
            //get the user's id from session
            Guid userId = GetCurrentUserId();
            //get the user's stats from the database
            Stats userStats = _dbContext.Stats.FirstOrDefault(s => s.Id == userId);

            //Load all items from the database
            List<Blank_item> blank_items = await _dbContext.Blank_items.ToListAsync();
            List<Quiz_item> quiz_items = await _dbContext.Quiz_items.ToListAsync();
            List<Match_item> match_items = await _dbContext.Match_items.ToListAsync();

            //get the results from the session
            string jsonResults = HttpContext.Session.GetString(gameResultsSession);
            List<GameAnswerViewModel> results = [];
            results = JsonSerializer.Deserialize<List<GameAnswerViewModel>>(jsonResults);
            //get the correct games
            List<GameAnswerViewModel> correctAnswers = results
                .Where(r => r.SelectedValue == "True")
                .ToList();
            //get the failed games
            List<GameAnswerViewModel> wrongAnswers = results
                .Where(r => r.SelectedValue == "False")
                .ToList();
            
            //append the correct games to the user's stats
            List<StatAnswerViewModel> correctAnswersList = [];
            foreach (var game in correctAnswers)
            {//all the names being hardcoded is scary, but *shrug*
                if (game.GameType == "Quiz")
                {
                    ushort sectionId = quiz_items.FirstOrDefault(q => q.Id == game.QuestionId).SectionId;
                    correctAnswersList.Add(new StatAnswerViewModel
                    {
                        SectionId = sectionId,
                        GameType = game.GameType,
                        GameId = (ushort)game.QuestionId
                    });
                }
                else if (game.GameType == "FillBlank")
                {
                    ushort sectionId = blank_items.FirstOrDefault(b => b.Id == game.QuestionId).SectionId;
                    correctAnswersList.Add(new StatAnswerViewModel
                    {
                        SectionId = sectionId,
                        GameType = game.GameType,
                        GameId = (ushort)game.QuestionId
                    });
                }
                else if (game.GameType == "WordMatch")
                {
                    ushort sectionId = match_items.FirstOrDefault(m => m.Id == game.QuestionId).SectionId;
                    correctAnswersList.Add(new StatAnswerViewModel
                    {
                        SectionId = sectionId,
                        GameType = game.GameType,
                        GameId = (ushort)game.QuestionId
                    });
                }
            }
            //add the correct answers to the user's stats
            //check for duplicates to
            if (userStats.CorrectAnswers == null)
            {
                userStats.CorrectAnswers = correctAnswersList;
            }
            else
            {
                var existing = userStats.CorrectAnswers
                    .Select(a => (a.GameType, a.GameId))
                    .ToHashSet();
                var newUnique = correctAnswersList
                    .Where(a => !existing.Contains((a.GameType, a.GameId)))
                    .ToList();
                userStats.CorrectAnswers.AddRange(newUnique);

            }

            //append the failed games to the user's stats
            List<StatAnswerViewModel> wrongAnswersList = [];
            foreach (var game in wrongAnswers)
            {//all the names being hardcoded is scary, but *shrug*
                if (game.GameType == "Quiz")
                {
                    ushort sectionId = quiz_items.FirstOrDefault(q => q.Id == game.QuestionId).SectionId;
                    wrongAnswersList.Add(new StatAnswerViewModel
                    {
                        SectionId = sectionId,
                        GameType = game.GameType,
                        GameId = (ushort)game.QuestionId
                    });
                }
                else if (game.GameType == "FillBlank")
                {
                    ushort sectionId = blank_items.FirstOrDefault(b => b.Id == game.QuestionId).SectionId;
                    wrongAnswersList.Add(new StatAnswerViewModel
                    {
                        SectionId = sectionId,
                        GameType = game.GameType,
                        GameId = (ushort)game.QuestionId
                    });
                }
                else if (game.GameType == "WordMatch")
                {
                    ushort sectionId = match_items.FirstOrDefault(m => m.Id == game.QuestionId).SectionId;
                    wrongAnswersList.Add(new StatAnswerViewModel
                    {
                        SectionId = sectionId,
                        GameType = game.GameType,
                        GameId = (ushort)game.QuestionId
                    });
                }
            }
            //add the wrong answers to the user's stats
            //check for duplicates too
            if (userStats.WrongAnswers == null)
            {
                userStats.WrongAnswers = wrongAnswersList;
            }
            else
            {
                var existing = userStats.WrongAnswers
                    .Select(a => (a.GameType, a.GameId))
                    .ToHashSet();
                var newUnique = wrongAnswersList
                    .Where(a => !existing.Contains((a.GameType, a.GameId)))
                    .ToList();
                userStats.WrongAnswers.AddRange(newUnique);
            }
            //remove Wrong answers from the correct answers
            if (userStats.CorrectAnswers != null && userStats.WrongAnswers != null)
            {
                var correctSet = userStats.CorrectAnswers
                    .Select(a => (a.GameType, a.GameId))
                    .ToHashSet();

                userStats.WrongAnswers.RemoveAll(a => correctSet.Contains((a.GameType, a.GameId)));
            }

            //update the user's stats in the database
            _dbContext.Stats.Update(userStats);
            await _dbContext.SaveChangesAsync();

            //combine WordMatch results if needed
            if (results.Any(r => r.GameType == "WordMatch"))
                results = CombineWordMatches(results);

            //calculate the points earned
            AddPoints(results);

            //Serialize the results
            string resultsSerialized = JsonSerializer.Serialize(results);

            return RedirectToAction(nameof(Results), new { json = resultsSerialized });
        }

        //gets the user's id from session
        private Guid GetCurrentUserId()
        {
            var currentUserId = User.FindFirst("User_id")?.Value;
            return Guid.TryParse(currentUserId, out Guid userId) ? userId : Guid.Empty;
        }
        public async Task<IActionResult> Results(string json)
        {
            List<GameAnswerViewModel> results = JsonSerializer.Deserialize<List<GameAnswerViewModel>>(json);

            GameSubmissionViewModel model = new GameSubmissionViewModel
            {
                Answers = results
            };

            return View(model);
        }
    
        public List<GameAnswerViewModel> CombineWordMatches(List<GameAnswerViewModel> results)
        {
            //check if all the WordMatch items were correct
            bool wordMatchCorrect = true;
            if (results.Any(a => a.GameType == "WordMatch"))
            {
                if (results.Any(a => a.SelectedValue == "False" && a.GameType == "WordMatch"))
                {
                    wordMatchCorrect = false;
                }
                // keep only one WordMatch item in the results
                int FirstWordMatch = results.FindIndex(a => a.GameType == "WordMatch");
                results[FirstWordMatch] = new GameAnswerViewModel
                {
                    QuestionId = 0, //placeholder ID for WordMatch
                    SelectedValue = wordMatchCorrect ? "True" : "False",
                    GameType = "FinalWordMatch"
                };
                results.RemoveAll(a => a.GameType == "WordMatch");
            }
            return results;
        }

        public async void AddPoints(List<GameAnswerViewModel> results)
        {            
            //get the user's stats from the database
            Guid userId = GetCurrentUserId();
            Stats userStats = _dbContext.Stats.FirstOrDefault(s => s.Id == userId);
            PointsEarned allPoints = _dbContext.PointsEarned.FirstOrDefault(p => p.Id == userId);
            //find what game we playing
            string pointType = HttpContext.Session.GetString(gameDifficulty);
            
            //calculate the points earned
            int newPoints = 0;
            if (pointType == null || pointType == "skip")
            {
                return;
            }
            else if (pointType.Substring(1) == "easy")
            {
                newPoints = results.Count * pointsEasy;
            }
            else if (pointType.Substring(1) == "hard")
            { 
                newPoints = results.Count * pointsHard;
            }
            //find the old points
            if (pointType[0] == '1')
            {
                if (pointType.Substring(1) == "easy")
                {
                    int oldPoints = (int)allPoints.Section1Easy;
                    if (newPoints > oldPoints)
                        allPoints.Section1Easy = (uint)newPoints;
                }
                else if (pointType.Substring(1) == "hard")
                {
                    int oldPoints = (int)allPoints.Section1Hard;
                    if (newPoints > oldPoints)
                        allPoints.Section1Hard = (uint)newPoints;
                }
            }
            else if (pointType[0] == '2')
            {
                if (pointType.Substring(1) == "easy")
                {
                    int oldPoints = (int)allPoints.Section2Easy;
                    if (newPoints > oldPoints)
                        allPoints.Section2Easy = (uint)newPoints;
                }
                else if (pointType.Substring(1) == "hard")
                {
                    int oldPoints = (int)allPoints.Section2Hard;
                    if (newPoints > oldPoints)
                        allPoints.Section2Hard = (uint)newPoints;
                }
            }
            else if (pointType[0] == '3')
            {
                if (pointType.Substring(1) == "easy")
                {
                    int oldPoints = (int)allPoints.Section3Easy;
                    if (newPoints > oldPoints)
                        allPoints.Section3Easy = (uint)newPoints;
                }
                else if (pointType.Substring(1) == "hard")
                {
                    int oldPoints = (int)allPoints.Section3Hard;
                    if (newPoints > oldPoints)
                        allPoints.Section3Hard = (uint)newPoints;
                }
            }
            else if (pointType[0] == '4')
            {
                if (pointType.Substring(1) == "easy")
                {
                    int oldPoints = (int)allPoints.Section4Easy;
                    if (newPoints > oldPoints)
                        allPoints.Section4Easy = (uint)newPoints;
                }
                else if (pointType.Substring(1) == "hard")
                {
                    int oldPoints = (int)allPoints.Section4Hard;
                    if (newPoints > oldPoints)
                        allPoints.Section4Hard = (uint)newPoints;
                }
            }
            //update the user's points in the database
            _dbContext.PointsEarned.Update(allPoints);
            //calculate the total points
            uint totalPoints = allPoints.Section1Easy + allPoints.Section1Hard +
                               allPoints.Section2Easy + allPoints.Section2Hard +
                               allPoints.Section3Easy + allPoints.Section3Hard +
                               allPoints.Section4Easy + allPoints.Section4Hard;
            userStats.TotalPoints = totalPoints;
            _dbContext.Stats.Update(userStats);
            await _dbContext.SaveChangesAsync();
        }
    }
}
