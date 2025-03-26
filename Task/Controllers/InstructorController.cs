using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.BL;
using Task.Contexts;
using Task.Models;
using Task.ViewModels.Instructor;

namespace Task.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IInstructorService<InstructorVM> _InstructorService;
        public InstructorController(IInstructorService<InstructorVM> InstructorService)
        {
            _InstructorService = InstructorService;
        }

        public IActionResult Index()
        {
            return View(_InstructorService.PrepareDashboard().ToList());
        }

        public IActionResult Details(int id)
        {
            return View(_InstructorService.PrepareDetailsPage(id));
        }

        public IActionResult Edit(int id)
        {
            return View(_InstructorService.PrepareEditPage(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(InstructorVM form_data)
        {
            await _InstructorService.EditInstructor(form_data);
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            return View(_InstructorService.PrepareAddForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNew(InstructorVM FormData)
        {
            if(!ModelState.IsValid)
            {
                return View("Add", _InstructorService.PrepareAddForm());
            }
            await _InstructorService.AddInstructor(FormData);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int Id)
        {
            await _InstructorService.RemoveInstructor(Id);
            return RedirectToAction("Index");
        }
    }
}
