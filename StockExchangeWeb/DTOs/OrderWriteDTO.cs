using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.DTOs
{
    public class OrderWriteDTO
    {
        [NotNull]
        public OrderType OrderType { get; set; } = OrderType.LIMIT_ORDER;
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