using System;
using AutoMapper;
using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockExchangeWeb.Data;
using StockExchangeWeb.Services.CacheService;
using StockExchangeWeb.Services.CacheService.Implementation;
using StockExchangeWeb.Services.ExchangeService;
using StockExchangeWeb.Services.HistoryService;
using StockExchangeWeb.Services.MarketTimesService;
using StockExchangeWeb.Services.MarketTimesService.MarketTimes;
using StockExchangeWeb.Services.OrderTracingService;
using StockExchangeWeb.Services.TradedEntitiesService;
using EasyCaching.Core.Serialization;
using EasyCaching.Redis;

namespace StockExchangeWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<DBContext>(opts =>
            {
                opts.UseNpgsql(Configuration["PostgresConnectionString"]);
            });

            // IF YOU WANT YOUR OWN TIMES THEN PLEASE CUSTOMIZE THE STRATEGY
            // ALL TIME IS IN UTC
            // THIS IS JUST AN EXAMPLE BELOW OF BUILTIN STRATEGY
            // Open on specific days and times
            // services.AddSingleton<IMarketOpeningTimesService, MarketOpeningTimesRepository>(provider =>
            // {
            //     TimesStrategy timesStrategy = new AllDaysSameOperatingHoursBasicStrategy
            //     (true, true, false, true, true
            //         // UTC
            //         , false, false, 
            //         new TimeSpan(3, 7, 0), 
            //         new TimeSpan(3, 14, 0));
            //     
            //     return new MarketOpeningTimesRepository(timesStrategy);
            // });
            // Open all the time
            services.AddSingleton<IMarketOpeningTimesService, MarketOpeningTimesRepository>();
            
            services.AddSingleton<IOrdersHistory, OrdersHistoryRepository>();
            services.AddHostedService<OrdersSynchroniserBackgroundService>();

            services.AddSingleton<OrderTraceRepository>();
            services.AddHostedService<OrderTracingDBPushBackgroundService>();
            
            services.AddSingleton<ISecuritiesProvider, SecuritiesProvider>();
            
            services.AddControllers();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.AddSingleton<IStockExchange, InMemoryStockExchangeRepository>();
            
            // Caching
            services.AddEasyCaching(opts =>
            {
                opts.UseRedis(config =>
                {
                    // TODO extract endpoint for appsettings.json
                    config.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));
                }, "redis1");

                opts.WithMessagePack();
            });
            services.AddSingleton<CacheStrategy, RedisCacheStrategy>();
            services.AddSingleton<IOrderCacheService, OrderCacheRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}