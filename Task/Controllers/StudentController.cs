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

namespace Task.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolContext _Context;
        IRepository<Student> _StudentRepo;
        IRepository<Course> _CourseRepo;
        IRepository<Department> _DepRepo;
        IJointRepository<CourseStudents> _StudentCourseRepo;
        public StudentController(SchoolContext Context, IRepository<Student> StudentRepo, IJointRepository<CourseStudents> StudentCourseRepo, IRepository<Course> CourseRepo, IRepository<Department> DepRepo)
        {
            _Context = Context ?? throw new ArgumentNullException(nameof(Context));
            _StudentRepo = StudentRepo ?? throw new ArgumentNullException(nameof(StudentRepo));
            _StudentCourseRepo = StudentCourseRepo ?? throw new ArgumentNullException(nameof(StudentCourseRepo));
            _CourseRepo = CourseRepo ?? throw new ArgumentNullException(nameof(CourseRepo));
            _DepRepo = DepRepo?? throw new ArgumentNullException(nameof(DepRepo));
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
                    courses = _StudentCourseRepo.GetRangeById(id, ["Course"]).ToDictionary(c => c.Course.name, c=> c.Degree),
                };
                return View("Details", StudentModel);
            }
            catch(Exception E)
            {
                Debug.WriteLine(E.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        public IActionResult Edit(int id)
        {
            Student Target = null;
            try
            {
                Target = _StudentRepo.GetById(id, ["Department"]);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

            var CourseData = _StudentCourseRepo.GetRangeById(id, ["Course"]);
            List<CourseDetails> Courses = new List<CourseDetails>();
            foreach(var Course in CourseData)
            {
                Courses.Add(new CourseDetails()
                {
                    course_id = (int)Course.CourseId,
                    CourseName = Course.Course.name,
                    Degree = Course.Degree
                });
            }

            var student = new StudentAddVM()
            {
                id = id,
                name = Target.name,
                age = Target.age,
                address = Target.address,
                ImagePath = Target.image,
                selected_department_id = Target.DepartmentId,
                course_details = Courses,
                departments = _DepRepo.GetAll().ToList(),
                courses = _CourseRepo.GetAll().ToList(),
            };

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(StudentAddVM FormData, int id)
        {
            if (!ModelState.IsValid)
            {
                var CourseData = _StudentCourseRepo.GetRangeById(id, ["Course"]);
                List<CourseDetails> Courses = new List<CourseDetails>();
                foreach (var Course in CourseData)
                {
                    Courses.Add(new CourseDetails()
                    {
                        course_id = (int)Course.CourseId,
                        CourseName = Course.Course.name,
                        Degree = Course.Degree
                    });
                }

                FormData.id = id;
                FormData.age = _StudentRepo.GetById(id).age;
                FormData.departments = _DepRepo.GetAll().ToList();
                FormData.courses = _CourseRepo.GetAll().ToList();
                FormData.course_details = Courses;
                return View("Edit", FormData);
            }

            var curr_student = _StudentRepo.GetById(id, ["CourseStudents"]);

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
                        _StudentCourseRepo.Delete(Course);
                    }
                }

                foreach (var NewCourse in FormData.course_details)
                {
                    _StudentCourseRepo.Create(new CourseStudents()
                    {
                        StudentId = id,
                        CourseId = NewCourse.course_id,
                        Degree = NewCourse.Degree,
                    });
                }
            }

            await _StudentRepo.UploadToDatabaseAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            var departments = _DepRepo.GetAll().ToList();
            var courses = _CourseRepo.GetAll().ToList();

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

            // We assume each student MUST have atleast one course
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

            _StudentRepo.Create(newStudent);
            try
            {
                await _StudentRepo.UploadToDatabaseAsync();
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
            var courses = _StudentCourseRepo.GetRangeById(id).ToList();

            using IDbContextTransaction Transaction = await _Context.Database.BeginTransactionAsync();
            try
            {
                if (courses.Count > 0)
                {
                    _StudentCourseRepo.DeleteRange(courses);
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

        public ActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return PartialView("_SearchResults", _StudentRepo.GetAll().ToList()); ;
            }
            var results = _StudentRepo.GetBySubString(searchTerm, ["Department"]);
            return PartialView("_SearchResults", results);
        }
    }
}
