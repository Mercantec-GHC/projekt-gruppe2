using BlazorApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Endpoint to upload files
        [HttpPost("api/[controller]/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string destinationPath)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized("You must be logged in to upload files.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Upload en gyldig fil.");
            }

            var rootPath = Path.Combine(_env.ContentRootPath, "files");
			var filePath = Path.Combine(rootPath, destinationPath, file.FileName);

			var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

			using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { filePath });
        }

        // Endpoint to get files
        [HttpGet("files/{**fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
			//var currentDirectory = Directory.GetCurrentDirectory();
			var rootPath = Path.Combine(_env.ContentRootPath, "files");
			var filePath = Path.Combine(rootPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var mimeType = MimeTypes.GetMimeType(fileName);

            return PhysicalFile(filePath, mimeType);
        }
    }

}
