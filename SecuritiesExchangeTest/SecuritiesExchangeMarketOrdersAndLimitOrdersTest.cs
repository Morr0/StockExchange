using System.Threading.Tasks;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.ExchangeService;
using StockExchangeWeb.Services.HistoryService;
using StockExchangeWeb.Services.MarketTimesService;
using StockExchangeWeb.Services.OrderTracingService;
using StockExchangeWeb.Services.TradedEntitiesService;
using Xunit;

namespace SecuritiesExchangeTest
{
    public class SecuritiesExchangeMarketOrdersAndLimitOrdersTest
    {
        private static MarketOpeningTimesRepository _marketOpeningTimes = new MarketOpeningTimesRepository();
        private static IOrdersHistory _ordersHistory = new OrdersHistoryRepository();
        private static ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();        
        private static OrderTraceRepository _orderTraceRepository = new OrderTraceRepository();

        [Fact]
        public async Task PlaceMarketOrder()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository, _marketOpeningTimes);
            string ticker = "A";
            uint amount = 500;
            decimal askPrice = 13.0m;
            Order order = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = true,
                LimitOrder = false
            };

            // Act
            Order placedOrder = await stockExchange.PlaceOrder(order);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);

            // Assert
            Assert.False(placedOrder.LimitOrder);
        }

        [Fact]
        public async Task PlaceLimitOrderThenMarketOrderShouldExecute()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository, _marketOpeningTimes);
            string ticker = "A";
            uint amount = 500;
            decimal askPrice = 13.0m;
            Order limitOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = false
            };
            Order marketOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                BuyOrder = true,
                AskPrice = 1,
                LimitOrder = false
            };

            // Act
            Order placedLimitOrder = await stockExchange.PlaceOrder(limitOrder);
            Order placedMarketOrder = await stockExchange.PlaceOrder(marketOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(OrderStatus.Executed, placedLimitOrder.OrderStatus);
            Assert.Equal(OrderStatus.Executed, placedMarketOrder.OrderStatus);
            
            Assert.Equal(0u, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
            
            Assert.Equal(placedMarketOrder.ExecutedPrice, ordersPlaced.ClosestBidPrice);
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
        }
    }
}