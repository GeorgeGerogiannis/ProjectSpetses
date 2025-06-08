using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSpetses.Migrations
{
    /// <inheritdoc />
    public partial class PostMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WrongAwnsers",
                table: "Stats",
                newName: "WrongAnswers");

            migrationBuilder.RenameColumn(
                name: "CorrectAwnsers",
                table: "Stats",
                newName: "CorrectAnswers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WrongAnswers",
                table: "Stats",
                newName: "WrongAwnsers");

            migrationBuilder.RenameColumn(
                name: "CorrectAnswers",
                table: "Stats",
                newName: "CorrectAwnsers");
        }
    }
}
