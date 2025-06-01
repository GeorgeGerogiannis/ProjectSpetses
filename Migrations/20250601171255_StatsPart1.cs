using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSpetses.Migrations
{
    /// <inheritdoc />
    public partial class StatsPart1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blank_items_Content_ContentId",
                table: "Blank_items");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_items_Content_ContentId",
                table: "Match_items");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_items_Content_ContentId",
                table: "Quiz_items");

            migrationBuilder.RenameColumn(
                name: "ContentId",
                table: "Quiz_items",
                newName: "SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Quiz_items_ContentId",
                table: "Quiz_items",
                newName: "IX_Quiz_items_SectionId");

            migrationBuilder.RenameColumn(
                name: "ContentId",
                table: "Match_items",
                newName: "SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Match_items_ContentId",
                table: "Match_items",
                newName: "IX_Match_items_SectionId");

            migrationBuilder.RenameColumn(
                name: "ContentId",
                table: "Blank_items",
                newName: "SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Blank_items_ContentId",
                table: "Blank_items",
                newName: "IX_Blank_items_SectionId");

            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "Stats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PointsRequired",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Blank_items_Sections_SectionId",
                table: "Blank_items",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_items_Sections_SectionId",
                table: "Match_items",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_items_Sections_SectionId",
                table: "Quiz_items",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blank_items_Sections_SectionId",
                table: "Blank_items");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_items_Sections_SectionId",
                table: "Match_items");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_items_Sections_SectionId",
                table: "Quiz_items");

            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "PointsRequired",
                table: "Sections");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Quiz_items",
                newName: "ContentId");

            migrationBuilder.RenameIndex(
                name: "IX_Quiz_items_SectionId",
                table: "Quiz_items",
                newName: "IX_Quiz_items_ContentId");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Match_items",
                newName: "ContentId");

            migrationBuilder.RenameIndex(
                name: "IX_Match_items_SectionId",
                table: "Match_items",
                newName: "IX_Match_items_ContentId");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Blank_items",
                newName: "ContentId");

            migrationBuilder.RenameIndex(
                name: "IX_Blank_items_SectionId",
                table: "Blank_items",
                newName: "IX_Blank_items_ContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blank_items_Content_ContentId",
                table: "Blank_items",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_items_Content_ContentId",
                table: "Match_items",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_items_Content_ContentId",
                table: "Quiz_items",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
