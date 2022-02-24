using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LandingPage.WebApi.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(128, ErrorMessage = "không quá 128 kí tự")]
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDay { get; set; }
        public int PositionId { get; set; }
        public int BusinessTypeId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Ít nhất 6 kí tự tối đa 100 kí tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }
    }
}
