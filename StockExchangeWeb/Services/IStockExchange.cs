using StockExchangeWeb.Models;
using StockExchangeWeb.Results;

namespace StockExchangeWeb.Services
{
    public interface IStockExchange
    {
        OrderPlacedResult PlaceOrder(Order order);
    }
}