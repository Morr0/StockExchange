using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services
{
    public interface IStockExchange
    {
        /// <param name="order"></param>
        /// <returns>Null for was not able to place the order.</returns>
        Order PlaceOrder(Order order);

        Order RemoveOrder(string orderId);
        
        OrdersPlaced GetOrdersPlaced(string ticker);
    }
}