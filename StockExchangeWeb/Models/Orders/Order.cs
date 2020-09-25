using System;
using System.ComponentModel.DataAnnotations;

namespace StockExchangeWeb.Models.Orders
{
    [Serializable]
    public class Order
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// For use for deletion of open order
        /// </summary>
        public string DeletionReferenceKey { get; set; }

        public bool LimitOrder { get; set; } = true;

        public OrderTimeInForce OrderTimeInForce { get; set; } = OrderTimeInForce.GoodTillExecution;

        public OrderStatus OrderStatus { get; set; } = OrderStatus.InMarket;
        
        public bool BuyOrder { get; set; }
        
        public string Ticker { get; set; }
        
        public uint Amount { get; set; }
        
        public decimal AskPrice { get; set; }
        
        /// <summary>
        /// This is important for market orders as they do not have an asking price.
        /// </summary>
        public decimal ExecutedPrice { get; set; }

        public string OrderPutTime { get; set; } = DateTime.UtcNow.ToString();
        
        /// <summary>
        /// Only applicable to executed orders.
        /// </summary>
        public string OrderExecutionTime { get; set; }
        
        /// <summary>
        /// Only applicable to orders when requested a deletion.
        /// </summary>
        public string OrderDeletionTime { get; set; }
    }
}