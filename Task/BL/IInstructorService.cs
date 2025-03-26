using Task.Models;
using Task.ViewModels.Instructor;

namespace Task.BL
{
    public interface IInstructorService<ViewModel>
    {
        public IEnumerable<Instructor> PrepareDashboard();
        public ViewModel PrepareDetailsPage(int Id);
        public ViewModel PrepareAddForm();
        public System.Threading.Tasks.Task AddInstructor(ViewModel FormData);
        public System.Threading.Tasks.Task RemoveInstructor(int Id);
        public System.Threading.Tasks.Task EditInstructor(InstructorVM form_data);
        public ViewModel PrepareEditPage(int Id);
    }
}
