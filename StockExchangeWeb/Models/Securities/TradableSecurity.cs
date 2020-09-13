namespace StockExchangeWeb.Models
{
    public class TradableSecurity
    {
        public SecurityType SecurityType { get; set; }
        
        public string Ticker { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public uint OutstandingAmount { get; set; }
    }
}