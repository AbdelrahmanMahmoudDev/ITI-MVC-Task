﻿using System.ComponentModel.DataAnnotations;
using Task.Models;

namespace Task.ViewModels.Instructor
{
    public class InstructorVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The first name is a required field")]
        [MaxLength(50)]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First name can only contain letters")]
        public string fname { get; set; }

        [Required(ErrorMessage = "The last name is a required field")]
        [MaxLength(50)]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Last name can only contain letters")]
        public string lname { get; set; }

        [Required]
        [Range(18, 60, ErrorMessage = "Age can only be between 18 and 60")]
        public int? age { get; set; }
        public string full_name { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
        public string image_path { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "The hiring date field is required")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "The salary is a required field")]
        [Range(minimum: 1000, maximum: 1000000, ErrorMessage = "Please enter a salary within the company salary range")]
        public decimal salary { get; set; }

        public int SelectedDepartmentId { get; set; }
        public int SelectedCourseId { get; set; }

        public List<string> course_names { get; set; } = new List<string>();

        public List<Course> AvailableCourses { get; set; }
        public List<Department> AvailableDepartments { get; set; }
    }
}
