using BookStoreAPI.Models;
using System;
using System.Linq;

namespace BookStoreAPI.Services
{
    public interface IUserService
    {
        bool VerifyUserCredentials(string username, string password);
        User GetUserByUsername(string username);
        string HashPassword(string password);
    }

    public class UserService : IUserService
    {
        private readonly DBCBookStore _context;

        public UserService(DBCBookStore context)
        {
            _context = context;
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool VerifyUserCredentials(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null)
            {
                return false;
            }

            return VerifyPassword(password, user.Password);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // If using plain text passwords as fallback (temporary migration strategy)
                // return password == passwordHash;

                // Otherwise, invalid hash should fail verification
                return false;
            }
            catch (Exception)
            {
                // Any other exception should also fail verification
                return false;
            }
        }
    }
}