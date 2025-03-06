using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
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
            var student_data = Context.CourseStudents
                .Where(s => s.StudentId == id)
                .Include(s => s.Student)
                .Include(s => s.Course)
                .Include(s => s.Student.Department)
                .AsEnumerable()
                .GroupBy(s => new {
                    s.Student.name,
                    s.Student.image,
                    s.Student.age,
                    s.Student.address,
                    dept_name = s.Student.Department.name,
                    course_min_deg = s.Course.MinimumDegree,
                })
                .Select(g => new StudentDetailsVM
                {
                    name = g.Key.name,
                    image = g.Key.image,
                    age = g.Key.age,
                    address = g.Key.address,
                    dept_name = g.Key.dept_name,
                    course_min_degree = g.Key.course_min_deg,
                    courses = g.ToDictionary(c => c.Course.name, c => c.Degree)
                })
                .FirstOrDefault();
            HttpContext.Session.SetString("last_student", student_data.name ?? "Something's wrong");
            return View(student_data);
        }
    }
}
