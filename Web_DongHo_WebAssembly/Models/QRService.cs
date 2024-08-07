// QRCodeService.cs
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_DongHo_WebAssembly.Models
{
    public class QRCodeService
    {
        private static Random random = new Random();

        public string GenerateRandomString()
        {
            int length = random.Next(4, 9);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public int GenerateRandomDiscount()
        {
            int minDiscount = 10000;
            int maxDiscount = 100000;
            int step = 1000;
            int discountRange = (maxDiscount - minDiscount) / step + 1;
            return minDiscount + (random.Next(discountRange) * step);
        }
    }

}
