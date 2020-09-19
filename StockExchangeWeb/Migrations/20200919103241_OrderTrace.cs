using Microsoft.EntityFrameworkCore.Migrations;

namespace StockExchangeWeb.Migrations
{
    public partial class OrderTrace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderTrace",
                columns: table => new
                {
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<string>(type: "text", nullable: true),
                    OrderStatus = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTrace", x => x.TraceId);
                    table.ForeignKey(
                        name: "FK_OrderTrace_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderTrace_OrderId",
                table: "OrderTrace",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderTrace");
        }
    }
}
