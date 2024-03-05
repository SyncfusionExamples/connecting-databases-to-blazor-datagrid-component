using System.Data;
using System.Data.SqlClient;

namespace Grid_MSSQL.Data
{
    public class OrderData
    {
        //TODO: Enter the connectionstring of database
        public string ConnectionString = @"<Enter a valid connection string>";
        public async Task<List<Order>> GetOrdersAsync()
        {
            //Create query to fetch data from database
            string Query = "SELECT * FROM dbo.Orders ORDER BY OrderID;";
            List<Order> Orders = null;
            //Create SQL Connection
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                //Using SqlDataAdapter and Query create connection with database 
                SqlDataAdapter Adapter = new SqlDataAdapter(Query, Connection);
                DataSet Data = new DataSet();
                Connection.Open();
                // Using SqlDataAdapter, process the query string and fill the data into the dataset
                Adapter.Fill(Data);
                //Cast the data fetched from Adaptor to List<T>
                Orders = Data.Tables[0].AsEnumerable().Select(r => new Order
                {
                    OrderID = r.Field<int>("OrderID"),
                    CustomerID = r.Field<string>("CustomerID"),
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
            string Query = $"Insert into Orders(CustomerID,Freight,ShipCity,EmployeeID) values('{(Value as Order).CustomerID}','{(Value as Order).Freight}','{(Value as Order).ShipCity}','{(Value as Order).EmployeeID}')";
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            //Execute the SQL Command
            SqlCommand SqlCommand = new SqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            SqlCommand.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task UpdateOrderAsync(Order Value)
        {
            //Create query to update the changes into the database by accessing its properties
            string Query = $"Update Orders set CustomerID='{(Value as Order).CustomerID}', Freight='{(Value as Order).Freight}',EmployeeID='{(Value as Order).EmployeeID}',ShipCity='{(Value as Order).ShipCity}' where OrderID='{(Value as Order).OrderID}'";
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            //Execute the SQL Command
            SqlCommand SqlCommand = new SqlCommand(Query, Connection);
            //Execute this code to reflect the changes into the database
            SqlCommand.ExecuteNonQuery();
            Connection.Close();
        }

        public async Task RemoveOrderAsync(int? Key)
        {
            //Create query to remove the specific from database by passing the primary key column value.
            string Query = $"Delete from Orders where OrderID={Key}";
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            //Execute the SQL Command
            SqlCommand SqlCommand = new SqlCommand(Query, Connection);
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
