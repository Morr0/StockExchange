using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StockExchangeWeb.Controllers.Bodies
{
    public class OrderDeletionBody
    {
        [Required, NotNull]
        public string OrderId { get; set; }
    }
}