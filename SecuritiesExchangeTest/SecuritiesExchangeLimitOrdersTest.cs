using System;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.TradedEntitiesService;
using Xunit;

namespace SecuritiesExchangeTest
{
    public class SecuritiesExchangeBasicOrdersTest
    {
        private static ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();
        
        [Fact]
        public void BasicPlaceASingleOrderTest()
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
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);

            // Assert
            Assert.Equal(ticker, placedOrder.Ticker);
            Assert.Equal(amount, placedOrder.Amount);
            Assert.Equal(askPrice, order.AskPrice);
            Assert.Equal(OrderStatus.IN_MARKET, placedOrder.OrderStatus);
            Assert.True(placedOrder.BuyOrder);
            Assert.Equal(OrderType.LIMIT_ORDER, placedOrder.OrderType);

            Assert.True(ordersPlaced.BuyOrders.Count == 1);
            Assert.True(ordersPlaced.SellOrders.Count == 1);

            
            Assert.Equal(amount, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
            
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
            Assert.Equal(askPrice, ordersPlaced.ClosestSpread);
        }

        [Fact]
        public void TwoLimitOrdersSamePriceSameAmountBeingExecutedTest()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider);
            string ticker = "A";
            uint amount = 500;
            decimal askPrice = 13.0m;
            Order buyOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = true,
            };
            Order sellOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = false,
            };

            // Act
            Order placedBuyOrder = stockExchange.PlaceOrder(buyOrder);
            Order placedSellOrder = stockExchange.PlaceOrder(sellOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.True(placedBuyOrder.OrderStatus == OrderStatus.EXECUTED);
            Assert.True(placedSellOrder.OrderStatus == OrderStatus.EXECUTED);
            
            Assert.Equal(0u, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
            
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
            Assert.Equal(askPrice, ordersPlaced.ClosestBidPrice);
            Assert.Equal(0m, ordersPlaced.ClosestSpread);
        }

        [Fact]
        public void TwoLimitOrdersDifferentPricesSameAmountNotExecuted()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider);
            string ticker = "A";
            uint amount = 500;
            decimal askBuyPrice = 13.0m;
            decimal askSellPrice = 13.4m;
            decimal expectedSpread = Math.Abs(askBuyPrice - askSellPrice);
            Order buyOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askBuyPrice,
                BuyOrder = true,
            };
            Order sellOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askSellPrice,
                BuyOrder = false,
            };

            // Act
            Order placedBuyOrder = stockExchange.PlaceOrder(buyOrder);
            Order placedSellOrder = stockExchange.PlaceOrder(sellOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(OrderStatus.IN_MARKET, placedBuyOrder.OrderStatus);
            Assert.Equal(OrderStatus.IN_MARKET, placedSellOrder.OrderStatus);
            
            Assert.Equal(amount, ordersPlaced.BuyOrders[askBuyPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.BuyOrders[askSellPrice.ToString()]);
            
            Assert.Equal(0u, ordersPlaced.SellOrders[askBuyPrice.ToString()]);
            Assert.Equal(amount, ordersPlaced.SellOrders[askSellPrice.ToString()]);
            
            Assert.Equal(askBuyPrice, ordersPlaced.ClosestAskPrice);
            Assert.Equal(askSellPrice, ordersPlaced.ClosestBidPrice);
            Assert.Equal(expectedSpread, ordersPlaced.ClosestSpread);
        }

        [Fact]
        public void TwoLimitOrdersSamePriceSameAmountExecutedThenPlaceADifferentOrder()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider);
            string ticker = "A";
            uint amount = 500;
            decimal askPrice = 13.0m;
            decimal thirdAskPrice = 14.3m;
            decimal expectedSpread = Math.Abs(askPrice - thirdAskPrice);
            Order buyOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = true,
            };
            Order sellOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = askPrice,
                BuyOrder = false,
            };
            Order thirdOrder = new Order
            {
                Ticker = ticker,
                Amount = amount,
                AskPrice = thirdAskPrice,
                BuyOrder = false,
            };

            // Act
            Order placedBuyOrder = stockExchange.PlaceOrder(buyOrder);
            Order placedSellOrder = stockExchange.PlaceOrder(sellOrder);
            Order placedThridOrder = stockExchange.PlaceOrder(thirdOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
            
            Assert.Equal(amount, ordersPlaced.SellOrders[thirdAskPrice.ToString()]);
            Assert.Equal(thirdAskPrice, placedThridOrder.AskPrice);
            Assert.Equal(thirdAskPrice, ordersPlaced.ClosestBidPrice);   
            Assert.Equal(expectedSpread, ordersPlaced.ClosestSpread); 
        }
    }
}