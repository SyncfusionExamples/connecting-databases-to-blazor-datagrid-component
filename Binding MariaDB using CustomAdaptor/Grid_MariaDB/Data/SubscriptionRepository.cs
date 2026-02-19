using Microsoft.EntityFrameworkCore;

namespace Grid_MariaDB.Data
{
    /// <summary>
    /// Repository pattern implementation for Subscription entity using Entity Framework Core
    /// Handles all CRUD operations and business logic for subscriptions
    /// </summary>
    public class SubscriptionRepository
    {
        private readonly SubscriptionDbContext _context;
        private const string PublicSubscriptionIdPrefix = "SUB";

        public SubscriptionRepository(SubscriptionDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all subscriptions from the database ordered by ID descending
        /// </summary>
        /// <returns>List of all subscriptions</returns>
        public async Task<List<SubscriptionModel>> GetSubscriptionsAsync()
        {
            try
            {
                return await _context.Subscriptions
                    .OrderByDescending(s => s.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving subscriptions: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates a unique public subscription ID
        /// Format: SUB + YYMMDD (from SubscriptionStartDate) + 5-digit ID
        /// Example: SUB26010901000 (Year 26, Month 01, Day 09, ID 01000)
        /// </summary>
        /// <param name="subscriptionStartDate">The subscription start date</param>
        /// <param name="primaryKeyId">The primary key ID</param>
        /// <returns>Generated subscription ID</returns>
        private string GeneratePublicSubscriptionId(DateTime? subscriptionStartDate, int primaryKeyId)
        {
            try
            {
                DateTime dateToUse = subscriptionStartDate ?? DateTime.Now;
                string datepart = dateToUse.ToString("yyMMdd");

                string formattedId = primaryKeyId.ToString("D5");

                string subscriptionId = $"{PublicSubscriptionIdPrefix}{datepart}{formattedId}";

                return subscriptionId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating subscription ID: {ex.Message}");

                string datepart = DateTime.Now.ToString("yyMMdd");
                return $"{PublicSubscriptionIdPrefix}{datepart}{primaryKeyId:D5}";
            }
        }

        /// <summary>
        /// Adds a new subscription to the database
        /// Generates PublicID before insert using SubscriptionStartDate and a temporary sequence number
        /// After insert, updates PublicID with the actual Primary Key ID
        /// </summary>
        /// <param name="subscription">The subscription model to add</param>
        public async Task AddSubscriptionAsync(SubscriptionModel? subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription), "Subscription cannot be null");

            if (subscription.SubscriptionStartDate == null)
                subscription.SubscriptionStartDate = DateTime.Now;

            if (string.IsNullOrWhiteSpace(subscription.CurrencyCode))
                subscription.CurrencyCode = "INR";

            string temporarySubscriptionId = GeneratePublicSubscriptionId(subscription.SubscriptionStartDate, 99999);
            subscription.PublicID = temporarySubscriptionId;

            _context.Subscriptions.Add(subscription);

            await _context.SaveChangesAsync();

            string finalSubscriptionId = GeneratePublicSubscriptionId(subscription.SubscriptionStartDate, subscription.Id);

            subscription.PublicID = finalSubscriptionId;

            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// UPDATE Operation: Modifies an existing subscription in the database.
        /// This method is called when the user clicks the "Edit" button, modifies field values,
        /// and clicks the save button on the edit dialog.
        /// </summary>
        /// <param name="subscription">The subscription object with updated values.</param>
        /// <exception cref="ArgumentNullException">Thrown if the subscription object is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the subscription to update does not exist in the database.</exception>
        public async Task UpdateSubscriptionAsync(SubscriptionModel? subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription), "Subscription cannot be null");

            var existingSubscription = await _context.Subscriptions.FindAsync(subscription.Id);
            if (existingSubscription == null)
                throw new KeyNotFoundException($"Subscription with ID {subscription.Id} not found in the database.");

            existingSubscription.PublicID = subscription.PublicID;
            existingSubscription.CustomerId = subscription.CustomerId;
            existingSubscription.SubscriptionID = subscription.SubscriptionID;
            existingSubscription.InvoiceNumber = subscription.InvoiceNumber;
            existingSubscription.Description = subscription.Description;
            existingSubscription.Amount = subscription.Amount;
            existingSubscription.CurrencyCode = subscription.CurrencyCode;
            existingSubscription.SubscriptionType = subscription.SubscriptionType;
            existingSubscription.PaymentGateway = subscription.PaymentGateway;
            existingSubscription.SubscriptionEndDate = subscription.SubscriptionEndDate;
            existingSubscription.Status = subscription.Status;

            _context.Subscriptions.Update(existingSubscription);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a subscription from the database
        /// </summary>
        /// <param name="key">The subscription ID to delete</param>
        public async Task RemoveSubscriptionAsync(int? key)
        {
            try
            {
                if (key == null || key <= 0)
                    throw new ArgumentException("Subscription ID cannot be null or invalid", nameof(key));

                var subscription = await _context.Subscriptions.FindAsync(key);
                if (subscription == null)
                    throw new KeyNotFoundException($"Subscription with ID {key} not found");

                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while deleting subscription: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting subscription: {ex.Message}");
                throw;
            }
        }
    }
}
