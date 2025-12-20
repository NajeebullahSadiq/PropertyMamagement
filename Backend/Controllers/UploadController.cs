using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace UploadFilesServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB

        private readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly string[] AllowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };

        [HttpGet("test/{*filePath}")]
        [AllowAnonymous]
        public IActionResult TestFile(string filePath)
        {
            try
            {
                filePath = System.Net.WebUtility.UrlDecode(filePath);
                filePath = filePath.Replace("\\", "/");
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                var realPath = Path.GetFullPath(fullPath);
                var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Resources"));

                return Ok(new
                {
                    requestedPath = filePath,
                    fullPath = fullPath,
                    realPath = realPath,
                    basePath = basePath,
                    fileExists = System.IO.File.Exists(realPath),
                    isWithinResources = realPath.StartsWith(basePath)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload([FromQuery] string? documentType = null)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.FirstOrDefault();

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "فایلی انتخاب نشده است" });
                }

                if (file.Length > MaxFileSize)
                {
                    return BadRequest(new { message = "حجم فایل بیش از 50 مگابایت است" });
                }

                // Determine folder based on document type
                string folderName = GetFolderPath(documentType);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                // Create directory if it doesn't exist
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                // Validate file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!IsValidFileExtension(fileExtension))
                {
                    return BadRequest(new { message = "نوع فایل مجاز نیست" });
                }

                // Generate unique filename
                var fileName = GenerateUniqueFileName(file.FileName);
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new
                {
                    dbPath = dbPath,
                    fileName = fileName,
                    originalFileName = file.FileName,
                    fileSize = file.Length,
                    contentType = file.ContentType,
                    uploadedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"خطای سرور: {ex.Message}" });
            }
        }

        [HttpGet("view/{*filePath}")]
        [AllowAnonymous]
        public IActionResult ViewFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { message = "مسیر فایل مشخص نشده است" });
                }

                // Decode URL-encoded path
                filePath = System.Net.WebUtility.UrlDecode(filePath);

                // Normalize path separators
                filePath = filePath.Replace("\\", "/").Replace("//", "/");
                
                // Convert forward slashes to OS-specific separators for Path.Combine
                var normalizedPath = filePath.Replace("/", Path.DirectorySeparatorChar.ToString());
                
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), normalizedPath);
                var realPath = Path.GetFullPath(fullPath);
                var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Resources"));

                // Ensure the file is within the Resources directory
                if (!realPath.StartsWith(basePath))
                {
                    return Forbid();
                }

                if (!System.IO.File.Exists(realPath))
                {
                    return NotFound(new { message = $"فایل یافت نشد: {realPath}" });
                }

                var fileExtension = Path.GetExtension(realPath).ToLower();
                var contentType = GetContentType(fileExtension);
                var fileName = Path.GetFileName(realPath);

                var fileBytes = System.IO.File.ReadAllBytes(realPath);
                
                System.Diagnostics.Debug.WriteLine($"ViewFile: Serving {fileName} ({fileBytes.Length} bytes) with content-type: {contentType}");
                
                // Set proper headers for CORS and caching
                Response.Headers["Access-Control-Allow-Origin"] = "*";
                Response.Headers["Access-Control-Allow-Methods"] = "GET, OPTIONS";
                Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
                Response.Headers["Cache-Control"] = "public, max-age=3600";
                
                // For PDFs and images, serve inline so they display in the browser
                // For other files, serve as attachment for download
                if (fileExtension == ".pdf" || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".bmp" || fileExtension == ".webp")
                {
                    // Serve inline for viewing with proper Content-Disposition header
                    Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
                    return File(fileBytes, contentType);
                }
                else
                {
                    // Serve as attachment for download
                    return File(fileBytes, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ViewFile Error: {ex.Message}");
                return StatusCode(500, new { message = $"خطای سرور: {ex.Message}", details = ex.StackTrace });
            }
        }

        [HttpGet("download/{*filePath}")]
        [AllowAnonymous]
        public IActionResult DownloadFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { message = "مسیر فایل مشخص نشده است" });
                }

                // Decode URL-encoded path
                filePath = System.Net.WebUtility.UrlDecode(filePath);

                filePath = filePath.Replace("\\", "/");
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                var realPath = Path.GetFullPath(fullPath);
                var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Resources"));

                if (!realPath.StartsWith(basePath))
                {
                    return Forbid();
                }

                if (!System.IO.File.Exists(realPath))
                {
                    return NotFound(new { message = "فایل یافت نشد" });
                }

                var fileBytes = System.IO.File.ReadAllBytes(realPath);
                var fileName = Path.GetFileName(realPath);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"خطای سرور: {ex.Message}" });
            }
        }

        [HttpDelete("delete/{*filePath}")]
        public IActionResult DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { message = "مسیر فایل مشخص نشده است" });
                }

                // Decode URL-encoded path
                filePath = System.Net.WebUtility.UrlDecode(filePath);

                filePath = filePath.Replace("\\", "/");
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                var realPath = Path.GetFullPath(fullPath);
                var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Resources"));

                if (!realPath.StartsWith(basePath))
                {
                    return Forbid();
                }

                if (!System.IO.File.Exists(realPath))
                {
                    return NotFound(new { message = "فایل یافت نشد" });
                }

                System.IO.File.Delete(realPath);
                return Ok(new { message = "فایل با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"خطای سرور: {ex.Message}" });
            }
        }

        private string GetFolderPath(string? documentType)
        {
            return documentType?.ToLower() switch
            {
                "property" => Path.Combine("Resources", "Documents", "Property"),
                "vehicle" => Path.Combine("Resources", "Documents", "Vehicle"),
                "company" => Path.Combine("Resources", "Documents", "Company"),
                "profile" => Path.Combine("Resources", "Documents", "Profile"),
                "license" => Path.Combine("Resources", "Documents", "License"),
                "identity" => Path.Combine("Resources", "Documents", "Identity"),
                _ => Path.Combine("Resources", "Images")
            };
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
            var extension = Path.GetExtension(originalFileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var sanitizedName = System.Text.RegularExpressions.Regex.Replace(nameWithoutExtension, @"[^a-zA-Z0-9_\-]", "_");
            return $"{sanitizedName}_{timestamp}{extension}";
        }

        private bool IsValidFileExtension(string extension)
        {
            return AllowedImageExtensions.Contains(extension) || AllowedDocumentExtensions.Contains(extension);
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
