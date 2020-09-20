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
            // Order model
            modelBuilder.Entity<Order>()
                .HasIndex(model => model.OrderStatus);

            modelBuilder.Entity<Order>()
                .Property(model => model.OrderStatus)
                .HasConversion<int>();

            // Order trace model
            // modelBuilder.Entity<OrderTrace>()
            //     .HasIndex(model => model.OrderId);
            
            modelBuilder.Entity<OrderTrace>()
                .Property(model => model.OrderStatus)
                .HasConversion<int>();

        }
    }
}