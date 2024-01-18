using K_Burns_Assessment_2.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace K_Burns_Assessment_2.Controllers
{
    //deals with all file names that follow the "Staff" path
    [RedirectUnauthorizedUsersFilter]
    public class StaffController : AccountController
    {
        //creating an instance of the Mefisto DB Context
        private MefistoDbContext db = new MefistoDbContext();

        //create an instance of blog to access the TruncateWords method
        private BlogPost blog = new BlogPost();
        public StaffController() : base()
        {
        }

        public StaffController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)

        {

        }
        // GET: Staff
        public ActionResult Index()
        {
            return View();
        }

        //**********************************************************************************************************************************************************
        //GET blog posts page
        public ActionResult MyBlogPost()
        {
            //get the Id of the logged in user, using Identity
            var userId = User.Identity.GetUserId();

            //get all posts from posts table
            var blogPosts = db.BlogPosts
                .Include(bp => bp.Category)
                .Include(bp => bp.User);


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
        public ActionResult ManageUsersPosts()
        {
            //get current logged in user staff
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

        //  CRUD METHODS
        //**********************************************************************************************************************************************************

        //GET: Blog Posts Create
        public ActionResult Create()
        {

            //Get the list of categories from the database
            var categories = db.Categories.ToList();

            //Exclude a the Post Announcement Category
            //as this is to be reserved only for the Admin
            var excludedCategory = categories.FirstOrDefault(c => c.Name == "Post Announcement");

            if (excludedCategory != null)
            {
                //Remove the excluded category from the list
                categories.Remove(excludedCategory);
            }

            //send the list of categories to the view using a ViewBag
            //so user can select the category for the post from a dropdown box
            ViewBag.CategoryId = new SelectList(categories, "CategoryID", "Name");

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

            //Get the list of categories from the database
            var categories = db.Categories.ToList();

            //Exclude a the Post Announcement Category
            //as this is to be reserved only for the Admin
            var excludedCategory = categories.FirstOrDefault(c => c.Name == "Post Announcement");

            if (excludedCategory != null)
            {
                //Remove the excluded category from the list
                categories.Remove(excludedCategory);
            }

            //if the parameter blogPost is null then send the list categories back to the create view
            //and try to create a blogPost again
            ViewBag.CategoryId = new SelectList(categories, "CategoryID", "Name", blogPost.CategoryID);

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

        //GET Blog Posts Remove Comments/ 3
        //this method returns the comments if any on post to be filtered through
        //so that the user can remove it if they want
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

        ////POST Blog Posts Remove Comments/4
        ////This method gets the edited/modified post and updates the changes in the database
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
            return RedirectToAction("ManageUsersPosts");
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
            return RedirectToAction("ManageUsersPosts");
        }

        //GET Blog Posts Edit/ 4
        //this method returns the dit form with the instance of blog post
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

            //Get the list of categories from the database
            var categories = db.Categories.ToList();

            //Exclude a the Post Announcement Category
            //as this is to be reserved only for the Admin
            var excludedCategory = categories.FirstOrDefault(c => c.Name == "Post Announcement");

            if (excludedCategory != null)
            {
                //Remove the excluded category from the list
                categories.Remove(excludedCategory);
            }

            //get a list of all the categories & comments from the category table
            //send the list to the view using a viewbag
            ViewBag.CategoryId = new SelectList(categories, "CategoryID", "Name", blogPost.CategoryID);
            ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentText", blogPost.Comments);

            //send blog post to the Edit view
            //Users can chagne the details of the post
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

            //Get the list of categories from the database
            var categories = db.Categories.ToList();

            //Exclude a the Post Announcement Category
            //as this is to be reserved only for the Admin
            var excludedCategory = categories.FirstOrDefault(c => c.Name == "Post Announcement");

            if (excludedCategory != null)
            {
                //Remove the excluded category from the list
                categories.Remove(excludedCategory);
            }

            //else the blogPost parameter is null,
            //send the list categories and comments back to the edit form
            ViewBag.CategoryId = new SelectList(categories, "CategoryID", "Name", blogPost.CategoryID);
            ViewBag.CommentId = new SelectList(db.Comments, "CommentID", "CommentText", blogPost.Comments);

            //return the blog post to the edit form
            return View(blogPost);
        }
    }
}