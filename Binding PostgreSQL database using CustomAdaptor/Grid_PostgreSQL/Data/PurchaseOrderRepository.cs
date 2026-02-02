using Microsoft.EntityFrameworkCore;

namespace Grid_PostgreSQL.Data
{
    /// <summary>
    /// Repository pattern implementation for PurchaseOrder entity using Entity Framework Core
    /// Handles all CRUD operations and business logic for the Purchase Order Management System
    /// Provides a data access abstraction layer between the application and PostgreSQL database
    /// </summary>
    public class PurchaseOrderRepository
    {
        private readonly PurchaseOrderDbContext _context;

        /// <summary>
        /// Initializes a new instance of the PurchaseOrderRepository class.
        /// </summary>
        /// <param name="context">The DbContext instance for database operations</param>
        public PurchaseOrderRepository(PurchaseOrderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all purchase orders from the database ordered by ID in descending order (newest first).
        /// </summary>
        /// <returns>List of all purchase orders</returns>
        /// <exception cref="Exception">Thrown when database operation fails</exception>
        public async Task<List<PurchaseOrder>> GetPurchaseOrdersDataAsync()
        {
            try
            {
                return await _context.PurchaseOrders
                    .OrderByDescending(p => p.PurchaseOrderId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving purchase orders: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a new purchase order to the database.
        /// Automatically generates a unique PoNumber and sets creation timestamps.
        /// Calculates TotalAmount from Quantity and UnitPrice if not provided.
        /// </summary>
        /// <param name="value">The purchase order model to add</param>
        /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
        /// <exception cref="DbUpdateException">Thrown when database update fails</exception>
        /// <exception cref="Exception">Thrown for other database errors</exception>
        public async Task AddPurchaseOrderAsync(PurchaseOrder value)
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Purchase order cannot be null");

                // Generate unique PoNumber if not provided
                if (string.IsNullOrEmpty(value.PoNumber))
                {
                    value.PoNumber = await GeneratePoNumberAsync();
                }

                // Calculate TotalAmount from Quantity and UnitPrice
                if (value.Quantity > 0 && value.UnitPrice > 0)
                {
                    value.TotalAmount = value.Quantity * value.UnitPrice;
                }

                // Set default status if not provided
                if (string.IsNullOrEmpty(value.Status))
                {
                    value.Status = "Pending";
                }

                // Set timestamps
                if (value.CreatedAt == null)
                    value.CreatedAt = DateTime.Now;

                if (value.UpdatedAt == null)
                    value.UpdatedAt = DateTime.Now;

                // Add the purchase order to the context
                _context.PurchaseOrders.Add(value);

                // Save changes to the database
                await _context.SaveChangesAsync();

                Console.WriteLine($"Purchase order '{value.PoNumber}' added successfully");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while adding purchase order: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding purchase order: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing purchase order in the database.
        /// Validates the purchase order exists before updating.
        /// Recalculates TotalAmount and updates the modification timestamp.
        /// </summary>
        /// <param name="value">The purchase order model with updated values</param>
        /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
        /// <exception cref="ArgumentException">Thrown when PurchaseOrderId is invalid</exception>
        /// <exception cref="KeyNotFoundException">Thrown when purchase order is not found</exception>
        /// <exception cref="DbUpdateException">Thrown when database update fails</exception>
        /// <exception cref="Exception">Thrown for other database errors</exception>
        public async Task UpdatePurchaseOrderAsync(PurchaseOrder value)
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Purchase order cannot be null");

                if (value.PurchaseOrderId <= 0)
                    throw new ArgumentException("Purchase order ID must be valid", nameof(value.PurchaseOrderId));

                // Find the existing purchase order
                var existingPurchaseOrder = await _context.PurchaseOrders.FindAsync(value.PurchaseOrderId);
                if (existingPurchaseOrder == null)
                    throw new KeyNotFoundException($"Purchase order with ID {value.PurchaseOrderId} not found");

                // Update all properties from the provided value
                existingPurchaseOrder.PoNumber = value.PoNumber;
                existingPurchaseOrder.VendorID = value.VendorID;
                existingPurchaseOrder.ItemName = value.ItemName;
                existingPurchaseOrder.ItemCategory = value.ItemCategory;
                existingPurchaseOrder.Quantity = value.Quantity;
                existingPurchaseOrder.UnitPrice = value.UnitPrice;
                existingPurchaseOrder.Status = value.Status;
                existingPurchaseOrder.OrderedBy = value.OrderedBy;
                existingPurchaseOrder.ApprovedBy = value.ApprovedBy;
                existingPurchaseOrder.OrderDate = value.OrderDate;
                existingPurchaseOrder.ExpectedDeliveryDate = value.ExpectedDeliveryDate;

                existingPurchaseOrder.TotalAmount = existingPurchaseOrder.Quantity * existingPurchaseOrder.UnitPrice;

                // Update the modification timestamp
                existingPurchaseOrder.UpdatedAt = DateTime.Now;

                // Save changes to the database
                await _context.SaveChangesAsync();

                Console.WriteLine($"Purchase order '{existingPurchaseOrder.PoNumber}' updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency error while updating purchase order: {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while updating purchase order: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating purchase order: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a purchase order from the database.
        /// Validates the purchase order ID before deletion.
        /// </summary>
        /// <param name="key">The purchase order ID to delete</param>
        /// <exception cref="ArgumentException">Thrown when key is null or invalid</exception>
        /// <exception cref="KeyNotFoundException">Thrown when purchase order is not found</exception>
        /// <exception cref="DbUpdateException">Thrown when database update fails</exception>
        /// <exception cref="Exception">Thrown for other database errors</exception>
        public async Task RemovePurchaseOrderAsync(int? key)
        {
            try
            {
                if (key == null || key <= 0)
                    throw new ArgumentException("Purchase order ID cannot be null or invalid", nameof(key));

                // Find the purchase order to delete
                var purchaseOrder = await _context.PurchaseOrders.FindAsync(key);
                if (purchaseOrder == null)
                    throw new KeyNotFoundException($"Purchase order with ID {key} not found");

                // Remove the purchase order from the context
                _context.PurchaseOrders.Remove(purchaseOrder);

                // Save changes to the database
                await _context.SaveChangesAsync();

                Console.WriteLine($"Purchase order '{purchaseOrder.PoNumber}' deleted successfully");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while deleting purchase order: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting purchase order: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates a unique purchase order number based on the current count and date.
        /// Format: PO-YYYY-XXXX (e.g., PO-2026-0001, PO-2026-0002)
        /// </summary>
        /// <returns>A unique purchase order number</returns>
        /// <exception cref="Exception">Thrown when database query fails</exception>
        private async Task<string> GeneratePoNumberAsync()
        {
            try
            {
                // Get the current year
                int currentYear = DateTime.Now.Year;

                // Count existing purchase orders for this year
                int count = await _context.PurchaseOrders
                    .Where(p => p.OrderDate.HasValue && p.OrderDate.Value.Year == currentYear)
                    .CountAsync();

                // Generate the next sequential number
                int nextNumber = count + 1;

                // Format: PO-YYYY-XXXX
                string poNumber = $"PO-{currentYear}-{nextNumber:D4}";

                // Ensure uniqueness (backup check)
                while (await _context.PurchaseOrders.AnyAsync(p => p.PoNumber == poNumber))
                {
                    nextNumber++;
                    poNumber = $"PO-{currentYear}-{nextNumber:D4}";
                }

                return poNumber;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PoNumber: {ex.Message}");
                throw;
            }
        }

    }
}
