using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreWeb.Migrations
{
    public partial class remove_tax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tax",
                table: "ShoppingCarts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "ShoppingCarts",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
