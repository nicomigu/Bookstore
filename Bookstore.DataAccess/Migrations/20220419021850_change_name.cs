using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreWeb.Migrations
{
    public partial class change_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderTotal",
                table: "OrderHeaders",
                newName: "Total");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Total",
                table: "OrderHeaders",
                newName: "OrderTotal");
        }
    }
}
