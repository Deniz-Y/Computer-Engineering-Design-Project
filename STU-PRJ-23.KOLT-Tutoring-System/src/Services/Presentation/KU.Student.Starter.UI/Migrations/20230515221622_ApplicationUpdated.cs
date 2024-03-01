using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "Courses",
                table: "TutorApplications");

            migrationBuilder.RenameColumn(
                name: "Periods",
                table: "TutorApplications",
                newName: "Status");

            migrationBuilder.AlterColumn<int>(
                name: "WeeklyHour",
                table: "TutorApplications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int[]>(
                name: "CourseIds",
                table: "TutorApplications",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "PeriodIds",
                table: "TutorApplications",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "CourseIds",
                table: "TutorApplications");

            migrationBuilder.DropColumn(
                name: "PeriodIds",
                table: "TutorApplications");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TutorApplications",
                newName: "Periods");

            migrationBuilder.AlterColumn<string>(
                name: "WeeklyHour",
                table: "TutorApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Courses",
                table: "TutorApplications",
                type: "text",
                nullable: false,
                defaultValue: "");*/
        }
    }
}
