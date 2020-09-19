using Microsoft.EntityFrameworkCore;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Models.OrderTrace;

namespace StockExchangeWeb.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions opts) : base(opts)
        {
            
        }
        
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderTrace> OrderTrace { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Order model, indexes excluding primary key
            modelBuilder.Entity<Order>()
                .HasIndex(model => model.OrderStatus);
            
            // Order trace model
            modelBuilder.Entity<OrderTrace>()
                .HasIndex(model => model.OrderId);

        }
    }
}