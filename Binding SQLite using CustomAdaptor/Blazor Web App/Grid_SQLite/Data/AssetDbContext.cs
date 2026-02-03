using Microsoft.EntityFrameworkCore;

namespace Grid_SQLite.Data
{
    /// <summary>
    /// DbContext for Asset entity
    /// Manages database connections and entity configurations for SQLite
    /// </summary>
    public class AssetDbContext : DbContext
    {
        public AssetDbContext(DbContextOptions<AssetDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for Asset entities
        /// </summary>
        public DbSet<Asset> Assets => Set<Asset>();

        /// <summary>
        /// Configures the entity mappings and constraints
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Asset>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AssetID)
                    .HasMaxLength(20)
                    .IsRequired(true);

                entity.Property(e => e.AssetName)
                    .HasMaxLength(255)
                    .IsRequired(true);

                entity.Property(e => e.AssetType)
                    .HasMaxLength(100)
                    .IsRequired(true);

                entity.Property(e => e.Model)
                    .HasMaxLength(150)
                    .IsRequired(false);

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(100)
                    .IsRequired(true);

                entity.Property(e => e.InvoiceID)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.AssignedTo)
                    .HasMaxLength(150)
                    .IsRequired(false);

                entity.Property(e => e.Department)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("DATE")
                    .IsRequired(false);

                entity.Property(e => e.PurchaseCost)
                    .HasPrecision(12, 2)
                    .IsRequired(false);

                entity.Property(e => e.WarrantyExpiry)
                    .HasColumnType("DATE")
                    .IsRequired(false);

                entity.Property(e => e.Condition)
                    .HasMaxLength(50)
                    .IsRequired(false)
                    .HasDefaultValue("New");

                entity.Property(e => e.LastMaintenance)
                    .HasColumnType("DATE")
                    .IsRequired(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .HasDefaultValue("Available");

                entity.ToTable("asset");
            });
        }
    }
}
