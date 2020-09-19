using System.Collections.Generic;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Models.OrderTrace;

namespace StockExchangeWeb.Services.OrderTracingService
{
    public class OrderTraceRepository
    {
        internal LinkedList<OrderTrace> _orderTraces = new LinkedList<OrderTrace>();
        
        public void Trace(ref Order order)
        {
            _orderTraces.AddLast(new OrderTrace
            {
                OrderId = order.Id,
                Order = order,
                OrderStatus = order.OrderStatus
            });
        }
    }
}