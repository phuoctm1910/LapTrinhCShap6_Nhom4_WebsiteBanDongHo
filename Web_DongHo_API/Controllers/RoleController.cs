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
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Role
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
        {
            var listRole = await _context.Roles.ToListAsync();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            return new JsonResult(listRole, options);
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        // PUT: api/Role/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("editRole/{roleId:int}")]
        public async Task<IActionResult> UpdateRole(int roleId, [FromBody] RoleVM roleVM)
        {
            if (roleVM == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid role data." });
            }
            var category = await _context.Roles.FirstOrDefaultAsync(c => c.RoleId == roleId);
            if (category != null)
            {
                category.RoleName = roleVM.RoleName;
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound(new {message = "Role Not Found"});
            }

            return Ok(new { message = "Role information updated successfully." });
        }

        // POST: api/Role
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("addRole")]
        public async Task<ActionResult> CreateRole([FromBody] RoleVM roleVM)
        {
            if (roleVM == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid role data." });
            }
            var role = new Role
            {
                RoleName = roleVM.RoleName
            };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role information add successfully." });
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
