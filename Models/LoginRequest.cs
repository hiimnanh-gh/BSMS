using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
