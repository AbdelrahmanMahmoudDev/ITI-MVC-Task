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
        SchoolContext context;
        private readonly IInstructorService<InstructorVM> _InstructorService;
        public InstructorController(IInstructorService<InstructorVM> InstructorService)
        {
            context = new SchoolContext();
            _InstructorService = InstructorService;
        }
        public IActionResult Index()
        {
            var instructor_list = context.Instructors.Include(i => i.Department).ToList();
            return View(instructor_list);
        }

        public IActionResult Details(int id)
        {
            var instructor = context.Instructors
                                    .Where(i => i.InstructorId == id)
                                    .Select(i => new InstructorVM
                                    {
                                        fname = i.fname,
                                        lname = i.lname,
                                        full_name = i.fname + " " + i.lname,
                                        image_path = i.image,
                                        HireDate = new DateTime(i.HireDate.Value.Year, i.HireDate.Value.Month,                                                i.HireDate.Value.Day),
                                        salary = i.salary,
                                        age = i.age
                                    })
                                    .FirstOrDefault();

            if (instructor != null)
            {
                instructor.course_names = context.Courses
                    .Where(c => c.Instructor.InstructorId == id)
                    .Select(c => c.name)
                    .ToList();
            }

            return View(instructor);
        }
        public IActionResult Edit(int id)
        {
            var instructor_details = context.Courses
                         .Where(c => c.Instructor.InstructorId == id)
                         .Include(c => c.Instructor)
                         .Include(c => c.Instructor.Department)
                         .Select(c => new InstructorEditVM
                         {
                             id = id,
                             relevant_course_id = context.Courses
                             .Where(c => c.Instructor.InstructorId == id)
                             .Select(c => c.CourseId)
                             .FirstOrDefault(),
                             existing_courses = context.Courses.ToList(),
                             relevant_dep_id = c.Instructor.Department.DepartmentId,
                             existing_departments = context.Departments.ToList(),
                             salary = c.Instructor.salary,
                         })
                         .FirstOrDefault();
            return View(instructor_details);
        }
        [HttpPost]
        public IActionResult SaveEdit(InstructorEditVM form_data)
        {
            Instructor relevant_instructor = context.Instructors
                .Where(inst => inst.InstructorId == form_data.id)
                .FirstOrDefault();

            relevant_instructor.salary = form_data.salary;
            relevant_instructor.DepartmentId = form_data.relevant_dep_id;

            Course relevant_course = context.Courses
                .Where(crs => crs.CourseId == form_data.relevant_course_id)
                .FirstOrDefault();

            relevant_course.InstructorId = form_data.id;

            context.SaveChanges();
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
