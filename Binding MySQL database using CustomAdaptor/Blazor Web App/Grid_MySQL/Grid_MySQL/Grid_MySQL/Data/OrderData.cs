using MySql.Data.MySqlClient;
using System.Data;


namespace Grid_MySQL.Data
{
    public class OrderData
    {

        //TODO: Enter the connectionstring of database
        public string ConnectionString = @"<Enter your connectionstring here>";
        public async Task<List<Order>> GetOrdersAsync()
        {
            //Create query to fetch data from database
            string Query = "SELECT * FROM orders ORDER BY OrderID;";
            List<Order> Orders = null;
            //Create SQL Connection
            using (MySqlConnection Connection = new MySqlConnection(ConnectionString))
            {
                //Using MySqlDataAdapter and Query create connection with database 
                MySqlDataAdapter adapter = new MySqlDataAdapter(Query, Connection);
                DataSet data = new DataSet();
                Connection.Open();
                // Using MySqlDataAdapter, process the query string and fill the data into the dataset
                adapter.Fill(data);
                //Cast the data fetched from MySqlDataAdapter to List<T>
                Orders = data.Tables[0].AsEnumerable().Select(r => new Order
                {
                    OrderID = r.Field<int>("OrderID"),
                    CustomerName = r.Field<string>("CustomerName"),
                    EmployeeID = r.Field<int>("EmployeeID"),
                    ShipCity = r.Field<string>("ShipCity"),
                    Freight = r.Field<decimal>("Freight")
                }).ToList();
                Connection.Close();
            }
            return Orders;
        }
        public async Task AddOrderAsync(Order Value)
        {
            //Create query to insert the specific into the database by accessing its properties
            string Query = $"Insert into Orders(OrderID,CustomerName,Freight,ShipCity,EmployeeID) values('{(Value as Order).OrderID}','{(Value as Order).CustomerName}','{(Value as Order).Freight}','{(Value as Order).ShipCity}','{(Value as Order).EmployeeID}')";
            MySqlConnection Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
            //Execute the MySQL Command
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task UpdateOrderAsync(Order Value)
        {
            //Create query to update the changes into the database by accessing its properties
            string Query = $"Update Orders set CustomerName='{(Value as Order).CustomerName}', Freight='{(Value as Order).Freight}',EmployeeID='{(Value as Order).EmployeeID}',ShipCity='{(Value as Order).ShipCity}' where OrderID='{(Value as Order).OrderID}'";
            MySqlConnection Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
            //Execute the MySQL Command
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task RemoveOrderAsync(int? Key)
        {
            //Create query to remove the specific from database by passing the primary key column value.
            string Query = $"Delete from Orders where OrderID={Key}";
            MySqlConnection Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
            //Execute the MySQL Command
            MySqlCommand Command = new MySqlCommand(Query, Connection);
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
