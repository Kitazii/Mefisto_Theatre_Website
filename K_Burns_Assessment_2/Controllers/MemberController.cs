using K_Burns_Assessment_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity;

namespace K_Burns_Assessment_2.Controllers
{
    //deals with all file names that follow the "Member" path
    [RedirectUnauthorizedUsersFilter]
    public class MemberController : AccountController
    {

        public MemberController() : base()
        { 

        }

        public MemberController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)
        {

        }


        //create an instance of the database context
        private MefistoDbContext db = new MefistoDbContext();

        //create an instance of blog to access the TruncateWords method
        private BlogPost blog = new BlogPost();

        //GET blog posts page
        public ActionResult MyPostBlog()
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

        //  CRUD METHODS
        //**********************************************************************************************************************************************************

        //GET Blog Posts Details/ 4
        public ActionResult Details(int? id) //making the parameter variable nullable
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
                return RedirectToAction("MyPostBlog");
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

        //GET Blog Posts Delete/5
        //this method will delete a post by id
        public ActionResult Delete(int? id)
        {
            //if id is null then return a bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //first find a blog post in the blog post table by id
            BlogPost blogPost = db.BlogPosts.Find(id);

            //next find the post category by id in the categories table
            var category = db.Categories.Find(blogPost.CategoryID);

            //assigning the category to the navigational property in the BlogPost class
            blogPost.Category = category;

            //if the blog post is a null object
            //then return a not found error message
            if (blogPost == null)
            {
                return HttpNotFound();
            }

            //otherwise return the Delete view and send the blog post to the view
            //so the blog post details can be viewed
            return View(blogPost);
        }

        // POST: Blog Posts Delete/5
        //Using ActionName data annotation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //find post by id in Posts tables
            BlogPost blogPost = db.BlogPosts.Find(id);

            //remove the post from the blog posts table
            db.BlogPosts.Remove(blogPost);

            //save changes in the database
            db.SaveChanges();

            //send the user back to the my blog post page
            return RedirectToAction("MyPostBlog");
        }

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
                return RedirectToAction("MyPostBlog");
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
    }
}