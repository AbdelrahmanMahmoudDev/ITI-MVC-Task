using Microsoft.AspNetCore.Mvc;
using Task.BL;
using Task.Models;
using Task.ViewModels;

namespace Task.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService<CourseVM> _CourseService;

        public CourseController(ICourseService<CourseVM> CourseService)
        {
            _CourseService = CourseService ?? throw new ArgumentNullException(nameof(CourseService));
        }

        public IActionResult Index()
        {
            List<Course> Courses = _CourseService.PrepareDashboard().ToList();
            return View("Index", Courses);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd(CourseVM FormData)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", FormData);
            }

            try
            {
                await _CourseService.CreateAsync(FormData);
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
                CourseVM CourseModel = _CourseService.PrepareEdit(Id);
                return View(CourseModel);
            }
            catch(NullReferenceException)
            {
                return StatusCode(500, "An error occured while processing your request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(int Id, CourseVM FormData)
        {
            if(!ModelState.IsValid)
            {
                try
                {
                    CourseVM CourseModel = _CourseService.PrepareEdit(Id);
                    return View(CourseModel);
                }
                catch(NullReferenceException)
                {
                    return StatusCode(500, "An error occured while processing your request.");
                }
            }

            try
            {
                await _CourseService.UpdateAsync(FormData, Id);
                return RedirectToAction("Index");
            }
            catch(InvalidOperationException)
            {
                return StatusCode(500, "An error occured while processing your request.");
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                await _CourseService.DeleteAsync(Id);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(500, "An error occured processing your request.");
            }

            return RedirectToAction("Index");
        }
    }
}
