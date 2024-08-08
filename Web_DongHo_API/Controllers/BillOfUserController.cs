using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_DongHo_API.Data;

namespace Web_DongHo_API.Controllers
{
    public class HistoryBillRequest
    {
        public string UserName { get; set; }
    }

    public class HistoryBillDetailsRequest
    {
        public int BillId { get; set; }
        public string UserName { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class BillOfUserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BillOfUserController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("historyBill")]
        public async Task<IActionResult> HistoryBill([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Người dùng chưa đăng nhập." });
            }

            var findUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (findUser == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var completedBills = await _context.Bills
                .Where(b => b.UserID == findUser.UserID && b.Status == "Completed")
                .ToListAsync();

            if (!completedBills.Any())
            {
                return Ok(new { message = "Không có hóa đơn nào hoàn thành.", bills = new List<Bill>() });
            }

            var totalAllUnit = await _context.BillDetails
                .Where(bd => completedBills.Select(b => b.BillId).Contains(bd.BillId))
                .SumAsync(bd => bd.TotalPrice);

            var result = completedBills.Select(b => new
            {
                b.BillId,
                b.UserID,
                UserName = b.User.FullName,
                b.Quantity,
                b.TotalAmount,
                b.Status,
                b.RecipientName,
                b.RecipientPhoneNumber,
                b.RecipientAddress,
                b.PaymentMethod,
                DeliveryType = (b.TotalAmount - totalAllUnit == 15000) ? "Vận chuyển chậm" : "Vận chuyển Nhanh"
            }).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("historyBillDetail/{billId:int}")]
        public async Task<IActionResult> HistoryBillDetails(int billId, [FromQuery] int userId)
        {

            if (userId == null)
            {
                return NotFound(new {message = "Không tìm thấy người dùng." });
            }

            var billDetails = await _context.BillDetails
                .Where(bd => bd.BillId == billId && bd.Bill.UserID == userId)
                .Include(bd => bd.Product)
                .GroupBy(bd => new { bd.ProductId, bd.Product.ProductName, bd.UnitPrice })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    g.Key.UnitPrice,
                    Quantity = g.Sum(bd => bd.Quantity),
                    TotalPrice = g.Sum(bd => bd.TotalPrice)
                })
                .ToListAsync();

            if (!billDetails.Any())
            {
                return NotFound(new { message = "Không tìm thấy thông tin chi tiết của bill này." });
            }

            return Ok(billDetails);
        }
    }
}
