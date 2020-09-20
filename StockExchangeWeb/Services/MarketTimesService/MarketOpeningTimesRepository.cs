using System.Collections.Generic;

namespace StockExchangeWeb.Services.MarketTimesService
{
    public class MarketOpeningTimesRepository
    {
        public MarketOpeningTimesRepository()
        {
            // TODO load opening times
        }
        
        public bool IsMarketOpen(string ticker)
        {
            return true;
        }
    }
}