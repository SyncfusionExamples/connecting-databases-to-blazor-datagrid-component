﻿using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Grid_PostgreSQL.Controllers
{
    [ApiController]
    public class GridController : ControllerBase
    {
        //TODO: Enter the connectionstring of database
        string ConnectionString = @"<Enter Your connectionstring here>";
        [HttpPost]
        [Route("api/[controller]")]
        /// <summary>
        /// Returns the data collection as result and count after performing data operations based on request from <see cref=”DataManagerRequest”/>
        /// </summary>
        /// <param name="DataManagerRequest">DataManagerRequest contains the information regarding searching, filtering, sorting, aggregates and paging which is handled on the Blazor DataGrid component side</param>
        /// <returns>The data collection's type is determined by how this method has been implemented.</returns>
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
            int count = DataSource.Cast<Order>().Count();
            // Handling Aggregation in UrlAdaptor.
            IDictionary<string, object> Aggregates = null;
            if (DataManagerRequest.Aggregates != null) // Aggregation
            {
                Aggregates = DataUtil.PerformAggregation(DataSource, DataManagerRequest.Aggregates);
            }
            // Handling paging in UrlAdaptor.
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
            return new { result = DataSource, count = count, aggregates = Aggregates };
        }
        [Route("api/[controller]")]
        public List<Order> GetOrderData()
        {
            //TODO: Create query string based on your table
            string QueryStr = "SELECT * FROM public.\"Orders\" ORDER BY \"OrderID\"";
            //Create Npgsql Connection
            NpgsqlConnection sqlConnection = new(ConnectionString);
            sqlConnection.Open();
            //Using NpgsqlCommand and Query create connection with database
            NpgsqlCommand SqlCommand = new(QueryStr, sqlConnection);
            //Using NpgsqlDataAdapter execute the NpgsqlCommand  
            NpgsqlDataAdapter DataAdapter = new(SqlCommand);
            DataTable DataTable = new();
            //Process the query string and fill the data into the dataset
            DataAdapter.Fill(DataTable);
            sqlConnection.Close();
            //Cast the data fetched from NpgsqlDataAdapter to List<T>
            var DataSource = (from DataRow Data in DataTable.Rows
                              select new Order()
                              {
                                  OrderID = Convert.ToInt32(Data["OrderID"]),
                                  CustomerName = Data["CustomerName"].ToString(),
                                  EmployeeID = Convert.ToInt32(Data["EmployeeID"]),
                                  ShipCity = Data["ShipCity"].ToString(),
                                  Freight = Convert.ToDecimal(Data["Freight"])
                              }).ToList();
            return DataSource;

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
            //TODO: Create query to insert the specific into the database by accessing its properties 
            string Query = $"Insert into \"Orders\" (\"OrderID\", \"CustomerName\", \"Freight\", \"ShipCity\", \"EmployeeID\") values( '{Value.Value.OrderID}','{Value.Value.CustomerName}','{Value.Value.Freight}','{Value.Value.ShipCity}','{Value.Value.EmployeeID}')";
            NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
            //Execute the Npgsql Command
            NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
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
            //TODO: Create query to update the changes into the database by accessing its properties
            string Query = $"Update \"Orders\" set \"CustomerName\"='{Value.Value.CustomerName}', \"Freight\"={Value.Value.Freight},\"EmployeeID\"={Value.Value.EmployeeID},\"ShipCity\"='{Value.Value.ShipCity}' where \"OrderID\"={Value.Value.OrderID}";
            NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
            //Execute the Npgsql Command
            NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
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
            //TODO: Create query to remove the specific from database by passing the primary key column value.
            string Query = $"DELETE FROM \"Orders\" WHERE \"OrderID\" = {Value.Key}";
            NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
            //Execute the Npgsql Command
            NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
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
                    //TODO: Create query to update the changes into the database by accessing its properties
                    string Query = $"Update \"Orders\" set \"CustomerName\"='{Record.CustomerName}', \"Freight\"={Record.Freight},\"EmployeeID\"={Record.EmployeeID},\"ShipCity\"='{Record.ShipCity} ' where \"OrderID\"= {Record.OrderID}";
                    NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
                    Connection.Open();
                    //Execute the Npgsql Command
                    NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
                    //Execute this code to reflect the changes into the database
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }

            }
            if (Value.Added != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Added)
                {
                    //TODO: Create query to insert the specific into the database by accessing its properties 
                    string Query = $"Insert into \"Orders\" (\"OrderID\", \"CustomerName\", \"Freight\", \"ShipCity\", \"EmployeeID\") values('{Record.OrderID}', '{Record.CustomerName}','{Record.Freight}','{Record.ShipCity}','{Record.EmployeeID}')";
                    NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
                    Connection.Open();
                    //Execute the Npgsql Command
                    NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
                    //Execute this code to reflect the changes into the database
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }

            }
            if (Value.Deleted != null)
            {
                foreach (var Record in (IEnumerable<Order>)Value.Deleted)
                {
                    //TODO: Create query to remove the specific from database by passing the primary key column value.
                    string Query = $"DELETE FROM \"Orders\" WHERE \"OrderID\"={Record.OrderID}";
                    NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
                    Connection.Open();
                    //Execute the Npgsql Command
                    NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
                    //Execute this code to reflect the changes into the database
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }
            }
        }
        public class Order
        {
            [Key]
            public int? OrderID { get; set; }
            public string? CustomerName { get; set; }
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
