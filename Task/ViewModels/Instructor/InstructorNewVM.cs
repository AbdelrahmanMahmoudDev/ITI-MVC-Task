using Task.Models;

namespace Task.ViewModels.Instructor
{
    public class InstructorNewVM
    {
        public List<Course> available_courses { get; set; }
        public List<Department> available_departments { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public decimal salary { get; set; }
        public int? age { get; set; }
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
