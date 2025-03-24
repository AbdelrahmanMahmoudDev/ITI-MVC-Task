using System.Diagnostics;
using Task.Models;
using Task.Repositories.Base;
using Task.ViewModels;

namespace Task.BL
{
    public class DepartmentService : IDepartmentService<DepartmentVM>
    {
        private readonly IUnitOfWork _UnitOfWork;
        public DepartmentService(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork ?? throw new ArgumentNullException(nameof(UnitOfWork));
        }

        public async System.Threading.Tasks.Task CreateAsync(DepartmentVM Data)
        {
            Department NewDepartment = new Department
            {
                name = Data.name,
                description = Data.description,
                location = Data.location,
            };


            try
            {
                _UnitOfWork.Departments.Create(NewDepartment);
                await _UnitOfWork.Departments.UploadToDatabaseAsync();
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        public System.Threading.Tasks.Task DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Department> PrepareDashboard()
        {
            IEnumerable<Department> Result = _UnitOfWork.Departments.GetAll();

            return Result;
        }

        public System.Threading.Tasks.Task Update(DepartmentVM Data, int Id)
        {
            throw new NotImplementedException();
        }
    }
}
