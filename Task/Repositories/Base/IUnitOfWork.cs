﻿using Microsoft.EntityFrameworkCore.Storage;
using Task.Models;

namespace Task.Repositories.Base
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Course> Courses { get; }
        IRepository<Department> Departments { get; }
        IRepository<Student> Students { get; }
        IRepository<Instructor> Instructors { get; }
        IJointRepository<CourseStudents> CourseStudents { get; }

        int CommitChanges();
        System.Threading.Tasks.ValueTask<IDbContextTransaction> BeginTransaction();
    }
}
