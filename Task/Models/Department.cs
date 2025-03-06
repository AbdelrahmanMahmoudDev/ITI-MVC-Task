namespace Task.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public string? location { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Instructor> Instructors { get; set; }
    }
}
