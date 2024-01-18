using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using K_Burns_Assessment_2.Models;
using K_Burns_Assessment_2.Models.ViewModels;
using System.Runtime.CompilerServices;
using System.Data.OleDb;
using System.Xml.Serialization;
using System.Web.Services.Description;
using System.IO;
using System.Web.Security;
using System.Web.WebPages;

namespace K_Burns_Assessment_2.Controllers
{
    //only allow admin to have authority to use this controller
    //and redirect to main page if user is already logged in
    [RedirectUnauthorizedUsersFilter] //deals with all file names that follow the "Admin" path

    //We need to inherit from the AccountController to use the login/registration methods
    public class AdminController : AccountController
    {
        //creating an instance of the Mefisto DB Context
        private MefistoDbContext db = new MefistoDbContext();

        //create an instance of blog to access the TruncateWords method
        private BlogPost blog = new BlogPost();

        public AdminController() : base()
        {

        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)
        {

        }

        // GET: Admin
        public ActionResult Index() 
        {
            return View();
        }

        //GET blog posts page
        public ActionResult MyBlogPost()
        {
            //get all posts from posts table
            var blogPosts = db.BlogPosts
                .Include(bp => bp.Category)
                .Include(bp => bp.User);

            //get the Id of the logged in user, using Identity
            var userId = User.Identity.GetUserId();

            //from the lsits of posts from the blog posts table
            //select only the posts that match the current logged in User ID
            //I will order the posts from most current to oldest post
            blogPosts = blogPosts
                .Where(bp => bp.UserId == userId)
                .OrderByDescending(bp => bp.DateEdited);

            var blogPostsList = blogPosts.ToList();

            // Truncate the content of certain properties to a maximum of 3 words
            foreach (var post in blogPostsList)
            {
                //truncating title and content
                post.Title = blog.TruncateWords(post.Title, 3);
                post.Content = blog.TruncateWords(post.Content, 3);
            }

            //send the posts list to the view
            return View(blogPostsList);
        }

        //**********************************************************************************************************************************************************

        public ActionResult ViewUsers()
        {
            //get all the users and order them by username
            var users = db.Users.ToList().OrderBy(u => u.UserName);

            //send the list of users to the Index view
            return View(users);
        }

        [HttpPost]
        //this is the action that will process the search form on the index page
        //the name of the string parameter SearchString must be the same
        //with the name of the textbox on the view
        public ActionResult ViewUsers(string SearchString)
        {
            var user = db.Users.Where(u => u.UserName == SearchString);

            if (SearchString.Equals(""))
            {
                return RedirectToAction("ViewUsers");
            }
            return View(user);
        }

        //**********************************************************************************************************************************************************

        public ActionResult ViewAllPosts()
        {
            //get current logged in user admin
            string currentUserId = User.Identity.GetUserId();

            //get all posts from the database including their category and the user who created the post
            List<BlogPost> blogPosts = db.BlogPosts
                .Include(bp => bp.Category)
                .Include(bp => bp.User)
                .ToList();

            // Truncate the content of certain properties to a maximum of 2 words
            foreach (var post in blogPosts)
            {
                //truncating title and content
                post.Title = blog.TruncateWords(post.Title, 2);
                post.Content = blog.TruncateWords(post.Content, 2);
            }

            ViewBag.LoggedInUserId = currentUserId;

            //send the list to the view named ViewAllPosts
            return View(blogPosts);
        }

        //**********************************************************************************************************************************************************

        //  CREATE A NEW EMPLOYEE
        //**********************************************************************************************************************************************************

        [HttpGet]
        public ActionResult CreateEmployee()
        {

            CreateEmployeeViewModel employee = new CreateEmployeeViewModel();


            //get all the roles from the database (except "Member") and store them as a SelectedListItem so that we can display the roles in a dropdownlist
            var roles = db.Roles
                .Where(r => r.Name != "Member")
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToList();

            //assign the roles to the employee roles property
            employee.Roles = roles;

            //send the employees model to the view
            return View(employee);
        }

        [HttpPost]
        public ActionResult CreateEmployee(CreateEmployeeViewModel model)
        {
            //to ensure model.Roles isn't null
            //get all the roles from the database (except "Member") and store them as a SelectedListItem so that we can display the roles in a dropdownlist
            var roles = db.Roles
                .Where(r => r.Name != "Member")
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToList();

            //assign the roles to the model roles property
            model.Roles = roles;

            //if model is not null
            if (ModelState.IsValid)
            {
                //build the employee
                Employee newEmployee = new Employee
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    Street = model.Street,
                    City = model.City,
                    Postcode = model.Postcode,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmployementStatus = model.EmployementStatus,
                    IsActive = true,
                    IsSuspended = false,
                    RegisteredAt = DateTime.Now

                };

                //create the user, store it in the database and pass the password over to be hashed
                var result = UserManager.Create(newEmployee, model.Password);
                //if user was stored in the database successfully
                if (result.Succeeded)
                {
                    //then add user to the selected role
                    UserManager.AddToRole(newEmployee.Id, model.Role);

                    return RedirectToAction("ViewUsers", "Admin");
                }
            }

