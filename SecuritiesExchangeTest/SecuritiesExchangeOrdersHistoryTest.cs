using System;
using System.Threading.Tasks;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.HistoryService;
using StockExchangeWeb.Services.OrderTracingService;
using StockExchangeWeb.Services.TradedEntitiesService;
using Xunit;

namespace SecuritiesExchangeTest
{
    public class SecuritiesExchangeOrdersHistoryTest
    {
        private OrdersHistoryRepository _ordersHistory = new OrdersHistoryRepository();
        private static ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();
        private static OrderTraceRepository _orderTraceRepository = new OrderTraceRepository();

        [Fact]
        public async Task PerformBasicTradeWhileCheckingHistoryIsCorrect()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory, _orderTraceRepository);
            string ticker = "A";
            uint amount = 500;
            decimal askPrice = 13.0m;
            string buyOrderId = Guid.NewGuid().ToString();
            string sellOrderId = Guid.NewGuid().ToString();
            Order sellOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = true,
                Id = sellOrderId
            };
            Order buyOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = false,
                Id = buyOrderId
            };
            
            // Act I
            await stockExchange.PlaceOrder(sellOrder);
            
            // Assert I
            Assert.True(_ordersHistory._archivedOrders.ContainsKey(sellOrderId));
            Assert.Equal(OrderStatus.IN_MARKET, _ordersHistory._archivedOrders[sellOrderId].OrderStatus);
            
            // Act II
            await stockExchange.PlaceOrder(buyOrder);
            
            // Assert II
            Assert.True(_ordersHistory._archivedOrders.ContainsKey(buyOrderId));
            Assert.Equal(OrderStatus.EXECUTED, _ordersHistory._archivedOrders[sellOrderId].OrderStatus);
            Assert.Equal(OrderStatus.EXECUTED, _ordersHistory._archivedOrders[buyOrderId].OrderStatus);

        }
    }
}