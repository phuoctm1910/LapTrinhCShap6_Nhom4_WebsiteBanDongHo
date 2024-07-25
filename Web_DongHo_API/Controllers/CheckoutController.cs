using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web_DongHo_API.Data;

namespace Web_DongHo_API.Controllers
{

    public class CompletePurchaseRequest
    {
        public string Email { get; set; }
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

        [HttpGet("pendingBill")]
        public async Task<IActionResult> GetPendingBill([FromBody] PendingBillRequest request)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            var pendingBill = await _context.Bills
                .Include(b => b.BillDetails)
                .ThenInclude(bd => bd.Product)
                .FirstOrDefaultAsync(b => b.UserID == user.UserID && b.Status == "Pending");

            if (pendingBill == null)
            {
                return NotFound(new { success = false, message = "Không có hóa đơn đang chờ xử lý." });
            }

            return Ok(pendingBill);
        }

        [HttpPost("completePurchase")]
        public async Task<IActionResult> CompletePurchase([FromBody] CompletePurchaseRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            var pendingBill = await _context.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefaultAsync(b => b.UserID == user.UserID && b.Status == "Pending");

            if (pendingBill == null)
            {
                return NotFound(new { success = false, message = "Không có hóa đơn đang chờ xử lý." });
            }

            // Update shipping information and complete purchase
            pendingBill.RecipientName = request.FullName;
            pendingBill.RecipientPhoneNumber = request.Phone;
            pendingBill.RecipientAddress = request.Address;
            pendingBill.PaymentMethod = request.PaymentMethod;
            pendingBill.Status = "Completed";

            _context.Bills.Update(pendingBill);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đặt hàng thành công." });
        }
    }

}
