using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace K_Burns_Assessment_2.Models.ViewModels
{
    public class PersonaliseDetailsViewModel
    {
        [Display(Name = "Change Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Change First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Change Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Change Email")]
        public string Email { get; set; }

        [Display(Name = "Change Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Change Street")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Change City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Change Post Code")]
        public string Postcode { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm new password")]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
    }
}