using System.ComponentModel.DataAnnotations;

namespace Task.ViewModels
{
    public class DepartmentVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage="*")]
        [MaxLength(25, ErrorMessage="Please keep department names below 26 characters")]
        public string name { get; set; }
        [MaxLength(40, ErrorMessage="Please keep department descriptions below 50 characters")]
        public string? description { get; set; }
        [MaxLength(25, ErrorMessage="Please keep department locations below 26 characters")]
        public string? location { get; set; }
    }
}
