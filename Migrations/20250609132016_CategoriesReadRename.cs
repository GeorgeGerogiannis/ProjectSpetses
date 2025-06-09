using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSpetses.Migrations
{
    /// <inheritdoc />
    public partial class CategoriesReadRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoriesRead",
                table: "Stats",
                newName: "CategoriesCompleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoriesCompleted",
                table: "Stats",
                newName: "CategoriesRead");
        }
    }
}
