﻿using System;
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
            
            Dictionary<string, TrackedOrder> ordersWorked = GetOrdersToBeArchived();
            // TODO Lock Order table
            
            List<Order> pulledOrders = await PullExistingArchivedOrders(ordersWorked);
            List<Order> newOrders = GetToUpdateOrders(ordersWorked, pulledOrders);
            
            // TODO deal with unsuccessful transactions
            await ToDB(pulledOrders, newOrders);

            // TODO Unlock Order table
        }

        // Push back everything back up
        private async Task ToDB(List<Order> pulledOrders, List<Order> newOrders)
        {
            // Update existings
            foreach (var order in pulledOrders)
            {
                if (order != null)
                {
                    _dbContext.Entry(order).State = EntityState.Modified;
                    // _dbContext.Order.Update(order);
                }
            }
            
            // Add new
            await _dbContext.Order.AddRangeAsync(newOrders);
            
            // Push as transaction
            await _dbContext.SaveChangesAsync();
        }

        // - Pulls orders with ids of orders passed in
        // - Updates pulled orders from DB so they can be updated in DB
        // - Separates new orders to its list
        private List<Order> GetToUpdateOrders(Dictionary<string, TrackedOrder> ordersWorked,
            List<Order> pulledOrders)
        {
            List<Order> newOrders = new List<Order>();

            // Add updated orders
            foreach (var pulledOrder in pulledOrders)
            {
                if (pulledOrder != null)
                {
                    TrackedOrder order = ordersWorked[pulledOrder.Id];
                    order.ToBeUpdated = true;
                    pulledOrder.RealizeChanges(order.Order);
                }
            }

            // Add new orders
            foreach (var orderPair in ordersWorked)
            {
                if (!orderPair.Value.ToBeUpdated)
                    newOrders.Add(orderPair.Value.Order);
            }
            
            return newOrders;
        }

        private async Task<List<Order>> PullExistingArchivedOrders(Dictionary<string, TrackedOrder> ordersWorked)
        {
            return await _dbContext.Order
                .Where(x => ordersWorked.Keys.Contains(x.Id))
                .ToListAsync();
        }

        // - Locks all in-memory orders in history
        // - Copies all data 
        // - Clears the original history
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
                
                _ordersHistory._archivedOrders.Clear();
            }
            
            return ordersChanged;
        }

        private bool EligibleToStartSync()
        {
            lock (_ordersHistory._archivedOrders)
            {
                return _ordersHistory._archivedOrders.Count > 0;
            }
        }
    }
}