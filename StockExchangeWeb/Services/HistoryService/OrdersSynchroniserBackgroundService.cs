using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockExchangeWeb.Data;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.HistoryService
{
    public class OrdersSynchroniserBackgroundService : BackgroundService
    {
        private OrdersHistoryRepository _ordersHistory;
        private IServiceScopeFactory _scopeFactory;
        private DBContext _dbContext;

        public OrdersSynchroniserBackgroundService(IOrdersHistory ordersHistory, IServiceScopeFactory scopeFactory)
        {
            _ordersHistory = ordersHistory as OrdersHistoryRepository;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        _dbContext = scope.ServiceProvider.GetService<DBContext>();
                        
                        await StartSynchronisationProcess();
                        // TODO handle errors and do retry
                        
                        // await _dbContext.DisposeAsync();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(250, stoppingToken);
            }
        }

        private async Task StartSynchronisationProcess()
        {
            if (!EligibleToStartSync())
                return;
            
            Console.WriteLine("Indeed eligible to update orders");

            // 1. Lock, copy, clear the history memory and unlock
            Dictionary<string, TrackedOrder> ordersWorked = GetOrdersToBeArchived();
            
            // TODO Lock Order table

            // 2. Lock table and pull ids in _ordersWorked, some or all maybe null
            // null ones are non-existent
            List<Order> pulledOrders = await PullExistingArchivedOrders(ordersWorked);
            (List<Order> toUpdateOrders, List<Order> newOrders) = GetToUpdateOrders(ordersWorked, pulledOrders);

            // 3. Update existing ids and insert new ones
            await TransactUpdatedExistingOrders(toUpdateOrders);
            await TransactNewOrders(newOrders);
            await PushTransaction();
            
            Console.WriteLine("Successfully pushed to DB");

            // Unlock
        }

        private async Task PushTransaction()
        {
            await _dbContext.SaveChangesAsync();
        }

        private async Task TransactNewOrders(List<Order> newOrders)
        {
            await _dbContext.Order.AddRangeAsync(newOrders);
        }

        private async Task TransactUpdatedExistingOrders(List<Order> toUpdateOrders)
        {
            _dbContext.Order.UpdateRange(toUpdateOrders);
            // await _dbContext.Order.AddRangeAsync(toUpdateOrders);
        }

        private (List<Order>, List<Order>) GetToUpdateOrders(Dictionary<string, TrackedOrder> ordersWorked,
            List<Order> pulledOrders)
        {
            List<Order> toUpdateOrders = new List<Order>();
            List<Order> newOrders = new List<Order>();

            if (pulledOrders.Count > 0)
                Console.WriteLine("Some pulled orders");
            
            // Add updated orders
            foreach (var pulledOrder in pulledOrders)
            {
                if (pulledOrder != null)
                {
                    TrackedOrder order = ordersWorked[pulledOrder.Id];
                    order.ToBeUpdated = true;
                    Order updatedPulledOrder = pulledOrder.RealizeChanges(order.Order);
                    toUpdateOrders.Add(updatedPulledOrder);
                }
                else
                {
                    Console.WriteLine("Null");
                }
            }

            // Add new orders
            foreach (var orderPair in ordersWorked)
            {
                if (!orderPair.Value.ToBeUpdated)
                    newOrders.Add(orderPair.Value.Order);
            }
            
            return (toUpdateOrders, newOrders);
        }

        private async Task<List<Order>> PullExistingArchivedOrders(Dictionary<string, TrackedOrder> ordersWorked)
        {
            var queryable = _dbContext.Order.AsTracking();
            foreach (var orderPair in ordersWorked)
            {
                string id = orderPair.Key;
                queryable = queryable.Where(order => order.Id == id);
            }
            
            return await queryable.ToListAsync();
        }

        private Dictionary<string, TrackedOrder> GetOrdersToBeArchived()
        {
            Dictionary<string, TrackedOrder> ordersChanged = null;
            lock (_ordersHistory._archivedOrders)
            {
                // Copy to another variable
                ordersChanged = new Dictionary<string, TrackedOrder>(_ordersHistory._archivedOrders.Count);
                foreach (var pair in _ordersHistory._archivedOrders)
                {
                    ordersChanged.Add(pair.Key, new TrackedOrder(pair.Value));
                }
                
                // Clear old 
                _ordersHistory._archivedOrders.Clear();
            }
            
            return ordersChanged;
        }

        private bool EligibleToStartSync()
        {
            Console.WriteLine("Is eligible to push history to database");
            return _ordersHistory._archivedOrders.Count > 0;
        }
    }
}