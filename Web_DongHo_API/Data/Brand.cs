using System.Collections.Generic;

namespace Web_DongHo_API.Data
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}