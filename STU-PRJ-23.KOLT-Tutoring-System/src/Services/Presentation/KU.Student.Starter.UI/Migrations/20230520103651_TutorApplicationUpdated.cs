using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    public partial class TutorApplicationUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Manually execute the SQL statement to update the column type
            migrationBuilder.Sql("ALTER TABLE \"TutorApplications\" ALTER COLUMN \"GPA\" TYPE double precision USING \"GPA\"::double precision;");

            migrationBuilder.AlterColumn<double>(
                name: "GPA",
                table: "TutorApplications",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldDefaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GPA",
                table: "TutorApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: "0");
        }
    }
}
