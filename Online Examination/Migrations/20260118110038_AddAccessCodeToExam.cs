using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExamination.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessCodeToExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessCode",
                table: "Exams",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_AccessCode",
                table: "Exams",
                column: "AccessCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Exams_AccessCode",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "AccessCode",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Exams");
        }
    }
}
