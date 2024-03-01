using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    /// <inheritdoc />
    public partial class PeriodUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.RenameColumn(
                name: "DayHour",
                table: "Periods",
                newName: "StartHour");

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "Periods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndHour",
                table: "Periods",
                type: "text",
                nullable: false,
                defaultValue: "");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "Day",
                table: "Periods");

            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "Periods");

            migrationBuilder.RenameColumn(
                name: "StartHour",
                table: "Periods",
                newName: "DayHour");*/
        }
    }
}
