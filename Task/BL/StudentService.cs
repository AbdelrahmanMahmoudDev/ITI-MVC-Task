using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Task.Models;
using Task.Repositories;
using Task.Utilities;
using Task.ViewModels.Student;
using Task.Contexts;
using System.Diagnostics;
using Microsoft.SqlServer.Server;
using Task.Repositories.Base;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Task.BL
{
    public class StudentService : IStudentService<StudentAddVM>
    {
        private readonly IUnitOfWork _UnitOfWork;

        public StudentService(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork ?? throw new ArgumentNullException(nameof(UnitOfWork));
        }
        public async System.Threading.Tasks.Task CreateAsync(StudentAddVM Data)
        {
            // We assume each student MUST have atleast one course
            Student NewStudent = new Student
            {
                name = Data.name,
                age = Data.age,
                address = Data.address,
                DepartmentId = Data.selected_department_id,
                CourseStudents = Data.course_details.Select(course => new CourseStudents
                {
                    CourseId = course.course_id,
                    Degree = course.Degree
                }).ToList()
            };

            try
            {
                if (Data.image != null && Data.image.Length > 0)
                {
                    NewStudent.image = await FileUtility.SaveFile(Data.image, "images/students", [".jpg", ".jpeg", ".png"]);
                }
            }
            catch (Exception fileEx)
            {
                throw new Exception($"File upload failed: {fileEx.Message}");
            }


            _UnitOfWork.Students.Create(NewStudent);
            try
            {
                await _UnitOfWork.Students.UploadToDatabaseAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error uploading to database\n{ex.Message}");
            }
        }

        public async System.Threading.Tasks.Task DeleteAsync(int Id)
        {
            var Target = _UnitOfWork.Students.GetById(Id);
            var Courses = _UnitOfWork.CourseStudents.GetRangeById(Id).ToList();

            using var Transaction = await _UnitOfWork.BeginTransaction();
            try
            {
                if (Courses.Count > 0)
                {
                    _UnitOfWork.CourseStudents.DeleteRange(Courses);
                    _UnitOfWork.CourseStudents.UploadToDatabase();
                }
                if (Target != null)
                {
                    _UnitOfWork.Students.Delete(Target);
                    // Delete image in server asset store
                    FileUtility.DeleteFile(Target.image);
                    _UnitOfWork.Students.UploadToDatabase();
                }
                await Transaction.CommitAsync();
            }
            catch (Exception E)
            {
                throw new Exception($"There was an error uploading to database\n{E.Message}");
            }
        }

        public async System.Threading.Tasks.Task Update(StudentAddVM FormData, int Id)
        {
            var curr_student = _UnitOfWork.Students.GetById(Id, ["CourseStudents"]);

            curr_student.name = FormData.name;


            try
            {
                if (FormData.image != null && FormData.image.Length > 0)
                {
                    FileUtility.DeleteFile(curr_student.image);
                    curr_student.image = await FileUtility.SaveFile(FormData.image, "images/students", [".jpg", ".jpeg", ".png"]);
                }
            }
            catch (Exception fileEx)
            {
                throw new Exception($"File upload failed: {fileEx.Message}");
            }

            curr_student.address = FormData.address;
            curr_student.DepartmentId = FormData.selected_department_id;
            curr_student.address = FormData.address;
            if (FormData.course_details.Any())
            {
                var ExistingCourses = curr_student.CourseStudents;
                foreach (var Course in ExistingCourses)
                {
                    if (Course.StudentId == Id)
                    {
                        _UnitOfWork.CourseStudents.Delete(Course);
                    }
                }

                foreach (var NewCourse in FormData.course_details)
                {
                    _UnitOfWork.CourseStudents.Create(new CourseStudents()
                    {
                        StudentId = Id,
                        CourseId = NewCourse.course_id,
                        Degree = NewCourse.Degree,
                    });
                }
            }

            await _UnitOfWork.Students.UploadToDatabaseAsync();
        }

        public IEnumerable<Student> GetBySearch(string SearchTerm)
        {
            IEnumerable<Student> Target = _UnitOfWork.Students.GetBySubString(SearchTerm, ["Department"]);
            if (Target.ToList().Count <= 0)
            {
                return _UnitOfWork.Students.GetAll();
            }
            return Target;
        }

        public StudentAddVM PrepareCreateForm()
        {
            var Departments = _UnitOfWork.Departments.GetAll().ToList();
            var Courses = _UnitOfWork.Courses.GetAll().ToList();

            StudentAddVM DepartmentsAndCourses = new StudentAddVM()
            {
                departments = Departments,
                courses = Courses,
            };

            return DepartmentsAndCourses;
        }

        public StudentAddVM PrepareEditForm(int Id)
        {
            Student Target = null;
            try
            {
                Target = _UnitOfWork.Students.GetById(Id, ["Department"]);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine($"Error: {e.Message}");
            }

            var CourseData = _UnitOfWork.CourseStudents.GetRangeById(Id, ["Course"]);
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

            var student = new StudentAddVM()
            {
                id = Id,
                name = Target.name,
                age = Target.age,
                address = Target.address,
                image_path = Target.image,
                selected_department_id = Target.DepartmentId,
                course_details = Courses,
                departments = _UnitOfWork.Departments.GetAll().ToList(),
                courses = _UnitOfWork.Courses.GetAll().ToList(),
            };

            return student;
        }

        public IEnumerable<Student> PrepareDashboardData()
        {
            return _UnitOfWork.Students.GetAll(["Department"]);
        }

        public StudentAddVM PrepareDetails(int Id)
        {
            var CourseData = _UnitOfWork.CourseStudents.GetRangeById(Id, ["Course"]);
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

            var Student = _UnitOfWork.CourseStudents.GetById(Id, ["Student", "Course", "Student.Department"]);
            StudentAddVM StudentModel = new StudentAddVM()
            {
                name = Student.Student.name,
                image_path = Student.Student.image,
                age = Student.Student.age,
                address = Student.Student.address,
                dept_name = Student.Student.Department.name,
                course_min_degree = Student.Course.MinimumDegree,
                course_details = Courses
            };

            return StudentModel;
        }
    }
}
