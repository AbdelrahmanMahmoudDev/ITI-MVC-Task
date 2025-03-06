namespace Task.ViewModels
{
    public class StudentDetailsVM
    {
        public string name { get; set; }
        public string image { get; set; }
        public int? age { get; set; }
        public string? address { get; set; }
        public string dept_name { get; set; }
        public int course_min_degree { get; set; }
        public int stud_degree { get; set; }
        public string course_name { get; set; }
        public Dictionary<string, int> courses { get; set; }
        public StudentDetailsVM()
        {
            courses = new Dictionary<string, int>();
        }
}
}
