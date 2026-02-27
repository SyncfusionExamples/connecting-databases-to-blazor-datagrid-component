using Microsoft.EntityFrameworkCore;

namespace Web_API.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Explicitly map DbSet<Order> to the [Order] table
            modelBuilder.Entity<Order>().ToTable("Order");
        }
    }
}
