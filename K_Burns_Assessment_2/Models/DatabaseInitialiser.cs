using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI.WebControls.Expressions;

namespace K_Burns_Assessment_2.Models
{
    public class DatabaseInitialiser : DropCreateDatabaseAlways<MefistoDbContext>
    {
        protected override void Seed(MefistoDbContext context)
        {
            base.Seed(context);

            //if there are no records stored in the Users table
            if(!context.Users.Any())
            {
                //creating some the roles and storing them in the Roles table

                //we need a RoleManager to create and store roles
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                //if the Admin role doesn't exist
                if (!roleManager.RoleExists("Admin"))
                {
                    //then we create one
                    roleManager.Create(new IdentityRole("Admin"));
                }

                //if the Staff role doesn't exist
                if (!roleManager.RoleExists("Staff"))
                {
                    //then we create one
                    roleManager.Create(new IdentityRole("Staff"));
                }

                //if the Member role doesn't exist
                if (!roleManager.RoleExists("Member"))
                {
                    //then we create one
                    roleManager.Create(new IdentityRole("Member"));
                }

                context.SaveChanges();

                //******************************Create Users************************************

                //we need a UserManager to create and store our users (member and staff)
                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));

                //Create an ADMIN
                //first check if the admin exists in the database
                if (userManager.FindByName("admin@MefistoTheatre.com") == null)
                {
                    //Create simple password for testing
                    userManager.PasswordValidator = new PasswordValidator
                    {
                        RequireDigit = false,
                        RequiredLength = 1,
                        RequireLowercase = false,
                        RequireUppercase = false,
                        RequireNonLetterOrDigit = false
                    };

                    //create an Admin User
                    var admin = new Employee
                    {
                        UserName = "admin@MefistoTheatre.com",
                        Email = "admin@MefistoTheatre.com",
                        FirstName = "Adam",
                        LastName = "Smith",
                        Street = "123 Backbone St",
                        City = "Glasgow",
                        Postcode = "G6 8LP",
                        RegisteredAt = DateTime.Now.AddYears(-3),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = false,
                        EmployementStatus = EmployementStatus.FullTime,
                        PhoneNumber = "07283742267"
                    };

                    //add the admin to the Users table
                    userManager.Create(admin, "admin123");
                    //assign it to the Admin role
                    userManager.AddToRole(admin.Id, "Admin");


                    //create a moderator User
                    var tom = new Employee
                    {
                        UserName = "Tom@MefistoTheatre.com",
                        Email = "Tom@MefistoTheatre.com",
                        FirstName = "Tommy",
                        LastName = "Peterson",
                        Street = "14 Cloud St",
                        City = "Edinburgh",
                        Postcode = "E5 8E7",
                        RegisteredAt = DateTime.Now.AddYears(-1),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = false,
                        EmployementStatus = EmployementStatus.FullTime,
                        PhoneNumber = "07283745643"
                    };

                    //Adding a few employees
                    //first check if this moderator exists in the database
                    if (userManager.FindByName("Tom@MefistoTheatre.com") == null)
                    {
                        //add the moderator to the Users table
                        userManager.Create(tom, "staff0");
                        //assign it to the Moderator role
                        userManager.AddToRole(tom.Id, "Staff");
                    }

                    //create a staff user
                    var sarah = new Employee
                    {
                        UserName = "Sarah@MefistoTheatre.com",
                        Email = "Sarah@MefistoTheatre.com",
                        FirstName = "Sarah",
                        LastName = "Brown",
                        Street = "2 Care Ave",
                        City = "Glasgow",
                        Postcode = "G4 43R",
                        RegisteredAt = DateTime.Now.AddMonths(-6),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = false,
                        EmployementStatus = EmployementStatus.PartTime,
                        PhoneNumber = "07563245643"
                    };

                    //first check if this staff exists in the database
                    if (userManager.FindByName("Sarah@MefistoTheatre.com") == null)
                    {
                        userManager.Create(sarah, "staff1");
                        userManager.AddToRole(sarah.Id, "Staff");
                    }

                    var gill = new Member
                    {
                        UserName = "Gill@gmail.com",
                        Email = "Gill@gmail.com",
                        FirstName = "Gill",
                        LastName = "Morrison",
                        Street = "45 Uptown st",
                        City = "Glasgow",
                        Postcode = "G4 55T",
                        RegisteredAt = DateTime.Now.AddMonths(-4),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = false,
                        MemberType = MemberType.Premium,
                        PhoneNumber = "07445252343"
                    };

                    //Adding some Members
                    if (userManager.FindByName("Gill@gmail.com") == null)
                    {
                        userManager.Create(gill, "member1");
                        userManager.AddToRole(gill.Id, "Member");
                    }


                    var henry = new Member
                    {
                        UserName = "Henry@yahoo.com",
                        Email = "Henry@yahoo.com",
                        FirstName = "Henry",
                        LastName = "Button",
                        Street = "1 Round st",
                        City = "Glasgow",
                        Postcode = "G1 1TL",
                        RegisteredAt = DateTime.Now.AddMonths(-1),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = false,
                        MemberType = MemberType.Standard,
                        PhoneNumber = "07445252343"
                    };

                    if (userManager.FindByName("Henry@yahoo.com") == null)
                    {
                        userManager.Create(henry, "member2");
                        userManager.AddToRole(henry.Id, "Member");
                    }

                    var alana = new Member
                    {
                        UserName = "Alana@gmail.com",
                        Email = "Alana@gmail.com",
                        FirstName = "Alana",
                        LastName = "Duff",
                        Street = "100 Pear st",
                        City = "Edinburgh",
                        Postcode = "E4 14R",
                        RegisteredAt = DateTime.Now.AddMonths(-2),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = false,
                        MemberType = MemberType.Standard,
                        PhoneNumber = "07445252343"
                    };

                    if (userManager.FindByName("Alana@gmail.com") == null)
                    {
                        userManager.Create(alana, "member3");
                        userManager.AddToRole(alana.Id, "Member");
                    }

                    var garry = new Member
                    {
                        UserName = "Garry@gmail.com",
                        Email = "Garry@gmail.com",
                        FirstName = "Garry",
                        LastName = "Allan",
                        Street = "14 Tire st",
                        City = "Glagow",
                        Postcode = "G1 234",
                        RegisteredAt = DateTime.Now.AddMonths(-5),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = false,
                        MemberType = MemberType.Premium,
                        PhoneNumber = "07445252343"
                    };

                    if (userManager.FindByName("Garry@gmail.com") == null)
                    {
                        userManager.Create(garry, "member4");
                        userManager.AddToRole(garry.Id, "Member");
                    }

                    var paul = new Member
                    {
                        UserName = "Paul@gmail.com",
                        Email = "Paul@gmail.com",
                        FirstName = "Paul",
                        LastName = "Mcelroy",
                        Street = "153 Pearl st",
                        City = "Glagow",
                        Postcode = "G3 PTH",
                        RegisteredAt = DateTime.Now.AddDays(-15),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsSuspended = true,
                        MemberType = MemberType.Standard,
                        PhoneNumber = "07445252343"
                    };

                    // a suspended member
                    if (userManager.FindByName("Paul@gmail.com") == null)
                    {
                        userManager.Create(paul, "suspended1");
                        userManager.AddToRole(paul.Id, "Member");
                    }

                    context.SaveChanges();

                    //******************************Create Categories************************************

                    var postAnnouncement = new Category { Name = "Post Announcement" };
                    var movieReviews = new Category { Name = "Movie Reviews" };
                    var performanceReviews = new Category { Name = "Performance Reviews" };
                    var otherRelatedInfo = new Category { Name = "Other Related Info" };

                    context.Categories.Add(postAnnouncement);
                    context.Categories.Add(movieReviews);
                    context.Categories.Add(performanceReviews);
                    context.Categories.Add(otherRelatedInfo);
                    context.SaveChanges();


                    //******************************Create Blog Posts************************************

                    var postAnnouncementPost1 = new BlogPost()
                    {
                        Title = "Upcoming Updates",
                        Content = "NEW WEBSITE UPGRADES INCLUDE Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddMonths(-2),
                        DateEdited = DateTime.Now.AddDays(-2),
                        User = admin,
                        Category = postAnnouncement
                    };

                    context.BlogPosts.Add(postAnnouncementPost1);

                    var postAnnouncementPost2 = new BlogPost()
                    {
                        Title = "New Upcoming Shows",
                        Content = "NEW SHOWS LIST HERE Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddDays(-13),
                        DateEdited = DateTime.Now.AddDays(-13),
                        User = admin,
                        Category = postAnnouncement
                    };

                    context.BlogPosts.Add(postAnnouncementPost2);

                    var moviePost1 = new BlogPost()
                    {
                        Title = "Why Most Movies SUCK!",
                        Content = "Most movies are not up to par because Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddMonths(-5),
                        DateEdited = DateTime.Now.AddMonths(-3),
                        User = henry,
                        Category = movieReviews
                    };

                    context.BlogPosts.Add(moviePost1);

                    var moviePost2 = new BlogPost()
                    {
                        Title = "Why Most Movies are GREAT!",
                        Content = "Most movies are amazing because Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddMonths(-8),
                        DateEdited = DateTime.Now.AddMonths(-1),
                        User = garry,
                        Category = movieReviews
                    };

                    context.BlogPosts.Add(moviePost2);

                    var moviePost3 = new BlogPost()
                    {
                        Title = "This Is Absurd!",
                        Content = "No way this film showcases this... Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddMonths(-4),
                        DateEdited = DateTime.Now.AddMonths(-2),
                        User = garry,
                        Category = movieReviews
                    };

                    context.BlogPosts.Add(moviePost3);

                    var performancePost1 = new BlogPost()
                    {
                        Title = "Great Performance!",
                        Content = "The performance was great because Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddMonths(-7),
                        DateEdited = DateTime.Now.AddDays(-25),
                        User = henry,
                        Category = performanceReviews
                    };

                    context.BlogPosts.Add(performancePost1);

                    var performancePost2 = new BlogPost()
                    {
                        Title = "Poor Performance!",
                        Content = "The performance was poor because Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddDays(-30),
                        DateEdited = DateTime.Now.AddDays(-8),
                        User = alana,
                        Category = performanceReviews
                    };

                    context.BlogPosts.Add(performancePost2);

                    var otherPost1 = new BlogPost()
                    {
                        Title = "Top 10 best shows we have hosted",
                        Content = "Here are the members favourites Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " +
                        "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                        "eu fugiat nulla pariatur.",
                        BlogPosted = true,
                        DatePosted = DateTime.Now.AddMonths(-3),
                        DateEdited = DateTime.Now.AddMonths(-3),
                        User = tom,
                        Category = otherRelatedInfo
                    };

                    context.BlogPosts.Add(otherPost1);

                    //save the changes to the database
                    context.SaveChanges();

                    //******************************Create Comments************************************

                    //comments on garrys review
                    var comment1 = new Comment()
                    {
                        CommentText = "I love this take!",
                        DatePosted = DateTime.Now.AddDays(25),
                        BlogPost = moviePost2,
                        User = alana
                    };
                    context.Comments.Add(comment1);

                    var comment2 = new Comment()
                    {
                        CommentText = "I disagree with this one, I think blah blah blah blah",
                        DatePosted = DateTime.Now.AddDays(13),
                        BlogPost = moviePost2,
                        User = tom
                    };
                    context.Comments.Add(comment2);

                    //save the changes to the database
                    context.SaveChanges();


                }//end of if admin doesn't exist
            }//end of if any users
        }//end of Seed method
    }//end of class
}//end of namespace