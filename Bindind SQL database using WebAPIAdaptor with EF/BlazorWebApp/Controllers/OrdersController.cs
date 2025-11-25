using BlazorApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private OrdersDetailsContext _context;
        
        public OrdersController (OrdersDetailsContext context )
        {
            _context = context;
        }
        
        [HttpGet]
        public object Get ()
        {
            return new { Items = _context.Orders, Count = _context.Orders.Count() };
        }
        
        [HttpPost]
        public void Post ( [FromBody]Order newOrder )
        {
            _context.Orders.Add(newOrder);
            _context.SaveChanges();
        }
     
        [HttpPut]
        public void Put ( [FromBody]Order updatedOrder )
        {
            Order _updatedOrder = _context.Orders.Where(x => x.OrderId.Equals(updatedOrder.OrderId)).FirstOrDefault();
            _updatedOrder.CustomerId = updatedOrder.CustomerId;
            _updatedOrder.Freight = updatedOrder.Freight;
            _updatedOrder.OrderDate = updatedOrder.OrderDate;
            _context.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void Delete(int id )
        {
            Order deletingOrder = _context.Orders.Where(x => x.OrderId.Equals(id)).FirstOrDefault();
            _context.Orders.Remove(deletingOrder);
            _context.SaveChanges();
        }
    }
}
