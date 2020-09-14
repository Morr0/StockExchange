namespace StockExchangeWeb.Models.Orders
{
    public enum OrderStatus : byte
    {
        IN_MARKET,
        EXECUTED,
        DELETED,
        /// <summary>
        /// When using an immediate limit order and no match exists
        /// </summary>
        NO_MATCH
    }
}