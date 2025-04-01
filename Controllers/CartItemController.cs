using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        DBCBookStore dbc;
        public CartItemController(DBCBookStore db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("GetList")]
        public IActionResult GetList()
        {
            return Ok(dbc.CartItems.ToList());
        }


        [HttpGet("Check")]
        public IActionResult CheckCartItem(string cartID, string bookID, int quantity)
        {
            if (quantity == null)
            {
                quantity = 1;
            }

            Console.WriteLine($"CartID: {cartID}, BookID: {bookID}"); // Log tham số
            var cartItem = dbc.CartItems.FirstOrDefault(ci => ci.CartId == cartID && ci.BookId == bookID && ci.Quantity == quantity);

            if (cartItem != null)
            {
                return Ok(new { message = "Sản phẩm đã có trong giỏ hàng", cartItem});
            }

            return Ok(new { message = "Sản phẩm chưa có trong giỏ hàng" });
        }




        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(string cartID, string bookID, int quantity)
        {
            // 🔍 Lấy CartID từ session
            if (string.IsNullOrEmpty(cartID))
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ" });
            }

            Console.WriteLine($"🛒 Thêm vào giỏ: CartID={cartID}, BookID={bookID}, Quantity={quantity}");

            if (string.IsNullOrEmpty(bookID) || quantity <= 0)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            // 🔍 Kiểm tra sản phẩm có trong giỏ chưa
            var existingCartItem = dbc.CartItems.FirstOrDefault(c => c.CartId == cartID && c.BookId == bookID);
            if (existingCartItem != null)
            {
                // 🔄 Cập nhật số lượng
                existingCartItem.Quantity += quantity;
                dbc.CartItems.Update(existingCartItem);
            }
            else
            {
                // 🆕 Thêm mới sản phẩm vào giỏ
                var cartItem = new CartItem
                {
                    CartId = cartID,
                    BookId = bookID,
                    Quantity = quantity
                };
                dbc.CartItems.Add(cartItem);
            }

            dbc.SaveChanges();
            Console.WriteLine("✅ Đã lưu vào DB thành công");
            return Ok(new { message = "Sản phẩm đã được thêm vào giỏ" });
        }



        [HttpPut("Update")]
        public IActionResult Update(string cartID, string bookID, int quantity)
        {
            if (string.IsNullOrEmpty(cartID) || string.IsNullOrEmpty(bookID))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng không hợp lệ" });
            }

            if (quantity == null)
            {
                return BadRequest(new { message = "đéo có gì" });
            }

            var existingCartItem = dbc.CartItems.FirstOrDefault(c => c.CartId == cartID && c.BookId == bookID);
            if (existingCartItem == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ" });
            }

            existingCartItem.Quantity = quantity;
            dbc.CartItems.Update(existingCartItem);
            dbc.SaveChanges();

            return Ok(new { message = "Cập nhật thành công", cartItem = existingCartItem });
        }




        [HttpDelete("Delete")]
        public IActionResult Delete(string cartID, string bookID)
        {
            if (string.IsNullOrEmpty(cartID) || string.IsNullOrEmpty(bookID))
            {
                return BadRequest(new { message = "Thiếu cartID hoặc bookID" });
            }

            var cartItem = dbc.CartItems.FirstOrDefault(c => c.CartId == cartID && c.BookId == bookID);
            if (cartItem == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ" });
            }

            dbc.CartItems.Remove(cartItem);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa thành công" });
        }

    }
}
