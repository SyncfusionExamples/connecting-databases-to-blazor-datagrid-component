using Microsoft.EntityFrameworkCore;

namespace Grid_MySQL.Data
{
    /// <summary>
    /// DbContext for Transaction entity
    /// Manages database connections and entity configurations
    /// </summary>
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for Transaction entities
        /// </summary>
        public DbSet<TransactionModel> Transactions => Set<TransactionModel>();

        /// <summary>
        /// Configures the entity mappings and constraints
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Configure Transaction entity
            modelBuilder.Entity<TransactionModel>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Auto-increment for Primary Key
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Column configurations
                entity.Property(e => e.TransactionId)
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

                entity.Property(e => e.TransactionType)
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

                entity.Property(e => e.OrderId)
                    .IsRequired(false);

                // DateTime columns
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.CompletedAt)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                // Add indexes for frequently queried columns
                entity.HasIndex(e => e.TransactionId)
                    .HasDatabaseName("IX_TransactionId");

                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("IX_CustomerId");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_CreatedAt");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Status");

                // Table name (optional - by default uses DbSet property name)
                entity.ToTable("transactions");
            });
        }
    }
}
