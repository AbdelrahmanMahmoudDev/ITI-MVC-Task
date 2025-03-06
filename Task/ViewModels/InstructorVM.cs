using Task.Models;

namespace Task.ViewModels
{
    public class InstructorVM
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public int? age { get; set; }
        public string full_name { get; set; }
        public string image_path { get; set; }
        public DateTime hire_date { get; set; }
        public decimal salary { get; set; }
        public List<string> course_names { get; set; } = new List<string>();
    }
}
