using Dapper;
using System.Data;

namespace Grid_Dapper.Data
{
    /// <summary>
    /// Repository pattern implementation for Reservation using Dapper
    /// Handles all CRUD operations and business logic for hotel room reservations
    /// </summary>
    public class ReservationRepository
    {
        private readonly IDbConnection _connection;
        private const string ReservationIdPrefix = "RES";
        private const int ReservationIdStartNumber = 1001;

        public ReservationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Retrieves all reservations from the database ordered by check-in date descending
        /// </summary>
        /// <returns>List of all reservations</returns>
        public async Task<List<Reservation>> GetReservationsAsync()
        {
            try
            {
                const string query = @"SELECT * FROM [dbo].[Rooms] ORDER BY Id DESC";
                var reservations = await _connection.QueryAsync<Reservation>(query);
                return reservations.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving reservations: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Calculates the number of days between check-in and check-out dates
        /// If dates are the same, returns 1 day
        /// </summary>
        /// <param name="checkInDate">Check-in date</param>
        /// <param name="checkOutDate">Check-out date</param>
        /// <returns>Number of days (minimum 1)</returns>
        private int CalculateNoOfDays(DateTime checkInDate, DateTime checkOutDate)
        {
            TimeSpan dateDifference = checkOutDate.Date - checkInDate.Date;
            int noOfDays = (int)dateDifference.TotalDays;
            return noOfDays < 1 ? 1 : noOfDays;
        }

        /// <summary>
        /// Generates a unique reservation ID
        /// Format: RES-NNNNN (where NNNNN is a 5-digit sequential number)
        /// Example: RES-1001, RES-1002, etc.
        /// </summary>
        /// <returns>Generated reservation ID</returns>
        private async Task<string> GenerateReservationIdAsync()
        {
            try
            {
                var existingReservations = await GetReservationsAsync();

                int maxNumber = existingReservations
                    .Where(reservation => !string.IsNullOrEmpty(reservation.ReservationId) && reservation.ReservationId.StartsWith(ReservationIdPrefix))
                    .Select(reservation =>
                    {
                        string numberPart = reservation.ReservationId.Substring((ReservationIdPrefix).Length);
                        if (int.TryParse(numberPart, out int number))
                            return number;
                        return 0;
                    })
                    .DefaultIfEmpty(ReservationIdStartNumber - 1)
                    .Max();

                int nextNumber = maxNumber + 1;
                string newReservationId = $"{ReservationIdPrefix}00{nextNumber}";

                return newReservationId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating reservation ID: {ex.Message}");
                return $"{ReservationIdPrefix}{ReservationIdStartNumber}";
            }
        }

        /// <summary>
        /// Adds a new reservation to the database
        /// Generates ReservationId before insert and auto-calculates NoOfDays and TotalAmount based on dates
        /// </summary>
        /// <param name="value">The reservation model to add</param>
        public async Task AddReservationAsync(Reservation value)
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Reservation cannot be null");

                if (string.IsNullOrEmpty(value.GuestName))
                    throw new ArgumentException("Guest name is required", nameof(value));

                string generatedReservationId = await GenerateReservationIdAsync();
                value.ReservationId = generatedReservationId;

                if (value.CheckInDate != default && value.CheckOutDate != default)
                {
                    value.NoOfDays = CalculateNoOfDays(value.CheckInDate, value.CheckOutDate);
                }

                if (value.AmountPerDay.HasValue && value.NoOfDays.HasValue && value.NoOfDays > 0)
                {
                    value.TotalAmount = value.AmountPerDay.Value * value.NoOfDays.Value;
                }

                if (string.IsNullOrEmpty(value.PaymentStatus))
                    value.PaymentStatus = "Pending";

                if (string.IsNullOrEmpty(value.ReservationStatus))
                    value.ReservationStatus = "Confirmed";

                const string query = @"
                    INSERT INTO [dbo].[Rooms] 
                    (ReservationId, GuestName, GuestEmail, CheckInDate, CheckOutDate,
                     RoomType, RoomNumber, AmountPerDay, NoOfDays, TotalAmount, PaymentStatus, ReservationStatus)
                    VALUES 
                    (@ReservationId, @GuestName, @GuestEmail, @CheckInDate, @CheckOutDate,
                     @RoomType, @RoomNumber, @AmountPerDay, @NoOfDays, @TotalAmount, @PaymentStatus, @ReservationStatus)";

                await _connection.ExecuteAsync(query, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding reservation: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing reservation
        /// Auto-calculates NoOfDays based on dates and TotalAmount if AmountPerDay changed
        /// </summary>
        /// <param name="value">The reservation model with updated values</param>
        public async Task UpdateReservationAsync(Reservation value)
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Reservation cannot be null");

                if (value.Id <= 0)
                    throw new ArgumentException("Reservation ID must be valid", nameof(value));

                const string checkQuery = "SELECT COUNT(*) FROM [dbo].[Rooms] WHERE Id = @Id";
                var exists = await _connection.QueryFirstOrDefaultAsync<int>(checkQuery, new { value.Id });
                
                if (exists == 0)
                    throw new KeyNotFoundException($"Reservation with ID {value.Id} not found");

                if (value.CheckInDate != default && value.CheckOutDate != default)
                {
                    value.NoOfDays = CalculateNoOfDays(value.CheckInDate, value.CheckOutDate);
                }

                if (value.AmountPerDay.HasValue && value.NoOfDays.HasValue && value.NoOfDays > 0)
                {
                    value.TotalAmount = value.AmountPerDay.Value * value.NoOfDays.Value;
                }

                const string query = @"
                    UPDATE [dbo].[Rooms]
                    SET ReservationId = @ReservationId, GuestName = @GuestName, 
                        GuestEmail = @GuestEmail, CheckInDate = @CheckInDate, CheckOutDate = @CheckOutDate,
                        RoomType = @RoomType, RoomNumber = @RoomNumber, AmountPerDay = @AmountPerDay, 
                        NoOfDays = @NoOfDays, TotalAmount = @TotalAmount, PaymentStatus = @PaymentStatus, 
                        ReservationStatus = @ReservationStatus
                    WHERE Id = @Id";

                await _connection.ExecuteAsync(query, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating reservation: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a reservation from the database
        /// </summary>
        /// <param name="key">The reservation ID to delete</param>
        public async Task RemoveReservationAsync(int? key)
        {
            try
            {
                if (key == null || key <= 0)
                    throw new ArgumentException("Reservation ID cannot be null or invalid", nameof(key));

                const string checkQuery = "SELECT COUNT(*) FROM [dbo].[Rooms] WHERE Id = @Id";
                var exists = await _connection.QueryFirstOrDefaultAsync<int>(checkQuery, new { Id = key });
                
                if (exists == 0)
                    throw new KeyNotFoundException($"Reservation with ID {key} not found");

                const string query = "DELETE FROM [dbo].[Rooms] WHERE Id = @Id";
                await _connection.ExecuteAsync(query, new { Id = key });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting reservation: {ex.Message}");
                throw;
            }
        }
    }
}
