using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExamination.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationLevelToExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Exams");
        }
    }
}
