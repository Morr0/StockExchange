using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.TradedEntitiesService;
using Xunit;

namespace SecuritiesExchangeTest
{
    public class SecuritiesExchangeDeleteOrdersTest
    {
        private static ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();
        
        [Fact]
        public void DeleteOrderAndExpectAskPriceToBeZero()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider);
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
            Order placedOrder = stockExchange.PlaceOrder(order);
            Order deletedOrder = stockExchange.RemoveOrder(placedOrder.Id);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(OrderStatus.DELETED, deletedOrder.OrderStatus);
            Assert.False(string.IsNullOrEmpty(deletedOrder.OrderDeletionTime));
            
            Assert.Equal(0u, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
        }

        [Fact]
        public void ExecuteASuccessfulOrderAtSamePriceAndAddAnotherBuyOrderAtDifferentPriceThenRemoveIt()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider);
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
            Order placedOrder1 = stockExchange.PlaceOrder(order1);
            Order placedOrder2 = stockExchange.PlaceOrder(order2);
            OrdersPlaced ordersPlaced1 = stockExchange.GetOrdersPlaced(ticker);

            Order placedOrder3 = stockExchange.PlaceOrder(order3);
            OrdersPlaced ordersPlaced2 = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(firstAskPrice, ordersPlaced1.ClosestAskPrice);
            Assert.Equal(firstAskPrice, ordersPlaced1.ClosestBidPrice);
            
            Assert.Equal(secondAskPrice, ordersPlaced2.ClosestAskPrice);
            Assert.Equal(firstAskPrice, ordersPlaced2.ClosestBidPrice);
        }
    }
}