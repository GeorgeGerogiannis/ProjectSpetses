using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProjectSpetses.Models.Entities;
using ProjectSpetses.Models;
using System.Security.Claims;
using ProjectSpetses.Data;
using ProjectSpetses.Other;

namespace ProjectSpetses.Controllers
{
    public class LoginController(ApplicationDbContext dbContext) : Controller
    {
        //get access to the database
        private readonly ApplicationDbContext _dbContext = dbContext;

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            //check if user exists in the database
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Username == viewModel.Username);

            if (!VerifyUser(user, viewModel.Password))
            {
                //if login fails, add error and return to view
                ModelState.AddModelError("LoginCredentials", "Λανθασμένο όνομα χρήστη ή κωδικός πρόσβασης");
                return View(viewModel);
            }

            //sign in the user
            await CreateCookieAsync(user.Id, user.Username);

            //redirect to the home page after successful login
            return RedirectToAction(nameof(HomeController.Index),nameof(HomeController)[..nameof(HomeController).LastIndexOf("Controller")]);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            //sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //redirect to the home page after logout
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController)[..nameof(HomeController).LastIndexOf("Controller")]);
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            //check if the username already exists
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Username == viewModel.Username);
            if (user != null)
            {
                //if username already exists, add error and return to view
                ModelState.AddModelError("RegisterCredentials", "Υπάρχει ήδη χρήστης με το όνομα " + viewModel.Username + "!");
                return View(viewModel);
            }

            Hashing obj = new();
            //generate the salt
            var salt = obj.GenerateSalt();
            //hash the password
            var hashedPassword = obj.HashPassword(viewModel.Password, salt);

            var userId = Guid.NewGuid();

            //create the user and their stats
            var newUser = new User
            {
                Id = userId,
                Username = viewModel.Username,
                Password = hashedPassword,
                Salt = salt,
            };

            var stats = new Stats
            {
                Id = userId,
                TotalPoints = 0,
                CategoriesRead = [],
                NotificationsGiven = [],
                CreatedAt = DateTime.UtcNow,
                CorrectAnswers = [],
                WrongAnswers = []
            };

            //add the user and the stats to the database
            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.Stats.AddAsync(stats);
            await _dbContext.SaveChangesAsync();

            //sign in the user
            await CreateCookieAsync(userId, viewModel.Username);

            //redirect to the home page after successful registration
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController)[..nameof(HomeController).LastIndexOf("Controller")]);
        }

        //checks if the user exists and verifies the password
        private static bool VerifyUser(User user, string password)
        {
            if (user == null)
            {
                return false;
            }

            Hashing obj = new();
            var hashedPassword = obj.HashPassword(password, user.Salt);

            return hashedPassword == user.Password;
        }

        //creates a cookie for the user after successful login or registration
        private async Task CreateCookieAsync(Guid userId, string userName)
        {
            var claims = new List<Claim>
            {
                new("User_id", userId.ToString()),
                new(ClaimTypes.Name, userName),
            };

            //cookies-session management
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}
