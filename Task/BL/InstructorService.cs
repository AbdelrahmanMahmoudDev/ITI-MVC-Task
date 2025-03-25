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
            throw new NotImplementedException();
            //var instructor = context.Instructors
            //            .Where(i => i.InstructorId == id)
            //            .Select(i => new InstructorVM
            //            {
            //                fname = i.fname,
            //                lname = i.lname,
            //                full_name = i.fname + " " + i.lname,
            //                image_path = i.image,
            //                hire_date = new DateTime(i.HireDate.Value.Year, i.HireDate.Value.Month, i.HireDate.Value.Day),
            //                salary = i.salary,
            //                age = i.age
            //            })
            //            .FirstOrDefault();

            //if (instructor != null)
            //{
            //    instructor.course_names = context.Courses
            //        .Where(c => c.Instructor.InstructorId == id)
            //        .Select(c => c.name)
            //        .ToList();
            //}

            //var Instructor = _UnitOfWork.Instructors.GetById(Id);

            //if(Instructor == null)
            //{
            //    throw new InvalidOperationException();
            //}

            //InstructorVM InstructorModel = new InstructorVM()
            //{
            //    fname = Instructor.fname,
            //    lname = Instructor.lname,
            //    full_name = Instructor.fname + " " + Instructor.lname,
            //    imahe_path = i
            //};
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
                _UnitOfWork.Instructors.UploadToDatabase();

                Course ChosenCourse = _UnitOfWork.Courses.GetById(FormData.SelectedCourseId);
                ChosenCourse.InstructorId = NewGuy.InstructorId;
                _UnitOfWork.Courses.UploadToDatabase();

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
    }
}
