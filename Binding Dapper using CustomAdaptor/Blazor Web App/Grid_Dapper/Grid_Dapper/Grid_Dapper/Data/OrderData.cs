using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace Grid_Dapper.Data
{
    public class OrderData
    {
        //TODO: Enter the connectionstring of database
        string ConnectionString = $"<Enter your connection string>";
        public async Task<List<Order>> GetOrdersAsync()
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
        public async Task AddOrderAsync(Order Value)
        {
            //Create query to insert the specific into the database by accessing its properties 
            string query = "INSERT INTO Orders(CustomerID, Freight, ShipCity, EmployeeID) VALUES(@CustomerID, @Freight, @ShipCity, @EmployeeID)";
            using (IDbConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                //Execute this code to reflect the changes into the database
                await Connection.ExecuteAsync(query, Value);
            }
        }

        public async Task UpdateOrderAsync(Order Value)
        {
            //Create query to update the changes into the database by accessing its properties
            string query = "UPDATE Orders SET CustomerID = @CustomerID, Freight = @Freight, EmployeeID = @EmployeeID, ShipCity = @ShipCity WHERE OrderID = @OrderID";
            using (IDbConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                //Execute this code to reflect the changes into the database
                await Connection.ExecuteAsync(query, Value);
            }
        }

        public async Task RemoveOrderAsync(int? Key)
        {
            //Create query to remove the specific from database by passing the primary key column value.
            string query = "DELETE FROM Orders WHERE OrderID = @OrderID";
            using (IDbConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                //Execute this code to reflect the changes into the database
                await Connection.ExecuteAsync(query, new { OrderID = Key });
            }
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
