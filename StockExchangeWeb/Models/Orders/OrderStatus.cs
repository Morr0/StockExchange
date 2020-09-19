namespace StockExchangeWeb.Models.Orders
{
    public enum OrderStatus : byte
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