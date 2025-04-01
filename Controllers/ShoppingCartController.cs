using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        DBCBookStore dbc;

        public ShoppingCartController(DBCBookStore db)
        {
            dbc = db;
        }

        // Lấy danh sách giỏ hàng
        [HttpGet]
        [Route("GetList")]
        public IActionResult GetList()
        {
            return Ok(dbc.ShoppingCarts.ToList());
        }

        // Thêm giỏ hàng mới hoặc cập nhật giỏ hàng cho người dùng
        [HttpPost("Insert")]
        public IActionResult Insert(string cartID, string bookID, int quantity)
        {
            // Kiểm tra nếu cartID chưa có trong ShoppingCart
            var cartExists = dbc.ShoppingCarts.Any(c => c.CartId == cartID);
            if (!cartExists)
            {
                // Tạo giỏ hàng mới nếu chưa có
                var newCart = new ShoppingCart { CartId = cartID };
                dbc.ShoppingCarts.Add(newCart);
                dbc.SaveChanges();
            }

            // Thêm sản phẩm vào giỏ hàng
            var newItem = new CartItem
            {
                CartId = cartID,
                BookId = bookID,
                Quantity = quantity
            };

            dbc.CartItems.Add(newItem);
            dbc.SaveChanges();

            return Ok(new { message = "Thêm vào giỏ hàng thành công!" });
        }

        // Cập nhật giỏ hàng
        [HttpPost]
        [Route("Update")]
        public IActionResult Update(string cartID, string userID, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(cartID) || string.IsNullOrEmpty(userID))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (totalAmount <= 0)
            {
                return BadRequest(new { message = "Giá tiền không hợp lệ" });
            }

            var existingShoppingCart = dbc.ShoppingCarts.Find(cartID);
            if (existingShoppingCart == null)
            {
                return NotFound(new { message = "Không tìm thấy giỏ hàng" });
            }

            existingShoppingCart.CartId = cartID;
            existingShoppingCart.UserId = userID;
            existingShoppingCart.TotalAmount = totalAmount;

            dbc.ShoppingCarts.Update(existingShoppingCart);
            dbc.SaveChanges();
            return Ok(existingShoppingCart);
        }

        // Xóa giỏ hàng
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(string cartID)
        {
            if (string.IsNullOrEmpty(cartID))
            {
                return BadRequest(new { message = "ID không hợp lệ" });
            }

            var xoaShoppingCart = dbc.ShoppingCarts.Find(cartID);

            if (xoaShoppingCart == null)
            {
                return NotFound(new { message = "Không tìm thấy giỏ hàng" });
            }

            dbc.ShoppingCarts.Remove(xoaShoppingCart);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa thành công" });
        }

        // Tạo giỏ hàng mới cho người dùng nếu chưa có giỏ hàng
        [HttpGet("GetOrCreateCart")]
        public IActionResult GetOrCreateCart(string userID)
        {
            var cart = dbc.ShoppingCarts.FirstOrDefault(c => c.UserId == userID);

            if (cart != null)
            {
                // Nếu đã có giỏ hàng, trả về cartID
                return Ok(new { cartID = cart.CartId });
            }
            else
            {
                // Tạo mới giỏ hàng cho người dùng mỗi lần
                var newCartID = Guid.NewGuid().ToString(); // Tạo cartID mới, có thể dùng GUID để tạo cartID duy nhất
                var newCart = new ShoppingCart
                {
                    CartId = newCartID,
                    UserId = userID,
                    TotalAmount = 0 // Giỏ hàng mới sẽ có tổng tiền ban đầu là 0
                };

                // Lưu giỏ hàng mới vào cơ sở dữ liệu
                dbc.ShoppingCarts.Add(newCart);
                dbc.SaveChanges();

                // Trả về cartID mới cho frontend
                return Ok(new { cartID = newCartID });
            }
        }


        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            if (string.IsNullOrEmpty(request.CartID) || string.IsNullOrEmpty(request.BookID) || request.Quantity <= 0)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            var existingCart = dbc.ShoppingCarts.FirstOrDefault(c => c.CartId == request.CartID);
            if (existingCart == null)
            {
                return NotFound(new { message = "Giỏ hàng không tồn tại" });
            }

            var existingCartItem = dbc.CartItems
                .FirstOrDefault(item => item.CartId == request.CartID && item.BookId == request.BookID);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += request.Quantity;
                dbc.CartItems.Update(existingCartItem);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    CartId = request.CartID,
                    BookId = request.BookID,
                    Quantity = request.Quantity
                };
                dbc.CartItems.Add(newCartItem);
            }

            dbc.SaveChanges();

            return Ok(new { message = "Thêm sách vào giỏ thành công" });
        }
        public class AddToCartRequest
        {
            public string CartID { get; set; }
            public string BookID { get; set; }
            public int Quantity { get; set; }
        }
    }



}


