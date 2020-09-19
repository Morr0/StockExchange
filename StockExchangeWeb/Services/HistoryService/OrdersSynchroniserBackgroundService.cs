using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using StockExchangeWeb.Data;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.HistoryService
{
    public class OrdersSynchroniserBackgroundService : BackgroundService
    {
        private OrdersHistoryRepository _ordersHistory;
        private DBContext _dbContext;

        public OrdersSynchroniserBackgroundService(IOrdersHistory ordersHistory, DBContext dbContext)
        {
            _ordersHistory = ordersHistory as OrdersHistoryRepository;
            _dbContext = dbContext;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await StartSynchronisationProcess();
                    // TODO handle errors and do retry
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
            if (!EligableToStartSync())
                return;
            
            // 1. Lock, copy, clear the history memory and unlock
            Dictionary<string, Order> ordersWorked = GetOrdersToBeArchived();

            // 2. Lock table and pull ids in _ordersWorked, some or all maybe null
            // null ones are non-existent
            List<Order> pulledOrders = PullExistingArchivedOrders();
            (List<Order> toUpdateOrders, List<Order> newOrders) = GetToUpdateOrders(ordersWorked, pulledOrders);

            // 3. Update existing ids and insert new ones
            TransactUpdatedExistingOrders(toUpdateOrders);
            TransactNewOrders();
            PushTransaction();

            // Unlock
        }

        private void PushTransaction()
        {
            // TODO implement
        }

        private void TransactNewOrders()
        {
            // TODO implement
        }

        private void TransactUpdatedExistingOrders(List<Order> toUpdateOrders)
        {
            // TODO implement
        }

        private (List<Order>, List<Order>) GetToUpdateOrders(Dictionary<string, Order> ordersWorked, List<Order> pulledOrders)
        {
            // TODO implement
            return (null, null);
        }

        private List<Order> PullExistingArchivedOrders()
        {
            // TODO implement
            return null;
        }

        private Dictionary<string, Order> GetOrdersToBeArchived()
        {
            // TODO implement
            return null;
        }

        private bool EligableToStartSync()
        {
            // TODO implement
            return false;
        }
    }
}