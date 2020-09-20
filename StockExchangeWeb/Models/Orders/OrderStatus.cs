namespace StockExchangeWeb.Models.Orders
{
    public enum OrderStatus
    {
        InMarket,
        Executed,
        Deleted,
        /// <summary>
        /// When using an immediate limit order and no match exists
        /// </summary>
        NoMatch
    }
}