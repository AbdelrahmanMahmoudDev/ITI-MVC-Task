using Task.Models;

namespace Task.ViewModels.Instructor
{
    public class InstructorEditVM
    {
        public int id { get; set; }
        public int relevant_course_id { get; set; }
        public List<Course> existing_courses { get; set; }
        public int relevant_dep_id { get; set; }
        public List<Department> existing_departments { get; set; }
        public decimal salary { get; set; }
    }
}
