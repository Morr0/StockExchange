using System.ComponentModel.DataAnnotations;

namespace StockExchangeWeb.Models
{
    public class TradableSecurity
    {
        [Key]
        public string Ticker { get; set; }
        
        public SecurityType SecurityType { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public uint OutstandingAmount { get; set; }
    }
}