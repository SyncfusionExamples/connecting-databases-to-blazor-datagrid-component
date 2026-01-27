using System.ComponentModel.DataAnnotations;

namespace Grid_MySQL.Data
{
    public class TransactionModel
    {
        [Key]
        public int Id { get; set; }
        public string? TransactionId { get; set; }
        public int? CustomerId { get; set; }
        public int? OrderId { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public string? TransactionType { get; set; }
        public string? PaymentGateway { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Status { get; set; }
    }
}