            //if we reach here
            //Something went wrong so go back to the create employee view
            return View(model);
        }

        //**********************************************************************************************************************************************************

        //  ADMIN WILL BE ABLE TO SUSPEND, PROMOTE NAD PERSONALISE ANY USER
        //**********************************************************************************************************************************************************
        [HttpGet]
        public async Task<ActionResult> Promote(string id)
        {
            //if the id being passed from the front end is empty
            //Return a bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //can't change your own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            //find user by id and store in User
            User user = await UserManager.FindByIdAsync(id);
            //get user's current role
            string oldRole = (await UserManager.GetRolesAsync(id)).Single(); //only ever a single role

            //get all the roles from the database and store them as a list of selectedlistitems
            var items = db.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = r.Name == oldRole
                }).ToList();

            //build the changeroleviewmodel object including the list of roles
            //and send it to the view displaying the roles in a dropdownlist with the user's current role displayed as selected
            return View("Promote", new ChangeRoleViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = user.Street,
                City = user.City,
                Postcode = user.Postcode,
                RegisteredAt = user.RegisteredAt,
                IsActive = user.IsActive,
                IsSuspended = user.IsSuspended,
                Roles = items,
                OldRole = oldRole

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Promote")]
        public async Task<ActionResult> PromotionConfirmed(string id, [Bind(Include = "Role")] ChangeRoleViewModel model)
        {
            string oldRole = null;

            // Can't change your own role.
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByIdAsync(id); //get user by id

                //get user's current role
                oldRole = (await UserManager.GetRolesAsync(id)).Single(); //Only ever a single role.

                //if current role is the same with selected role then there is no point to update the database
                if (oldRole == model.Role)
                {
                    return RedirectToAction("ViewUsers", "Admin");
                }

                //Remove user from the old role first.
                await UserManager.RemoveFromRoleAsync(id, oldRole);
                //now we are adding the user to the new role
                await UserManager.AddToRoleAsync(id, model.Role);


                return RedirectToAction("ViewUsers", "Admin");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Suspend(string id)
        {
            //if the id being passed from the front end is empty
            //Return a bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //can't change your own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            //find user by id and store in User
            User user = await UserManager.FindByIdAsync(id);

            //build the changeroleviewmodel object
            //and send it to the view displaying the properties
            return View("Suspend", new ChangeRoleViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = user.Street,
                City = user.City,
                Postcode = user.Postcode,
                RegisteredAt = user.RegisteredAt,
                IsActive = user.IsActive,
                IsSuspended = user.IsSuspended,
                Role = user.CurrentRole
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Suspend")]
        public async Task<ActionResult> SuspensionConfirmed(string id, ChangeRoleViewModel model)
        {

            // Can't change your own role.
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            User user = await UserManager.FindByIdAsync(id); //get user by id

            if (ModelState.IsValid)
            {
                if (model.IsSuspended)
                {
                    //get IsSuspended
                    user.IsSuspended = model.IsSuspended;

                }
                else
                {
                    user.IsSuspended = model.IsSuspended;
                }

                //update user's details in the database
                await UserManager.UpdateAsync(user);

                return RedirectToAction("ViewUsers", "Admin");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Personalise(string id)
        {
            //if the id being passed from the front end is empty
            //Return a bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //can't change your personalise self
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            //find user by id and store in User
            User user = await UserManager.FindByIdAsync(id);

            return View("Personalise", new PersonaliseDetailsViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = user.Street,
                City = user.City,
                Postcode = user.Postcode,
                Role = user.CurrentRole

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Personalise")]
        public async Task<ActionResult> PersonaliseConfirmed(string id, PersonaliseDetailsViewModel model)
        {
            // Can't change your own details.
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            //get user by id
            User user = await UserManager.FindByIdAsync(id); //get user by id

            if (ModelState.IsValid)
            {
                // Update user details
                user.UserName = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Street = model.Street;
                user.City = model.City;
                user.Postcode = model.Postcode;

                // Update the password if the NewPassword is not null or empty
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var newPasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                    user.PasswordHash = newPasswordHash;
                }

                // Save changes
                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Redirect to a success page or display a success message
                    return RedirectToAction("ViewUsers", "Admin");
                }
                else
                {
                    // If there are errors during the update, add them to the ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If ModelState is not valid, send the model back to the view,
            //the validation check will deal with the errors.
            return View(model);
        }
            //**********************************************************************************************************************************************************

            //  ADMIN WILL BE ABLE TO CHANGE A USERS ROLE
            //**********************************************************************************************************************************************************

        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id, string pageName)
        {
            //if the id being passed from the front end is empty
            //Return a bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //can't change your own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            //find user by id and store in User
            User user = await UserManager.FindByIdAsync(id);
            //get user's current role
            string oldRole = (await UserManager.GetRolesAsync(id)).Single(); //only ever a single role

            //get all the roles from the database and store them as a list of selectedlistitems
            var items = db.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = r.Name == oldRole
                }).ToList();

            //Change Suspend Role, if "Suspend" link is clicked
            if (pageName.Equals("Suspend"))
            {
                //pass id to view to be caught
                //ViewBag.UserId = id;
                //build the changeroleviewmodel object
                //and send it to the view displaying the properties
                return View("Suspend", new ChangeRoleViewModel
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Street = user.Street,
                    City = user.City,
                    Postcode = user.Postcode,
                    RegisteredAt = user.RegisteredAt,
                    IsActive = user.IsActive,
                    IsSuspended = user.IsSuspended,
                    Roles = items,
                    OldRole = oldRole,
                    Role = user.CurrentRole

                });
            }

            //ensures you can't suspend a user from the dropdown list
            items = db.Roles
                .Where(r => r.Name != "Suspended")
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = r.Name == oldRole
                }).ToList();

            if (pageName.Equals("Promote"))
            {
                //build the changeroleviewmodel object including the list of roles
                //and send it to the view displaying the roles in a dropdownlist with the user's current role displayed as selected
                return View("Promote", new ChangeRoleViewModel
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Street = user.Street,
                    City = user.City,
                    Postcode = user.Postcode,
                    RegisteredAt = user.RegisteredAt,
                    IsActive = user.IsActive,
                    IsSuspended = user.IsSuspended,
                    Roles = items,
                    OldRole = oldRole

                });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, ChangeRoleViewModel model)
        {
            string oldRole = null;

            // Can't change your own role.
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewUsers", "Admin");
            }

            User user = await UserManager.FindByIdAsync(id); //get user by id

            //get user's current role
            oldRole = (await UserManager.GetRolesAsync(id)).Single(); //Only ever a single role.


            if (ModelState.IsValid)
            {
                if (model.IsSuspended)
                {
                    //get IsSuspended
                    user.IsSuspended = model.IsSuspended;

                }
                else
                {
                    user.IsSuspended = model.IsSuspended;
                }
                //update user's details in the database
                await UserManager.UpdateAsync(user);


                //if current role is the same with selected role then there is no point to update the database
                if (oldRole == model.Role)
                {
                    return RedirectToAction("ViewUsers", "Admin");
                }

                //Remove user from the old role first.
                await UserManager.RemoveFromRoleAsync(id, oldRole);
                //now we are adding the user to the new role
                await UserManager.AddToRoleAsync(id, model.Role);


                return RedirectToAction("ViewUsers", "Admin");
            }

            return View(model);
        }

        //**********************************************************************************************************************************************************

        //  CRUD METHODS
        //**********************************************************************************************************************************************************


        //GET: Blog Posts Create
        public ActionResult Create()
        {
            //send the list of categories to the view using a ViewBag
            //so user can select the category for the post from a dropdown box
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryID", "Name");

            //return the Create view to the browser
            return View();
        }

        //POST: Blog Posts Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogPostID,Title,Content,CategoryID")] BlogPost blogPost)
        {
            //if parameter blogPost is not null
            if (ModelState.IsValid)
            {
                //assign todays date for DatePosted
                blogPost.DatePosted = DateTime.Now;

                //assign todays date fore DateEdited
                blogPost.DateEdited = DateTime.Now;

                //turning blogPosted to true
                blogPost.BlogPosted = true;

                //assign the registered user id as a foreign key
                //this is the user who creates the post
                blogPost.UserId = User.Identity.GetUserId();

                //add the blogPost to the BlogPost table
                db.BlogPosts.Add(blogPost);

                //save changes in the database
                db.SaveChanges();

                //return to MyPostBlog action in MemberController
                return RedirectToAction("MyBlogPost");
            }

            //if the parameter blogPost is null then send the list categories back to the create view
            //and try to create a blogPost again
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryID", "Name", blogPost.CategoryID);

            //send the post back to the create view
            return View(blogPost);
        }

        //GET Blog Posts Details/ 1
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the blog post that is in the BlogPosts table by id
            BlogPost blogPost = db.BlogPosts
                .Include(bp => bp.Category)
                .Include(bp => bp.User)
                .Include(bp => bp.Comments)
                .SingleOrDefault(bp => bp.BlogPostID == id);

            //if blogPost is empty then return a not found error message
            if (blogPost == null)
            {
                return HttpNotFound();
            }

            //otherwise send the blogPost info to the details view
            //and display the values stored in the properties

            return View(blogPost);
        }

        //GET Blog Posts Remove Comments/ 4
        //this method returns comments to be viewed
        //so that the user can make changes
        public ActionResult RemoveComments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find post by Id in the Posts table
            BlogPost post = db.BlogPosts
                .Include(p => p.Comments)
                .Include(p => User)
                .SingleOrDefault(p => p.BlogPostID == id);

            List<Comment> comments = post.Comments;


            if (post == null)
            {
                return HttpNotFound();
            }

            if (comments.Count == 0)
            {
                return View("NoComments");
            }

            return View(comments);
        }

        //POST Blog Posts Remove Comments/4
        //This method gets the edited/modified post and updates the changes in the database
        [HttpPost, ActionName("RemoveComments")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSelected(int CommentID)
        {  
            //find comment id
            Comment comment = db.Comments.Find(CommentID);

            db.Comments.Remove(comment);

            //saves changes to the database
            db.SaveChanges();

            //redirect to the blogpost page
            return RedirectToAction("ViewAllPosts");
        }

        //GET: BlogPosts/Delete
        //this method will delete a post by id
        public ActionResult Delete(int? id)
        {
            //if id is null then return a bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //first find a post in the Posts tables by id
            BlogPost blogPost = db.BlogPosts
                .Include(bp => bp.Category)
                .Include(bp => bp.User)
                .SingleOrDefault(bp => bp.BlogPostID == id);

            //next find the post category by searching through the Categories table
            //for a category by id which is the foreign key in that post
            var category = db.Categories.Find(blogPost.CategoryID);

            //assign the category to the Category naviational property Category
            //so we can display the category name
            blogPost.Category = category;

            //if the post is a null object then return a not found error message
            if (blogPost == null)
            {
                return HttpNotFound();
            }

            //otherwise return the Delete view and send the post to the view
            //so post details can be viewed
            return View(blogPost);
        }

        // POST: Blog Posts Delete/5
        //Using ActionName data annotation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            //get the Id of the logged in user, using Identity
            var userId = User.Identity.GetUserId();

            //find post by id in Posts tables
            BlogPost blogPost = db.BlogPosts
                .Include(bp => bp.Comments)
                .Include(bp => bp.User)
                .SingleOrDefault(bp => bp.BlogPostID == id);


            if (userId == blogPost.UserId)
            {
                db.BlogPosts.Remove(blogPost);

                //save changes in the database
                db.SaveChanges();

                return RedirectToAction("MyBlogPost");
            }

            //remove the post from the blog posts table
            //Also removes any comments related to the post from the comments table
            db.BlogPosts.Remove(blogPost);

            //save changes in the database
            db.SaveChanges();

            //send the user back to the my blog post page
            return RedirectToAction("ViewAllPosts");
        }

        //GET Blog Posts Edit/ 4
        //this method returns the edit form with the instance of blog post
        //so that the user can make changes
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find post by Id in the Posts table
            BlogPost blogPost = db.BlogPosts
                .Include(bp => bp.Comments)
                .SingleOrDefault(bp => bp.BlogPostID == id);

            if (blogPost == null)
            {
                return HttpNotFound();
            }

            //get a list of all the categories & comments from the category table
            //send the list to the view using a viewbag
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryID", "Name", blogPost.CategoryID);
            ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentText", blogPost.Comments);

            //send blog post to the Edit view
            //Users can change the details of the post
            return View(blogPost);
        }

        //POST Blog Posts Edit/4
        //This method gets the edited/modified post and updates the changes in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BlogPostID,Title,Content,BlogPosted,CategoryID,DatePosted")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                //record the new date
                blogPost.DateEdited = DateTime.Now;

                //get the logged in user id
                //assign it as a fk in the blog post
                blogPost.UserId = User.Identity.GetUserId();

                //update the database
                db.Entry(blogPost).State = EntityState.Modified;

                //saves changes to the database
                db.SaveChanges();

                //redirect to the blogpost page
                return RedirectToAction("MyBlogPost");
            }

            //else the blogPost parameter is null,
            //send the list categories and comments back to the edit form
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryID", "Name", blogPost.CategoryID);
            //ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentText", blogPost.Comments);

            //return the blog post to the edit form
            return View(blogPost);
        }
    }
}