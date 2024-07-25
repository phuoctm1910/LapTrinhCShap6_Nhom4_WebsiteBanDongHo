using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web_DongHo_API.Data;

namespace Web_DongHo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("homeProducts")]
        public async Task<IActionResult> GetHomeProducts()
        {
            var productfirst8 = await _context.Products
               .Include(d => d.Category)
               .Include(d => d.Brand)
            .Take(8)
               .ToListAsync();

            var productsecond8 = await _context.Products
               .Include(d => d.Category)
               .Include(d => d.Brand)
               .Skip(productfirst8.Count)
            .Take(8)
               .ToListAsync();

            var productthird8 = await _context.Products
               .Include(d => d.Category)
               .Include(d => d.Brand)
               .Skip(productfirst8.Count() + productsecond8.Count())
               .Take(8)
               .ToListAsync();

            return Ok(new
            {
                Productfirst8 = productfirst8,
                Productsecond8 = productsecond8,
                ProductThird8 = productthird8,
            });
        }
    }
}
