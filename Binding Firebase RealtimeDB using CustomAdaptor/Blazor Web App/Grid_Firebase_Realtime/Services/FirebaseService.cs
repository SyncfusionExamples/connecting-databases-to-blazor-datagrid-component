using Grid_Firebase_Realtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grid_Firebase_Realtime.Services
{
    /// <summary>
    /// Service class for Firebase Realtime Database operations.
    /// Handles all CRUD operations and business logic for books.
    /// Uses Firebase REST API for direct database interactions.
    /// </summary>
    public class FirebaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _databaseUrl;
        private readonly string _databaseSecret = string.Empty;
        private const string BooksNode = "Books";
        private const string BookIdPrefix = "BOOK";
        private const int BookIdStartNumber = 1;

        public FirebaseService(IConfiguration configuration, HttpClient httpClient)
        {
            try
            {
                _httpClient = httpClient;
                _databaseUrl = configuration["Firebase:RealtimeDatabaseURL"] ?? string.Empty;
                _databaseSecret = configuration["Firebase:DatabaseSecret"] ?? string.Empty;

                if (string.IsNullOrEmpty(_databaseUrl))
                {
                    throw new InvalidOperationException("Firebase Realtime Database URL not found in configuration. Please add 'Firebase:RealtimeDatabaseURL' to appsettings.json");
                }

                // Ensure URL ends without trailing slash
                if (_databaseUrl.EndsWith("/"))
                {
                    _databaseUrl = _databaseUrl.TrimEnd('/');
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error initializing Firebase: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Constructs the Firebase REST API URL with authentication parameter
        /// </summary>
        private string BuildUrl(string path, bool includeAuth = true)
        {
            string url = $"{_databaseUrl}/{path}.json";
            if (includeAuth && !string.IsNullOrEmpty(_databaseSecret))
            {
                url += $"?auth={_databaseSecret}";
            }
            return url;
        }

        /// <summary>
        /// Retrieves all books from the Firebase Realtime Database.
        /// </summary>
        /// <returns>List of all books</returns>
        public async Task<List<Book>> GetBooksAsync()
        {
            try
            {
                string url = BuildUrl(BooksNode);
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    
                    if (content == "null")
                    {
                        return new List<Book>();
                    }

                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    };

                    var booksDict = JsonSerializer.Deserialize<Dictionary<string, Book>>(content, options);
                    
                    if (booksDict == null || booksDict.Count == 0)
                    {
                        return new List<Book>();
                    }

                    return booksDict.Select(item => new Book
                    {
                        BookId = item.Key,
                        Title = item.Value.Title,
                        Author = item.Value.Author,
                        ISBN = item.Value.ISBN,
                        Category = item.Value.Category,
                        PublishDate = item.Value.PublishDate,
                        Language = item.Value.Language,
                        TotalCopies = item.Value.TotalCopies,
                        AvailableCopies = item.Value.AvailableCopies,
                        Location = item.Value.Location,
                        BorrowedBy = item.Value.BorrowedBy,
                        Status = item.Value.Status,
                        LastUpdated = item.Value.LastUpdated
                    }).ToList();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Firebase API Error: {response.StatusCode} - {errorContent}");
                    return new List<Book>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                throw new Exception($"Error fetching books: {ex.Message}");
            }
        }

        /// <summary>
        /// Inserts a new book into the Firebase database.
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

                // Serialize book to JSON
                string jsonContent = JsonSerializer.Serialize(book);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Insert book into Firebase
                string url = BuildUrl($"{BooksNode}/{book.BookId}");
                var response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to insert book: {response.StatusCode}");
                }

                return book;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting book: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing book in the Firebase database.
        /// </summary>
        /// <param name="bookId">The ID of the book to update</param>
        /// <param name="book">The updated book object</param>
        /// <returns>True if the book was updated successfully; otherwise, false</returns>
        public async Task<bool> UpdateBookAsync(string bookId, Book book)
        {
            try
            {
                // Set LastUpdated to current UTC time
                book.LastUpdated = DateTime.UtcNow;

                // Serialize book to JSON
                string jsonContent = JsonSerializer.Serialize(book);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Update book in Firebase
                string url = BuildUrl($"{BooksNode}/{bookId}");
                var response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to update book: {response.StatusCode}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating book: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a book from the Firebase database.
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

                // Delete book from Firebase
                string url = BuildUrl($"{BooksNode}/{bookId}");
                var response = await _httpClient.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to delete book: {response.StatusCode}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting book: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a unique BookId in the format BOOK001, BOOK002, etc.
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
                    .DefaultIfEmpty(BookIdStartNumber - 1)
                    .Max();

                int nextNumber = maxNumber + 1;
                string newBookId = $"{BookIdPrefix}{nextNumber:D3}"; // D3 for 3-digit formatting (001, 002, etc.)
                return newBookId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating BookId: {ex.Message}");
            }
        }
    }
}
