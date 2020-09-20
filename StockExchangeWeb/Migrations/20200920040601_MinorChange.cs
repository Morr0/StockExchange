using Microsoft.EntityFrameworkCore.Migrations;

namespace StockExchangeWeb.Migrations
{
    public partial class MinorChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderTrace_Order_OrderId",
                table: "OrderTrace");

            migrationBuilder.DropIndex(
                name: "IX_OrderTrace_OrderId",
                table: "OrderTrace");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderTrace_OrderId",
                table: "OrderTrace",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTrace_Order_OrderId",
                table: "OrderTrace",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
