using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

[Route("api/upload")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly string _uploadPath = "wwwroot/uploads"; // Thư mục lưu ảnh

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("No file uploaded");

        try
        {
            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Trả về đường dẫn ảnh để lưu vào DB
            string imageUrl = $"/uploads/{fileName}";
            return Ok(new { imageUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
