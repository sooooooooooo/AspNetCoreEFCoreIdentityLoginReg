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
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return View();
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
