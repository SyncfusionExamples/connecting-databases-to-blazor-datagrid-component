using Microsoft.EntityFrameworkCore;

namespace Grid_MySQL.Data
{
    /// <summary>
    /// Repository pattern implementation for Transaction entity using Entity Framework Core
    /// Handles all CRUD operations and business logic for transactions
    /// </summary>
    public class TransactionRepository
    {
        private readonly TransactionDbContext _context;
        private const string PublicTransactionIdPrefix = "TXN";

        public TransactionRepository(TransactionDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all transactions from the database ordered by ID descending
        /// </summary>
        /// <returns>List of all transactions</returns>
        public async Task<List<TransactionModel>> GetTransactionsAsync()
        {
            try
            {
                return await _context.Transactions
                    .OrderByDescending(t => t.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving transactions: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates a unique public transaction ID
        /// Format: TXN + YYMMDD (from CreatedAt) + 5-digit ID
        /// Example: TXN26010901000 (Year 26, Month 01, Day 09, ID 01000)
        /// </summary>
        /// <param name="createdAtDate">The creation date for the transaction</param>
        /// <param name="primaryKeyId">The primary key ID</param>
        /// <returns>Generated transaction ID</returns>
        private string GeneratePublicTransactionId(DateTime? createdAtDate, int primaryKeyId)
        {
            try
            {
                DateTime dateToUse = createdAtDate ?? DateTime.Now;
                string datepart = dateToUse.ToString("yyMMdd");

                string formattedId = primaryKeyId.ToString("D5");

                string transactionId = $"{PublicTransactionIdPrefix}{datepart}{formattedId}";

                return transactionId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating transaction ID: {ex.Message}");

                string datepart = DateTime.Now.ToString("yyMMdd");
                return $"{PublicTransactionIdPrefix}{datepart}{primaryKeyId:D5}";
            }
        }

        /// <summary>
        /// Adds a new transaction to the database
        /// Generates TransactionId before insert using CreatedAt date and a temporary sequence number
        /// After insert, updates TransactionId with the actual Primary Key ID
        /// </summary>
        /// <param name="value">The transaction model to add</param>
        public async Task AddTransactionAsync(TransactionModel? transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null");

            if (transaction.CreatedAt == null)
                transaction.CreatedAt = DateTime.Now;

            if (string.IsNullOrWhiteSpace(transaction.CurrencyCode))
                transaction.CurrencyCode = "INR";

            string temporaryTransactionId = GeneratePublicTransactionId(transaction.CreatedAt, 99999);
            transaction.TransactionId = temporaryTransactionId;

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            string finalTransactionId = GeneratePublicTransactionId(transaction.CreatedAt, transaction.Id);

            transaction.TransactionId = finalTransactionId;

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// UPDATE Operation: Modifies an existing transaction in the database.
        /// This method is called when the user clicks the "Edit" button, modifies field values,
        /// and clicks the save button on the edit dialog.
        /// </summary>
        /// <param name="transaction">The transaction object with updated values.</param>
        /// <exception cref="ArgumentNullException">Thrown if the transaction object is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the transaction to update does not exist in the database.</exception>
        public async Task UpdateTransactionAsync(TransactionModel? transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null");

            var existingTransaction = await _context.Transactions.FindAsync(transaction.Id);
            if (existingTransaction == null)
                throw new KeyNotFoundException($"Transaction with ID {transaction.Id} not found in the database.");

            existingTransaction.TransactionId = transaction.TransactionId;
            existingTransaction.CustomerId = transaction.CustomerId;
            existingTransaction.OrderId = transaction.OrderId;
            existingTransaction.InvoiceNumber = transaction.InvoiceNumber;
            existingTransaction.Description = transaction.Description;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.CurrencyCode = transaction  .CurrencyCode;
            existingTransaction.TransactionType = transaction.TransactionType;
            existingTransaction.PaymentGateway = transaction.PaymentGateway;
            existingTransaction.CompletedAt = transaction.CompletedAt;
            existingTransaction.Status = transaction.Status;

            _context.Transactions.Update(existingTransaction);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a transaction from the database
        /// </summary>
        /// <param name="key">The transaction ID to delete</param>
        public async Task RemoveTransactionAsync(int? key)
        {
            try
            {
                if (key == null || key <= 0)
                    throw new ArgumentException("Transaction ID cannot be null or invalid", nameof(key));

                var transaction = await _context.Transactions.FindAsync(key);
                if (transaction == null)
                    throw new KeyNotFoundException($"Transaction with ID {key} not found");

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while deleting transaction: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting transaction: {ex.Message}");
                throw;
            }
        }
    }
}
