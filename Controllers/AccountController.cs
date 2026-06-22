using Microsoft.AspNetCore.Mvc;
using HOLA.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;


namespace HOLA.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // 1. Connect the Identity Database Managers
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 2. Actually check the Database for the user's email and password
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Success! Send them to the Q&A Forum
                    return RedirectToAction("Forum", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 3. Create a new user object
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                // 4. Save the new user to the SQLite Database
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Log them in immediately after registering
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Forum", "Home");
                }

                // If registration fails (e.g., password too weak), show the errors!
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // POST: Account/ForgotPassword
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            TempData["Message"] = "Reset link sent.";
            return RedirectToAction("Login");
        }
    }
}

