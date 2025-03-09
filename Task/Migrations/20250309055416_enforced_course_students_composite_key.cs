using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Migrations
{
    /// <inheritdoc />
    public partial class enforced_course_students_composite_key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Departments_DepartmentId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_DepartmentId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId1",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartmentId1",
                table: "Students",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Departments_DepartmentId1",
                table: "Students",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "DepartmentId");
        }
    }
}
