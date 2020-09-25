using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Models.OrderTrace;

[assembly: InternalsVisibleTo("SecuritiesExchangeTest")]
namespace StockExchangeWeb.Services.OrderTracingService
{
    public class OrderTraceRepository
    {
        internal readonly LinkedList<OrderTrace> _orderTraces;

        public OrderTraceRepository()
        {
            _orderTraces = new LinkedList<OrderTrace>();
        }
        
        public void Trace(Order order)
        {
            _orderTraces?.AddLast(new OrderTrace
            {
                OrderId = order.Id,
                OrderStatus = order.OrderStatus
            });
        }

        public void Trace(Dictionary<string, Order> orders)
        {
            foreach (var orderPair in orders)
                Trace(orderPair.Value);
        }
    }
}