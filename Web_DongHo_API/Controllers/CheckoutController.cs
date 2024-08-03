using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Web_DongHo_API.Data;
using System;

namespace Web_DongHo_API.Controllers
{

    public class CompletePurchaseRequest
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class PendingBillRequest
    {
        public string Email { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("getBill")]
        public async Task<IActionResult> GetPendingBill([FromQuery] string username)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            var pendingBill = await _context.Bills
                .Include(b => b.BillDetails)
                .ThenInclude(bd => bd.Product)
                .FirstOrDefaultAsync(b => b.UserID == user.UserID && b.Status == "Pending");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            if (pendingBill == null)
            {
                return NotFound(new { success = false, message = "Không có hóa đơn đang chờ xử lý." });
            }

            return new JsonResult(pendingBill, options);
        }

        [HttpPost("completePurchase")]
        public async Task<IActionResult> CompletePurchase([FromQuery] string username, [FromBody] CompletePurchaseRequest request)
        {
            Console.WriteLine("Received completePurchase request for username: " + username);
            Console.WriteLine("Received completePurchase: " + request);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                Console.WriteLine("User not found: " + username);
                return NotFound(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            var pendingBill = await _context.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefaultAsync(b => b.UserID == user.UserID && b.Status == "Pending");

            if (pendingBill == null)
            {
                Console.WriteLine("No pending bill found for user: " + user.UserID);
                return NotFound(new { success = false, message = "Không có hóa đơn đang chờ xử lý." });
            }

            // Update shipping information and complete purchase
            Console.WriteLine("Updating pending bill for user: " + user.UserID);
            pendingBill.RecipientName = request.FullName;
            pendingBill.RecipientPhoneNumber = request.Phone;
            pendingBill.RecipientAddress = request.Address;
            pendingBill.PaymentMethod = request.PaymentMethod;
            pendingBill.Status = "Completed";

            _context.Bills.Update(pendingBill);
            await _context.SaveChangesAsync();
            Console.WriteLine("Bill updated successfully for user: " + user.UserID);

            return Ok(new { success = true, message = "Đặt hàng thành công." });
        }
    }

}
