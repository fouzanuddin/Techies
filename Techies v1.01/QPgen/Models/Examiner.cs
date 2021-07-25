using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QPgen.Models
{
    public class Examiner
    {
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required(ErrorMessage ="The Password field is required")]
        [MaxLength(30)]
        public string Pass1 { get; set; }

        [Required(ErrorMessage = "The Confirm Password field is required")]
        [MaxLength(30)]
        [Compare("Pass1", ErrorMessage = "Password doesn't match.")]
        public string Pass2 { get; set; }
    }
}