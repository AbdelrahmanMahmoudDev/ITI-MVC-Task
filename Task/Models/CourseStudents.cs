using Microsoft.EntityFrameworkCore;

namespace Task.Models
{
    [PrimaryKey(nameof(CourseId), nameof(StudentId))]
    public class CourseStudents
    {
        public int? CourseId { get; set; }
        public virtual Course Course { get; set; }
        public int? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public int Degree { get; set; }
    }
}
