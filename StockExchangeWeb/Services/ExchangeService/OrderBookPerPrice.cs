using System.Collections.Generic;

namespace StockExchangeWeb.Services.ExchangeService
{
    public class OrderBookPerPrice
    {
        internal Dictionary<decimal, OrderBookPerShares> SharesOrderBooks;
        
        public OrderBookPerPrice()
        {
            SharesOrderBooks = new Dictionary<decimal, OrderBookPerShares>();
        }

        public OrderBookPerShares this[decimal price]
        {
            get
            {
                if (!SharesOrderBooks.ContainsKey(price))
                    SharesOrderBooks.Add(price, new OrderBookPerShares());

                return SharesOrderBooks[price];
            }
        }
    }
}