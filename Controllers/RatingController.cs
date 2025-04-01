using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        DBCBookStore dbc;
        public RatingController(DBCBookStore db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("GetList")]
        public IActionResult GetList()
        {
            return Ok(dbc.Ratings.ToList());
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(string ratingID, string userID, string bookID, int rate)
        {
            if (string.IsNullOrEmpty(ratingID) || string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(bookID))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (rate < 0 && rate > 5)
            {
                return BadRequest(new { message = "Đánh giá không hợp lệ" });
            }

            var rating = new Rating
            {
                RatingId = ratingID,
                UserId = userID,
                BookId = bookID,
                Rate = rate,
                RatingDate = DateTime.Now
            };

            dbc.Ratings.Add(rating);
            dbc.SaveChanges();
            return Ok(rating);
        }

        [HttpPost]
        [Route("Update")]
        public IActionResult Update(string ratingID, string userID, string bookID, int rate)
        {
            if (string.IsNullOrEmpty(ratingID) || string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(bookID))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (rate < 0 && rate > 5)
            {
                return BadRequest(new { message = "Đánh giá không hợp lệ" });
            }

            var existingRating = dbc.Ratings.Find(ratingID);
            if (existingRating == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá" });
            }

            existingRating.RatingId = ratingID;
            existingRating.UserId = userID;
            existingRating.BookId = bookID;
            existingRating.Rate = rate;
            existingRating.RatingDate = DateTime.Now;

            dbc.Ratings.Update(existingRating);
            dbc.SaveChanges();
            return Ok(existingRating);
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(string ratingID)
        {
            if (string.IsNullOrEmpty(ratingID))
            {
                return BadRequest(new { message = "ID không hợp lệ" });
            }
            var xoaRating = dbc.Ratings.Find(ratingID);

            if (xoaRating == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá" });
            }

            dbc.Ratings.Remove(xoaRating);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
