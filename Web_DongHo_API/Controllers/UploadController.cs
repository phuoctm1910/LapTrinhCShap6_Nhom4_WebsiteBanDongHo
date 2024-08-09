using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Web_DongHo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Path to Blazor WebAssembly wwwroot folder
            var blazorWebAssemblyRootPath = @"D:\FPT POLYTECHNIC\Hoc Ki 6\C#6\ASM_Nhom4_Web_DongHo\Web_DongHo_WebAssembly\wwwroot\uploads";
            if (!Directory.Exists(blazorWebAssemblyRootPath))
            {
                Directory.CreateDirectory(blazorWebAssemblyRootPath);
            }

            var filePath = Path.Combine(blazorWebAssemblyRootPath, file.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(new { filePath });
        }
    }
}
