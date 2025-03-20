using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Task.Models;
using Task.Repositories;
using Task.Utilities;
using Task.ViewModels.Student;
using Task.Contexts;
using System.Diagnostics;
using Microsoft.SqlServer.Server;

namespace Task.BL
{
    public class StudentService : IService<StudentAddVM, Student>
    {
        private readonly SchoolContext _Context;
        private readonly IRepository<Student> _StudentRepo;
        private readonly IRepository<Department> _DepRepo;
        private readonly IRepository<Course> _CourseRepo;
        private readonly IJointRepository<CourseStudents> _StudCourseRepo;
        
        public StudentService(SchoolContext Context, IRepository<Student> StudentRepo, IRepository<Department> DepRepo, IRepository<Course> CourseRepo, IJointRepository<CourseStudents> StudCourseRepo)
        {
            _Context = Context ?? throw new ArgumentNullException(nameof(Context));
            _StudentRepo = StudentRepo ?? throw new ArgumentNullException(nameof(StudentRepo));
            _DepRepo = DepRepo ?? throw new ArgumentNullException(nameof(DepRepo));
            _CourseRepo = CourseRepo ?? throw new ArgumentNullException(nameof(CourseRepo));
            _StudCourseRepo = StudCourseRepo ?? throw new ArgumentNullException(nameof(StudentRepo));
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

            if (Data.image != null)
            {
                NewStudent.image = await FileUtility.SaveFile(Data.image, "images/students", [".jpg", ".jpeg", ".png"]);
            }

            _StudentRepo.Create(NewStudent);
            try
            {
                await _StudentRepo.UploadToDatabaseAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error uploading to database\n{ex.Message}");
            }
        }

        public async System.Threading.Tasks.Task DeleteAsync(int Id)
        {
            var Target = _StudentRepo.GetById(Id);
            var Courses = _StudCourseRepo.GetRangeById(Id).ToList();

            using IDbContextTransaction Transaction = await _Context.Database.BeginTransactionAsync();
            try
            {
                if (Courses.Count > 0)
                {
                    _StudCourseRepo.DeleteRange(Courses);
                    _StudCourseRepo.UploadToDatabase();
                }
                if (Target != null)
                {
                    _StudentRepo.Delete(Target);
                    // Delete image in server asset store
                    FileUtility.DeleteFile(Target.image);
                    _StudentRepo.UploadToDatabase();
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
            var curr_student = _StudentRepo.GetById(Id, ["CourseStudents"]);

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
                    if (Course.StudentId == Id)
                    {
                        _StudCourseRepo.Delete(Course);
                    }
                }

                foreach (var NewCourse in FormData.course_details)
                {
                    _StudCourseRepo.Create(new CourseStudents()
                    {
                        StudentId = Id,
                        CourseId = NewCourse.course_id,
                        Degree = NewCourse.Degree,
                    });
                }
            }

            await _StudentRepo.UploadToDatabaseAsync();
        }

        public IEnumerable<Student> GetBySearch(string SearchTerm)
        {
            IEnumerable<Student> Target = _StudentRepo.GetBySubString(SearchTerm, ["Department"]);
            if(Target.ToList().Count <= 0)
            {
                return _StudentRepo.GetAll();
            }
            return Target;
        }

        public StudentAddVM PrepareCreateForm()
        {
            var Departments = _DepRepo.GetAll().ToList();
            var Courses = _CourseRepo.GetAll().ToList();

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
                Target = _StudentRepo.GetById(Id, ["Department"]);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine($"Error: {e.Message}");
            }

            var CourseData = _StudCourseRepo.GetRangeById(Id, ["Course"]);
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
                ImagePath = Target.image,
                selected_department_id = Target.DepartmentId,
                course_details = Courses,
                departments = _DepRepo.GetAll().ToList(),
                courses = _CourseRepo.GetAll().ToList(),
            };

            return student;
        }
    }
}
