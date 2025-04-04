using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        DBCBookStore dbc;
        public OrderItemController(DBCBookStore db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("GetList")]
        public IActionResult GetList()
        {
            return Ok(dbc.OrderItems.ToList());
        }

        [HttpPost("InsertItems")]
        public async Task<IActionResult> InsertItems([FromBody] List<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            dbc.OrderItems.AddRange(orderItems);
            await dbc.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        [Route("Update")]
        public IActionResult Update(string orderID, string bookID, int quantity)
        {
            if (string.IsNullOrEmpty(orderID) || string.IsNullOrEmpty(bookID))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng không hợp lệ" });
            }

            var existingOrderItem = dbc.OrderItems.Find(orderID, bookID);
            if (existingOrderItem == null)
            {
                return NotFound(new { message = "Không tìm thấy mục đặt hàng" });
            }

            existingOrderItem.OrderId = orderID;
            existingOrderItem.BookId = bookID;
            existingOrderItem.Quantity = quantity;

            dbc.OrderItems.Update(existingOrderItem);
            dbc.SaveChanges();
            return Ok(existingOrderItem);
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(string orderID, string bookID)
        {
            if (string.IsNullOrEmpty(orderID) || string.IsNullOrEmpty(bookID))
            {
                return BadRequest(new { message = "ID không hợp lệ" });
            }
            var xoaOrderItem = dbc.OrderItems.Find(orderID, bookID);

            if (xoaOrderItem == null)
            {
                return NotFound(new { message = "Không tìm thấy mục đặt hàng" });
            }

            dbc.OrderItems.Remove(xoaOrderItem);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
