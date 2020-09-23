using System.Collections.Generic;
using StockExchangeWeb.Services.MarketTimesService.MarketTimes;

namespace StockExchangeWeb.Services.MarketTimesService
{
    public class MarketOpeningTimesRepository : IMarketOpeningTimesService
    {
        private readonly bool _openAllTheTime;
        private readonly TimesStrategy _timesStrategy;

        public MarketOpeningTimesRepository(bool openAllTheTime = true)
        {
            _openAllTheTime = openAllTheTime;
        }

        public MarketOpeningTimesRepository(TimesStrategy timesStrategy)
        {
            _timesStrategy = timesStrategy;
        }

        public bool IsMarketOpen(string ticker)
        {
            return _timesStrategy?.OpenNow() ?? _openAllTheTime;
        }
    }
}