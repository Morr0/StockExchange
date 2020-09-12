using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StockExchangeWeb.DTOs
{
    public class OrderWriteDTO
    {
        [Required]
        [NotNull]
        public bool BuyOrder { get; set; }
        [Required]
        [NotNull]
        public string Ticker { get; set; }
        [Required]
        [NotNull]
        public uint Amount { get; set; }
        [Required]
        [NotNull]
        public decimal AskPrice { get; set; }
    }
}