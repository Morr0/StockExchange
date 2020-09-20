using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockExchangeWeb.Data;
using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services.TradedEntitiesService
{
    public class SecuritiesProvider : ISecuritiesProvider
    {
        public SecuritiesProvider(IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            DBContext dbContext = scope.ServiceProvider.GetService<DBContext>();
                
            GetAllSecurities(dbContext);
        }

        private void GetAllSecurities(DBContext dbContext)
        {
            List<TradableSecurity> securities = dbContext.Security.AsNoTracking().ToList();

            foreach (var security in securities)
            {
                Securities.Add(security.Ticker, security);
            }
        }

        public Dictionary<string, TradableSecurity> Securities { get; private set; } 
            = new Dictionary<string, TradableSecurity>();
    }
}