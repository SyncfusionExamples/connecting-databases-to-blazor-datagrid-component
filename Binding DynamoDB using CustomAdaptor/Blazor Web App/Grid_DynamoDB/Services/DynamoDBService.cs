using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Grid_DynamoDB.Models;

namespace Grid_DynamoDB.Services
{
    /// <summary>
    /// Service class for interacting with AWS DynamoDB
    /// Handles all CRUD operations, queries, and data transformations for Inventory items
    /// </summary>
    public class DynamoDBService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private readonly DynamoDBContext _dynamoDBContext;
        private readonly string _tableName;
        private readonly ILogger<DynamoDBService> _logger;
        
        // Constants for InventoryID auto-generation
        private const string InventoryIdPrefix = "INV-";
        private const int InventoryIdStartNumber = 1;

        /// <summary>
        /// Initializes a new instance of the DynamoDBService
        /// </summary>
        /// <param name="dynamoDBClient">AWS DynamoDB client instance</param>
        /// <param name="configuration">Application configuration for table name</param>
        /// <param name="logger">Logger for diagnostic information</param>
        public DynamoDBService(IAmazonDynamoDB dynamoDBClient, IConfiguration configuration, ILogger<DynamoDBService> logger)
        {
            _dynamoDBClient = dynamoDBClient;
            _dynamoDBContext = new DynamoDBContext(dynamoDBClient);
            _tableName = configuration["AWS:TableName"] ?? "Inventory";
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all inventory items from DynamoDB
        /// </summary>
        /// <returns>List of all inventory items</returns>
        public async Task<List<Inventory>> GetAllInventoriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all inventory items from DynamoDB");
                var items = await _dynamoDBContext.ScanAsync<Inventory>(new List<ScanCondition>()).GetRemainingAsync();

                // Sort by InventoryID to display in arranged format (INV-001, INV-002, etc.)
                var sortedItems = items.OrderBy(x => x.InventoryID).ToList();

                _logger.LogInformation($"Retrieved {items.Count} inventory items");
                return sortedItems;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching inventory items: {ex.Message}");
                throw new Exception($"Error fetching inventory items: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generates a unique InventoryID based on existing inventory items
        /// </summary>
        /// <returns>A new unique InventoryID in the format INV-001, INV-002, etc.</returns>
        private async Task<string> GenerateInventoryIdAsync()
        {
            var existingInventories = await GetAllInventoriesAsync();
            int maxNumber = existingInventories
                .Where(inventory => !string.IsNullOrEmpty(inventory.InventoryID) && inventory.InventoryID.StartsWith(InventoryIdPrefix))
                .Select(inventory =>
                {
                    string numberPart = inventory.InventoryID.Substring(InventoryIdPrefix.Length);
                    if (int.TryParse(numberPart, out int number))
                        return number;
                    return 0;
                })
                .DefaultIfEmpty(InventoryIdStartNumber - 1)
                .Max();

            int nextNumber = maxNumber + 1;
            string newInventoryId = $"{InventoryIdPrefix}{nextNumber:D3}";
            return newInventoryId;
        }

        /// <summary>
        /// Inserts a new inventory item into DynamoDB
        /// </summary>
        /// <param name="inventory">The inventory item to insert</param>
        /// <returns>The inserted inventory item</returns>
        public async Task<Inventory> InsertInventoryAsync(Inventory inventory)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inventory.SKUID))
                {
                    throw new ArgumentException("SKUID is required");
                }

                // Auto-generate InventoryID if not provided
                if (string.IsNullOrWhiteSpace(inventory.InventoryID))
                {
                    inventory.InventoryID = await GenerateInventoryIdAsync();
                    _logger.LogInformation($"Auto-generated InventoryID: {inventory.InventoryID}");
                }

                // Set audit timestamps
                inventory.LastAuditDate = DateTime.UtcNow.ToString("O");
                if (string.IsNullOrWhiteSpace(inventory.LastRestockDate))
                {
                    inventory.LastRestockDate = DateTime.UtcNow.ToString("O");
                }

                _logger.LogInformation($"Inserting inventory item: Inventory={inventory.InventoryID}, SKU={inventory.SKUID}");
                await _dynamoDBContext.SaveAsync(inventory);
                _logger.LogInformation("Inventory item inserted successfully");
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inserting inventory item: {ex.Message}");
                throw new Exception($"Error inserting inventory item: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an existing inventory item in DynamoDB
        /// </summary>
        /// <param name="inventory">The inventory item with updated values</param>
        /// <returns>True if update was successful</returns>
        public async Task<bool> UpdateInventoryAsync(Inventory inventory)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inventory.InventoryID) || string.IsNullOrWhiteSpace(inventory.SKUID))
                {
                    throw new ArgumentException("InventoryId and SKUID are required");
                }

                // Update audit timestamp
                inventory.LastAuditDate = DateTime.UtcNow.ToString("O");

                _logger.LogInformation($"Updating inventory item: Inventory={inventory.InventoryID}, SKU={inventory.SKUID}");
                await _dynamoDBContext.SaveAsync(inventory);
                _logger.LogInformation("Inventory item updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating inventory item: {ex.Message}");
                throw new Exception($"Error updating inventory item: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes an inventory item from DynamoDB
        /// </summary>
        /// <param name="warehouseId">The warehouse ID (partition key)</param>
        /// <param name="skuId">The SKU ID (sort key)</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteInventoryAsync(string? inventoryId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inventoryId))
                {
                    throw new ArgumentException("InventoryId is required");
                }

                _logger.LogInformation($"Deleting inventory item: InventoryId={inventoryId}");
                await _dynamoDBContext.DeleteAsync<Inventory>(inventoryId);
                _logger.LogInformation("Inventory item deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting inventory item: {ex.Message}");
                throw new Exception($"Error deleting inventory item: {ex.Message}", ex);
            }
        }
    }
}
