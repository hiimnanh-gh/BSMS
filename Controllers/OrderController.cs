using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        DBCBookStore dbc;
        public OrderController(DBCBookStore db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("GetList")]
        public IActionResult GetList()
        {
            return Ok(dbc.Orders.ToList());
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(string orderID, string userID, string status, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(orderID) || string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (totalAmount <= 0)
            {
                return BadRequest(new { message = "Giá không hợp lệ" });
            }

            var order = new Order
            {
                OrderId = orderID,
                UserId = userID,
                OrderDate = DateTime.Now,
                Status = status,
                TotalAmount = totalAmount
            };

            dbc.Orders.Add(order);
            dbc.SaveChanges();
            return Ok(order);
        }

        [HttpPost]
        [Route("Update")]
        public IActionResult Update(string orderID, string userID, string status, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(orderID) || string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (totalAmount <= 0)
            {
                return BadRequest(new { message = "Giá không hợp lệ" });
            }
            var existingOrder = dbc.Orders.Find(orderID);
            if (existingOrder == null)
            {
                return NotFound(new { message = "Không tìm thấy hoá đơn" });
            }

            existingOrder.OrderId = orderID;
            existingOrder.UserId = userID;
            existingOrder.OrderDate = DateTime.Now;
            existingOrder.Status = status;
            existingOrder.TotalAmount = totalAmount;

            dbc.Orders.Update(existingOrder);
            dbc.SaveChanges();
            return Ok(existingOrder);
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(string orderID)
        {
            if (string.IsNullOrEmpty(orderID))
            {
                return BadRequest(new { message = "ID không hợp lệ" });
            }
            var xoaOrder = dbc.Orders.Find(orderID);

            if (xoaOrder == null)
            {
                return NotFound(new { message = "Không tìm thấy hoá đơn" });
            }

            dbc.Orders.Remove(xoaOrder);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
