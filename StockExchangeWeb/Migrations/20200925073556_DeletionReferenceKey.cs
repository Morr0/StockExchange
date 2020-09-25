using Microsoft.EntityFrameworkCore.Migrations;

namespace StockExchangeWeb.Migrations
{
    public partial class DeletionReferenceKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletionReferenceKey",
                table: "Order",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionReferenceKey",
                table: "Order");
        }
    }
}
