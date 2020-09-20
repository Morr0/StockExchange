using System.Collections.Generic;
using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services.TradedEntitiesService
{
    public class MockSecuritiesProvider : ISecuritiesProvider
    {
        public Dictionary<string, TradableSecurity> Securities { get; } = new Dictionary<string, TradableSecurity>
        {
            {"A", new TradableSecurity
            {
                Ticker = "A",
                OutstandingAmount = 100000
            }}
        };
    }
}