using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace K_Burns_Assessment_2.Models.ViewModels
{
    public class CreateEmployeeViewModel
    {
        [Display(Name = "First Name:")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name:")]
        [Required]
        public string LastName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Display(Name = "Post Code:")]
        [Required]
        public string Postcode { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address:")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "Email Confirm:")]
        public bool EmailConfirm { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number:")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Phone Confirm:")]
        public bool PhoneConfirm { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "Employment Status:")]
        public EmployementStatus EmployementStatus { get; set; }

        public string Role { get; set; }

        public ICollection<SelectListItem> Roles { get; set; }
    }
}