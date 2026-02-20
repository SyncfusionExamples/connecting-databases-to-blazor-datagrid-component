using System;
using Google.Cloud.Firestore;

namespace Grid_Firebase_Firestore.Models
{
    /// <summary>
    /// Represents a book record in the library management system.
    /// This model defines the structure of book-related data used throughout the application.
    /// Maps to Cloud Firestore 'books' collection.
    /// </summary>
    [FirestoreData]
    public class Book
    {
        /// <summary>
        /// Gets or sets the unique book identifier (e.g., BOOK001, BOOK002).
        /// This serves as the document ID in Firestore.
        /// </summary>
        [FirestoreProperty]
        public string BookId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        [FirestoreProperty]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the book author.
        /// </summary>
        [FirestoreProperty]
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique International Standard Book Number (ISBN).
        /// Must be unique across all books in the library.
        /// </summary>
        [FirestoreProperty]
        public string ISBN { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category or genre of the book.
        /// Examples: Fiction, Non-Fiction, Science, History, etc.
        /// </summary>
        [FirestoreProperty]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the publication date of the book.
        /// Stored as a Firestore Timestamp for proper date handling.
        /// </summary>
        [FirestoreProperty]
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Gets or sets the language in which the book is written.
        /// Examples: English, Spanish, French, etc.
        /// </summary>
        [FirestoreProperty]
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets the total number of copies available in the library.
        /// </summary>
        [FirestoreProperty]
        public int TotalCopies { get; set; }

        /// <summary>
        /// Gets or sets the number of copies currently available for borrowing.
        /// </summary>
        [FirestoreProperty]
        public int AvailableCopies { get; set; }

        /// <summary>
        /// Gets or sets the physical location or shelf number of the book in the library.
        /// </summary>
        [FirestoreProperty]
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who has borrowed the book (if any).
        /// Null if the book is not currently borrowed.
        /// </summary>
        [FirestoreProperty]
        public string? BorrowedBy { get; set; }

        /// <summary>
        /// Gets or sets the current status of the book.
        /// Valid values: Available, Borrowed, Reserved, Lost, Damaged, Maintenance
        /// </summary>
        [FirestoreProperty]
        public string Status { get; set; } = "Available";

        /// <summary>
        /// Gets or sets the date and time when the book record was last updated.
        /// Stored as a Firestore Timestamp for proper timestamp handling.
        /// Automatically set to the current UTC time on each update.
        /// </summary>
        [FirestoreProperty]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Normalizes all DateTime fields to UTC to ensure compatibility with Firestore.
        /// This is necessary because JSON deserialization may create DateTime objects
        /// with DateTimeKind.Unspecified, which Firestore cannot convert to Timestamp.
        /// </summary>
        public void NormalizeToUtc()
        {
            // Normalize PublishDate
            if (PublishDate.HasValue)
            {
                PublishDate = PublishDate.Value.Kind == DateTimeKind.Utc 
                    ? PublishDate.Value 
                    : PublishDate.Value.ToUniversalTime();
            }

            // Normalize LastUpdated
            if (LastUpdated.Kind != DateTimeKind.Utc)
            {
                LastUpdated = LastUpdated.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(LastUpdated, DateTimeKind.Utc)
                    : LastUpdated.ToUniversalTime();
            }
        }
    }
}
