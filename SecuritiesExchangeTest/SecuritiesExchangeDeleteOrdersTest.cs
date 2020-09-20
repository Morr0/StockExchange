using System.Threading.Tasks;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.HistoryService;
using StockExchangeWeb.Services.MarketTimesService;
using StockExchangeWeb.Services.OrderTracingService;
using StockExchangeWeb.Services.TradedEntitiesService;
using Xunit;

namespace SecuritiesExchangeTest
{
    public class SecuritiesExchangeDeleteOrdersTest
    {
        private static MarketOpeningTimesRepository _marketOpeningTimes = new MarketOpeningTimesRepository();
        private static IOrdersHistory _ordersHistory = new OrdersHistoryRepository();
        private static ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();
        private static OrderTraceRepository _orderTraceRepository = new OrderTraceRepository();
        
        [Fact]
        public async Task DeleteOrderAndExpectAskPriceToBeZero()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory, 
                _orderTraceRepository, _marketOpeningTimes);
            string ticker = "A";
            uint amount = 500;
            decimal askPrice = 13.0m;
            Order order = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = true,
            };

            // Act
            Order placedOrder = await stockExchange.PlaceOrder(order);
            Order deletedOrder = stockExchange.RemoveOrder(placedOrder.Id);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(OrderStatus.Deleted, deletedOrder.OrderStatus);
            Assert.False(string.IsNullOrEmpty(deletedOrder.OrderDeletionTime));
            
            Assert.Equal(0u, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
        }

        [Fact]
        public async Task ExecuteASuccessfulOrderAtSamePriceAndAddAnotherBuyOrderAtDifferentPriceThenRemoveIt()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory, 
                _orderTraceRepository, _marketOpeningTimes);
            string ticker = "A";
            uint amount = 500;
            decimal firstAskPrice = 13.0m;
            Order order1 = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = firstAskPrice,
                BuyOrder = true,
            };
            Order order2 = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = firstAskPrice,
                BuyOrder = false,
            };

            decimal secondAskPrice = 11.95m;
            Order order3 = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = secondAskPrice,
                BuyOrder = true,
            };

            // Act
            Order placedOrder1 = await stockExchange.PlaceOrder(order1);
            Order placedOrder2 = await stockExchange.PlaceOrder(order2);
            OrdersPlaced ordersPlaced1 = stockExchange.GetOrdersPlaced(ticker);

            Order placedOrder3 = await stockExchange.PlaceOrder(order3);
            OrdersPlaced ordersPlaced2 = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(firstAskPrice, ordersPlaced1.ClosestAskPrice);
            Assert.Equal(firstAskPrice, ordersPlaced1.ClosestBidPrice);
            
            Assert.Equal(secondAskPrice, ordersPlaced2.ClosestAskPrice);
            Assert.Equal(firstAskPrice, ordersPlaced2.ClosestBidPrice);
        }
    }
}