using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    /// <inheritdoc />
    public partial class HeadTutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tutors",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.CreateTable(
                name: "HeadTutors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HeadTutorsTutorId = table.Column<int>(type: "integer", nullable: false),
                    TutorIds = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadTutors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeadTutorConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HeadTutorId = table.Column<int>(type: "integer", nullable: false),
                    TutorIds = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadTutorConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeadTutorConnections_HeadTutors_HeadTutorId",
                        column: x => x.HeadTutorId,
                        principalTable: "HeadTutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeadTutorConnections_HeadTutorId",
                table: "HeadTutorConnections",
                column: "HeadTutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tutors_HeadTutorConnections_Id",
                table: "Tutors",
                column: "Id",
                principalTable: "HeadTutorConnections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Tutors_HeadTutorConnections_Id",
                table: "Tutors");

            migrationBuilder.DropTable(
                name: "HeadTutorConnections");

            migrationBuilder.DropTable(
                name: "HeadTutors");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tutors",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);*/
        }
    }
}
