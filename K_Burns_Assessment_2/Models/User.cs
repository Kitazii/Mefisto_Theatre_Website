using System.Data.Entity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace K_Burns_Assessment_2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public abstract class User : IdentityUser
    {
        //**************************************************************************************************************************
        //Extending the IdentityUser with these properties
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        [Display(Name = "Post Code")]
        public string Postcode { get; set; }

        [Display(Name = "Joined")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime RegisteredAt { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Suspended")]
        public bool IsSuspended { get; set; }

        //Navigational Properties - USER & BLOGPOST
        public List<BlogPost> BlogPosts { get; set; }//MANY

        //Navigational Properties - USER & COMMENTS
        public List<Comment> Comments{ get; set; }//MANY

        //[Display(Name = "Blog Posts:")]
        //public string BlogPosts { get; set; } //both users can post a blog (Only admin can post an announcement)

        //using the ApplicationUserManager to get the user's current role
        private ApplicationUserManager userManager;

        //**************************************************************************************************************************

        //the CurrentRole property is not mapped as a field in the Users table
        //its being used to get the current role that the user is logged in

        [Display(Name = "Role")]
        [NotMapped]
        public string CurrentRole
        {
            get
            {
                if (userManager == null)
                {
                    userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }

                return userManager.GetRoles(Id).Single();
            }
        }

        //**************************************************************************************************************************

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}