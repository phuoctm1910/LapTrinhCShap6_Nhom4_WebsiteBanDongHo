using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Web_DongHo_API.Data;
using System.Linq;
using Web_DongHo_API.Models;

namespace Web_DongHo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductDetailController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetProductDetail/{productId:int}")]
        public async Task<IActionResult> GetProductDetail(int productId)
        {
            var product = await _context.Products
                        .Include(p => p.Brand)
                        .Include(p => p.Category)
                        .Where(p => p.ProductId == productId)
                        .Select(b => new ProductVM
                        {
                            ProductId = b.ProductId,
                            ProductCode = b.ProductCode,
                            ProductName = b.ProductName,
                            ProductStock = b.ProductStock,
                            ProductPrice = b.ProductPrice,
                            ProductImages = b.ProductImages,
                            Origin = b.Origin,
                            MachineType = b.MachineType,
                            Diameter = b.Diameter,
                            ClockType = b.ClockType,
                            Insurrance = b.Insurrance,
                            Color = b.Color,
                            BrandId = b.BrandId,
                            BrandName = b.Brand.BrandName,
                            CategoryId = b.CategoryId,
                            CategoryName = b.Category.CategoryName
                        })
                        .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            return new JsonResult(product, options); 
        }
    }
}
