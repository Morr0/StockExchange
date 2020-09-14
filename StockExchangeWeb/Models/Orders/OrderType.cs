namespace StockExchangeWeb.Models.Orders
{
    public enum OrderType : byte
    {
        MARKET_ORDER,
        LIMIT_ORDER,
        STOP_ORDER,
        LIMIT_STOP_ORDER,
        /// <summary>
        /// If cannot execute now then get rid of order. Will not affect market bid/ask prices unless executed.
        /// </summary>
        LIMIT_ORDER_IMMEDIATE
    }
}