namespace StockExchangeWeb.Services.MarketTimesService
{
    public interface IMarketOpeningTimesService
    {
        bool IsMarketOpen(string ticker);
    }
}