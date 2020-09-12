using System;

namespace StockExchangeWeb.Models.Orders
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public OrderType OrderType { get; set; } = OrderType.LIMIT_ORDER;

        public OrderStatus OrderStatus { get; set; } = OrderStatus.IN_MARKET;
        
        public bool BuyOrder { get; set; }
        
        public string Ticker { get; set; }
        
        public uint Amount { get; set; }
        
        public decimal AskPrice { get; set; }

        public string OrderPutTime { get; set; } = DateTime.UtcNow.ToString();
        
        public string OrderExecutionTime { get; set; }
    }
}