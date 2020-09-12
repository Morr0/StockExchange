using System;

namespace StockExchangeWeb.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public OrderStatus OrderStatus { get; set; } = OrderStatus.IN_MARKET;
        
        public string Ticker { get; set; }
        
        public uint Amount { get; set; }
        
        public decimal AskPrice { get; set; }

        public string OrderPutTime { get; set; } = DateTime.UtcNow.ToString();
        
        public string OrderExecutionTime { get; set; }
    }
}