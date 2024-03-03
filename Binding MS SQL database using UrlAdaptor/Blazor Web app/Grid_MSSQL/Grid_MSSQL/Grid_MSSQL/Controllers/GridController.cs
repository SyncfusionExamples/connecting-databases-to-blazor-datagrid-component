using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace Grid_MSSQL.Controllers
{    
    [ApiController]
    public class GridController : ControllerBase
    {

        string ConnectionString = <Enter your connectionstring here>;
        [HttpPost]
        [Route("api/[controller]")]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<Order> DataSource = GetOrderData();
            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                // Searching
                DataSource = DataOperations.PerformSearching(DataSource, DataManagerRequest.Search);
            }
            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                // Filtering
                DataSource = DataOperations.PerformFiltering(DataSource, DataManagerRequest.Where, DataManagerRequest.Where[0].Operator);
            }
            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                // Sorting
                DataSource = DataOperations.PerformSorting(DataSource, DataManagerRequest.Sorted);
            }
            int count = DataSource.Cast<Order>().Count();

            if (DataManagerRequest.Skip != 0)
            {
                // Paging
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
            }
            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
            }
            DataResult DataObject = new DataResult();
            if (DataManagerRequest.Aggregates != null) // Aggregation
            {
                DataObject.Result = DataSource;
                DataObject.Count = count;
                DataObject.Aggregates = DataUtil.PerformAggregation(DataSource, DataManagerRequest.Aggregates);

                return DataManagerRequest.RequiresCounts ? DataObject : (object)DataSource;
            }
            return new { result = DataSource, count = count };
        }
        [Route("api/[controller]")]
        public List<Order> GetOrderData()
        {
            string QueryStr = "SELECT * FROM dbo.Orders ORDER BY OrderID;";
            SqlConnection sqlConnection = new(ConnectionString);
            sqlConnection.Open();
            SqlCommand SqlCommand = new(QueryStr, sqlConnection);
            SqlDataAdapter DataAdapter = new(SqlCommand);
            DataTable DataTable = new();
            DataAdapter.Fill(DataTable);
            sqlConnection.Close();
            var DataSource = (from DataRow Data in DataTable.Rows
                              select new Order()
                              {
                                  OrderID = Convert.ToInt32(Data["OrderID"]),
                                  CustomerID = Data["CustomerID"].ToString(),
                                  EmployeeID = Convert.IsDBNull(Data["EmployeeID"]) ? 0 : Convert.ToUInt16(Data["EmployeeID"]),
                                  ShipCity = Data["ShipCity"].ToString(),
                                  Freight = Convert.ToDecimal(Data["Freight"])
                              }).ToList();
            return DataSource;

        }
        [HttpPost]
        [Route("api/Grid/Insert")]
        public void Insert([FromBody] CRUDModel<Order> Value)
        {
            string Query = $"Insert into Orders(CustomerID,Freight,ShipCity,EmployeeID) values('{Value.Value.CustomerID}','{Value.Value.Freight}','{Value.Value.ShipCity}','{Value.Value.EmployeeID}')";
            SqlConnection SqlConnection = new SqlConnection(ConnectionString);
            SqlConnection.Open();
            SqlCommand SqlCommand = new SqlCommand(Query, SqlConnection);
            SqlCommand.ExecuteNonQuery();
            SqlConnection.Close();
        }

        //// PUT: api/Default/5
        [HttpPost]
        [Route("api/Grid/Update")]
        public void Update([FromBody] CRUDModel<Order> Value)
        {
            string Query = $"Update Orders set CustomerID='{Value.Value.CustomerID}', Freight='{Value.Value.Freight}',EmployeeID='{Value.Value.EmployeeID}',ShipCity='{Value.Value.ShipCity}' where OrderID='{Value.Value.OrderID}'";
            SqlConnection SqlConnection = new SqlConnection(ConnectionString);
            SqlConnection.Open();
            SqlCommand SqlCommand = new SqlCommand(Query, SqlConnection);
            SqlCommand.ExecuteNonQuery();
            SqlConnection.Close();
        }
        //// DELETE: api/ApiWithActions/5
        [HttpPost]
        [Route("api/Grid/Delete")]
        public void Delete([FromBody] CRUDModel<Order> Value)
        {
            string Query = $"Delete from Orders where OrderID={Value.Key}";
            SqlConnection SqlConnection = new SqlConnection(ConnectionString);
            SqlConnection.Open();
            SqlCommand SqlCommand = new SqlCommand(Query, SqlConnection);
            SqlCommand.ExecuteNonQuery();
            SqlConnection.Close();
        }
        [HttpPost]
        [Route("api/Grid/Batch")]
        public void Batch([FromBody] CRUDModel<Order> Value)
        {
            if (Value.Changed != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Changed)
                {
                    string Query = $"Update Orders set CustomerID='{Record.CustomerID}', Freight='{Record.Freight}',EmployeeID='{Record.EmployeeID}',ShipCity='{Record.ShipCity}' where OrderID='{Record.OrderID}'";
                    SqlConnection SqlConnection = new SqlConnection(ConnectionString);
                    SqlConnection.Open();
                    SqlCommand SqlCommand = new SqlCommand(Query, SqlConnection);
                    SqlCommand.ExecuteNonQuery();
                    SqlConnection.Close();
                }

            }
            if (Value.Added != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Added)
                {
                    string Query = $"Insert into Orders(CustomerID,Freight,ShipCity,EmployeeID) values('{Record.CustomerID}','{Record.Freight}','{Record.ShipCity}','{Record.EmployeeID}')";
                    SqlConnection SqlConnection = new SqlConnection(ConnectionString);
                    SqlConnection.Open();
                    SqlCommand SqlCommand = new SqlCommand(Query, SqlConnection);
                    SqlCommand.ExecuteNonQuery();
                    SqlConnection.Close();
                }

            }
            if (Value.Deleted != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Deleted)
                {
                    string Query = $"Delete from Orders where OrderID={Record.OrderID}";
                    SqlConnection SqlConnection = new SqlConnection(ConnectionString);
                    SqlConnection.Open();
                    SqlCommand SqlCommand = new SqlCommand(Query, SqlConnection);
                    SqlCommand.ExecuteNonQuery();
                    SqlConnection.Close();
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
