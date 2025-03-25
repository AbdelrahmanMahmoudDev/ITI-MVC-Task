using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;

namespace Task.Repositories
{
    public class InstructorRepository : IRepository<Instructor>
    {
        private readonly SchoolContext _Context;
        public InstructorRepository(SchoolContext Context)
        {
            _Context = Context ?? throw new ArgumentNullException(nameof(Context));
        }


        public void Create(Instructor obj)
        {
            try
            {
                _Context.Add(obj);
            }
            catch(Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        public void Delete(Instructor obj)
        {
            try
            {
                _Context.Remove(obj);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        public IEnumerable<Instructor> GetAll()
        {
            return _Context.Instructors;
        }

        public IEnumerable<Instructor> GetAll(List<string> NavProps)
        {
            IQueryable<Instructor> Result = _Context.Instructors;

            foreach(var Prop in NavProps)
            {
                Result = Result.Include(Prop);
            }

            return Result;
        }

        public Instructor GetById(int id)
        {
            Instructor Target = _Context.Instructors.FirstOrDefault(I => I.InstructorId == id);

            if(Target == null)
            {
                throw new NullReferenceException();
            }

            return Target;
        }

        public Instructor GetById(int id, List<string> NavProps)
        {
            IQueryable<Instructor> Instructors = _Context.Instructors;

            foreach(var Prop in NavProps)
            {
                Instructors = Instructors.Include(Prop);
            }

            Instructor Target = Instructors.FirstOrDefault(I => I.InstructorId == id);

            if(Target == null)
            {
                throw new NullReferenceException();
            }

            return Target;
        }

        public IEnumerable<Instructor> GetBySubString(string SubString)
        {
            IEnumerable<Instructor> Target = _Context.Instructors.Where(I => I.fname.Contains(SubString)
                                                                          || I.lname.Contains(SubString));

            return Target;
        }

        public IEnumerable<Instructor> GetBySubString(string SubString, List<string> NavProps)
        {
            IQueryable<Instructor> Instructors = _Context.Instructors;

            foreach(var Prop in NavProps)
            {
                Instructors = Instructors.Include(Prop);
            }

            IEnumerable<Instructor> Target = Instructors.Where(I => I.fname.Contains(SubString)
                                                              || I.lname.Contains(SubString));

            return Target;
        }

        public void Update(Instructor obj)
        {
            try
            {
                _Context.Update(obj);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        public void UploadToDatabase()
        {
            try
            {
                _Context.SaveChanges();
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        public async System.Threading.Tasks.Task UploadToDatabaseAsync()
        {
            try
            {
                await _Context.SaveChangesAsync();
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }
    }
}
