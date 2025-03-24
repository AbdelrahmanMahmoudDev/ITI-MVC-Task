using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;

namespace Task.Repositories
{
    public class DepartmentRepository : IRepository<Department>
    {
        private readonly SchoolContext _Context;
        public DepartmentRepository(SchoolContext Context)
        {
            _Context =  Context ?? throw new ArgumentNullException(nameof(Context));
        }
        public void Create(Department obj)
        {
            try
            {
                _Context.Add(obj);
            }
            catch(Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }

        public void Delete(Department obj)
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

        public IEnumerable<Department> GetAll()
        {
            return _Context.Departments;
        }

        public IEnumerable<Department> GetAll(List<string> NavProps)
        {
            IQueryable<Department> Result = _Context.Departments;
            foreach (var Prop in NavProps)
            {
                Result = Result.Include(Prop);
            }
            return Result;
        }

        public Department GetById(int id)
        {
            Department Result = null;
            try
            {
                Result = _Context.Departments.Where(Dep => Dep.DepartmentId == id).FirstOrDefault();
                if (Result == null)
                {
                    throw new InvalidOperationException("Cannot retrieve non-existant entity");
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            return Result;
        }

        public Department GetById(int id, List<string> NavProps)
        {
            IQueryable<Department> Result = null;
            try
            {
                Result = _Context.Departments.Where(Dep => Dep.DepartmentId == id);
                if (Result == null)
                {
                    throw new InvalidOperationException("Cannot retrieve non-existant entity");
                }
                foreach (var Prop in NavProps)
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

        public Department GetBySubString(string SubString)
        {
            throw new NotImplementedException();
        }

        public Department GetBySubString(string SubString, List<string> NavProps)
        {
            throw new NotImplementedException();
        }

        public void Update(Department obj)
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
            _Context.SaveChanges();
        }

        public async System.Threading.Tasks.Task UploadToDatabaseAsync()
        {
            await _Context.SaveChangesAsync();
        }

        IEnumerable<Department> IRepository<Department>.GetBySubString(string SubString)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Department> IRepository<Department>.GetBySubString(string SubString, List<string> NavProps)
        {
            throw new NotImplementedException();
        }
    }
}
