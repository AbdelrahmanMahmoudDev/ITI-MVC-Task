using Task.Contexts;
using Task.Models;
using Task.Repositories.Base;
using Task.Utilities;
using Task.ViewModels;
using Task.ViewModels.Instructor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Task.BL
{
    public class InstructorService : IInstructorService<InstructorVM>
    {
        private readonly SchoolContext _Context;
        private readonly IUnitOfWork _UnitOfWork;
        public InstructorService(IUnitOfWork UnitOfWork, SchoolContext Context)
        {
            _UnitOfWork = UnitOfWork;
            _Context = Context;
        }

        public IEnumerable<Instructor> PrepareDashboard()
        {
            return _UnitOfWork.Instructors.GetAll(["Department"]);
        }

        public InstructorVM PrepareDetailsPage(int Id)
        {
            var Instructor = _UnitOfWork.Instructors.GetById(Id);

            if (Instructor == null)
            {
                throw new InvalidOperationException();
            }

            InstructorVM InstructorModel = new InstructorVM()
            {
                fname = Instructor.fname,
                lname = Instructor.lname,
                full_name = Instructor.fname + " " + Instructor.lname,
                image_path = Instructor.image,
                HireDate = new DateTime(Instructor.HireDate.Value.Year, Instructor.HireDate.Value.Month, Instructor.HireDate.Value.Day),
                salary = Instructor.salary,
                age = Instructor.age,
                course_names = _Context.Courses
                    .Where(c => c.Instructor.InstructorId == Id)
                    .Select(c => c.name)
                    .ToList(),
            };

            return InstructorModel;
        }

        public InstructorVM PrepareAddForm()
        {
            InstructorVM PrepData = new InstructorVM()
            {
                AvailableCourses = _UnitOfWork.Courses.GetAll().ToList(),
                AvailableDepartments = _UnitOfWork.Departments.GetAll().ToList(),
            };

            return PrepData;
        }

        public async System.Threading.Tasks.Task AddInstructor(InstructorVM FormData)
        {
            Instructor NewGuy = new Instructor()
            {
                fname = FormData.fname,
                lname = FormData.lname,
                salary = FormData.salary,
                age = FormData.age,
                HireDate = new DateTime(FormData.HireDate.Year, FormData.HireDate.Month, FormData.HireDate.Day),
                DepartmentId = FormData.SelectedDepartmentId,
            };

            try
            {
                if (FormData.ImageFile != null && FormData.ImageFile.Length > 0)
                {
                    NewGuy.image = await FileUtility.SaveFile(FormData.ImageFile, "images/instructors", [".jpg", ".jpeg", ".png"]);
                }
            }
            catch (Exception fileEx)
            {
                throw new Exception($"File upload failed: {fileEx.Message}");
            }

            await using var Transaction = await _UnitOfWork.BeginTransaction();
            try
            {
                _UnitOfWork.Instructors.Create(NewGuy);
                await _UnitOfWork.Instructors.UploadToDatabaseAsync();

                Course ChosenCourse = _UnitOfWork.Courses.GetById(FormData.SelectedCourseId);
                ChosenCourse.InstructorId = NewGuy.InstructorId;
                await _UnitOfWork.Courses.UploadToDatabaseAsync();

                await Transaction.CommitAsync();
            }
            catch (Exception E)
            {
                throw new Exception($"There was an error uploading to database\n{E.Message}");
            }
        }

        public async System.Threading.Tasks.Task RemoveInstructor(int Id)
        {
            var target = _UnitOfWork.Instructors.GetById(Id);
            if (target != null)
            {
                _UnitOfWork.Instructors.Delete(target);
            }

            var referencing_courses = _Context.Courses.Where(c => c.InstructorId == Id).ToList();
            foreach (var crs in referencing_courses)
            {
                crs.InstructorId = null;
            }

            await _UnitOfWork.Courses.UploadToDatabaseAsync();
        }

        public async System.Threading.Tasks.Task EditInstructor(InstructorVM form_data)
        {
            Instructor relevant_instructor = _UnitOfWork.Instructors.GetById(form_data.Id);

            relevant_instructor.salary = form_data.salary;
            relevant_instructor.DepartmentId = form_data.SelectedDepartmentId;

            Course relevant_course = _UnitOfWork.Courses.GetById(form_data.SelectedCourseId); 

            relevant_course.InstructorId = form_data.Id;

            await _UnitOfWork.Courses.UploadToDatabaseAsync();
        }

        public InstructorVM PrepareEditPage(int Id)
        {
            List<Course> RequiredData = _UnitOfWork.Courses.GetAll(["Instructor", "Instructor.Department"]).ToList();

            Instructor TargetInstructor = _UnitOfWork.Instructors.GetById(Id);

            Course RegisteredCourse = RequiredData.Find(D => D.InstructorId == Id);

            InstructorVM Result = new InstructorVM()
            {
                Id = TargetInstructor.InstructorId,
                fname = TargetInstructor.fname,
                lname = TargetInstructor.lname,
                age = TargetInstructor.age,
                full_name = TargetInstructor.fname + " " + TargetInstructor.lname,
                image_path = TargetInstructor.image,
                HireDate = new DateTime(TargetInstructor.HireDate.Value.Year, TargetInstructor.HireDate.Value.Month,
                TargetInstructor.HireDate.Value.Day),
                salary = TargetInstructor.salary,
                SelectedDepartmentId = TargetInstructor.DepartmentId,
                SelectedCourseId = RegisteredCourse.CourseId,
                AvailableCourses = _UnitOfWork.Courses.GetAll().ToList(),
                AvailableDepartments = _UnitOfWork.Departments.GetAll().ToList()
            };

            return Result;
        }
    }
}
