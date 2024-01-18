using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace K_Burns_Assessment_2.Models
{
    public class Employee : User
    {
        //[Display(Name = "Username")]
        //public override string UserName { get; set; }//MAKE SURE THIS IS OK

        //[Display(Name = "Phone Number")]
        // public override string PhoneNumber { get; set; }//MAKE SURE THIS IS OK


        public bool Promote { get; set; }

        public bool Suspend { get; set; }

        [Display(Name = "Personalise")]
        public bool PersonaliseStaff { get; set; }

        [Display(Name = "Employment Status")]
        public EmployementStatus EmployementStatus { get; set; }

        //Navigational Properties

    }

    public enum EmployementStatus
    {
        FullTime,
        PartTime
    }
}