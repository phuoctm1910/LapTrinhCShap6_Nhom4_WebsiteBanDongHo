using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Web_DongHo_API.Data;

namespace Web_DongHo_API.Controllers
{
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public string Username { get; set; }
    }
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }

    }

    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("items/count")]
        public async Task<IActionResult> GetCartItemCount([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }

            var count = await _context.BillDetails
                                       .Where(bd => bd.Bill.User.UserName == username && bd.Bill.Status =="Pending")
                                       .SumAsync(bd => bd.Quantity);

            return Ok(count);
        }
        [HttpPost]
        [Route("addToCart")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                // Find the user based on the provided email
                var findUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.Username);
                if (findUser == null)
                {
                    return NotFound(new ApiResponse { Success = false, Message = "Bạn chưa đăng nhập." });
                }

                // Check for a pending bill for the user
                var findCurrentBill = await _context.Bills
                    .Include(b => b.BillDetails)
                    .FirstOrDefaultAsync(x => x.UserID == findUser.UserID && x.Status == "Pending");
                if (findCurrentBill == null)
                {
                    // Create a new bill if none exists
                    findCurrentBill = new Bill
                    {
                        UserID = findUser.UserID,
                        Quantity = 0,
                        TotalAmount = 0,
                        Status = "Pending"
                    };
                    _context.Bills.Add(findCurrentBill);
                    await _context.SaveChangesAsync();
                }

                // Find the product to be added to the cart
                var productInBillDetails = await _context.Products.FindAsync(request.ProductId);
                if (productInBillDetails == null)
                {
                    return NotFound(new ApiResponse { Success = false, Message = "Không tìm thấy sản phẩm." });
                }

                // Check if the product is already in the bill details
                var billDetail = findCurrentBill.BillDetails
                    .FirstOrDefault(bd => bd.BillId == findCurrentBill.BillId && bd.ProductId == productInBillDetails.ProductId);

                if (billDetail == null)
                {
                    // Add new bill detail if the product is not already in the cart
                    billDetail = new BillDetails
                    {
                        BillId = findCurrentBill.BillId,
                        ProductId = productInBillDetails.ProductId,
                        Quantity = 1,
                        UnitPrice = (float)productInBillDetails.ProductPrice,
                        TotalPrice = (float)productInBillDetails.ProductPrice
                    };
                    findCurrentBill.BillDetails.Add(billDetail);
                }
                else
                {
                    // Update the quantity and total price if the product is already in the cart
                    billDetail.Quantity += 1;
                    billDetail.TotalPrice += (float)productInBillDetails.ProductPrice;
                }

                // Update the bill's total quantity and amount
                findCurrentBill.Quantity += 1;
                findCurrentBill.TotalAmount += (float)productInBillDetails.ProductPrice;

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse { Success = true, Message = "Đã thêm sản phẩm vào giỏ hàng của bạn." });
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                // Add logging here (e.g., using ILogger)
                return StatusCode(500, new ApiResponse { Success = false, Message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng.", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("getCart")]
        public async Task<IActionResult> GetCartOfUser([FromQuery] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username is required.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                {
                    return Unauthorized("Bạn cần đăng nhập tài khoản.");
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
                    return NoContent();
                }

                return new JsonResult(pendingBill, options);
            }
            catch (Exception ex)
            {
                // Log the exception (use your preferred logging method)
                // For example: _logger.LogError(ex, "An error occurred while fetching the cart.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost]
        [Route("removeProduct/{billDetailId}")]
        public async Task<IActionResult> RemoveProductInCart(int billDetailId)
        {
            var billDetail = await _context.BillDetails.FirstOrDefaultAsync(bd => bd.Id == billDetailId);
            if (billDetail == null)
            {
                return NotFound(new { success = false, message = "Không thấy sản phẩm trong giỏ hàng của bạn." });
            }

            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillId == billDetail.BillId);
            if (bill == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy hóa đơn của bạn." });
            }

            bill.Quantity -= billDetail.Quantity;
            bill.TotalAmount -= billDetail.TotalPrice;

            _context.BillDetails.Remove(billDetail);

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đã xóa sản phẩm khỏi giỏ hàng của bạn." });
        }
        [HttpPost]
        [Route("changeProductQuantity")]
        public async Task<IActionResult> ChangeProductQuantity([FromQuery] int billDetailId, [FromBody] int change)
        {
            var billDetail = await _context.BillDetails.FirstOrDefaultAsync(b => b.Id == billDetailId);
            if (billDetail == null)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy sản phẩm đó trong giỏ hàng của bạn" });
            }

            var product = await _context.Products.FirstOrDefaultAsync(c => c.ProductId == billDetail.ProductId);
            if (product == null)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            int? maxAllowedQuantity = product.ProductStock;
            int newQuantity = billDetail.Quantity + change;

            if (newQuantity <= 0 || newQuantity > maxAllowedQuantity)
            {
                return BadRequest(new { success = false, message = $"Không thể giảm số lượng bé hơn 0 và lớn hơn số lượng sản phẩm có trong cửa hàng {maxAllowedQuantity}" });
            }

            billDetail.Quantity = newQuantity;
            billDetail.TotalPrice = billDetail.Quantity * billDetail.UnitPrice;
            _context.BillDetails.Update(billDetail);
            await _context.SaveChangesAsync();

            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillId == billDetail.BillId);
            if (bill != null)
            {
                bill.Quantity = await _context.BillDetails
                    .Where(bd => bd.BillId == bill.BillId)
                    .SumAsync(bd => bd.Quantity);

                bill.TotalAmount = await _context.BillDetails
                    .Where(bd => bd.BillId == bill.BillId)
                    .SumAsync(bd => bd.TotalPrice);

                _context.Bills.Update(bill);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, message = "Đã cập nhật số lượng sản phẩm có trong giỏ hàng của bạn" });
        }

        [HttpPost]
        [Route("updateTotalBill")]
        public async Task<IActionResult> UpdateTotalAmount([FromQuery]int billId,[FromBody] float lastTotalAmount)
        {
            var billUpdateTotalAmount = await _context.Bills.FirstOrDefaultAsync(b => b.BillId == billId);
            if (billUpdateTotalAmount == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy hóa đơn" });
            }

            billUpdateTotalAmount.TotalAmount = lastTotalAmount;

            try
            {
                _context.Bills.Update(billUpdateTotalAmount);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Đã cập nhật tổng giá tiền cuối" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Không thể cập nhật tổng giá tiền cuối hóa đơn của bạn" });
            }
        }
    }
}
