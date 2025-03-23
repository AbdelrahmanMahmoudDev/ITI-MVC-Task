using Task.Models;

namespace Task.BL
{
    public interface IStudentService<ViewModel>
    {
        public System.Threading.Tasks.Task CreateAsync(ViewModel Data);
        public System.Threading.Tasks.Task Update(ViewModel Data, int Id);
        public System.Threading.Tasks.Task DeleteAsync(int Id);
        public IEnumerable<Student> GetBySearch(string SearchTerm);
        public IEnumerable<Student> PrepareDashboardData();
        public ViewModel PrepareCreateForm();
        public ViewModel PrepareEditForm(int Id);
        public ViewModel PrepareDetails(int Id);
    }
}
