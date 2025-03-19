using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;

namespace Task.Repositories
{
    public class CourseRepository : IRepository<Course>
    {
        private readonly SchoolContext _Context;
        public CourseRepository(SchoolContext Context)
        {
            _Context = Context ?? throw new ArgumentNullException(nameof(Course));
        }
        public void Create(Course obj)
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

        public void Delete(Course obj)
        {
            try
            {
                _Context.Remove(obj);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public IEnumerable<Course> GetAll()
        {
            return _Context.Courses;
        }

        public IEnumerable<Course> GetAll(List<string> NavProps)
        {
            IQueryable<Course> Result = _Context.Courses;
            foreach(var Prop in NavProps)
            {
                Result = Result.Include(Prop);
            }
            return Result;
        }

        public Course GetById(int id)
        {
            Course Result = null;
            try
            {
                Result = _Context.Courses.Where(Crs => Crs.CourseId == id).FirstOrDefault();
                if(Result == null)
                {
                    throw new InvalidOperationException("Cannot retrieve non-existant entity");
                }
            }
            catch(Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result;
        }

        public Course GetById(int id, List<string> NavProps)
        {
            IQueryable<Course> Result = null;
            try
            {
                Result = _Context.Courses.Where(Crs => Crs.CourseId == id);
                if (Result == null)
                {
                    throw new InvalidOperationException("Cannot retrieve non-existant entity");
                }
                foreach(var Prop in NavProps)
                {
                    Result = Result.Include(Prop);
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result.FirstOrDefault();
        }

        public IEnumerable<Course> GetBySubString(string SubString)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Course> GetBySubString(string SubString, List<string> NavProps)
        {
            throw new NotImplementedException();
        }

        public void Update(Course obj)
        {
            try
            {
                _Context.Update(obj);
            }
            catch(Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public void UploadToDatabase()
        {
            _Context.SaveChanges();
        }

        public async System.Threading.Tasks.Task UploadToDatabaseAsync()
        {
            await _Context.SaveChangesAsync();
        }
    }
}
