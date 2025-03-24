using Task.Models;

namespace Task.BL
{
    public interface ICourseService<ViewModel>
    {
        public IEnumerable<Course> PrepareDashboard();
        public ViewModel PrepareEdit(int Id);
        public System.Threading.Tasks.Task CreateAsync(ViewModel Data);
        public System.Threading.Tasks.Task UpdateAsync(ViewModel Data, int Id);
        public System.Threading.Tasks.Task DeleteAsync(int Id);
    }
}
