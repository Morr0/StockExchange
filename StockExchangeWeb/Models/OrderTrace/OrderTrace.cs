using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Models.OrderTrace
{
    public class OrderTrace
    {
        [Key]
        public string TraceId { get; set; } = Guid.NewGuid().ToString();

        public string Timestamp { get; set; } = DateTime.UtcNow.ToString();
        
        [ForeignKey(nameof(Orders.Order))]
        public string OrderId { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}