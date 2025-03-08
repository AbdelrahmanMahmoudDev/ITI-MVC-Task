using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;
using Task.ViewModels.Student;
using Task.ViewModels.Instructor;
using System.Linq;

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
        public IActionResult Edit(int id)
        {
            var student = Context.Students
                .Where(s => s.StudentId == id)
                .Include(s => s.Department)
                .Select(s => new StudentEditVM
                {
                    address = s.address,
                    selected_department_id = s.DepartmentId,
                    selected_department_name = s.Department.name,
                })
                .FirstOrDefault();

            if(student != null)
            {
                student.registered_courses = Context.CourseStudents
                        .Where(c => c.StudentId == id)
                        .Include(s => s.Course)
                        .ToDictionary(c => c.Course.name, c => c.Degree);
                student.departments = Context.Departments.ToList();
                student.courses = Context.Courses.ToList();
                student.student_id = id;
            }
            return View(student);
        }
        [HttpPost]
        public IActionResult SaveEdit(StudentEditVM form_data, List<int> selected_courses, int id)
        {
            var curr_student = Context.Students
                .Include(s => s.CourseStudents)
                .FirstOrDefault(s => s.StudentId == id);

            if (curr_student == null)
            {
                return NotFound("Student does not exist.");
            }

            curr_student.DepartmentId = form_data.selected_department_id;
            curr_student.address = form_data.address;

            Context.SaveChanges(); 
            var existing_courses = curr_student.CourseStudents.ToList();

            foreach (var existing in existing_courses)
            {
                if (!selected_courses.Contains((int)existing.CourseId))
                {
                    Context.CourseStudents.Remove(existing);
                }
            }

            foreach (var crs_id in selected_courses)
            {
                if (!existing_courses.Any(c => c.CourseId == crs_id))
                {
                    Context.CourseStudents.Add(new CourseStudents
                    {
                        CourseId = crs_id,
                        StudentId = id,
                        Degree = 0
                    });
                }
            }

            Context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
