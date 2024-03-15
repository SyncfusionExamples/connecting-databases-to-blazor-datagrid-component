using Npgsql;
using System.Data;

namespace Grid_PostgreSQL.Data
{
    public class OrderData
    {

        //TODO: Enter the connectionstring of database
        public string ConnectionString = @"<Enter your connectionstring here>";
        public async Task<List<Order>> GetOrdersAsync()
        {
            //TODO: Create query based on your table to fetch data from database
            string Query = "SELECT * FROM public.\"Orders\" ORDER BY \"OrderID\"";
            List<Order> Orders = new List<Order>();
            //Create Npgsql Connection
            NpgsqlConnection sqlConnection = new(ConnectionString);
            sqlConnection.Open();
            //Using NpgsqlCommand and Query create connection with database
            NpgsqlCommand SqlCommand = new(Query, sqlConnection);
            //Using NpgsqlDataAdapter execute the NpgsqlCommand 
            NpgsqlDataAdapter DataAdapter = new(SqlCommand);
            DataTable DataTable = new();
            // Using NpgsqlDataAdapter, process the query string and fill the data into the dataset
            DataAdapter.Fill(DataTable);
            sqlConnection.Close();
            //Cast the data fetched from NpgsqlDataAdapter to List<T>
            Orders = (from DataRow Data in DataTable.Rows
                      select new Order()
                      {
                          OrderID = Convert.ToInt32(Data["OrderID"]),
                          CustomerName = Data["CustomerName"].ToString(),
                          EmployeeID = Convert.ToInt32(Data["EmployeeID"]),
                          ShipCity = Data["ShipCity"].ToString(),
                          Freight = Convert.ToDecimal(Data["Freight"])
                      }).ToList();
            return Orders;
        }
        public async Task AddOrderAsync(Order Value)
        {
            //TODO: Create query to insert the specific into the database by accessing its properties
            string Query = $"Insert into \"Orders\" (\"OrderID\", \"CustomerName\", \"Freight\", \"ShipCity\", \"EmployeeID\") values({(Value as Order).OrderID}, '{(Value as Order).CustomerName}', {(Value as Order).Freight}, '{(Value as Order).ShipCity}', {(Value as Order).EmployeeID})";
            NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
            //Execute the NpgSQL Command
            NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task UpdateOrderAsync(Order Value)
        {
            //TODO: Create query to update the changes into the database by accessing its properties
            string Query = $"Update \"Orders\" set \"CustomerName\"='{(Value as Order).CustomerName}', \"Freight\"={(Value as Order).Freight},\"EmployeeID\"={(Value as Order).EmployeeID},\"ShipCity\"='{(Value as Order).ShipCity}' where \"OrderID\"={(Value as Order).OrderID}";
            NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
            //Execute the NpgSQL Command
            NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task RemoveOrderAsync(int? Key)
        {
            //TODO: Create query to remove the specific from database by passing the primary key column value.
            string Query = $"DELETE FROM \"Orders\" WHERE \"OrderID\" = {Key}";
            NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
            //Execute the NpgSQL Command
            NpgsqlCommand Command = new NpgsqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
        }
    }
    public class Order
    {
        public int? OrderID { get; set; }
        public string CustomerName { get; set; }
        public int EmployeeID { get; set; }
        public decimal Freight { get; set; }
        public string ShipCity { get; set; }
    }
}
