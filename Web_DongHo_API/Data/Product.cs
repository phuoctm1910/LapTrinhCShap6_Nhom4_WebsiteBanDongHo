using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DongHo_API.Data
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string ProductCode { get; set; }
        [Required]
        public string ProductName { get; set; }
        public int? ProductStock { get; set; }
        [Required]
        public float ProductPrice { get; set; }
        public int CategoryId { get; set; }
        public string? ProductImages { get; set; }
        public string? Origin { get; set; }
        public string? MachineType { get; set; }
        public int? Diameter { get; set; }
        public string? ClockType { get; set; }
        public int? Insurrance { get; set; }
        public string? Color { get; set; }
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<BillDetails> BillDetails { get; set; } = new HashSet<BillDetails>();

        [NotMapped]
        public List<string> ProductImageList
        {
            get
            {
                if (string.IsNullOrEmpty(ProductImages))
                {
                    return new List<string>();
                }

                try
                {
                    var cleanedProductImages = ProductImages;

                    while (cleanedProductImages.Contains("\\"))
                    {
                        cleanedProductImages = cleanedProductImages.Replace("\\", string.Empty);
                    }

                    cleanedProductImages = cleanedProductImages.Trim('"');

                    return JsonConvert.DeserializeObject<List<string>>(cleanedProductImages);
                }
                catch (JsonReaderException)
                {
                    return new List<string>();
                }
            }
            set
            {
                ProductImages = JsonConvert.SerializeObject(value);
            }
        }
    }
}
