using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;
using Task.ViewModels.Student;
using Task.ViewModels.Instructor;
using System.Linq;
using Task.Utilities;
using Microsoft.IdentityModel.Tokens;
using Task.Repositories;
using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore.Storage;
using Task.BL;
using Microsoft.SqlServer.Server;
using Task.Repositories.Base;

namespace Task.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService<StudentAddVM> _StudentService;
        public StudentController(IStudentService<StudentAddVM> StudentService)
        {
            _StudentService = StudentService ?? throw new ArgumentNullException(nameof(StudentService));
        }
        public IActionResult Index()
        {
            try
            {
                var Students = _StudentService.PrepareDashboardData();
                return View("Index", Students.ToList());
            }
            catch (Exception Ex)
            {
                return StatusCode(500, "An error occured while processing your request.");
            }

        }
        public IActionResult Details(int id)
        {
            try
            {
                StudentAddVM StudentModel = _StudentService.PrepareDetails(id);
                return View("Details", StudentModel);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        public IActionResult Edit(int id)
        {
            try
            {
                StudentAddVM StudentModel = _StudentService.PrepareEditForm(id);
                return View(StudentModel);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(StudentAddVM FormData, int id)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    StudentAddVM StudentModel = _StudentService.PrepareEditForm(id);
                    return View(StudentModel);
                }
                catch (Exception Ex)
                {
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            try
            {
                await _StudentService.Update(FormData, id);
                return RedirectToAction("Index");
            }
            catch (Exception Ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        public IActionResult Add()
        {
            try
            {
                StudentAddVM StudentModel = _StudentService.PrepareCreateForm();
                return View(StudentModel);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd(StudentAddVM FormData)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", FormData);
            }

            try
            {
                await _StudentService.CreateAsync(FormData);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, "An error occured processing your request.");
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _StudentService.DeleteAsync(id);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, "An error occured processing your request.");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Search(string SearchTerm)
        {
            return PartialView("_SearchResults", _StudentService.GetBySearch(SearchTerm)); ;
        }
    }
}
