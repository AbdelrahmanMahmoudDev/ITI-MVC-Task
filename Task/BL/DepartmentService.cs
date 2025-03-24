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

        public async System.Threading.Tasks.Task DeleteAsync(int Id)
        {
            Department TargetDepartment = _UnitOfWork.Departments.GetById(Id);

            if(TargetDepartment == null)
            {
                Debug.WriteLine($"Entity with Id {Id} does not exist.");
                throw new InvalidOperationException();
            }

            try
            {
                _UnitOfWork.Departments.Delete(TargetDepartment);
                await _UnitOfWork.Departments.UploadToDatabaseAsync();
            }
            catch(Exception)
            {
                Debug.WriteLine($"Deleting entity {Id} failed.");
                throw new InvalidOperationException();
            }
        }

        public IEnumerable<Department> PrepareDashboard()
        {
            IEnumerable<Department> Result = _UnitOfWork.Departments.GetAll();

            return Result;
        }

        public async System.Threading.Tasks.Task Update(DepartmentVM Data, int Id)
        {
            Department TargetDepartment = _UnitOfWork.Departments.GetById(Id);

            if(TargetDepartment == null)
            {
                Debug.WriteLine($"An entity with Id {Id} does not exist");
                throw new NullReferenceException();
            }

            TargetDepartment.name = Data.name;
            TargetDepartment.description = Data.description;
            TargetDepartment.location = Data.location;

            try
            {
                _UnitOfWork.Departments.Update(TargetDepartment);
                await _UnitOfWork.Departments.UploadToDatabaseAsync();
            }
            catch(Exception)
            {
                Debug.WriteLine($"Updating Entity {Id} failed");
                throw new InvalidOperationException();
            }
        }

        public DepartmentVM PrepareEdit(int Id)
        {
            Department TargetDepartment = _UnitOfWork.Departments.GetById(Id);

            if(TargetDepartment == null)
            {
                Debug.WriteLine($"Entity with given Id: {Id} does not exist.");

                throw new NullReferenceException($"Entity with given Id: {Id} does not exist.");
            }

            DepartmentVM TargetViewModel = new DepartmentVM()
            {
                Id = TargetDepartment.DepartmentId,
                name = TargetDepartment.name,
                description = TargetDepartment.description,
                location = TargetDepartment.location
            };

            return TargetViewModel;
        }
    }
}
