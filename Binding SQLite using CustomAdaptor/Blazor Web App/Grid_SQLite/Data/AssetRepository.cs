using Microsoft.EntityFrameworkCore;

namespace Grid_SQLite.Data
{
    /// <summary>
    /// Repository pattern implementation for Asset entity using Entity Framework Core
    /// Handles all CRUD operations and business logic for assets
    /// </summary>
    public class AssetRepository
    {
        private readonly AssetDbContext _context;
        private const string PublicAssetIdPrefix = "AST";

        public AssetRepository(AssetDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all assets from the database ordered by AssetID descending
        /// </summary>
        /// <returns>List of all assets</returns>
        public async Task<List<Asset>> GetAssetsAsync()
        {
            try
            {
                return await _context.Assets
                    .OrderByDescending(a => a.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving assets: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates a temporary asset ID with AST prefix for initial database insert
        /// This temporary ID will be replaced with the final ID after the record is saved
        /// Format: AST-99999 (placeholder sequence number)
        /// </summary>
        /// <returns>Temporary asset ID</returns>
        private string GenerateTemporaryAssetId()
        {
            try
            {
                string temporaryAssetId = $"{PublicAssetIdPrefix}-99999";
                return temporaryAssetId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating temporary asset ID: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates the final unique asset ID based on the actual database ID
        /// This is called after the asset is inserted and receives its primary key
        /// Format: AST-XXXXX (e.g., AST-001, AST-002, AST-1000)
        /// Using the database ID ensures no duplicates even if records are deleted
        /// </summary>
        /// <param name="databaseId">The actual database primary key ID of the inserted asset</param>
        /// <returns>Final asset ID</returns>
        private string GenerateAssetId(int databaseId)
        {
            try
            {
                string assetId = $"{PublicAssetIdPrefix}-{databaseId:D3}";
                return assetId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating final asset ID: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates a unique serial number for a new asset
        /// Format: SN-XXX-YYYY-ZZZZ
        /// Where:
        /// - SN = static prefix
        /// - XXX = first 3 letters of AssetName (uppercase)
        /// - YYYY = purchase year from PurchaseDate
        /// - ZZZZ = asset ID number (extracted from AssetID, e.g., AST-001 -> 001)
        /// </summary>
        /// <param name="assetName">The name of the asset</param>
        /// <param name="purchaseDate">The purchase date of the asset</param>
        /// <param name="assetId">The generated asset ID</param>
        /// <returns>Generated serial number</returns>
        private string GenerateSerialNumber(string assetType, DateTime? purchaseDate, string assetId)
        {
            try
            {
                string assetNamePrefix = string.IsNullOrWhiteSpace(assetType) 
                    ? "UNK" 
                    : assetType.Substring(0, Math.Min(3, assetType.Length)).ToUpper();

                int purchaseYear = purchaseDate?.Year ?? DateTime.Now.Year;

                string assetIdNumber = assetId.Contains("-") 
                    ? assetId.Split('-')[1] 
                    : assetId;

                string serialNumber = $"SN-{assetNamePrefix}-{purchaseYear}-{assetIdNumber}";

                return serialNumber;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating serial number: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a new asset to the database
        /// Automatically generates AssetID and SerialNumber if not provided
        /// </summary>
        /// <param name="asset">The asset model to add</param>
        public async Task AddAssetAsync(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset), "Asset cannot be null");

            if (string.IsNullOrWhiteSpace(asset.AssetID))
            {
                asset.AssetID = GenerateTemporaryAssetId();
            }

            if (string.IsNullOrWhiteSpace(asset.SerialNumber))
            {
                asset.SerialNumber = GenerateSerialNumber(asset.AssetType, asset.PurchaseDate, asset.AssetID);
            }

            if (string.IsNullOrWhiteSpace(asset.Condition))
                asset.Condition = "New";

            if (string.IsNullOrWhiteSpace(asset.Status))
                asset.Status = "Available";

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            string finalAssetId = GenerateAssetId(asset.Id);
            asset.AssetID = finalAssetId;
            asset.SerialNumber = GenerateSerialNumber(asset.AssetType, asset.PurchaseDate, finalAssetId);

            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// UPDATE Operation: Modifies an existing asset in the database.
        /// This method is called when the user clicks the "Edit" button, modifies field values,
        /// and clicks the update button in toolbar.
        /// </summary>
        /// <param name="asset">The asset object with updated values.</param>
        /// <exception cref="ArgumentNullException">Thrown if the transaction object is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the transaction to update does not exist in the database.</exception>
        public async Task UpdateAssetAsync(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset), "Asset cannot be null");

            var existingAsset = await _context.Assets.FindAsync(asset.Id);
            if (existingAsset == null)
                throw new KeyNotFoundException($"Asset with ID {asset.Id} not found");

            existingAsset.AssetName = asset.AssetName;
            existingAsset.AssetType = asset.AssetType;
            existingAsset.Model = asset.Model;
            existingAsset.SerialNumber = asset.SerialNumber;
            existingAsset.InvoiceID = asset.InvoiceID;
            existingAsset.AssignedTo = asset.AssignedTo;
            existingAsset.Department = asset.Department;
            existingAsset.PurchaseDate = asset.PurchaseDate;
            existingAsset.PurchaseCost = asset.PurchaseCost;
            existingAsset.WarrantyExpiry = asset.WarrantyExpiry;
            existingAsset.Condition = asset.Condition;
            existingAsset.LastMaintenance = asset.LastMaintenance;
            existingAsset.Status = asset.Status;

            _context.Assets.Update(existingAsset);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an asset from the database
        /// </summary>
        /// <param name="key">The asset ID to delete</param>
        public async Task RemoveAssetAsync(int? key)
        {
            try
            {
                if (key == null || key <= 0)
                    throw new ArgumentException("Asset ID cannot be null or invalid", nameof(key));

                var asset = await _context.Assets.FindAsync(key);
                if (asset == null)
                    throw new KeyNotFoundException($"Asset with ID {key} not found");

                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while deleting asset: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting asset: {ex.Message}");
                throw;
            }
        }
    }
}
