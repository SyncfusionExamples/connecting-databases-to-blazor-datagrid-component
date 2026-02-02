using System.ComponentModel.DataAnnotations;

namespace Grid_PostgreSQL.Data
{
    /// <summary>
    /// Represents a purchase order record mapped to the 'PurchaseOrder' table in PostgreSQL.
    /// This model defines the structure of purchase order data used throughout the application.
    /// </summary>
    public class PurchaseOrder
    {
        /// <summary>
        /// Gets or sets the unique identifier for the purchase order record.
        /// Auto-generated primary key using SERIAL sequence.
        /// </summary>
        [Key]
        public int PurchaseOrderId { get; set; }

        /// <summary>
        /// Gets or sets the public-facing purchase order number (e.g., PO-2026-001).
        /// This is a unique identifier for external reference and tracking.
        /// </summary>
        public string? PoNumber { get; set; }

        /// <summary>
        /// Gets or sets the vendor identifier associated with this purchase order.
        /// Links to the vendor providing the items.
        /// </summary>
        public string? VendorID { get; set; }

        /// <summary>
        /// Gets or sets the name or description of the item being purchased.
        /// </summary>
        public string? ItemName { get; set; }

        /// <summary>
        /// Gets or sets the category of the item (e.g., Electronics, Office Supplies, Hardware).
        /// Helps organize and classify purchases.
        /// </summary>
        public string? ItemCategory { get; set; }

        /// <summary>
        /// Gets or sets the quantity of items being ordered.
        /// Must be a positive integer value.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price of each item.
        /// Stored as NUMERIC(12,2) for precise decimal calculations.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the total amount for the purchase order (Quantity × UnitPrice).
        /// Stored as NUMERIC(14,2) to accommodate larger values.
        /// Automatically calculated from Quantity and UnitPrice.
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the current status of the purchase order.
        /// Possible values: Pending, Approved, Ordered, Received, Cancelled, Completed.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who created/ordered the purchase order.
        /// </summary>
        public string? OrderedBy { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who approved the purchase order.
        /// </summary>
        public string? ApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when the purchase order was placed.
        /// </summary>
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the expected delivery date for the ordered items.
        /// </summary>
        public DateTime? ExpectedDeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets the timestamp indicating when the purchase order record was created.
        /// Automatically set to current timestamp when the record is inserted.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp indicating when the purchase order record was last updated.
        /// Automatically updated to current timestamp whenever the record is modified.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
