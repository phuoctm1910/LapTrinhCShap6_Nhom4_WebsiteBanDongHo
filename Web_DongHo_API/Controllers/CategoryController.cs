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
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var listCategory = await _context.Categories.ToListAsync();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            return new JsonResult(listCategory, options);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("editCategory/{categortId:int}")]
        public async Task<IActionResult> UpdateCategory(int categortId, [FromBody] CategoryVM categoryVM)
        {
            if (categortId != categoryVM.CategoryID)
            {
                return BadRequest(new { message = "Invalid category data." });
            }
            var findCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categortId);
            if (findCategory == null)
            {
                return NotFound(new { message = "Category Not Found" });
            }
            findCategory.CategoryName = categoryVM.CategoryName;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category information updated successfully." });
        }

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("addCategory")]
        public async Task<ActionResult> CreateCategory([FromBody] CategoryVM categoryVM)
        {
            if (categoryVM == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid category data." });
            }
            var category = new Category
            {
                CategoryName = categoryVM.CategoryName
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category information add successfully." });
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
