using System.Collections.Generic;
using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services.TradedEntitiesService
{
    public class SecuritiesProvider : ISecuritiesProvider
    {
        public Dictionary<string, TradableSecurity> GetSecurities()
        {
            return new Dictionary<string, TradableSecurity>
            {
                {"A", new TradableSecurity
                    {
                        Ticker = "A",
                        SecurityType = SecurityType.STOCK,
                        Description = "This is a security",
                        Name = "Aaaa",
                        OutstandingAmount = 15000
                    } 
                }
            };
        }
    }
}