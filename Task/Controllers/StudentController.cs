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
        private readonly SchoolContext Context;
        StudentRepository _StudentRepo;
        public StudentController(SchoolContext Context, StudentRepository StudentRepo)
        {
            this.Context = Context ?? throw new ArgumentNullException(nameof(Context));
            _StudentRepo = StudentRepo ?? throw new ArgumentNullException(nameof(StudentRepo));
        }
        public IActionResult Index()
        {
            var Students = _StudentRepo.GetAll(new List<string> { "Department" }).ToList();

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
            Student Target = null;
            try
            {
                Target = _StudentRepo.GetById(id, new List<string> { "Department" });
            }
            catch (InvalidOperationException e)
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
                course_details = Context.CourseStudents
                    .Where(crs => crs.StudentId == id)
                    .Include(crs => crs.Course)
                    .Select(crs => new CourseDetails() { course_id = (int)crs.CourseId, CourseName = crs.Course.name, Degree = crs.Degree }).ToList(),
                departments = Context.Departments.ToList(),
                courses = Context.Courses.ToList(),
            };

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(StudentAddVM FormData, int id)
        {
            if (!ModelState.IsValid)
            {
                FormData.id = id;
                FormData.age = _StudentRepo.GetById(id).age;
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

            var curr_student = _StudentRepo.GetById(id, new List<string> { "CourseStudents" });

            if (curr_student == null)
            {
                return NotFound("Student does not exist.");
            }

            curr_student.name = FormData.name;
            if (FormData.image != null)
            {
                FileUtility.DeleteFile(curr_student.image);
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

            await _StudentRepo.UploadToDatabaseAsync();
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
            _StudentRepo.Create(newStudent);
            try
            {
                await _StudentRepo.UploadToDatabaseAsync();
                // await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving student and courses: {ex.InnerException?.Message}");
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var target = _StudentRepo.GetById(id);
            var courses = Context.CourseStudents.Where(c => c.StudentId == id).ToList();

            using var Transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                if (courses.Any())
                {
                    Context.CourseStudents.RemoveRange(courses);
                    _StudentRepo.UploadToDatabase();
                }
                if (target != null)
                {
                    _StudentRepo.Delete(target);
                    // Delete image in server asset store
                    FileUtility.DeleteFile(target.image);
                    _StudentRepo.UploadToDatabase();
                }
                await Transaction.CommitAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error saving student and courses: {e.InnerException?.Message}");
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
