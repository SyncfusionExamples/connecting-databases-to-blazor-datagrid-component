using Microsoft.EntityFrameworkCore;

namespace Grid_MSSQL.Data
{
    /// <summary>
    /// Repository pattern implementation for Tickets entity using Entity Framework Core
    /// Handles all CRUD operations and business logic for network support tickets
    /// </summary>
    public class TicketRepository
    {
        private readonly TicketsDbContext _context;
        private const string PublicTicketIdPrefix = "NET";
        private const string PublicTicketIdSeparator = "-";
        private const int PublicTicketIdStartNumber = 1001;

        public TicketRepository(TicketsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all tickets from the database ordered by ID descending
        /// </summary>
        /// <returns>List of all tickets</returns>
        public async Task<List<Tickets>> GetTicketsDataAsync()
        {
            try
            {
                return await _context.Tickets
                    .OrderByDescending(t => t.TicketId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates a unique public ticket ID
        /// Format: NET-NNNNN (where NNNNN is a 5-digit sequential number)
        /// Example: NET-1001, NET-1002, etc.
        /// </summary>
        /// <returns>Generated ticket ID</returns>
        private async Task<string> GeneratePublicTicketIdAsync()
        {
            try
            {
                var existingTickets = await GetTicketsDataAsync();

                int maxNumber = existingTickets
                    .Where(ticket => !string.IsNullOrEmpty(ticket.PublicTicketId) && ticket.PublicTicketId.StartsWith(PublicTicketIdPrefix))
                    .Select(ticket =>
                    {
                        string numberPart = ticket.PublicTicketId.Substring((PublicTicketIdPrefix + PublicTicketIdSeparator).Length);
                        if (int.TryParse(numberPart, out int number))
                            return number;
                        return 0;
                    })
                    .DefaultIfEmpty(PublicTicketIdStartNumber - 1)
                    .Max();
                int nextNumber = maxNumber + 1;
                string newPublicTicketId = $"{PublicTicketIdPrefix}{PublicTicketIdSeparator}{nextNumber}";

                return newPublicTicketId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating ticket ID: {ex.Message}");
                return $"{PublicTicketIdPrefix}{PublicTicketIdSeparator}{PublicTicketIdStartNumber}";
            }
        }

        /// <summary>
        /// Adds a new ticket to the database
        /// Generates PublicTicketId before insert
        /// </summary>
        /// <param name="value">The ticket model to add</param>
        public async Task AddTicketAsync(Tickets value)
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Ticket cannot be null");

                string generatedPublicTicketId = await GeneratePublicTicketIdAsync();
                value.PublicTicketId = generatedPublicTicketId;

                if (value.CreatedAt == null)
                    value.CreatedAt = DateTime.Now;

                if (value.UpdatedAt == null)
                    value.UpdatedAt = DateTime.Now;

                _context.Tickets.Add(value);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while adding ticket: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding ticket: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing ticket
        /// </summary>
        /// <param name="value">The ticket model with updated values</param>
        public async Task UpdateTicketAsync(Tickets value)
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Ticket cannot be null");

                if (value.TicketId <= 0)
                    throw new ArgumentException("Ticket ID must be valid", nameof(value.TicketId));

                var existingTicket = await _context.Tickets.FindAsync(value.TicketId);
                if (existingTicket == null)
                    throw new KeyNotFoundException($"Ticket with ID {value.TicketId} not found");

                existingTicket.PublicTicketId = value.PublicTicketId;
                existingTicket.Title = value.Title;
                existingTicket.Description = value.Description;
                existingTicket.Category = value.Category;
                existingTicket.Department = value.Department;
                existingTicket.Assignee = value.Assignee;
                existingTicket.CreatedBy = value.CreatedBy;
                existingTicket.Status = value.Status;
                existingTicket.Priority = value.Priority;
                existingTicket.ResponseDue = value.ResponseDue;
                existingTicket.DueDate = value.DueDate;
                existingTicket.CreatedAt = value.CreatedAt;
                existingTicket.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency error while updating ticket: {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while updating ticket: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating ticket: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a ticket from the database
        /// </summary>
        /// <param name="key">The ticket ID to delete</param>
        public async Task RemoveTicketAsync(int? key)
        {
            try
            {
                if (key == null || key <= 0)
                    throw new ArgumentException("Ticket ID cannot be null or invalid", nameof(key));

                var ticket = await _context.Tickets.FindAsync(key);
                if (ticket == null)
                    throw new KeyNotFoundException($"Ticket with ID {key} not found");

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error while deleting ticket: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting ticket: {ex.Message}");
                throw;
            }
        }
    }
}
