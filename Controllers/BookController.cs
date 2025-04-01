using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BookStoreAPI.Models;

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
}




















