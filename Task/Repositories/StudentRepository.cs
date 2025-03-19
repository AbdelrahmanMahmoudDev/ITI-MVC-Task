using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;

namespace Task.Repositories
{
    public class StudentRepository : IRepository<Student>
    {
        private readonly SchoolContext _Context;
        public StudentRepository(SchoolContext Context)
        {
            _Context = Context ?? throw new ArgumentNullException(nameof(Context));
        }
        public void Create(Student obj)
        {
            _Context.Students.Add(obj);
        }

        public void Delete(Student obj)
        {
            if (obj == null)
            {
                throw new InvalidOperationException("Cannot delete a non-existant entity");
            }
            try
            {
                _Context.Students.Remove(obj);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }

        public IEnumerable<Student> GetAll()
        {
            return _Context.Students;
        }

        public IEnumerable<Student> GetAll(List<string> NavProps)
        {
            IQueryable<Student> Students = _Context.Students;
            foreach (var Prop in NavProps)
            {
                Students = Students.Include($"{Prop}");
            }
            return Students;
        }

        public Student GetById(int id)
        {
            Student target = _Context.Students.Where(s => s.StudentId == id).FirstOrDefault();
            if (target == null)
            {
                throw new InvalidOperationException("Cannot retrieve a non-existant entity");
            }
            return target;
        }

        public Student GetById(int id, List<string> NavProps)
        {
            IQueryable<Student> Students = _Context.Students.Where(T => T.StudentId == id);
            foreach (var Prop in NavProps)
            {
                Students = Students.Include($"{Prop}");
            }
            var Target = Students.FirstOrDefault();

            return Target ?? throw new InvalidOperationException("Cannot retrieve a non-existant entity");
        }

        public void Update(Student obj)
        {
            _Context.Update(obj);
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
