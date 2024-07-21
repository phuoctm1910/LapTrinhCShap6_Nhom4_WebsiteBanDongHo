namespace Web_DongHo_API.Models
{
    public class BillVM
    {
        public int BillId { get; set; }
        public int UserID { get; set; }
        public int Quantity { get; set; }
        public float TotalAmount { get; set; }
        public string Status { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhoneNumber { get; set; }
        public string? RecipientAddress { get; set; }
    }
}
