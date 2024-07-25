using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DongHo_API.Models
{
    public class ProductVM
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? ProductStock { get; set; }
        public float ProductPrice { get; set; }
        public string ProductImages { get; set; }
        public string? Origin { get; set; }
        public string MachineType { get; set; }
        public int? Diameter { get; set; }
        public string ClockType { get; set; }
        public int? Insurrance { get; set; }
        public string Color { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } // Add BrandName property
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

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

                    // Loop to remove all backslashes until none are left
                    while (cleanedProductImages.Contains("\\"))
                    {
                        cleanedProductImages = cleanedProductImages.Replace("\\", string.Empty);
                    }

                    // Trim any leading or trailing quotes
                    cleanedProductImages = cleanedProductImages.Trim('"');

                    // Deserialize the cleaned string to a list
                    return JsonConvert.DeserializeObject<List<string>>(cleanedProductImages);
                }
                catch (JsonReaderException)
                {
                    // Return an empty list if deserialization fails
                    return new List<string>();
                }
            }
            set
            {
                // Serialize the list to a JSON string
                ProductImages = JsonConvert.SerializeObject(value);
            }
        }
    }
}
