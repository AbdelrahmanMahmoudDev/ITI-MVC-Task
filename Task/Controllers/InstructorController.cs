using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;
using Task.Models;
using Task.ViewModels.Instructor;

namespace Task.Controllers
{
    public class InstructorController : Controller
    {
        SchoolContext context;
        public InstructorController()
        {
            context = new SchoolContext();
        }
        public IActionResult Index()
        {
            var instructor_list = context.Instructors.Include(i => i.Department).ToList();
            return View(instructor_list);
        }

        public IActionResult Details(int id)
        {
            var instructor = context.Instructors
                                    .Where(i => i.InstructorId == id)
                                    .Select(i => new InstructorVM
                                    {
                                        fname = i.fname,
                                        lname = i.lname,
                                        full_name = i.fname + " " + i.lname,
                                        image_path = i.image,
                                        hire_date = new DateTime(i.HireDate.Value.Year, i.HireDate.Value.Month,                                                i.HireDate.Value.Day),
                                        salary = i.salary,
                                        age = i.age
                                    })
                                    .FirstOrDefault();

            if (instructor != null)
            {
                instructor.course_names = context.Courses
                    .Where(c => c.Instructor.InstructorId == id)
                    .Select(c => c.name)
                    .ToList();
            }

            return View(instructor);
        }
        public IActionResult Edit(int id)
        {
            var instructor_details = context.Courses
                         .Where(c => c.Instructor.InstructorId == id)
                         .Include(c => c.Instructor)
                         .Include(c => c.Instructor.Department)
                         .Select(c => new InstructorEditVM
                         {
                             id = id,
                             relevant_course_id = context.Courses
                             .Where(c => c.Instructor.InstructorId == id)
                             .Select(c => c.CourseId)
                             .FirstOrDefault(),
                             existing_courses = context.Courses.ToList(),
                             relevant_dep_id = c.Instructor.Department.DepartmentId,
                             existing_departments = context.Departments.ToList(),
                             salary = c.Instructor.salary,
                         })
                         .FirstOrDefault();
            return View(instructor_details);
        }
        [HttpPost]
        public IActionResult SaveEdit(InstructorEditVM form_data)
        {
            Instructor relevant_instructor = context.Instructors
                .Where(inst => inst.InstructorId == form_data.id)
                .FirstOrDefault();

            relevant_instructor.salary = form_data.salary;
            relevant_instructor.DepartmentId = form_data.relevant_dep_id;

            Course relevant_course = context.Courses
                .Where(crs => crs.CourseId == form_data.relevant_course_id)
                .FirstOrDefault();

            relevant_course.InstructorId = form_data.id;

            context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Add()
        {
            InstructorNewVM required_data = new InstructorNewVM()
            {
                available_courses = context.Courses.ToList(),
                available_departments = context.Departments.ToList(),
            };

            return View(required_data);
        }
        [HttpPost]
        public IActionResult SaveNew(InstructorNewVM form_data)
        {
            if(!ModelState.IsValid)
            {
                InstructorNewVM required_data = new InstructorNewVM()
                {
                    available_courses = context.Courses.ToList(),
                    available_departments = context.Departments.ToList(),
                };
                return View("Add", required_data);
            }
            Instructor new_guy = new Instructor()
            {
                fname = form_data.fname,
                lname = form_data.lname,
                salary = form_data.salary,
                image = "/images/male.jpg",
                age = form_data.age,
                HireDate = new DateTime(form_data.HireDate.Value.Year,
                form_data.HireDate.Value.Month,
                form_data.HireDate.Value.Day),
                DepartmentId = form_data.selected_department_id,
            };
            context.Instructors.Add(new_guy);
            context.SaveChanges();
            context.Courses.Where(c => c.CourseId == form_data.selected_course_id)
                           .FirstOrDefault()
                           .InstructorId = context.Instructors.OrderBy(inst => inst.InstructorId).LastOrDefault().InstructorId;
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var target = context.Instructors.Find(id);
            if(target != null)
            {
                context.Instructors.Remove(target);
            }

            var referencing_courses = context.Courses.Where(c => c.InstructorId == id).ToList();
            foreach(var crs in referencing_courses)
            {
                crs.InstructorId = null;
            }

            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
