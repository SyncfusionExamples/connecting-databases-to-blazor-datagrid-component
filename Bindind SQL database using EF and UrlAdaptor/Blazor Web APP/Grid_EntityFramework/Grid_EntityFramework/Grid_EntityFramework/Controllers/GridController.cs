using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Grid_EntityFramework.Controllers
{
    [ApiController]
    public class GridController : ControllerBase
    {
        //TODO: Enter the connection string of database
        string ConnectionString = $"<Enter Your connection string here>";
        /// <summary>
        /// Returns the data collection as result and count after performing data operations based on request from <see cref=”DataManagerRequest”/>
        /// </summary>
        /// <param name="dataManagerRequest">DataManagerRequest contains the information regarding searching, filtering, sorting, aggregates and paging which is handled on the DataGrid component side</param>
        /// <returns>The data collection's type is determined by how this method has been implemented.</returns>
        [HttpPost]
        [Route("api/[controller]")]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<Order> DataSource = GetOrderData();
            // Handling Searching in UrlAdaptor.
            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                // Searching
                DataSource = DataOperations.PerformSearching(DataSource, DataManagerRequest.Search);
            }
            // Handling Filtering in UrlAdaptor.
            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                // Filtering
                DataSource = DataOperations.PerformFiltering(DataSource, DataManagerRequest.Where, DataManagerRequest.Where[0].Operator);
            }
            // Handling Sorting in UrlAdaptor.
            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                // Sorting
                DataSource = DataOperations.PerformSorting(DataSource, DataManagerRequest.Sorted);
            }
            int TotalRecordsCount = DataSource.Cast<Order>().Count();
            // Handling Aggregation in UrlAdaptor.
            IDictionary<string, object> Aggregates = null;
            if (DataManagerRequest.Aggregates != null) // Aggregation
            {
                Aggregates = DataUtil.PerformAggregation(DataSource, DataManagerRequest.Aggregates);
            }
            // Handling Paging in UrlAdaptor.
            if (DataManagerRequest.Skip != 0)
            {
                // Paging
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
            }
            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
            }
            //Here RequiresCount is passed from the control side itself, where ever the on-demand data fetching is needed then the RequiresCount is set as true in component side itself.
            // In the above case we are using paging so data are loaded in on-demand bases whenever the next page is clicked in Blazor DataGrid side.
            return new { result = DataSource, count = TotalRecordsCount, aggregates = Aggregates };
        }
        [Route("api/[controller]")]
        public List<Order> GetOrderData()
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                // Retrieve orders from the Orders DbSet and convert to list asynchronously
                List<Order> orders = context.Orders.ToList();
                return orders;
            }
        }

        [HttpPost]
        [Route("api/Grid/Insert")]
        /// <summary>
        /// Inserts a new data item into the data collection.
        /// </summary>
        /// <param name="CRUDModel<T>">The set of information along with new record detail which is need to be inserted.</param>
        /// <returns>Returns void</returns>
        public void Insert([FromBody] CRUDModel<Order> Value)
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                // Add the provided order to the Orders DbSet
                context.Orders.Add(Value.Value);
                // Save changes to the database
                context.SaveChanges();
            }
        }

        //// PUT: api/Default/5
        [HttpPost]
        [Route("api/Grid/Update")]
        /// <summary>
        /// Update a existing data item from the data collection.
        /// </summary>
        /// <param name="CRUDModel<T>">The set of information along with updated record detail which is need to be updated.</param>
        /// <returns>Returns void</returns>
        public void Update([FromBody] CRUDModel<Order> Value)
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                var existingOrder = context.Orders.Find(Value.Value.OrderID);
                if (existingOrder != null)
                {
                    // Update the existing order with the new values
                    context.Entry(existingOrder).CurrentValues.SetValues(Value.Value);
                    // Save changes to the database
                    context.SaveChanges();
                }
            }
        }


        //// DELETE: api/ApiWithActions/5
        [HttpPost]
        [Route("api/Grid/Delete")]
        /// <summary>
        /// Remove a specific data item from the data collection.
        /// </summary>
        /// <param name="CRUDModel<T>">The set of information along with specific record detail which is need to be removed.</param>
        /// <returns>Returns void</returns>
        public void Delete([FromBody] CRUDModel<Order> Value)
        {
            int OrderId = Convert.ToInt32(Value.Key.ToString());
            using (var context = new OrderDbContext(ConnectionString))
            {
                var Order = context.Orders.Find(OrderId);
                if (Order != null)
                {
                    // Remove the order from the Orders DbSet
                    context.Orders.Remove(Order);
                    // Save changes to the database
                    context.SaveChanges();
                }
            }
        }
        [HttpPost]
        [Route("api/Grid/Batch")]
        /// <summary>
        /// Batch update (Insert, Update, Delete) a collection of data items from the data collection.
        /// </summary>
        /// <param name="CRUDModel<T>">The set of information along with details about the CRUD actions to be executed from the database.</param>
        /// <returns>Returns void</returns>
        public void Batch([FromBody] CRUDModel<Order> Value)
        {
            using (var context = new OrderDbContext(ConnectionString))
            {
                if (Value.Changed != null)
                {
                    foreach (var Record in Value.Changed)
                    {
                        // Update the changed records
                        context.UpdateRange(Record);
                    }
                }
                if (Value.Added != null)
                {
                    // Add new records
                    context.Orders.AddRange(Value.Added);
                }
                if (Value.Deleted != null)
                {
                    foreach (var Record in Value.Deleted)
                    {
                        // Find and delete the records
                        var ExistingOrder = context.Orders.Find(Record.OrderID);
                        if (ExistingOrder != null)
                        {
                            context.Orders.Remove(ExistingOrder);
                        }
                    }
                }
                // Save changes to the database
                context.SaveChanges();
            }
        }
        public class Order
        {
            [Key]
            public int? OrderID { get; set; }
            public string? CustomerID { get; set; }
            public int? EmployeeID { get; set; }
            public decimal? Freight { get; set; }
            public string? ShipCity { get; set; }
        }
        public class CRUDModel<T> where T : class
        {
            [JsonProperty("action")]
            public string? Action { get; set; }
            [JsonProperty("keyColumn")]
            public string? KeyColumn { get; set; }
            [JsonProperty("key")]
            public object? Key { get; set; }
            [JsonProperty("value")]
            public T? Value { get; set; }
            [JsonProperty("added")]
            public List<T>? Added { get; set; }
            [JsonProperty("changed")]
            public List<T>? Changed { get; set; }
            [JsonProperty("deleted")]
            public List<T>? Deleted { get; set; }
            [JsonProperty("params")]
            public IDictionary<string, object>? Params { get; set; }
        }
        public class OrderDbContext : DbContext
        {
            private readonly string _ConnectionString;

            // Constructor to initialize the DbContext with a connection string
            public OrderDbContext(string ConnectionString)
            {
                _ConnectionString = ConnectionString;
            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // Configure DbContext to use SQL Server with the provided connection string
                optionsBuilder.UseSqlServer(_ConnectionString);
            }

            // DbSet representing a collection of Order entities in the database
            public DbSet<Order> Orders { get; set; }
        }
    }

}
