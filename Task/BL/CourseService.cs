using System.Diagnostics;
using Task.Models;
using Task.Repositories.Base;
using Task.ViewModels;

namespace Task.BL
{
    public class CourseService : ICourseService<CourseVM>
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CourseService(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork ?? throw new ArgumentNullException(nameof(UnitOfWork));
        }
        public async System.Threading.Tasks.Task CreateAsync(CourseVM Data)
        {
            Course NewCourse = new Course
            {
                name = Data.Name,
                topic = Data.Topic,
                MinimumDegree = Data.MinimumDegree
            };

            try
            {
                _UnitOfWork.Courses.Create(NewCourse);
                await _UnitOfWork.Courses.UploadToDatabaseAsync();
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        public async System.Threading.Tasks.Task DeleteAsync(int Id)
        {
            Course TargetCourse = _UnitOfWork.Courses.GetById(Id);

            if (TargetCourse == null)
            {
                Debug.WriteLine($"Entity with Id {Id} does not exist.");
                throw new InvalidOperationException();
            }

            try
            {
                _UnitOfWork.Courses.Delete(TargetCourse);
                await _UnitOfWork.Courses.UploadToDatabaseAsync();
            }
            catch (Exception)
            {
                Debug.WriteLine($"Deleting entity {Id} failed.");
                throw new InvalidOperationException();
            }
        }

        public IEnumerable<Course> PrepareDashboard()
        {
            IEnumerable<Course> Result = _UnitOfWork.Courses.GetAll();

            return Result;
        }

        public CourseVM PrepareEdit(int Id)
        {
            Course TargetCourse = _UnitOfWork.Courses.GetById(Id);

            if (TargetCourse == null)
            {
                Debug.WriteLine($"Entity with given Id: {Id} does not exist.");

                throw new NullReferenceException($"Entity with given Id: {Id} does not exist.");
            }

            CourseVM TargetViewModel = new CourseVM()
            {
                Id = TargetCourse.CourseId,
                Name = TargetCourse.name,
                Topic = TargetCourse.topic,
                MinimumDegree = TargetCourse.MinimumDegree,
            };

            return TargetViewModel;
        }

        public async System.Threading.Tasks.Task UpdateAsync(CourseVM Data, int Id)
        {
            Course TargetCourse = _UnitOfWork.Courses.GetById(Id);

            if (TargetCourse == null)
            {
                Debug.WriteLine($"An entity with Id {Id} does not exist");
                throw new NullReferenceException();
            }

            TargetCourse.name = Data.Name;
            TargetCourse.topic = Data.Topic;
            TargetCourse.MinimumDegree = Data.MinimumDegree;

            try
            {
                _UnitOfWork.Courses.Update(TargetCourse);
                await _UnitOfWork.Courses.UploadToDatabaseAsync();
            }
            catch (Exception)
            {
                Debug.WriteLine($"Updating Entity {Id} failed");
                throw new InvalidOperationException();
            }
        }
    }
}
