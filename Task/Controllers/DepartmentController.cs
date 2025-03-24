using Microsoft.AspNetCore.Mvc;
using Task.BL;
using Task.Models;
using Task.Repositories.Base;
using Task.ViewModels;

namespace Task.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService<DepartmentVM> _DepartmentService;
        public DepartmentController(IDepartmentService<DepartmentVM> DepartmentService)
        {
            _DepartmentService = DepartmentService ?? throw new ArgumentNullException(nameof(DepartmentService));
        }

        public IActionResult Index()
        {
            List<Department> Departments = _DepartmentService.PrepareDashboard().ToList();
            return View("Index", Departments);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd(DepartmentVM FormData)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", FormData);
            }

            try
            {
                await _DepartmentService.CreateAsync(FormData);
            }
            catch(Exception)
            {
                return StatusCode(500, "An error occured while processing your request.");
            }

            return RedirectToAction("Index");
        }
    }
}
