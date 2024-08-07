using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DongHo_API.Data;
using Web_DongHo_API.Models;

namespace Web_DongHo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Brand
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetAllBrands()
        {
            var listBrand = await _context.Brands.ToListAsync();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            return new JsonResult(listBrand, options);
        }

        // GET: api/Brand/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrandById(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        // PUT: api/Brand/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("editBrand/{brandId:int}")]
        public async Task<IActionResult> UpdateBrand(int brandId, [FromBody] BrandVM brandVM)
        {
            if (brandId != brandVM.BrandId)
            {
                return BadRequest(new { message = "Invalid brand data." });
            }
            var findBrand = await _context.Brands.FirstOrDefaultAsync(c => c.BrandId == brandId);
            if (findBrand == null)
            {
                return NotFound(new { message = "Brand Not Found" });
            }
            findBrand.BrandName = brandVM.BrandName;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Brand information updated successfully." });
        }

        // POST: api/Brand
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("addBrand")]
        public async Task<ActionResult> CreateBrand([FromBody] BrandVM brandVM)
        {
            if (brandVM == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid brand data." });
            }
            var brand = new Brand
            {
                BrandName = brandVM.BrandName
            };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Brand information add successfully." });
        }

        // DELETE: api/Brand/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.BrandId == id);
        }
    }
}
