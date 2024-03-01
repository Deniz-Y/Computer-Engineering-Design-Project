using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    /// <inheritdoc />
    public partial class PeriodTutor1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodTutor_Periods_PeriodId",
                table: "PeriodTutor");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodTutor_Tutors_TutorId",
                table: "PeriodTutor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PeriodTutor",
                table: "PeriodTutor");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "Periods");

            migrationBuilder.RenameTable(
                name: "PeriodTutor",
                newName: "TutorPeriods");

            migrationBuilder.RenameColumn(
                name: "Hour",
                table: "Periods",
                newName: "DayHour");

            migrationBuilder.RenameIndex(
                name: "IX_PeriodTutor_PeriodId",
                table: "TutorPeriods",
                newName: "IX_TutorPeriods_PeriodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorPeriods",
                table: "TutorPeriods",
                columns: new[] { "TutorId", "PeriodId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TutorPeriods_Periods_PeriodId",
                table: "TutorPeriods",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorPeriods_Tutors_TutorId",
                table: "TutorPeriods",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorPeriods_Periods_PeriodId",
                table: "TutorPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorPeriods_Tutors_TutorId",
                table: "TutorPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorPeriods",
                table: "TutorPeriods");

            migrationBuilder.RenameTable(
                name: "TutorPeriods",
                newName: "PeriodTutor");

            migrationBuilder.RenameColumn(
                name: "DayHour",
                table: "Periods",
                newName: "Hour");

            migrationBuilder.RenameIndex(
                name: "IX_TutorPeriods_PeriodId",
                table: "PeriodTutor",
                newName: "IX_PeriodTutor_PeriodId");

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "Periods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeriodTutor",
                table: "PeriodTutor",
                columns: new[] { "TutorId", "PeriodId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodTutor_Periods_PeriodId",
                table: "PeriodTutor",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodTutor_Tutors_TutorId",
                table: "PeriodTutor",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
