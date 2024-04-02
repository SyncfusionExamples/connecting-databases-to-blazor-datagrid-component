using System.Data;
using Microsoft.Data.Sqlite;

namespace Grid_SQLite.Data
{
    public class OrderData
    {
        // TODO: Enter the connection string of the database
        string ConnectionString = "<Enter your connectionstring here>";
        public async Task<List<Order>> GetOrdersAsync()
        {
            // Create query to fetch data from the database
            string Query = "SELECT * FROM Orders ORDER BY OrderID;";
            List<Order> Orders = new List<Order>();
            // Create sqlite Connection
            SqliteConnection Connection = new SqliteConnection(ConnectionString);
            Connection.Open();
            //Using sqliteCommand and Query create connection with database
            SqliteCommand Command = new SqliteCommand(Query, Connection);
            // Execute the SQLite command and retrieve data using SqliteDataReader
            using (SqliteDataReader reader = await Command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Order order = new Order
                    {
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.GetString(reader.GetOrdinal("CustomerID")),
                        EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                        ShipCity = reader.GetString(reader.GetOrdinal("ShipCity")),
                        Freight = reader.GetDecimal(reader.GetOrdinal("Freight"))
                    };
                    Orders.Add(order);
                }
            }
            return Orders;
        }

        public async Task AddOrderAsync(Order Value)
        {
            //Create query to insert the specific into the database by accessing its properties 
            string Query = $"Insert into Orders(OrderID,CustomerID,Freight,ShipCity,EmployeeID) values('{(Value as Order).OrderID}','{(Value as Order).CustomerID}','{(Value as Order).Freight}','{(Value as Order).ShipCity}','{(Value as Order).EmployeeID}')";
            SqliteConnection Connection = new SqliteConnection(ConnectionString);
            Connection.Open();
            //Execute the SQLite Command
            SqliteCommand SqlCommand = new SqliteCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            SqlCommand.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task UpdateOrderAsync(Order Value)
        {
            //Create query to update the changes into the database by accessing its properties
            string Query = $"Update Orders set CustomerID='{(Value as Order).CustomerID}', Freight='{(Value as Order).Freight}',EmployeeID='{(Value as Order).EmployeeID}',ShipCity='{(Value as Order).ShipCity}' where OrderID='{(Value as Order).OrderID}'";
            SqliteConnection Connection = new SqliteConnection(ConnectionString);
            Connection.Open();
            //Execute the SQLite Command
            SqliteCommand SqlCommand = new SqliteCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            SqlCommand.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task RemoveOrderAsync(int? Key)
        {
            //Create query to remove the specific from database by passing the primary key column value.
            string Query = $"Delete from Orders where OrderID={Key}";
            SqliteConnection Connection = new SqliteConnection(ConnectionString);
            Connection.Open();
            //Execute the SQLite Command
            SqliteCommand SqlCommand = new SqliteCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            SqlCommand.ExecuteNonQuery();
            Connection.Close();
        }
    }
    public class Order
    {
        public int? OrderID { get; set; }
        public string CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public decimal Freight { get; set; }
        public string ShipCity { get; set; }
    }
}
