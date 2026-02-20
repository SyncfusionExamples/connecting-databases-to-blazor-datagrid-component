using Google.Cloud.Firestore;
using Grid_Firebase_Firestore.Models;

namespace Grid_Firebase_Firestore.Services
{
    /// <summary>
    /// Service class for Cloud Firestore Database operations.
    /// Handles all CRUD operations and business logic for books.
    /// Uses Firebase Admin SDK for secure database interactions.
    /// </summary>
    public class FirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string BooksCollection = "books";
        private const string BookIdPrefix = "BOOK";

        public FirebaseService(IConfiguration configuration)
        {
            // Try environment variable first, then fall back to appsettings
            var serviceAccountPath = configuration["Firebase:ServiceAccountPath"]
                ?? Path.Combine(AppContext.BaseDirectory, "serviceAccountKey.json");

            if (!File.Exists(serviceAccountPath))
            {
                throw new FileNotFoundException($"Service account key not found at: {serviceAccountPath}");
            }

            // Set the service account credentials environment variable for Google Cloud authentication
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountPath);

            // Get project ID from configuration or service account key
            var projectId = configuration["Firebase:ProjectId"];

            if (string.IsNullOrEmpty(projectId))
            {
                throw new InvalidOperationException("Firebase project ID not found. Please configure 'Firebase:ProjectId' in appsettings.json");
            }

            // Initialize Firestore database
            _firestoreDb = FirestoreDb.Create(projectId);
        }


        /// <summary>
        /// Retrieves all books from the Firestore collection.
        /// </summary>
        /// <returns>List of all books</returns>
        public async Task<List<Book>> GetBooksAsync()
        {
            try
            {
                var query = _firestoreDb.Collection(BooksCollection);
                var snapshot = await query.GetSnapshotAsync();

                var books = new List<Book>();
                foreach (var doc in snapshot.Documents)
                {
                    var book = doc.ConvertTo<Book>();
                    if (book != null)
                    {
                        book.BookId = doc.Id; // Use Firestore document ID
                        books.Add(book);
                    }
                }

                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                throw new Exception($"Error fetching books: {ex.Message}");
            }
        }

        /// <summary>
        /// Inserts a new book into the Firestore collection.
        /// </summary>
        /// <param name="book">The book object to insert</param>
        /// <returns>The inserted book with generated BookId</returns>
        public async Task<Book> InsertBookAsync(Book book)
        {
            try
            {
                // Auto-generate BookId if not provided
                if (string.IsNullOrEmpty(book.BookId))
                {
                    book.BookId = await GenerateBookIdAsync();
                }

                // Set LastUpdated to current UTC time
                book.LastUpdated = DateTime.UtcNow;

                // Add book to Firestore with custom document ID
                await _firestoreDb.Collection(BooksCollection).Document(book.BookId).SetAsync(book);

                return book;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting book: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing book in the Firestore collection.
        /// </summary>
        /// <param name="bookId">The ID of the book to update</param>
        /// <param name="book">The updated book object</param>
        /// <returns>True if the book was updated successfully; otherwise, false</returns>
        public async Task<bool> UpdateBookAsync(string bookId, Book book)
        {
            try
            {
                if (string.IsNullOrEmpty(bookId))
                {
                    return false;
                }

                // Set LastUpdated to current UTC time
                book.LastUpdated = DateTime.UtcNow;
                book.BookId = bookId;

                // Update book in Firestore
                await _firestoreDb.Collection(BooksCollection).Document(bookId).SetAsync(book, SetOptions.MergeAll);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating book: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a book from the Firestore collection.
        /// </summary>
        /// <param name="bookId">The ID of the book to delete</param>
        /// <returns>True if the book was deleted successfully; otherwise, false</returns>
        public async Task<bool> DeleteBookAsync(string? bookId)
        {
            try
            {
                if (string.IsNullOrEmpty(bookId))
                {
                    return false;
                }

                // Delete book from Firestore
                await _firestoreDb.Collection(BooksCollection).Document(bookId).DeleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting book: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a new unique BookId based on existing books.
        /// Format: BOOK001, BOOK002, etc.
        /// </summary>
        /// <returns>A new unique BookId</returns>
        private async Task<string> GenerateBookIdAsync()
        {
            try
            {
                var existingBooks = await GetBooksAsync();

                int maxNumber = existingBooks
                    .Where(book => !string.IsNullOrEmpty(book.BookId) && book.BookId.StartsWith(BookIdPrefix))
                    .Select(book =>
                    {
                        string numberPart = book.BookId.Substring(BookIdPrefix.Length);
                        if (int.TryParse(numberPart, out int number))
                            return number;
                        return 0;
                    })
                    .DefaultIfEmpty(0)
                    .Max();

                int nextNumber = maxNumber + 1;
                string newBookId = $"{BookIdPrefix}{nextNumber:D3}";
                return newBookId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating BookId: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs bulk import of books from JSON data using batch writes.
        /// Stops at the first failure to ensure data integrity.
        /// </summary>
        /// <param name="books">List of books to import</param>
        /// <returns>True if all books imported successfully; false otherwise</returns>
        public async Task<bool> BulkImportBooksAsync(List<Book> books)
        {
            if (books == null || books.Count == 0)
            {
                return false;
            }

            try
            {
                int batchSize = 500; // Firestore batch limit

                // Process books in batches using transactions
                for (int i = 0; i < books.Count; i += batchSize)
                {
                    var batchBooks = books.Skip(i).Take(batchSize).ToList();

                    // Use a transaction to write multiple documents atomically
                    await _firestoreDb.RunTransactionAsync(async transaction =>
                    {
                        foreach (var book in batchBooks)
                        {
                            var docRef = _firestoreDb.Collection(BooksCollection).Document(book.BookId);
                            transaction.Set(docRef, book);
                        }
                    });
                }

                return true;
            }
            catch
            {
                // Stop at first failure and return false
                return false;
            }
        }
    }
}
