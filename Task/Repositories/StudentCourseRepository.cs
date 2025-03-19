using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;

namespace Task.Repositories
{
    public class StudentCourseRepository : IJointRepository<CourseStudents>
    {
        private readonly SchoolContext _Context;
        public StudentCourseRepository(SchoolContext Context)
        {
            _Context = Context ?? throw new ArgumentNullException(nameof(Context));
        }

        public void Create(CourseStudents obj)
        {
            try
            {
                _Context.Add(obj);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public void Delete(CourseStudents obj)
        {
            try
            {
                _Context.Remove(obj);
            }
            catch(Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public void DeleteRange(IEnumerable<CourseStudents> obj)
        {
            try
            {
                _Context.RemoveRange(obj);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public IEnumerable<CourseStudents> GetAll()
        {
            return _Context.CourseStudents;
        }

        public IEnumerable<CourseStudents> GetAll(List<string> NavProps)
        {
            IQueryable<CourseStudents> Result = null;
            try
            {
                Result = _Context.CourseStudents;
                foreach (var Prop in NavProps)
                {
                    Result = Result.Include(Prop);
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result;
        }

        public CourseStudents GetById(int id)
        {
            CourseStudents Result = null;
            try
            {
                Result = _Context.CourseStudents.Where(Crs => Crs.StudentId == id).FirstOrDefault();
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result;
        }

        public CourseStudents GetById(int id, List<string> NavProps)
        {
            IQueryable <CourseStudents> Result = null;
            try
            {
                Result = _Context.CourseStudents.Where(Crs => Crs.StudentId == id);
                foreach (var Prop in NavProps)
                {
                    Result = Result.Include(Prop);
                }
                //Result = Result.Where(Crs => Crs.StudentId == id);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result.FirstOrDefault();
        }

        public IEnumerable<CourseStudents> GetRangeById(int id)
        {
            IEnumerable<CourseStudents> Result = null;
            try
            {
                Result = _Context.CourseStudents.Where(Crs => Crs.StudentId == id);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result;
        }

        public IEnumerable<CourseStudents> GetRangeById(int id, List<string> NavProps)
        {
            IQueryable<CourseStudents> Result = null;
            try
            {
                Result = _Context.CourseStudents.Where(Crs => Crs.StudentId == id);
                foreach (var Prop in NavProps)
                {
                    Result = Result.Include(Prop);
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result;
        }

        public void Update(CourseStudents obj)
        {
            try
            {
                _Context.Update(obj);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public void UploadToDatabase()
        {
            try
            {
                _Context.SaveChanges();
            }
            catch(Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public async System.Threading.Tasks.Task UploadToDatabaseAsync()
        {
            try
            {
                await _Context.SaveChangesAsync();
            }
            catch(Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }
    }
}
