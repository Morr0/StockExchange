using System.Collections.Generic;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.DTOs
{
    // NOTE: Return string instead of decimal as ASP.NET CORE 3.1 doesn't support a decimal key of dictionary
    public class OrdersPlaced
    {
        public OrdersPlaced(string ticker, decimal lastExecutedPrice, decimal closestAskPrice, decimal closestBidPrice,
            decimal closestSpread)
        {
            Ticker = ticker;
            LastExecutedPrice = lastExecutedPrice;
            ClosestAskPrice = closestAskPrice;
            ClosestBidPrice = closestBidPrice;
            ClosestSpread = closestSpread;
        }
        
        public string Ticker { get; }
        
        public decimal LastExecutedPrice { get; }

        public decimal ClosestAskPrice { get; set; }
        
        public decimal ClosestBidPrice { get; set; }
        
        public decimal ClosestSpread { get; set; }
        
        /// <summary>
        /// Key -> Price.
        /// Value -> Outstanding shares at that price point
        /// </summary>
        public Dictionary<string, uint> BuyOrders { get; } = new Dictionary<string, uint>();
        
        /// <summary>
        /// Key -> Price.
        /// Value -> Outstanding shares at that price point
        /// </summary>
        public Dictionary<string, uint> SellOrders { get; } = new Dictionary<string, uint>();
    }
}