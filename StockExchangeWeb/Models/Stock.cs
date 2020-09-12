namespace StockExchangeWeb.Models
{
    public class Stock
    {
        public string Ticker { get; set; }
        
        public string CompanyName { get; set; }
        
        public uint OutstandingShares { get; set; }
        
        public decimal LastPrice { get; set; }
    }
}