using System.ComponentModel.DataAnnotations.Schema;

namespace Task.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public decimal salary { get; set; }
        public string? image { get; set; }
        public int? age { get; set; }
        public DateTime? HireDate { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
    }
}
