using System.ComponentModel.DataAnnotations.Schema;

namespace Task.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string name { get; set; }
        public string topic { get; set; }
        public int MinimumDegree { get; set; }
        public int? InstructorId { get; set; }

        [ForeignKey(nameof(InstructorId))]
        public virtual Instructor Instructor { get; set; }
        public ICollection<CourseStudents> CourseStudents { get; set; }
    }
}
