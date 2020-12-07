using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks; // for using: Because our User management functions are all asynchronous, we'll have to declare our Controller methods as async to make use of them.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginRegWithIdentity.Models;
using LoginRegWithIdentity.ViewModels;
using Microsoft.AspNetCore.Identity; // dependency injection

namespace LoginRegWithIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(ILogger<HomeController> logger, MyContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("register")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Create a new User object, without adding a Password
                User NewUser = new User { Email = viewModel.Email, UserName = viewModel.UserName };
                // CreateAsync will attempt to create the User in the database, simultaneously hashing the password
                IdentityResult result = await _userManager.CreateAsync(NewUser, viewModel.Password);
                // If the User was added to the database successfully
                if (result.Succeeded)
                {
                    // Sign In the newly created User
                    // We're using the SignInManager, not the UserManager!
                    await _signInManager.SignInAsync(NewUser, isPersistent: false);
                    return RedirectToAction("Dashboard");
                }
                // If the creation failed, add the errors to the View Model
                // string errors = String.Empty;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult LoginForm()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(viewModel.UserNameLogin, viewModel.PasswordLogin, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View("LoginForm");
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            User CurrentUser = await GetCurrentUserAsync();
            // if (CurrentUser == null)
            // {
            //     TempData["NotYetLoginOrRegister"] = "Please login or register to access the app.";
            //     return RedirectToAction("LoginForm");
            // }

            // You don't need to use the SigninManager or something similar. The user is injected on the pipeline (on the User property of the base controller) and it's info is filled automatically by the authentication middleware (cookie or token). So, on your controller:
            bool isAuthenticated = User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                TempData["NotYetLoginOrRegister"] = "Please login or register to access the app.";
                return RedirectToAction("LoginForm");
            }
            return View(CurrentUser);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        // One operation we often want to perform multiple times in our code is fetching the user that's currently logged in.
        // Because we know we'll be doing this a lot it's a good idea to pull it out into a little helper function.
        private Task<User> GetCurrentUserAsync()
        {
            // We have global access to HttpContext.User which will give us the ClaimsPrincipal of the currently logged in user, an object that contains the identity and security privileges of the user.
            return _userManager.GetUserAsync(HttpContext.User);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
