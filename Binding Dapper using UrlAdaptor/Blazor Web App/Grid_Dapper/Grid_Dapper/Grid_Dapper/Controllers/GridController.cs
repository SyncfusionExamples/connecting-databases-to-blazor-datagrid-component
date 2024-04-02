using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Text.Json;

namespace Grid_Dapper.Controllers
{
    [ApiController]
    public class GridController : ControllerBase
    {
        //TODO: Enter the connectionstring of database
        string ConnectionString = $"<Enter Your connectionstring here>";
        /// <summary>
        /// Returns the data collection as result and count after performing data operations based on request from <see cref=”DataManagerRequest”/>
        /// </summary>
        /// <param name="dataManagerRequest">DataManagerRequest containes the information regarding searching, filtering, sorting, aggregates and paging which is handled on the DataGrid component side</param>
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
            //Create query to fetch data from database
            string Query = "SELECT * FROM dbo.Orders ORDER BY OrderID;";
            //Create SQL Connection
            using (IDbConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                // Dapper automatically handles mapping to your Order class
                List<Order> orders = Connection.Query<Order>(Query).ToList();
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
            //Create query to insert the specific into the database by accessing its properties 
            string Query = "INSERT INTO Orders(CustomerID, Freight, ShipCity, EmployeeID) VALUES(@CustomerID, @Freight, @ShipCity, @EmployeeID)";
            using (IDbConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                //Execute this code to reflect the changes into the database
                Connection.Execute(Query, Value.Value);
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
            //Create query to update the changes into the database by accessing its properties
            string Query = "UPDATE Orders SET CustomerID = @CustomerID, Freight = @Freight, ShipCity = @ShipCity, EmployeeID = @EmployeeID WHERE OrderID = @OrderID";
            using (IDbConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                //Execute this code to reflect the changes into the database
                Connection.Execute(Query, Value.Value);
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
            //Create query to remove the specific from database by passing the primary key column value.
            string Query = "DELETE FROM Orders WHERE OrderID = @OrderID";
            using (IDbConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                int orderID = Convert.ToInt32(Value.Key.ToString());
                //Execute this code to reflect the changes into the database
                Connection.Execute(Query, new { OrderID = orderID });
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
            if (Value.Changed != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Changed)
                {
                    //Create query to update the changes into the database by accessing its properties
                    string Query = "UPDATE Orders SET CustomerID = @CustomerID, Freight = @Freight, ShipCity = @ShipCity, EmployeeID = @EmployeeID WHERE OrderID = @OrderID";
                    using (IDbConnection Connection = new SqlConnection(ConnectionString))
                    {
                        Connection.Open();
                        //Execute this code to reflect the changes into the database
                        Connection.Execute(Query, Record);
                    }
                }

            }
            if (Value.Added != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Added)
                {
                    //Create query to insert the specific into the database by accessing its properties 
                    string Query = "INSERT INTO Orders (CustomerID, Freight, ShipCity, EmployeeID) VALUES (@CustomerID, @Freight, @ShipCity, @EmployeeID)";
                    using (IDbConnection Connection = new SqlConnection(ConnectionString))
                    {
                        Connection.Open();
                        //Execute this code to reflect the changes into the database
                        Connection.Execute(Query, Record);
                    }
                }

            }
            if (Value.Deleted != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Deleted)
                {
                    //Create query to remove the specific from database by passing the primary key column value.
                    string Query = "DELETE FROM Orders WHERE OrderID = @OrderID";
                    using (IDbConnection Connection = new SqlConnection(ConnectionString))
                    {
                        Connection.Open();
                        //Execute this code to reflect the changes into the database
                        Connection.Execute(Query, new { OrderID = Record.OrderID });
                    }
                }
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
    }
}
