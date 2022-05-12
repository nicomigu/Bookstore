using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreWeb.Migrations
{
    public partial class tax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "ShoppingCarts",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tax",
                table: "ShoppingCarts");
        }
    }
}
