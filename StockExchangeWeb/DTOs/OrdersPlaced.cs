using System.Collections.Generic;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.DTOs
{
    // NOTE: Return string instead of decimal as ASP.NET CORE 3.1 doesn't support a decimal key of dictionary
    public class OrdersPlaced
    {
        public OrdersPlaced(string ticker)
        {
            Ticker = ticker;
        }
        
        public string Ticker { get; }
        
        /// <summary>
        /// Key -> Price.
        /// Value -> Outstanding shares at that price point
        /// </summary>
        public Dictionary<string, uint> BuyOrders { get; set; } = new Dictionary<string, uint>();
        
        /// <summary>
        /// Key -> Price.
        /// Value -> Outstanding shares at that price point
        /// </summary>
        public Dictionary<string, uint> SellOrders { get; set; } = new Dictionary<string, uint>();
    }
}