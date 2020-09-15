using Microsoft.EntityFrameworkCore;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Data
{
    public class DBContext : DbContext
    {
        public DbSet<Order> Order { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO Use do not hardcode
            optionsBuilder.UseNpgsql("Host=localhost;Database=SecuritiesExchange;Username=postgres;Password=root");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Order model, indexes excluding primary key
            modelBuilder.Entity<Order>()
                .HasIndex(model => model.OrderStatus);

        }
    }
}