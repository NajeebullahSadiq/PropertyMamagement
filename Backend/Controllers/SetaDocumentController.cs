using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models.Property;

namespace WebAPIBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SetaDocumentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SetaDocumentController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] string setaNumber)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "لطفاً فایل را انتخاب کنید" });
                }

                if (string.IsNullOrWhiteSpace(setaNumber))
                {
                    return BadRequest(new { message = "لطفاً سټه نمبر را وارد کنید" });
                }

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { message = "حجم فایل نباید بیشتر از 10 مگابایت باشد" });
                }

                // Validate file extension
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    return BadRequest(new { message = "فرمت فایل مجاز نیست. فرمت‌های مجاز: PDF, JPG, PNG, DOC, DOCX" });
                }

                // Create upload directory if not exists
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Resources", "SetaDocuments");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Save to database
                var document = new SetaDocument
                {
                    SetaNumber = setaNumber,
                    FilePath = $"Resources/SetaDocuments/{uniqueFileName}",
                    OriginalFileName = file.FileName,
                    FileType = fileExtension,
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    UploadedBy = User.Identity?.Name
                };

                _context.SetaDocuments.Add(document);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "فایل با موفقیت آپلود شد",
                    documentId = document.Id,
                    setaNumber = document.SetaNumber,
                    fileName = document.OriginalFileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در آپلود فایل", error = ex.Message });
            }
        }

        [HttpGet("by-seta-number/{setaNumber}")]
        public async Task<IActionResult> GetBySetaNumber(string setaNumber)
        {
            try
            {
                var documents = await _context.SetaDocuments
                    .AsNoTracking()
                    .Where(d => d.SetaNumber == setaNumber)
                    .OrderByDescending(d => d.UploadedAt)
                    .ToListAsync();

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت اطلاعات", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var document = await _context.SetaDocuments.FindAsync(id);
                if (document == null)
                {
                    return NotFound(new { message = "فایل پیدا نشد" });
                }

                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت اطلاعات", error = ex.Message });
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var document = await _context.SetaDocuments.FindAsync(id);
                if (document == null)
                {
                    return NotFound(new { message = "فایل پیدا نشد" });
                }

                var filePath = Path.Combine(_environment.ContentRootPath, document.FilePath);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "فایل در سرور موجود نیست" });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(document.FileType ?? "");
                return File(fileBytes, contentType, document.OriginalFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دانلود فایل", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var document = await _context.SetaDocuments.FindAsync(id);
                if (document == null)
                {
                    return NotFound(new { message = "فایل پیدا نشد" });
                }

                // Delete file from disk
                var filePath = Path.Combine(_environment.ContentRootPath, document.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Delete from database
                _context.SetaDocuments.Remove(document);
                await _context.SaveChangesAsync();

                return Ok(new { message = "فایل با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در حذف فایل", error = ex.Message });
            }
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}
