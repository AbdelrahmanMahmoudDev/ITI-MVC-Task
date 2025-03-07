using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;
using Task.ViewModels;

namespace Task.Controllers
{
    public class StudentController : Controller
    {
        SchoolContext Context = new SchoolContext();
        public IActionResult Index()
        {
            var students = Context.Students
                                  .Include(studs=>studs.Department)
                                  .ToList();    
            return View(students);
        }
        public IActionResult Details(int id)
        {
            var student = Context.CourseStudents
                .Where(s => s.StudentId == id)
                .Include(s => s.Student)
                .Include(s => s.Course)
                .Include(s => s.Student.Department)
                .Select(s => new StudentDetailsVM
                {
                    name = s.Student.name,
                    image = s.Student.image,
                    age = s.Student.age,
                    address = s.Student.address,
                    dept_name = s.Student.Department.name,
                    course_min_degree = s.Course.MinimumDegree,
                }).FirstOrDefault();

            if (student != null)
            {
                student.courses = Context.CourseStudents
                    .Where(c => c.StudentId == id)
                    .Include(s => s.Course)
                    .ToDictionary(c => c.Course.name, c => c.Degree);
            }
            HttpContext.Session.SetString("last_student", student.name ?? "Something's wrong");
            return View(student);
        }
    }
}
