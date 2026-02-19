using System.ComponentModel.DataAnnotations;

namespace Grid_MariaDB.Data
{
    public class SubscriptionModel
    {
        [Key]
        public int Id { get; set; }
        public string? PublicID { get; set; }
        public int? CustomerId { get; set; }
        public int? SubscriptionID { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public string? SubscriptionType { get; set; }
        public string? PaymentGateway { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string? Status { get; set; }
    }
}
