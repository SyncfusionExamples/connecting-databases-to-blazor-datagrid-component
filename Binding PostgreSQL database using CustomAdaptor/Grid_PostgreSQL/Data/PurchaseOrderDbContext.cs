using Microsoft.EntityFrameworkCore;

namespace Grid_PostgreSQL.Data
{
    /// <summary>
    /// DbContext for Purchase Order entity
    /// Manages database connections and entity configurations for the Purchase Order Management System
    /// This context bridges the application with PostgreSQL database
    /// </summary>
    public class PurchaseOrderDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the PurchaseOrderDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by a DbContext</param>
        public PurchaseOrderDbContext(DbContextOptions<PurchaseOrderDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet for Purchase Order entities.
        /// Represents the collection of all purchase orders in the database.
        /// </summary>
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

        /// <summary>
        /// Configures the entity mappings, constraints, and database-specific configurations
        /// This method is called by Entity Framework Core during model creation.
        /// </summary>
        /// <param name="modelBuilder">Provides a simple API for configuring the EF model</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PurchaseOrder entity
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                // Set table name and schema
                entity.ToTable("purchaseorder", schema: "public");

                // ===== PRIMARY KEY CONFIGURATION =====
                entity.HasKey(e => e.PurchaseOrderId)
                    .HasName("pk_purchaseorder_id");

                // Auto-increment for Primary Key (SERIAL type in PostgreSQL)
                entity.Property(e => e.PurchaseOrderId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("purchaseorderid")
                    .HasColumnType("integer");

                // ===== COLUMN CONFIGURATIONS =====

                // PoNumber - Unique public-facing identifier
                entity.Property(e => e.PoNumber)
                    .HasColumnName("ponumber")
                    .HasColumnType("character varying(30)")
                    .HasMaxLength(30)
                    .IsRequired(true);

                // Add unique constraint for PoNumber
                entity.HasIndex(e => e.PoNumber)
                    .IsUnique()
                    .HasDatabaseName("uq_purchaseorder_ponumber");

                // VendorID - Vendor reference
                entity.Property(e => e.VendorID)
                    .HasColumnName("vendorid")
                    .HasColumnType("character varying(50)")
                    .HasMaxLength(50)
                    .IsRequired(true);

                // ItemName - Item description
                entity.Property(e => e.ItemName)
                    .HasColumnName("itemname")
                    .HasColumnType("character varying(200)")
                    .HasMaxLength(200)
                    .IsRequired(true);

                // ItemCategory - Item classification
                entity.Property(e => e.ItemCategory)
                    .HasColumnName("itemcategory")
                    .HasColumnType("character varying(100)")
                    .HasMaxLength(100)
                    .IsRequired(false);

                // Quantity - Order quantity (must be positive)
                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("integer")
                    .IsRequired(true);

                // UnitPrice - Price per unit with decimal precision
                entity.Property(e => e.UnitPrice)
                    .HasColumnName("unitprice")
                    .HasColumnType("numeric(12,2)")
                    .HasPrecision(12, 2)
                    .IsRequired(true);

                // TotalAmount - Total cost (Quantity × UnitPrice)
                entity.Property(e => e.TotalAmount)
                    .HasColumnName("totalamount")
                    .HasColumnType("numeric(14,2)")
                    .HasPrecision(14, 2)
                    .IsRequired(false);

                // Status - Purchase order status
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("character varying(30)")
                    .HasMaxLength(30)
                    .IsRequired(true)
                    .HasDefaultValue("Pending");

                // OrderedBy - Person who created the order
                entity.Property(e => e.OrderedBy)
                    .HasColumnName("orderedby")
                    .HasColumnType("character varying(100)")
                    .HasMaxLength(100)
                    .IsRequired(true);

                // ApprovedBy - Person who approved the order
                entity.Property(e => e.ApprovedBy)
                    .HasColumnName("approvedby")
                    .HasColumnType("character varying(100)")
                    .HasMaxLength(100)
                    .IsRequired(false);

                // OrderDate - Date when PO was placed
                entity.Property(e => e.OrderDate)
                    .HasColumnName("orderdate")
                    .HasColumnType("date")
                    .IsRequired(true);

                // ExpectedDeliveryDate - Expected delivery date
                entity.Property(e => e.ExpectedDeliveryDate)
                    .HasColumnName("expecteddeliverydate")
                    .HasColumnType("date")
                    .IsRequired(false);

                // CreatedAt - Record creation timestamp
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdat")
                    .HasColumnType("timestamp without time zone")
                    .IsRequired(true)
                    .HasDefaultValueSql("NOW()");

                // UpdatedAt - Record modification timestamp
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updatedat")
                    .HasColumnType("timestamp without time zone")
                    .IsRequired(true)
                    .HasDefaultValueSql("NOW()");

                // ===== INDEX CONFIGURATIONS =====
                // Indexes improve query performance on frequently queried columns

                // Index on Status for filtering by status
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("ix_purchaseorder_status");

                // Index on VendorID for vendor-based queries
                entity.HasIndex(e => e.VendorID)
                    .HasDatabaseName("ix_purchaseorder_vendorid");

                // Index on OrderDate for date-range queries
                entity.HasIndex(e => e.OrderDate)
                    .HasDatabaseName("ix_purchaseorder_orderdate");

                // Index on OrderedBy for filtering by requester
                entity.HasIndex(e => e.OrderedBy)
                    .HasDatabaseName("ix_purchaseorder_orderedby");

                // Index on CreatedAt for sorting and filtering by creation date
                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("ix_purchaseorder_createdat");

                // Composite index on Status and OrderDate for common filter combinations
                entity.HasIndex(e => new { e.Status, e.OrderDate })
                    .HasDatabaseName("ix_purchaseorder_status_orderdate");
            });
        }
    }
}
