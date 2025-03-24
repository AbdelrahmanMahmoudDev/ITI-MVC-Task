using Task.Models;

namespace Task.BL
{
    public interface IDepartmentService<ViewModel>
    {
        public IEnumerable<Department> PrepareDashboard();
        public System.Threading.Tasks.Task CreateAsync(ViewModel Data);
        public System.Threading.Tasks.Task Update(ViewModel Data, int Id);
        public System.Threading.Tasks.Task DeleteAsync(int Id);
    }
}
