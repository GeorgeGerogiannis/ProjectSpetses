using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSpetses.Migrations
{
    /// <inheritdoc />
    public partial class New3Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointsEarned",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Section1Easy = table.Column<long>(type: "bigint", nullable: false),
                    Section1Hard = table.Column<long>(type: "bigint", nullable: false),
                    Section2Easy = table.Column<long>(type: "bigint", nullable: false),
                    Section2Hard = table.Column<long>(type: "bigint", nullable: false),
                    Section3Easy = table.Column<long>(type: "bigint", nullable: false),
                    Section3Hard = table.Column<long>(type: "bigint", nullable: false),
                    Section4Easy = table.Column<long>(type: "bigint", nullable: false),
                    Section4Hard = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsEarned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointsEarned_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointsEarned");
        }
    }
}
