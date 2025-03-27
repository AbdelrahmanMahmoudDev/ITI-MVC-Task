using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task.Models;
using Task.ViewModels.Accounts;

namespace Task.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;


        public HomeController(UserManager<ApplicationUser> UserManager, SignInManager<ApplicationUser> SignInManager)
        {
            _UserManager = UserManager ?? throw new ArgumentNullException(nameof(UserManager));
            _SignInManager = SignInManager ?? throw new ArgumentNullException(nameof(SignInManager));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRegister(RegisterUserViewModel FormData)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", FormData);
            }
            ApplicationUser AppUser = new ApplicationUser()
            {
                UserName = FormData.Username,
                PasswordHash = FormData.Password
            };
            IdentityResult RegisterResult = await _UserManager.CreateAsync(AppUser, FormData.Password);
            if (RegisterResult.Succeeded)
            {
                // TODO: Roles
                await _SignInManager.SignInAsync(AppUser, false);
                return RedirectToAction("Index", "Student");
            }
            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLogin(LoginUserViewModel FormData)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", FormData);
            }

            ApplicationUser TargetUser = await _UserManager.FindByNameAsync(FormData.Username);
            if(TargetUser == null)
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
                return RedirectToAction("Login", "Home");
            }

            bool IsPasswordCorrect = await _UserManager.CheckPasswordAsync(TargetUser, FormData.Password);
            if(!IsPasswordCorrect)
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
                return RedirectToAction("Login", "Home");
            }
            await _SignInManager.SignInAsync(TargetUser, FormData.IsRemembered);
            return RedirectToAction("Index", "Student");
        }
    }
}
