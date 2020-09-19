using Microsoft.EntityFrameworkCore;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Data
{
    public class DBContext : DbContext
    {
        public DbSet<Order> Order { get; set; }

        public DBContext(DbContextOptions opts) : base(opts)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Order model, indexes excluding primary key
            modelBuilder.Entity<Order>()
                .HasIndex(model => model.OrderStatus);

        }
    }
}