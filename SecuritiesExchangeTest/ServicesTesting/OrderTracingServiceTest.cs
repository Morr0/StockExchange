using System.Threading.Tasks;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.HistoryService;
using StockExchangeWeb.Services.OrderTracingService;
using StockExchangeWeb.Services.TradedEntitiesService;
using Xunit;

namespace SecuritiesExchangeTest.ServicesTesting
{
    public class OrderTracingServiceTest
    {
        private static IOrdersHistory _ordersHistory = new OrdersHistoryRepository();
        private ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();
        private OrderTraceRepository _orderTraceRepository = new OrderTraceRepository();
        
        [Fact]
        public async Task ShouldShowCorrectTraceOnExecutedOrders()
        {
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository);
            string ticker = "A";
            uint amount = 500;
            decimal askPrice = 13.0m;
            Order order1 = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = true,
            };
            Order order2 = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = false,
            };

            // Act I
            Order placedOrder1 = await stockExchange.PlaceOrder(order1);

            // Assert I
            Assert.True(_orderTraceRepository._orderTraces.Count == 1);
            Assert.Equal(placedOrder1, _orderTraceRepository._orderTraces.First.Value.Order);
            
            // Act II
            Order placedOrder2 = await stockExchange.PlaceOrder(order2);
            
            // Assert II
            Assert.True(_orderTraceRepository._orderTraces.Count == 4);
            Assert.Equal(OrderStatus.Executed, _orderTraceRepository._orderTraces.Last.Previous.Value.OrderStatus);
            Assert.Equal(OrderStatus.Executed, _orderTraceRepository._orderTraces.Last.Value.OrderStatus);
        }
    }
}