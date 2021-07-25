using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QPgen.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string username { get; set; }

        [Required]
        [MaxLength(50)]
        public string password { get; set; }
    }
}