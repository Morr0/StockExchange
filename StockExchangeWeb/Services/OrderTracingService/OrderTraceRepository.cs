using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Models.OrderTrace;

[assembly: InternalsVisibleTo("SecuritiesExchangeTest")]
namespace StockExchangeWeb.Services.OrderTracingService
{
    public class OrderTraceRepository
    {
        internal LinkedList<OrderTrace> _orderTraces = new LinkedList<OrderTrace>();
        
        public void Trace(Order order)
        {
            _orderTraces.AddLast(new OrderTrace
            {
                OrderId = order.Id,
                Order = order,
                OrderStatus = order.OrderStatus
            });
        }

        public void Trace(Dictionary<string, Order> orders)
        {
            var enumerator = orders.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                Order order = pair.Value;
                Trace(order);
            }
        }
    }
}