using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSpetses.Migrations
{
    /// <inheritdoc />
    public partial class NotificationsRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificationsGiven",
                table: "Stats",
                newName: "SectionsCompleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SectionsCompleted",
                table: "Stats",
                newName: "NotificationsGiven");
        }
    }
}
