using Microsoft.AspNetCore.Mvc;
using BookStoreAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

[Route("User")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DBCBookStore _context;

    public UserController(DBCBookStore context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null)
        {
            return BadRequest(new { message = "Dữ liệu không hợp lệ!" });
        }

        // Kiểm tra tên đăng nhập và mật khẩu có được nhập chưa
        if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest(new { message = "Tên đăng nhập và mật khẩu không được để trống!" });
        }

        // Kiểm tra tài khoản người dùng có tồn tại không
        var user = await _context.Users
            .Where(u => u.Username == loginRequest.Username)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return Unauthorized(new { message = "Tên đăng nhập không tồn tại!" });
        }

        // Kiểm tra mật khẩu
        if (user.Password != loginRequest.Password)
        {
            return Unauthorized(new { message = "Mật khẩu không chính xác!" });
        }

        // Đăng nhập thành công, trả về thông tin người dùng
        return Ok(new
        {
            userId = user.UserId,
            username = user.Username,
            message = "Đăng nhập thành công!"
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest(new { message = "Dữ liệu không hợp lệ!" });
        }

        if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email))
        {
            return BadRequest(new { message = "Username, Password và Email không được để trống!" });
        }

        if (_context.Users.Any(u => u.Username == user.Username))
        {
            return BadRequest(new { message = "Username đã tồn tại!" });
        }

        // Kiểm tra role hợp lệ
        string[] validRoles = new string[] { "customer", "warehouse manager", "admin", "editor" };
        if (string.IsNullOrEmpty(user.Role) || !validRoles.Contains(user.Role.ToLower()))
        {
            return BadRequest(new { message = "Role không hợp lệ! Chỉ chấp nhận: customer, warehouse manager, admin, editor." });
        }

        // Không mã hóa mật khẩu nữa
        user.Password = user.Password;  // Mã hóa có thể bỏ qua nếu bạn không cần nữa

        user.UserId = string.IsNullOrEmpty(user.UserId) ? Guid.NewGuid().ToString() : user.UserId;

        // Lưu người dùng vào cơ sở dữ liệu
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đăng ký thành công!" });
    }

}
