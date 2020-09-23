using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.ExchangeService.Utilities
{
    /// <summary>
    /// Responsible for generating the key of the item to be stored in the caching layer.
    /// </summary>
    public static class CacheKeyGenerator
    {
        private const char Separator = ',';
        
        private const string BuyToken = "buy";
        private const string SellToken = "sell";
        
        /// <summary>
        /// Use this when wanting to access a specific order
        /// </summary>
        public static string ToStoreKey(ref Order order)
        {
            string side = order.BuyOrder ? BuyToken : SellToken;
            return $"{side}{Separator}{order.AskPrice}{Separator}{order.Amount}{Separator}{order.Id}";
        }
        
        /// <summary>
        /// Use this when wanting to access many orders
        /// </summary>
        public static string ToLookKey(ref bool buy, decimal askPrice, uint amount)
        {
            string side = buy ? BuyToken : SellToken;
            return $"{side}{Separator}{askPrice}{Separator}{amount}";
        }
    }
}