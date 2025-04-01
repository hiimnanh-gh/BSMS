using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        DBCBookStore dbc;
        public ReceiptController(DBCBookStore db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("GetList")]
        public IActionResult GetList()
        {
            return Ok(dbc.Receipts.ToList());
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(string receiptID, string orderID, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(receiptID) || string.IsNullOrEmpty(orderID))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (totalAmount <= 0)
            {
                return BadRequest(new { message = "Giá tiền không hợp lệ" });
            }

            var receipt = new Receipt
            {
                ReceiptId = receiptID,
                OrderId = orderID,
                TotalAmount = totalAmount,
                CreatedTime = DateTime.Now
            };

            dbc.Receipts.Add(receipt);
            dbc.SaveChanges();
            return Ok(receipt);
        }

        [HttpPost]
        [Route("Update")]
        public IActionResult Update(string receiptID, string orderID, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(receiptID) || string.IsNullOrEmpty(orderID))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (totalAmount <= 0)
            {
                return BadRequest(new { message = "Giá tiền không hợp lệ" });
            }

            var existingReceipt = dbc.Receipts.Find(receiptID);
            if (existingReceipt == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá" });
            }

            existingReceipt.ReceiptId = receiptID;
            existingReceipt.OrderId = orderID;
            existingReceipt.TotalAmount = totalAmount;
            existingReceipt.CreatedTime = DateTime.Now;

            dbc.Receipts.Update(existingReceipt);
            dbc.SaveChanges();
            return Ok(existingReceipt);
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(string receiptID)
        {
            if (string.IsNullOrEmpty(receiptID))
            {
                return BadRequest(new { message = "ID không hợp lệ" });
            }
            var xoaReceipt = dbc.Receipts.Find(receiptID);

            if (xoaReceipt == null)
            {
                return NotFound(new { message = "Không tìm thấy biên lai" });
            }

            dbc.Receipts.Remove(xoaReceipt);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
