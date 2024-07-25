using System;
using System.ComponentModel.DataAnnotations;

namespace Web_DongHo_API.Models
{
    public class UserVM
    {
        public string FullName { get; set; }
        public bool? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Image { get; set; }
        public string? Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
