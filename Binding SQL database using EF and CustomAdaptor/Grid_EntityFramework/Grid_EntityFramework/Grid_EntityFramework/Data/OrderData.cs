using Microsoft.EntityFrameworkCore;

namespace Grid_EntityFramework.Data
{
    public class OrderData
    {
        //TODO: Enter the connection string of database
        string ConnectionString = $"<Enter Your connection string here>";
        public async Task<List<Order>> GetOrdersAsync()
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                // Retrieve orders from the Orders DbSet and convert to list asynchronously
                var orders = await context.Orders.ToListAsync();
                return orders; 
            }
        }
        public async Task AddOrderAsync(Order Value)
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                // Add the provided order to the Orders DbSet
                context.Orders.Add(Value);
                // Save changes asynchronously to the database
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateOrderAsync(Order Value)
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                // Update the provided order in the Orders DbSet
                context.Orders.Update(Value);
                // Save changes asynchronously to the database
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveOrderAsync(int? Key)
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                var Order = await context.Orders.FindAsync(Key);
                if (Order != null)
                {
                    // Remove the order from the Orders DbSet
                    context.Orders.Remove(Order);
                    // Save changes asynchronously to the database
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    public class Order
    {
        public int? OrderID { get; set; }
        public string CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public decimal Freight { get; set; }
        public string ShipCity { get; set; }
    }

    public class OrderDbContext : DbContext
    {
        private readonly string _ConnectionString;

        // Constructor to initialize the DbContext with a connection string
        public OrderDbContext(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure DbContext to use SQL Server with the provided connection string
            optionsBuilder.UseSqlServer(_ConnectionString);
        }

        // DbSet representing a collection of Order entities in the database
        public DbSet<Order> Orders { get; set; }
    }
}
