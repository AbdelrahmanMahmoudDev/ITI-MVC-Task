using System.ComponentModel.DataAnnotations.Schema;

namespace Task.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string name { get; set; } = "";
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }
        public int? age { get; set; }
        public string? address { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
        public ICollection<CourseStudents> CourseStudents { get; set; }

    }
}
