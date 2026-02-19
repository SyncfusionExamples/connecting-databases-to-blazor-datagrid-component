using System;

namespace Grid_Firebase_Realtime.Models
{
    /// <summary>
    /// Represents a book record in the library management system.
    /// This model defines the structure of book-related data used throughout the application.
    /// Maps to Firebase Realtime Database structure.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets the unique book identifier (e.g., BOOK001, BOOK002).
        /// This serves as the primary key in Firebase.
        /// </summary>
        public string BookId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the book author.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique International Standard Book Number (ISBN).
        /// Must be unique across all books in the library.
        /// </summary>
        public string ISBN { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category or genre of the book.
        /// Examples: Fiction, Non-Fiction, Science, History, etc.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the publication date of the book.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Gets or sets the language in which the book is written.
        /// Examples: English, Spanish, French, etc.
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets the total number of copies available in the library.
        /// </summary>
        public int TotalCopies { get; set; }

        /// <summary>
        /// Gets or sets the number of copies currently available for borrowing.
        /// </summary>
        public int AvailableCopies { get; set; }

        /// <summary>
        /// Gets or sets the physical location or shelf number of the book in the library.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who has borrowed the book (if any).
        /// Null if the book is not currently borrowed.
        /// </summary>
        public string? BorrowedBy { get; set; }

        /// <summary>
        /// Gets or sets the current status of the book.
        /// Valid values: Available, Borrowed, Reserved, Lost, Damaged, Maintenance
        /// </summary>
        public string Status { get; set; } = "Available";

        /// <summary>
        /// Gets or sets the date and time when the book record was last updated.
        /// Automatically set to the current UTC time on each update.
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
