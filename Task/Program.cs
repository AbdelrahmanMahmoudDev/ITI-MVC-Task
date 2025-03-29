using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Task.BL;
using Task.Contexts;
using Task.Errors;
using Task.Models;
using Task.Repositories;
using Task.Repositories.Base;
using Task.Utilities;
using Task.ViewModels;
using Task.ViewModels.Instructor;
using Task.ViewModels.Student;

namespace Task
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddExceptionHandler<CustomException>();
            builder.Services.AddDbContext<SchoolContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<SchoolContext>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IStudentService<StudentAddVM>, StudentService>();
            builder.Services.AddScoped<IDepartmentService<DepartmentVM>, DepartmentService>();
            builder.Services.AddScoped<ICourseService<CourseVM>, CourseService>();
            builder.Services.AddScoped<IInstructorService<InstructorVM>, InstructorService>();

            var app = builder.Build();
            FileUtility.WebRootPath = app.Environment.WebRootPath;
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            else
            {
                app.UseStatusCodePages();
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
