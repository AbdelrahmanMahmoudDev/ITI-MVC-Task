using Microsoft.EntityFrameworkCore.Storage;
using Task.Contexts;
using Task.Models;

namespace Task.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchoolContext _Context;
        public UnitOfWork(SchoolContext Context)
        {
            _Context = Context;
            Courses = new CourseRepository(_Context);
            Departments = new DepartmentRepository(_Context);
            Students = new StudentRepository(_Context);
            CourseStudents = new StudentCourseRepository(_Context);
        }

        public IRepository<Course> Courses { get; private set; }

        public IRepository<Department> Departments { get; private set; }

        public IRepository<Student> Students { get; private set; }

        public IRepository<Instructor> Instructors => throw new NotImplementedException();

        public IJointRepository<CourseStudents> CourseStudents { get; private set; }

        public async System.Threading.Tasks.ValueTask<System.Data.Common.DbTransaction> BeginTransaction()
        {
            return (System.Data.Common.DbTransaction)await _Context.Database.BeginTransactionAsync();
        }

        public int CommitChanges()
        {
            return _Context.SaveChanges();
        }
    }
}
