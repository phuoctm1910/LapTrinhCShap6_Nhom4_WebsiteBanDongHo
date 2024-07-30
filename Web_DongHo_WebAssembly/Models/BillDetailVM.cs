namespace Web_DongHo_WebAssembly.Models
{
    public class BillDetailVM
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public float TotalPrice { get; set; }
    }
}
