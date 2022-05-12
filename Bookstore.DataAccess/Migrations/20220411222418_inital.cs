using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreWeb.Migrations
{
    public partial class inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CoverTypes",
                table: "CoverTypes");

            migrationBuilder.RenameTable(
                name: "CoverTypes",
                newName: "Covers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Covers",
                table: "Covers",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Covers",
                table: "Covers");

            migrationBuilder.RenameTable(
                name: "Covers",
                newName: "CoverTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoverTypes",
                table: "CoverTypes",
                column: "Id");
        }
    }
}
