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
            catch (Exception)
            {
                return StatusCode(500, "An error occured while processing your request.");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int Id)
        {
            try
            {
                DepartmentVM DepartmentModel = _DepartmentService.PrepareEdit(Id);
                return View(DepartmentModel);
            }
            catch (NullReferenceException)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(int Id, DepartmentVM FormData)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    DepartmentVM DepartmentModel = _DepartmentService.PrepareEdit(Id);
                    return View(DepartmentModel);
                }
                catch (NullReferenceException)
                {
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            try
            {
                await _DepartmentService.Update(FormData, Id);
                return RedirectToAction("Index");
            }
            catch(InvalidOperationException)
            {
                return StatusCode(500, "An error occcured while processing your request.");
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                await _DepartmentService.DeleteAsync(Id);
            }
            catch(InvalidOperationException)
            {
                return StatusCode(500, "An error occured processing your request.");
            }

            return RedirectToAction("Index");
        }
    }
}
