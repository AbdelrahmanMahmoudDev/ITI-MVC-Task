using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Task.Contexts;
using Task.Errors;
using Task.Models;
using Task.Repositories;
using Task.Utilities;

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
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB max file size
            });
            builder.Services.AddDbContext<SchoolContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });
            builder.Services.AddScoped<IRepository<Student>, StudentRepository>();
            builder.Services.AddScoped<IRepository<Course>, CourseRepository>();
            builder.Services.AddScoped<IRepository<Department>, DepartmentRepository>();
            builder.Services.AddScoped<IJointRepository<CourseStudents>, StudentCourseRepository>();
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
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
