using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.HistoryService
{
    public interface IOrdersHistory
    {
        /// <summary>
        /// Archive orders to the market. Will add new orders and update changed orders.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task ArchiveOrder(Dictionary<string, Order> orders);
    }
}