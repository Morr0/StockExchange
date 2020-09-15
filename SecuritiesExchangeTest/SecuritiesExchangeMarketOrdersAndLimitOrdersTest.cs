﻿using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.TradedEntitiesService;
using Xunit;

namespace SecuritiesExchangeTest
{
    public class SecuritiesExchangeMarketOrdersAndLimitOrdersTest
    {
        private static ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();
        
        [Fact]
        public void PlaceMarketOrder()
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
                OrderType = OrderType.MARKET_ORDER
            };

            // Act
            Order placedOrder = stockExchange.PlaceOrder(order);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);

            // Assert
            Assert.Equal(OrderType.MARKET_ORDER, placedOrder.OrderType);
        }

        [Fact]
        public void PlaceLimitOrderThenMarketOrderShouldExecute()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider);
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
                OrderType = OrderType.MARKET_ORDER
            };

            // Act
            Order placedLimitOrder = stockExchange.PlaceOrder(limitOrder);
            Order placedMarketOrder = stockExchange.PlaceOrder(marketOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(OrderStatus.EXECUTED, placedLimitOrder.OrderStatus);
            Assert.Equal(OrderStatus.EXECUTED, placedMarketOrder.OrderStatus);
            
            Assert.Equal(0u, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
            
            Assert.Equal(placedMarketOrder.ExecutedPrice, ordersPlaced.ClosestBidPrice);
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
        }
    }
}