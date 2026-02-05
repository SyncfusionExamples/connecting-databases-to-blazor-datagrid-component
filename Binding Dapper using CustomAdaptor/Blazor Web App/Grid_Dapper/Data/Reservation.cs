using System.ComponentModel.DataAnnotations;

namespace Grid_Dapper.Data
{
    /// <summary>
    /// Reservation model representing a hotel room reservation
    /// Maps to dbo.Rooms table in HotelBookingDB database
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Primary key identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Unique reservation reference number (e.g., RES-1001)
        /// </summary>
        public string? ReservationId { get; set; }

        /// <summary>
        /// Guest full name
        /// </summary>
        public string? GuestName { get; set; }

        /// <summary>
        /// Guest email address
        /// </summary>
        public string? GuestEmail { get; set; }

        /// <summary>
        /// Check-in date
        /// </summary>
        public DateTime? CheckInDate { get; set; }

        /// <summary>
        /// Check-out date
        /// </summary>
        public DateTime? CheckOutDate { get; set; }

        /// <summary>
        /// Room type (e.g., Standard, Deluxe, Suite)
        /// </summary>
        public string? RoomType { get; set; }

        /// <summary>
        /// Room number assigned
        /// </summary>
        public string? RoomNumber { get; set; }

        /// <summary>
        /// Price per night
        /// </summary>
        public decimal? AmountPerDay { get; set; }

        /// <summary>
        /// Number of nights reserved
        /// </summary>
        public int? NoOfDays { get; set; }

        /// <summary>
        /// Total reservation amount (AmountPerDay × NoOfDays)
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Payment status (Pending, Paid, Failed, Refunded)
        /// </summary>
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// Reservation status (Confirmed, Cancelled, CheckedIn, CheckedOut)
        /// </summary>
        public string? ReservationStatus { get; set; }
    }
}
