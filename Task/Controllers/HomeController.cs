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
            if(!ModelState.IsValid)
            {
                return View("Register", FormData);
            }

            ApplicationUser AppUser = new ApplicationUser()
            {
                UserName = FormData.Username,
                PasswordHash = FormData.Password
            };

            IdentityResult RegisterResult = await _UserManager.CreateAsync(AppUser, FormData.Password);
            if(RegisterResult.Succeeded)
            {
                await _SignInManager.SignInAsync(AppUser, false);
                return RedirectToAction("Index", "Student");
            }


            return View("Index");
        }
    }
}
