using System.ComponentModel.DataAnnotations;

namespace Grid_MSSQL.Data
{
    public class Tickets
    {
        [Key]
        public int TicketId { get; set; }
        public string? PublicTicketId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Department { get; set; }
        public string? Assignee { get; set; }
        public string? CreatedBy { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? ResponseDue { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
