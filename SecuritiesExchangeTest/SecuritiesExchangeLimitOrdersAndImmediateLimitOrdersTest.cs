using System;
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
    public class SecuritiesExchangeBasicOrdersTest
    {
        private static MarketOpeningTimesRepository _marketOpeningTimes = new MarketOpeningTimesRepository();
        private static IOrdersHistory _ordersHistory = new OrdersHistoryRepository();
        private static ISecuritiesProvider _securitiesProvider = new SecuritiesProvider();
        private static OrderTraceRepository _orderTraceRepository = new OrderTraceRepository();

        #region Normal limit orders
        
        [Fact]
        public async Task BasicPlaceASingleOrderTest()
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
            };

            // Act
            Order placedOrder = await stockExchange.PlaceOrder(order);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);

            // Assert
            Assert.Equal(ticker, placedOrder.Ticker);
            Assert.Equal(amount, placedOrder.Amount);
            Assert.Equal(askPrice, order.AskPrice);
            Assert.Equal(OrderStatus.InMarket, placedOrder.OrderStatus);
            Assert.True(placedOrder.BuyOrder);
            Assert.True(placedOrder.LimitOrder);

            Assert.True(ordersPlaced.BuyOrders.Count == 1);
            Assert.True(ordersPlaced.SellOrders.Count == 1);

            
            Assert.Equal(amount, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
            
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
            Assert.Equal(askPrice, ordersPlaced.ClosestSpread);
        }

        [Fact]
        public async Task TwoLimitOrdersSamePriceSameAmountBeingExecutedTest()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository, _marketOpeningTimes);
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
            Order placedBuyOrder = await stockExchange.PlaceOrder(buyOrder);
            Order placedSellOrder = await stockExchange.PlaceOrder(sellOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.True(placedBuyOrder.OrderStatus == OrderStatus.Executed);
            Assert.True(placedSellOrder.OrderStatus == OrderStatus.Executed);
            
            Assert.Equal(0u, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
            
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
            Assert.Equal(askPrice, ordersPlaced.ClosestBidPrice);
            Assert.Equal(0m, ordersPlaced.ClosestSpread);
        }

        [Fact]
        public async Task TwoLimitOrdersDifferentPricesSameAmountNotExecuted()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository, _marketOpeningTimes);
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
            Order placedBuyOrder = await stockExchange.PlaceOrder(buyOrder);
            Order placedSellOrder = await stockExchange.PlaceOrder(sellOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(OrderStatus.InMarket, placedBuyOrder.OrderStatus);
            Assert.Equal(OrderStatus.InMarket, placedSellOrder.OrderStatus);
            
            Assert.Equal(amount, ordersPlaced.BuyOrders[askBuyPrice.ToString()]);
            Assert.Equal(0u, ordersPlaced.BuyOrders[askSellPrice.ToString()]);
            
            Assert.Equal(0u, ordersPlaced.SellOrders[askBuyPrice.ToString()]);
            Assert.Equal(amount, ordersPlaced.SellOrders[askSellPrice.ToString()]);
            
            Assert.Equal(askBuyPrice, ordersPlaced.ClosestAskPrice);
            Assert.Equal(askSellPrice, ordersPlaced.ClosestBidPrice);
            Assert.Equal(expectedSpread, ordersPlaced.ClosestSpread);
        }

        [Fact]
        public async Task TwoLimitOrdersSamePriceSameAmountExecutedThenPlaceADifferentOrder()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository, _marketOpeningTimes);
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
            Order placedBuyOrder = await stockExchange.PlaceOrder(buyOrder);
            Order placedSellOrder = await stockExchange.PlaceOrder(sellOrder);
            Order placedThridOrder = await stockExchange.PlaceOrder(thirdOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(askPrice, ordersPlaced.ClosestAskPrice);
            
            Assert.Equal(amount, ordersPlaced.SellOrders[thirdAskPrice.ToString()]);
            Assert.Equal(thirdAskPrice, placedThridOrder.AskPrice);
            Assert.Equal(thirdAskPrice, ordersPlaced.ClosestBidPrice);   
            Assert.Equal(expectedSpread, ordersPlaced.ClosestSpread); 
        }
        #endregion
        
        #region Immediate limit orders

        [Fact]
        public async Task ImmediateOrderShouldNotPersistOnEmptyMarket()
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
                OrderTimeInForce = OrderTimeInForce.GoodOrKill
            };

            // Act
            Order placedOrder = await stockExchange.PlaceOrder(order);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);

            // Assert
            Assert.True(placedOrder.OrderStatus == OrderStatus.NoMatch);
            
            Assert.Equal(0m, ordersPlaced.ClosestAskPrice);
            Assert.Equal(0m, ordersPlaced.ClosestBidPrice);
        }

        [Fact]
        public async Task ImmediateOrderShouldExecuteOnMarketWithLiquidity()
        {
            // Arrange
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository, _marketOpeningTimes);
            string ticker = "A";
            uint amount = 1500;
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
                OrderTimeInForce = OrderTimeInForce.GoodOrKill
            };

            // Act
            Order placedOrder1 = await stockExchange.PlaceOrder(order1);
            Order placedOrder2 = await stockExchange.PlaceOrder(order2);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);

            // Assert
            Assert.Equal(OrderStatus.Executed, placedOrder2.OrderStatus);
            
            Assert.Equal(0u, ordersPlaced.BuyOrders[askPrice.ToString()]);
            // TODO there is a bug below, most likely due to the language, remedy it
            //Assert.Equal(0u, ordersPlaced.SellOrders[askPrice.ToString()]);
        }
        
        #endregion
        
        #region Ordering in closed market

        private sealed class MarketOpeningTimesTestingRepository : IMarketOpeningTimesService
        {
            public bool IsMarketOpen(string ticker)
            {
                return false;
            }
        }

        [Fact]
        public async Task OrderShouldNotExecuteInAClosedMarket()
        {
            // Arrange
            MarketOpeningTimesTestingRepository marketTimes = new MarketOpeningTimesTestingRepository();
            IStockExchange stockExchange = new InMemoryStockExchangeRepository(_securitiesProvider, _ordersHistory
                , _orderTraceRepository, marketTimes);
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
            Order placedBuyOrder1 = await stockExchange.PlaceOrder(buyOrder);
            Order placedSellOrder2 = await stockExchange.PlaceOrder(sellOrder);
            OrdersPlaced ordersPlaced = stockExchange.GetOrdersPlaced(ticker);
            
            // Assert
            Assert.Equal(amount, ordersPlaced.BuyOrders[askPrice.ToString()]);
            Assert.Equal(amount, ordersPlaced.SellOrders[askPrice.ToString()]);
        }
        
        #endregion
    }
}