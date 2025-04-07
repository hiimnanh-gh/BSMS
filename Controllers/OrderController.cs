using System.IdentityModel.Tokens.Jwt;
using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder(OrderRequest orderRequest)
        {
            Console.WriteLine("== BẮT ĐẦU XỬ LÝ ĐƠN HÀNG ==");

            if (orderRequest == null)
            {
                Console.WriteLine("❌ orderRequest null");
                return BadRequest("Thông tin đơn hàng không hợp lệ.");
            }

            // Debug xem có gì trong request
            Console.WriteLine($"➡ UserId nhận: {orderRequest.UserId}");
            Console.WriteLine($"➡ Address: {orderRequest.Address}");
            Console.WriteLine($"➡ PaymentMethod: {orderRequest.PaymentMethod}");
            Console.WriteLine($"➡ Số lượng OrderItems: {orderRequest.OrderItems?.Count}");

            // Trim UserId để tránh lỗi ký tự trắng
            var trimmedUserId = orderRequest.UserId?.Trim();

            // In danh sách userID trong database để đối chiếu
            Console.WriteLine("== Danh sách UserId trong DB ==");
            foreach (var u in dbc.Users.ToList())
            {
                Console.WriteLine($"- {u.UserId}");
            }

            // Kiểm tra UserId hợp lệ không
            if (string.IsNullOrWhiteSpace(trimmedUserId) ||
                !dbc.Users.Any(u => u.UserId == trimmedUserId))
            {
                Console.WriteLine("❌ UserId không hợp lệ hoặc không tồn tại trong DB");
                return BadRequest(new { message = "UserId không hợp lệ hoặc không tồn tại." });
            }

            // Kiểm tra các field khác
            if (string.IsNullOrWhiteSpace(orderRequest.Address) ||
                string.IsNullOrWhiteSpace(orderRequest.PaymentMethod) ||
                orderRequest.OrderItems == null || orderRequest.OrderItems.Count == 0)
            {
                Console.WriteLine("❌ Thiếu thông tin đơn hàng");
                return BadRequest("Vui lòng điền đầy đủ thông tin đơn hàng.");
            }

            // Tạo OrderId mới
            var newOrderId = Guid.NewGuid().ToString();
            decimal totalAmount = 0;

            foreach (var item in orderRequest.OrderItems)
            {
                var book = dbc.Books.FirstOrDefault(b => b.BookId == item.BookId);
                if (book != null)
                {
                    totalAmount += (book.Price.GetValueOrDefault() * item.Quantity);
                }
                else
                {
                    Console.WriteLine($"❌ Không tìm thấy sách với BookId: {item.BookId}");
                }
            }

            var user = dbc.Users.FirstOrDefault(u => u.UserId == trimmedUserId);
            if (user == null)
            {
                return BadRequest(new { message = "User không tồn tại." });
            }

            // Thêm để log cho chắc
            Console.WriteLine("➡ User Address: " + user.Address);
            Console.WriteLine("➡ User PaymentMethod: " + user.PaymentInfo);

            var order = new Order
            {
                OrderId = newOrderId,
                UserId = trimmedUserId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = totalAmount,
                PaymentMethod = orderRequest.PaymentMethod?.Trim()
            };

            dbc.Orders.Add(order);
            dbc.SaveChanges();

            // Tạo các mục OrderItems
            foreach (var item in orderRequest.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = newOrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity
                };
                dbc.OrderItems.Add(orderItem);
            }
            dbc.SaveChanges();

            Console.WriteLine("✅ Tạo đơn hàng thành công với OrderId: " + newOrderId);

            return Ok(new { orderId = newOrderId });
        }





        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            dbc.Orders.Add(order);
            await dbc.SaveChangesAsync();
            return Ok(new { orderID = order.OrderId }); // trả về ID của đơn hàng
        }


        [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            // Lấy tất cả đơn hàng của userId từ bảng Orders và bao gồm thông tin OrderItems và Books
            var orders = await dbc.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)  // Lấy thông tin người dùng
                .Include(o => o.OrderItems)  // Lấy chi tiết món hàng trong đơn
                    .ThenInclude(oi => oi.Book)  // Lấy thông tin sách từ OrderItems
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            // Chuyển đổi dữ liệu đơn hàng
            var orderDtos = orders.Select(order => new
            {
                order.OrderId,  // Id đơn hàng
                customerName = order.User.Username,  // Tên người dùng
                customerPhone = order.User.Email,  // Email người dùng
                customerAddress = order.User.Address,  // Địa chỉ người dùng
                orderDate = order.OrderDate,  // Ngày đặt hàng
                status = order.Status,  // Trạng thái đơn hàng
                totalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,// Tổng số tiền
                items = order.OrderItems.Select(oi => new  // Lấy thông tin sách từ OrderItems
                {
                    bookTitle = oi.Book.Title,  // Tên sách
                    quantity = oi.Quantity,  // Số lượng
                    price = oi.Book.Price  // Giá sách
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
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