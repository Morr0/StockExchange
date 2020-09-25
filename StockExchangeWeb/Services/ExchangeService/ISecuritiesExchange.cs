using System.Threading.Tasks;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.ExchangeService
{
    public interface IStockExchange
    {
        /// <param name="order"></param>
        /// <returns>Null for was not able to place the order.</returns>
        Task<Order> PlaceOrder(Order order);

        Task<Order> RemoveOrder(string orderDeletionKey);
        
        OrdersPlaced GetOrdersPlaced(string ticker);
    }
}