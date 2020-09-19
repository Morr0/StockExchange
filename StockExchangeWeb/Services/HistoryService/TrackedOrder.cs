using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.HistoryService
{
    /// <summary>
    /// This order is just to be used by the synchronizer system to maintain state
    /// </summary>
    internal sealed class TrackedOrder
    {
        public Order Order { get; }

        public TrackedOrder(Order order)
        {
            Order = order;
        }
        
        public bool ToBeUpdated { get; set; }
    }
}