using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.DTOs
{
    public class OrderWriteDTO
    {
        private decimal _askPrice;
        
        [NotNull]
        public OrderType OrderType { get; set; } = OrderType.LimitOrder;

        [NotNull] 
        public OrderTimeInForce OrderTimeInForce { get; set; } = OrderTimeInForce.GoodTillExecution;
        
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
        public decimal AskPrice
        {
            get => _askPrice;
            set => _askPrice = Math.Abs(value); 
        }
    }
}