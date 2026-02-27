using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Data
{
    [Table("Order")]
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public decimal Freight { get; set; }
        public string ShipCity { get; set; }
    }
}
