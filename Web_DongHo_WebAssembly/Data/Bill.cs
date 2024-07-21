using System.Collections.Generic;

namespace Web_DongHo_WebAssembly.Data
{
    public class Bill
    {
        public int BillId { get; set; }
        public int UserID { get; set; }
        public int Quantity { get; set; }
        public float TotalAmount { get; set; }
        public string Status { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhoneNumber { get; set; }
        public string? RecipientAddress { get; set; }
        public string? PaymentMethod { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<BillDetails> BillDetails { get; set; } = new HashSet<BillDetails>();
    }
}
