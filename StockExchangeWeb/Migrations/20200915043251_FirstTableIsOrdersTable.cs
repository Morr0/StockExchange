using Microsoft.EntityFrameworkCore.Migrations;

namespace StockExchangeWeb.Migrations
{
    public partial class FirstTableIsOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OrderType = table.Column<byte>(type: "smallint", nullable: false),
                    OrderStatus = table.Column<byte>(type: "smallint", nullable: false),
                    BuyOrder = table.Column<bool>(type: "boolean", nullable: false),
                    Ticker = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    AskPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ExecutedPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderPutTime = table.Column<string>(type: "text", nullable: true),
                    OrderExecutionTime = table.Column<string>(type: "text", nullable: true),
                    OrderDeletionTime = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatus",
                table: "Order",
                column: "OrderStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
