using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockExchangeWeb.Data;

namespace StockExchangeWeb.Services.OrderTracingService
{
    public class OrderTracingDBPushBackgroundService : BackgroundService
    {
        private IServiceScopeFactory _scopeFactory;
        private OrderTraceRepository _traceService;
        
        public OrderTracingDBPushBackgroundService(IServiceScopeFactory scopeFactory, OrderTraceRepository orderTraceService)
        {
            _scopeFactory = scopeFactory;
            _traceService = orderTraceService;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        DBContext dbContext = scope.ServiceProvider.GetService<DBContext>();
                        
                        PushTraces(dbContext);
                        // TODO handle errors and do retry
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private void PushTraces(DBContext dbContext)
        {
            lock (_traceService._orderTraces)
            {
                dbContext.OrderTrace.AddRange(_traceService._orderTraces);
                dbContext.SaveChanges();
                _traceService._orderTraces.Clear();
            }
        }
    }
}