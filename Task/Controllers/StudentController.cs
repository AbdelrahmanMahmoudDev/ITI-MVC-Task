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

namespace Task.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolContext _Context;
        IRepository<Student> _StudentRepo;
        IRepository<Course> _CourseRepo;
        IRepository<Department> _DepRepo;
        IJointRepository<CourseStudents> _StudentCourseRepo;
        IService<StudentAddVM, Student> _StudentService;
        public StudentController(SchoolContext Context, IRepository<Student> StudentRepo, IJointRepository<CourseStudents> StudentCourseRepo, IRepository<Course> CourseRepo, IRepository<Department> DepRepo, IService<StudentAddVM, Student> StudentService)
        {
            _Context = Context ?? throw new ArgumentNullException(nameof(Context));
            _StudentRepo = StudentRepo ?? throw new ArgumentNullException(nameof(StudentRepo));
            _StudentCourseRepo = StudentCourseRepo ?? throw new ArgumentNullException(nameof(StudentCourseRepo));
            _CourseRepo = CourseRepo ?? throw new ArgumentNullException(nameof(CourseRepo));
            _DepRepo = DepRepo ?? throw new ArgumentNullException(nameof(DepRepo));
            _StudentService = StudentService ?? throw new ArgumentNullException(nameof(StudentService));
        }
        public IActionResult Index()
        {
            var Students = _StudentRepo.GetAll(["Department"]).ToList();

            return View("Index", Students);
        }
        public IActionResult Details(int id)
        {
            try
            {
                var student = _StudentCourseRepo.GetById(id, ["Student", "Course", "Student.Department"]);

                StudentDetailsVM StudentModel = new StudentDetailsVM()
                {
                    name = student.Student.name,
                    image = student.Student.image,
                    age = student.Student.age,
                    address = student.Student.address,
                    dept_name = student.Student.Department.name,
                    course_min_degree = student.Course.MinimumDegree,
                    courses = _StudentCourseRepo.GetRangeById(id, ["Course"]).ToDictionary(c => c.Course.name, c => c.Degree),
                };
                return View("Details", StudentModel);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        public IActionResult Edit(int id)
        {
            return View(_StudentService.PrepareEditForm(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(StudentAddVM FormData, int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", _StudentService.PrepareEditForm(id));
            }

            await _StudentService.Update(FormData, id);

            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            return View(_StudentService.PrepareCreateForm());
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
