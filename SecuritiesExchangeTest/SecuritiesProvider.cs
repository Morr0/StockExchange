using System.Collections.Generic;
using StockExchangeWeb.Models;
using StockExchangeWeb.Services.TradedEntitiesService;

namespace SecuritiesExchangeTest
{
    public class SecuritiesProvider : ISecuritiesProvider
    {
        public Dictionary<string, TradableSecurity> Securities { get; } = new Dictionary<string, TradableSecurity>
        {
            {"A", new TradableSecurity
            {
                Name = "Aaa",
                Description = "A is a company stock",
                Ticker = "A",
                OutstandingAmount = 15000,
                SecurityType = SecurityType.Stock
            }}
        };
    }
}