using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    /// <inheritdoc />
    public partial class EmailPeriodUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TutorApplications_Email",
                table: "TutorApplications",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniquePeriod",
                table: "Periods",
                columns: new[] { "Day", "StartHour", "EndHour" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_Email",
                table: "AdminUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TutorApplications_Email",
                table: "TutorApplications");

            migrationBuilder.DropIndex(
                name: "IX_UniquePeriod",
                table: "Periods");

            migrationBuilder.DropIndex(
                name: "IX_AdminUsers_Email",
                table: "AdminUsers");
        }
    }
}
