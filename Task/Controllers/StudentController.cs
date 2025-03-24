using Microsoft.AspNetCore.Mvc;
using Task.ViewModels.Student;
using System.Diagnostics;
using Task.BL;

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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
                catch (Exception)
                {
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            try
            {
                await _StudentService.Update(FormData, id);
                return RedirectToAction("Index");
            }
            catch (Exception)
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
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd([FromForm] StudentAddVM FormData)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", FormData);
            }

            try
            {
                await _StudentService.CreateAsync(FormData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
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
            catch (Exception)
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
