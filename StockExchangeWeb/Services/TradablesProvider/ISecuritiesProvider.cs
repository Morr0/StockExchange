using System.Collections.Generic;
using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services.TradedEntitiesService
{
    public interface ISecuritiesProvider
    {
        Dictionary<string, TradableSecurity> GetSecurities();
    }
}