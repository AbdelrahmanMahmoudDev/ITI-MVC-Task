using System.ComponentModel.DataAnnotations;

namespace Task.ViewModels
{
    public class CourseVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "*")]
        [MaxLength(25, ErrorMessage = "Please keep course names below 26 characters")]
        public string Name { get; set; }
        [MaxLength(40, ErrorMessage = "Please keep course topics below 50 characters")]
        public string Topic { get; set; }
        [Required(ErrorMessage = "*")]
        public int MinimumDegree { get; set; }
    }
}
