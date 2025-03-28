using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task.ViewModels.Accounts;

namespace Task.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _RoleManager;
        public RoleController(RoleManager<IdentityRole> RoleManager)
        {
            _RoleManager = RoleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {   
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRole(RoleViewModel FormData)
        {
            if(!ModelState.IsValid)
            {
                return View("Index", FormData);
            }

            IdentityRole Role = new IdentityRole()
            {
                Name = FormData.RoleName,
            };

            IdentityResult OpResult = await _RoleManager.CreateAsync(Role);
            if(!OpResult.Succeeded)
            {
                foreach (var Item in OpResult.Errors)
                {
                    ModelState.AddModelError("", Item.Description);
                }
            }

            return RedirectToAction("Index", "Role");
        }
    }
}