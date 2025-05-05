using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSpetses.Migrations
{
    /// <inheritdoc />
    public partial class StatsLastRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastRead",
                table: "Stats",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRead",
                table: "Stats");
        }
    }
}
