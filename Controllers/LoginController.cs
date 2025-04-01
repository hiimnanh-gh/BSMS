using Microsoft.AspNetCore.Mvc;
using BookStoreAPI.Models;
using BookStoreAPI.Services;
using System;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IUserService _userService;

    public LoginController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Username và password không được để trống!" });
        }

        var user = _userService.GetUserByUsername(request.Username);
        if (user == null || user.Password != request.Password)
        {
            return Unauthorized(new { message = "Sai username hoặc password!" });
        }

        return Ok(new
        {
            message = "Đăng nhập thành công!",
            userId = user.UserId,
            username = user.Username,
            role = user.Role
        });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
