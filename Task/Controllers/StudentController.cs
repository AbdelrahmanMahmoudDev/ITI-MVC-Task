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

namespace Task.Controllers
{
    public class StudentController : Controller
    {
        SchoolContext Context = new SchoolContext();
        StudentRepository StudentRepo = new StudentRepository();
        public IActionResult Index()
        {
            //var students = Context.Students
            //                      .Include(studs => studs.Department)
            //                      .ToList();

            var Students = StudentRepo.GetAll(new List<string>{ "Department" }).ToList();

            return View("Index", Students);
        }
        // TODO: CourseStudent Repo
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
            //HttpContext.Session.SetString("last_student", student.name ?? "Something's wrong");
            return View(student);
        }
        public IActionResult Edit(int id)
        {
            //var student = Context.Students
            //    .Where(s => s.StudentId == id)
            //    .Include(s => s.Department)
            //    .Select(s => new StudentAddVM
            //    {
            //        id = id,
            //        name = s.name,
            //        age = s.age,
            //        address = s.address,
            //        ImagePath = s.image,
            //        selected_department_id = s.DepartmentId,
            //    })
            //    .FirstOrDefault();

            Student Target = null;
            try
            {
                Target = StudentRepo.GetById(id, new List<string> { "Department" });
            }
            catch(InvalidOperationException e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

            var student = new StudentAddVM()
            {
                id = id,
                name = Target.name,
                age = Target.age,
                address = Target.address,
                ImagePath = Target.image,
                selected_department_id = Target.DepartmentId,
            };

            if (student != null)
            {
                student.course_details = Context.CourseStudents
                    .Where(crs => crs.StudentId == id)
                    .Include(crs => crs.Course)
                    .Select(crs => new CourseDetails() { course_id = (int)crs.CourseId, CourseName = crs.Course.name, Degree = crs.Degree }).ToList();
                student.departments = Context.Departments.ToList();
                student.courses = Context.Courses.ToList();
            }
            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(StudentAddVM FormData, int id)
        {
            if (!ModelState.IsValid)
            {
                FormData.id = id;
                FormData.age = StudentRepo.GetById(id).age;
                FormData.departments = Context.Departments.ToList();
                FormData.courses = Context.Courses.ToList();
                FormData.course_details = Context.CourseStudents
                    .Where(crs => crs.StudentId == id)
                    .Include(crs => crs.Course)
                    .Select(crs => new CourseDetails
                    {
                        course_id = (int)crs.CourseId,
                        CourseName = crs.Course.name,
                        Degree = crs.Degree
                    })
                    .ToList();
                return View("Edit", FormData);
            }
            //var curr_student = Context.Students
            //    .Include(s => s.CourseStudents)
            //    .FirstOrDefault(s => s.StudentId == id);

            var curr_student = StudentRepo.GetById(id, new List<string> { "CourseStudents" });

            if (curr_student == null)
            {
                return NotFound("Student does not exist.");
            }

            curr_student.name = FormData.name;
            if (FormData.image != null)
            {
                curr_student.image = await FileUtility.SaveFile(FormData.image, "images/students", [".jpg", ".jpeg", ".png"]);
            }
            curr_student.address = FormData.address;
            curr_student.DepartmentId = FormData.selected_department_id;
            curr_student.address = FormData.address;
            if (FormData.course_details.Any())
            {
                var ExistingCourses = curr_student.CourseStudents;
                foreach (var Course in ExistingCourses)
                {
                    if (Course.StudentId == id)
                    {
                        Context.CourseStudents.Remove(Course);
                    }
                }

                foreach (var NewCourse in FormData.course_details)
                {
                    Context.CourseStudents.Add(new CourseStudents()
                    {
                        StudentId = id,
                        CourseId = NewCourse.course_id,
                        Degree = NewCourse.Degree
                    });
                }

            }

            await StudentRepo.UploadToDatabaseAsync(); 
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            var departments = Context.Departments.ToList();
            var courses = Context.Courses.ToList();

            StudentAddVM deps_courses = new StudentAddVM()
            {
                departments = departments,
                courses = courses,
            };
            return View(deps_courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd(StudentAddVM form_data)
        {
            if (!ModelState.IsValid)
            {
                return View("Add", form_data);
            }

            // We assume each student MUST have atleast one student
            var newStudent = new Student
            {
                name = form_data.name,
                age = form_data.age,
                address = form_data.address,
                DepartmentId = form_data.selected_department_id,
                CourseStudents = form_data.course_details.Select(course => new CourseStudents
                {
                    CourseId = course.course_id,
                    Degree = course.Degree
                }).ToList()
            };

            if (form_data.image != null)
            {
                newStudent.image = await FileUtility.SaveFile(form_data.image, "images/students", [".jpg", ".jpeg", ".png"]);
            }

            //Context.Students.Add(newStudent);
            StudentRepo.Create(newStudent);
            try
            {
                await StudentRepo.UploadToDatabaseAsync();
               // await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving student and courses: {ex.InnerException?.Message}");
            }

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            //var target = StudentRepo.GetById(id);
           var target = Context.Students.Find(id);
            var courses = Context.CourseStudents.Where(c => c.StudentId == id).ToList();

            if (courses != null)
            {
                Context.CourseStudents.RemoveRange(courses);

                if (target != null)
                {
                    //StudentRepo.Delete(target);
                    Context.Students.Remove(target); // PROBLEMATIC: Why does this work but not the repo func ??
                    // Delete file in server asset store
                    FileUtility.DeleteFile(target.image);
                }
                //StudentRepo.UploadToDatabase();
                Context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // TODO: Use StudentService
        public ActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return PartialView("_SearchResults", Context.Students.ToList());
            }
            var results = Context.Students
                .Where(e => e.name.Contains(searchTerm))
                .Include(e => e.Department)
                .ToList();
            return PartialView("_SearchResults", results);
        }
    }
}
