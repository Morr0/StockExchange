namespace StockExchangeWeb.Services.MarketTimesService.MarketTimes
{
    public abstract class TimesStrategy
    {
        public virtual bool OpenNow()
        {
            return false;
        }
    }
}