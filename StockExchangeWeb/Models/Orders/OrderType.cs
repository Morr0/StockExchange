namespace StockExchangeWeb.Models.Orders
{
    public enum OrderType : byte
    {
        MarketOrder,
        LimitOrder,
        /// <summary>
        /// If cannot execute now then get rid of order. Will not affect market bid/ask prices unless executed.
        /// </summary>
        LimitOrderImmediate
    }
}