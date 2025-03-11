using System.ComponentModel.DataAnnotations;
using Task.Models;

namespace Task.ViewModels.Instructor
{
    public class InstructorNewVM
    {
        public List<Course> available_courses { get; set; }
        public List<Department> available_departments { get; set; }
        [Required(ErrorMessage = "The first name is a required field")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First name can only contain letters")]
        [MaxLength(50)]
        public string fname { get; set; }
        [Required(ErrorMessage = "The last name is a required field")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Last name can only contain letters")]
        [MaxLength(50)]
        public string lname { get; set; }
        [Required(ErrorMessage = "The salary is a required field")]
        [Range(minimum: 1000, maximum: 1000000, ErrorMessage = "Please enter a salary within the company salary range")]
        public decimal salary { get; set; }
        [Required]
        [Range(18, 60, ErrorMessage = "Age can only be between 18 and 60")]
        public int? age { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "The hiring date field is required")]
        public DateTime? HireDate { get; set; }
        public int selected_department_id { get; set; }
        public int selected_course_id { get; set; }
        public InstructorNewVM()
        {
            available_courses = new List<Course>();
            available_departments = new List<Department>();
        }
    }
}
