using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    /// <inheritdoc />
    public partial class ConnectionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropPrimaryKey(
                name: "PK_TutorPeriods",
                table: "TutorPeriods");

            migrationBuilder.AddColumn<int>(
                name: "PeriodTutorId",
                table: "TutorPeriods",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorPeriods",
                table: "TutorPeriods",
                column: "PeriodTutorId");

            migrationBuilder.CreateTable(
                name: "PeriodCubicles",
                columns: table => new
                {
                    PeriodCubicleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PeriodTutorId = table.Column<int>(type: "integer", nullable: false),
                    CubicleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodCubicles", x => x.PeriodCubicleId);
                    table.ForeignKey(
                        name: "FK_PeriodCubicles_Cubicles_CubicleId",
                        column: x => x.CubicleId,
                        principalTable: "Cubicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeriodCubicles_TutorPeriods_PeriodTutorId",
                        column: x => x.PeriodTutorId,
                        principalTable: "TutorPeriods",
                        principalColumn: "PeriodTutorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutorPeriods_TutorId",
                table: "TutorPeriods",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodCubicles_CubicleId",
                table: "PeriodCubicles",
                column: "CubicleId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodCubicles_PeriodTutorId",
                table: "PeriodCubicles",
                column: "PeriodTutorId");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropTable(
                name: "PeriodCubicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorPeriods",
                table: "TutorPeriods");

            migrationBuilder.DropIndex(
                name: "IX_TutorPeriods_TutorId",
                table: "TutorPeriods");

            migrationBuilder.DropColumn(
                name: "PeriodTutorId",
                table: "TutorPeriods");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorPeriods",
                table: "TutorPeriods",
                columns: new[] { "TutorId", "PeriodId" });*/
        }
    }
}
