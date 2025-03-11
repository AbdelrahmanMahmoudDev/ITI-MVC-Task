using System.ComponentModel.DataAnnotations;
using Task.Models;

namespace Task.ViewModels.Student
{
    public class StudentAddVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string name { get; set; } = "";
        [Range(18, 40, ErrorMessage = "Student age must be between 18 and 40")]
        public int? age { get; set; }
        public string? address { get; set; }
        public int selected_department_id { get; set; }
        public List<Department> departments { get; set; } = new List<Department>();
        public List<int> chosen_courses { get; set; } = new List<int>();
        public List<int> chosen_grades { get; set; } = new List<int>();
        public Dictionary<int, int> registered_courses { get; set; } = new Dictionary<int, int>();
        public List<Course> courses { get; set; } = new List<Course>();
    }
}
