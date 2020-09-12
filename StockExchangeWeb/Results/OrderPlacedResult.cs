using StockExchangeWeb.Models;

namespace StockExchangeWeb.Results
{
    public class OrderPlacedResult
    {
        public Order Order { get; set; }
        
        public bool OrderExecuted { get; set; }
    }
}