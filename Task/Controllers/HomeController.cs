using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Models;
using Task.Repositories.Base;
using Task.ViewModels;
using Task.ViewModels.Accounts;

namespace Task.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IUnitOfWork _UnitOfWork;


        public HomeController(UserManager<ApplicationUser> UserManager, SignInManager<ApplicationUser> SignInManager, RoleManager<IdentityRole> RoleManager, IUnitOfWork UnitOfWork)
        {
            _UserManager = UserManager ?? throw new ArgumentNullException(nameof(UserManager));
            _SignInManager = SignInManager ?? throw new ArgumentNullException(nameof(SignInManager));
            _RoleManager = RoleManager ?? throw new ArgumentNullException(nameof(RoleManager));
            _UnitOfWork = UnitOfWork ?? throw new ArgumentNullException(nameof(UnitOfWork));
        }

        [HttpGet]
        public IActionResult Index()
        {
            DashboardVM RequiredData = new DashboardVM()
            {
                InstructorCount = _UnitOfWork.Instructors.GetAll().ToList().Count,
                DepartmentCount = _UnitOfWork.Departments.GetAll().ToList().Count,
                CourseCount = _UnitOfWork.Courses.GetAll().ToList().Count,
                StudentCount = _UnitOfWork.Students.GetAll().ToList().Count
            };
            ViewData["Title"] = "Home Page";
            return View(RequiredData);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            RegisterUserViewModel RequiredData = new RegisterUserViewModel();
            RequiredData.AvailableRoles = await _RoleManager.Roles.ToListAsync();
            return View(RequiredData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRegister(RegisterUserViewModel FormData)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser AppUser = new ApplicationUser()
                {
                    UserName = FormData.Username,
                    PasswordHash = FormData.Password,
                    Email = FormData.Email
                };

                IdentityResult RegisterResult = await _UserManager.CreateAsync(AppUser, FormData.Password);
                if (RegisterResult.Succeeded)
                {
                    IdentityRole AssignedRole = await _RoleManager.FindByIdAsync(FormData.ChosenRole);
                    await _UserManager.AddToRoleAsync(AppUser, AssignedRole.Name);

                    // TODO: Figure out where to keep user after sign in
                    await _SignInManager.SignInAsync(AppUser, false);
                    return RedirectToAction("Index", "Student");
                }
            }

            FormData.AvailableRoles = await _RoleManager.Roles.ToListAsync();
            return View("Register", FormData);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
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
            if (TargetUser == null)
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
                return RedirectToAction("Login", "Home");
            }

            bool IsPasswordCorrect = await _UserManager.CheckPasswordAsync(TargetUser, FormData.Password);
            if (!IsPasswordCorrect)
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
                return RedirectToAction("Login", "Home");
            }
            await _SignInManager.SignInAsync(TargetUser, FormData.IsRemembered);
            return RedirectToAction("Index", "Student");
        }
    }
}
