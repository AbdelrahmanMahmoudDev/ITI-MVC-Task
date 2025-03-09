using Task.Models;

namespace Task.ViewModels.Student
{
    public class StudentAddVM
    {
        public string name { get; set; } = "";
        public int? age { get; set; }
        public string? address { get; set; }
        public int selected_department_id { get; set; }
        public List<Department> departments { get; set; } = new List<Department>();
        public List<int> chosen_courses { get; set; } = new List<int>();
        public Dictionary<string, int> registered_courses { get; set; } = new Dictionary<string, int>();
        public List<Course> courses { get; set; } = new List<Course>();
    }
}
