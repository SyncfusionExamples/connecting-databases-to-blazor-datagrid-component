using Microsoft.EntityFrameworkCore;

namespace Grid_MariaDB.Data
{
    /// <summary>
    /// DbContext for Subscription entity
    /// Manages database connections and entity configurations
    /// </summary>
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for Subscription entities
        /// </summary>
        public DbSet<SubscriptionModel> Subscriptions => Set<SubscriptionModel>();

        /// <summary>
        /// Configures the entity mappings and constraints
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Configure Subscription entity
            modelBuilder.Entity<SubscriptionModel>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Auto-increment for Primary Key
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Column configurations
                entity.Property(e => e.PublicID)
                    .HasMaxLength(50)
                    .IsRequired(true);

                entity.Property(e => e.InvoiceNumber)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(3)
                    .HasDefaultValue("INR");

                entity.Property(e => e.SubscriptionType)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.PaymentGateway)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsRequired(false);

                // Decimal precision for currency
                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2);

                // Nullable integer columns
                entity.Property(e => e.CustomerId)
                    .IsRequired(false);

                entity.Property(e => e.SubscriptionID)
                    .IsRequired(false);

                // DateTime columns
                entity.Property(e => e.SubscriptionStartDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.SubscriptionEndDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                // Add indexes for frequently queried columns
                entity.HasIndex(e => e.PublicID)
                    .HasDatabaseName("IX_PublicID");

                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("IX_CustomerId");

                entity.HasIndex(e => e.SubscriptionStartDate)
                    .HasDatabaseName("IX_SubscriptionStartDate");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Status");

                // Table name (optional - by default uses DbSet property name)
                entity.ToTable("subscriptions");
            });
        }
    }
}
