using System;
using System.Reflection;

namespace StockExchangeWeb.Models.Orders
{
    public static class OrderExtensions
    {
        public static void RealizeChanges(this Order oldOrder, Order currentOrder)
        {
            Type currentOrderType = currentOrder.GetType();
            PropertyInfo[] props = oldOrder.GetType().GetProperties();
            
            foreach (var prop in props)
            {
                object oldVal = prop.GetValue(oldOrder);
                object currentVal = prop.GetValue(currentOrder);
                
                if (oldVal != currentVal)
                    prop.SetValue(oldOrder, currentVal);
            }
        }
    }
}