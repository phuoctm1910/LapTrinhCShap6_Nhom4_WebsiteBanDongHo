using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BillController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BillController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Bill
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetAllBills()
        {
            var bills = await _context.Bills
                .Include(b => b.User)
                .OrderBy(b => b.BillId)
                .ToListAsync();
            return Ok(bills);
        }

        // GET: api/Bill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBillById(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null)
            {
                return NotFound();
            }

            return Ok(bill);
        }

        // PUT: api/Bill/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] BillVM billVM)
        {
            var bill = await _context.Bills.FindAsync(id);

            if (bill == null)
            {
                return null;
            }

            bill.UserID = billVM.UserID;
            bill.Quantity = billVM.Quantity;
            bill.TotalAmount = billVM.TotalAmount;
            bill.Status = billVM.Status;

            _context.Bills.Update(bill);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Bill
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> CreateBill([FromBody] BillVM billVM)
        {
            var bill = new Bill
            {
                UserID = billVM.UserID,
                Quantity = billVM.Quantity,
                TotalAmount = billVM.TotalAmount,
                Status = "Pending"
            };

            _context.Bills.Add(bill);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBill", new { id = bill.BillId }, bill);
        }

        // DELETE: api/Bill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
