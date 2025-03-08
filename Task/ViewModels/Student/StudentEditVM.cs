using Task.Models;

namespace Task.ViewModels.Student
{
    public class StudentEditVM
    {
        public int student_id { get; set; }
        public string address { get; set; } = "";
        public int selected_department_id {get; set;}
        public string selected_department_name { get; set; } = "";
        public List<Department> departments { get; set; } = new List<Department>();
        public Dictionary<string, int> registered_courses { get; set; } = new Dictionary<string, int>();
        public List<Course> courses { get; set; } = new List<Course>();
    }
}
