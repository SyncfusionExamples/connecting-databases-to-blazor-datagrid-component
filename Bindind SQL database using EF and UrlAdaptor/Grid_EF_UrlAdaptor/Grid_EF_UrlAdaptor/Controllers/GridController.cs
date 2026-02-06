using Grid_EF_UrlAdaptor.Data;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.Collections;
using System.Text.Json.Serialization;

namespace Grid_EF_UrlAdaptor.Controllers
{
    [ApiController]
    public class GridController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public GridController(OrderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns data with search, filter, sort, and paging operations
        /// </summary>
        [HttpPost]
        [Route("api/[controller]")]
        public object Post([FromBody] DataManagerRequest dataManagerRequest)
        {
            try
            {
                IEnumerable dataSource = GetOrderData();

                // Handling Searching
                if (dataManagerRequest.Search != null && dataManagerRequest.Search.Count > 0)
                {
                    dataSource = DataOperations.PerformSearching(dataSource, dataManagerRequest.Search);
                }

                // Handling filtering operation.
                if (dataManagerRequest.Where != null && dataManagerRequest.Where.Count > 0)
                {
                    dataSource = DataOperations.PerformFiltering(dataSource, dataManagerRequest.Where, dataManagerRequest.Where[0].Operator);
                }

                // Handling Sorting
                if (dataManagerRequest.Sorted != null && dataManagerRequest.Sorted.Count > 0)
                {
                    dataSource = DataOperations.PerformSorting(dataSource, dataManagerRequest.Sorted);
                }

                int totalRecordsCount = dataSource.Cast<Order>().Count();

                // Handling Paging
                if (dataManagerRequest.Skip != 0)
                {
                    dataSource = DataOperations.PerformSkip(dataSource, dataManagerRequest.Skip);
                }

                if (dataManagerRequest.Take != 0)
                {
                    dataSource = DataOperations.PerformTake(dataSource, dataManagerRequest.Take);
                }

                return dataManagerRequest.RequiresCounts ? new DataResult() { Result = dataSource, Count = totalRecordsCount} : (object)dataSource;
            }
            catch (Exception ex)
            {
                return new { error = ex.Message, innerError = ex.InnerException?.Message };
            }
        }

        /// <summary>
        /// Retrieves all order data from the database
        /// </summary>
        [HttpGet]
        [Route("api/[controller]")]
        public List<Order> GetOrderData()
        {
            return _context.Orders.ToList();
        }

        /// <summary>
        /// Inserts a new order record
        /// </summary>
        [HttpPost("Insert")]
        [Route("api/[controller]/Insert")]
        public void Insert([FromBody] CRUDModel<Order> value)
        {
            try
            {
                _context.Orders.Add(value.Value!);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting order: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing order record
        /// </summary>
        [HttpPost("Update")]
        [Route("api/[controller]/Update")]
        public void Update([FromBody] CRUDModel<Order> value)
        {
            try
            {
                var existingOrder = _context.Orders.Find(value.Value?.OrderID);
                if (existingOrder != null)
                {
                    _context.Entry(existingOrder).CurrentValues.SetValues(value.Value!);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating order: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an order record
        /// </summary>
        [HttpPost("Delete")]
        [Route("api/[controller]/Delete")]
        public void Delete([FromBody] CRUDModel<Order> value)
        {
            try
            {
                //int orderId = Convert.ToInt32(value.Key);
                //int orderId = value.Key is System.Text.Json.JsonElement je ? Convert.ToInt32(je.GetInt32()) : Convert.ToInt32(value.Key);
                int orderId = Convert.ToInt32(value.Key?.ToString());
                var order = _context.Orders.Find(orderId);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting order: {ex.Message}");
            }
        }

        /// <summary>
        /// Batch operations for Insert, Update, and Delete
        /// </summary>
        [HttpPost("Batch")]
        [Route("api/[controller]/BatchUpdate")]
        public void Batch([FromBody] CRUDModel<Order> value)
        {
            try
            {
                if (value.Changed != null)
                {
                    foreach (var record in value.Changed)
                    {
                        _context.UpdateRange(record);
                    }
                }

                if (value.Added != null)
                {
                    _context.Orders.AddRange(value.Added);
                }

                if (value.Deleted != null)
                {
                    foreach (var record in value.Deleted)
                    {
                        var existingOrder = _context.Orders.Find(record.OrderID);
                        if (existingOrder != null)
                        {
                            _context.Orders.Remove(existingOrder);
                        }
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in batch operations: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// CRUD Model for handling data operations
    /// </summary>
    public class CRUDModel<T> where T : class
    {
        [JsonPropertyName("action")]
        public string? Action { get; set; }
        [JsonPropertyName("keyColumn")]
        public string? KeyColumn { get; set; }
        [JsonPropertyName("key")]
        public object? Key { get; set; }
        [JsonPropertyName("value")]
        public T? Value { get; set; }
        [JsonPropertyName("added")]
        public List<T>? Added { get; set; }
        [JsonPropertyName("changed")]
        public List<T>? Changed { get; set; }
        [JsonPropertyName("deleted")]
        public List<T>? Deleted { get; set; }
        [JsonPropertyName("params")]
        public IDictionary<string, object>? Params { get; set; }
    }
}
