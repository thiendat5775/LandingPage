using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LandingPage.WebApi.Models
{
    public class RefreshViewModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
