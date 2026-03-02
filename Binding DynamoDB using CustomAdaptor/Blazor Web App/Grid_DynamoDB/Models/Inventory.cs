using Amazon.DynamoDBv2.DataModel;

namespace Grid_DynamoDB.Models
{
    /// <summary>
    /// Represents an Inventory item stored in AWS DynamoDB
    /// Maps to the Inventory table with 16 columns for warehouse management
    /// </summary>
    [DynamoDBTable("Inventory")]
    public class Inventory
    {
        /// <summary>
        /// Gets or sets the Inventory identifier (Partition key)
        /// Example: "INV-001", "INV-002"
        /// </summary>
        [DynamoDBHashKey]
        public string InventoryID { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the product SKU
        /// Example: "SKU-001", "SKU-002"
        /// Enables efficient queries by warehouse and product
        /// </summary>
        [DynamoDBProperty]
        public string SKUID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product name
        /// Example: "Laptop Dell XPS"
        /// </summary>
        [DynamoDBProperty]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product category
        /// Example: "Electronics", "Accessories", "Supplies"
        /// </summary>
        [DynamoDBProperty]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current stock level
        /// Number of units currently available
        /// </summary>
        [DynamoDBProperty]
        public int CurrentStock { get; set; }

        /// <summary>
        /// Gets or sets the reserved stock count
        /// Number of units reserved for pending orders
        /// </summary>
        [DynamoDBProperty]
        public int ReservedStock { get; set; }

        /// <summary>
        /// Gets or sets the minimum threshold for reordering
        /// Alert trigger point when stock falls below this value
        /// </summary>
        [DynamoDBProperty]
        public int MinThreshold { get; set; }

        /// <summary>
        /// Gets or sets the date of last restock (ISO 8601 format)
        /// Example: "2026-02-15T10:00:00Z"
        /// </summary>
        [DynamoDBProperty]
        public string LastRestockDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the warehouse location or bin number
        /// Example: "A-01-01", "B-02-03"
        /// </summary>
        [DynamoDBProperty]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cost per unit in decimal format
        /// Used for inventory valuation
        /// </summary>
        [DynamoDBProperty]
        public decimal CostPerUnit { get; set; }

        /// <summary>
        /// Gets or sets the supplier name
        /// Source vendor for this product
        /// </summary>
        [DynamoDBProperty]
        public string Supplier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the standard reorder quantity
        /// Default quantity ordered when restocking
        /// </summary>
        [DynamoDBProperty]
        public int ReorderQuantity { get; set; }

        /// <summary>
        /// Gets or sets the product expiry date (ISO 8601 format)
        /// Example: "2027-12-31T00:00:00Z"
        /// Relevant for perishable items
        /// </summary>
        [DynamoDBProperty]
        public string ExpiryDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item status
        /// Valid values: "Active", "Discontinued", "Damaged", "Maintenance"
        /// </summary>
        [DynamoDBProperty]
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Gets or sets the date of last audit or physical count (ISO 8601 format)
        /// Example: "2026-02-17T08:30:00Z"
        /// </summary>
        [DynamoDBProperty]
        public string LastAuditDate { get; set; } = string.Empty;

    }
}
