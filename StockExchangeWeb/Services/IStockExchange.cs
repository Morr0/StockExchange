using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services
{
    public interface IStockExchange
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Null for was not able to place the order.</returns>
        Order PlaceOrder(Order order);
    }
}