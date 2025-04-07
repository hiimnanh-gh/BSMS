using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

[Route("[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly DBCBookStore dbc;
    private readonly IWebHostEnvironment _env;

    public BookController(DBCBookStore db, IWebHostEnvironment env)
    {
        dbc = db;
        _env = env;
    }

    // ✅ API Get List - Lấy danh sách sách + ảnh bìa
    [HttpGet]
    [Route("GetList")]
    public IActionResult GetList()
    {
        var books = dbc.Books.Select(b => new
        {
            b.BookId,
            b.Title,
            b.Author,
            b.ReleaseDate,
            b.Price,
            b.StockQuantity,
            b.Description,
            b.Category,
            CoverImagePath = string.IsNullOrEmpty(b.CoverImagePath)
                             ? "/uploads/default.jpg"  // Nếu không có ảnh, dùng ảnh mặc định
                             : b.CoverImagePath
        }).ToList();

        return Ok(books);
    }

    // ✅ API Upload Multiple Files (Lưu ảnh vào /uploads)
    [HttpPost]
    [Route("UploadBookImages")]
    public async Task<IActionResult> UploadBookImages([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { message = "No files uploaded." });
        }

        if (string.IsNullOrEmpty(_env.WebRootPath))
        {
            _env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uploadedFilePaths = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var sanitizedFileName = Path.GetFileNameWithoutExtension(file.FileName)
                                        .Replace(" ", "_")
                                        .Replace("-", "_");

                var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{uniqueFileName}";
                uploadedFilePaths.Add(relativePath);
            }
        }

        return Ok(new { message = "Upload successful", paths = uploadedFilePaths });
    }

    [HttpPost]
    [Route("AddBook")]
    public async Task<IActionResult> AddBook([FromForm] AddBookRequest request)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Author) || request.Price == null)
        {
            return BadRequest(new { message = "Dữ liệu không hợp lệ" });
        }

        string coverImagePath = null;

        // Nếu có ảnh bìa, tải lên ảnh và lưu đường dẫn
        if (request.CoverImage != null && request.CoverImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var sanitizedFileName = Path.GetFileNameWithoutExtension(request.CoverImage.FileName)
                                    .Replace(" ", "_")
                                    .Replace("-", "_");

            var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}{Path.GetExtension(request.CoverImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.CoverImage.CopyToAsync(stream);
            }

            coverImagePath = $"/uploads/{uniqueFileName}";
        }

        // Tạo BookId mới duy nhất với GUID
        var newBookId = Guid.NewGuid().ToString();

        var newBook = new Book
        {
            BookId = newBookId,
            Title = request.Title,
            Author = request.Author,
            ReleaseDate = request.ReleaseDate,
            Price = request.Price,
            StockQuantity = request.StockQuantity ?? 0,
            Description = request.Description,
            Category = request.Category,
            CoverImagePath = coverImagePath // Lưu đường dẫn ảnh bìa
        };

        // Lưu sách mới vào cơ sở dữ liệu
        dbc.Books.Add(newBook);
        dbc.SaveChanges();

        return Ok(new { message = "Thêm sách thành công", BookId = newBookId });
    }

    // Định nghĩa request model
    public class AddBookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        // Sử dụng IFormFile để nhận ảnh bìa
        public IFormFile CoverImage { get; set; }
    }



    [HttpGet("GetBook/{id}")]
    public async Task<IActionResult> GetBook(string id)
    {
        var book = await dbc.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new { message = "Sách không tồn tại." });
        }

        return Ok(book);  // Trả về thông tin sách
    }


    [HttpPut("UpdateBook/{id}")]
    public async Task<IActionResult> UpdateBook(string id, [FromBody] UpdateBookModel model)
    {
        // Kiểm tra xem sách có tồn tại trong database không
        var book = await dbc.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new { message = "Sách không tồn tại." });
        }

        // Cập nhật giá và số lượng
        book.Price = model.Price;
        book.StockQuantity = model.StockQuantity;

        try
        {
            // Lưu thay đổi vào database
            await dbc.SaveChangesAsync();
            return Ok(new { message = "Sách đã được cập nhật." });
        }
        catch (Exception ex)
        {
            // Xử lý lỗi khi lưu
            return StatusCode(500, new { message = "Lỗi khi cập nhật sách.", error = ex.Message });
        }
    }

    public class UpdateBookModel
    {
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }


    [HttpDelete("DeleteBook/{id}")]
    public async Task<IActionResult> DeleteBook(string id)
    {
        var book = await dbc.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new { message = "Sách không tồn tại." });
        }

        try
        {
            dbc.Books.Remove(book);  // Xóa sách khỏi database
            await dbc.SaveChangesAsync();
            return Ok(new { message = "Sách đã được xóa thành công." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi xóa sách.", error = ex.Message });
        }
    }



}
