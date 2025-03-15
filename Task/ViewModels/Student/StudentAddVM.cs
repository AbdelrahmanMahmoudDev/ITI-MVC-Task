using System.ComponentModel.DataAnnotations;
using Task.Models;

namespace Task.ViewModels.Student
{
    public class CourseDetails
    {
        [Required(ErrorMessage = "Please select a course")]
        public int course_id { get; set; }
        [Required(ErrorMessage = "Please enter the obtained degree for this course")]
        [Range(0, 100, ErrorMessage = "The obtained degree must be a value between 0 and 100")]
        public int Degree { get; set; }
    }
    public class StudentAddVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string name { get; set; } = "";
        [Range(18, 40, ErrorMessage = "Student age must be between 18 and 40")]
        public int? age { get; set; }
        public string? address { get; set; }
        public IFormFile image { get; set; }
        public int selected_department_id { get; set; }
        public List<CourseDetails> course_details { get; set; } = new List<CourseDetails>();
        public List<Department> departments { get; set; } = new List<Department>();
        public List<Course> courses { get; set; } = new List<Course>();
    }
}
