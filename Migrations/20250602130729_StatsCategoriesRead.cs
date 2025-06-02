using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSpetses.Migrations
{
    /// <inheritdoc />
    public partial class StatsCategoriesRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionsRead",
                table: "Stats");

            migrationBuilder.AddColumn<string>(
                name: "CategoriesRead",
                table: "Stats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriesRead",
                table: "Stats");

            migrationBuilder.AddColumn<string>(
                name: "SectionsRead",
                table: "Stats",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
