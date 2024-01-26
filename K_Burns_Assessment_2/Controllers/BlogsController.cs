using K_Burns_Assessment_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using K_Burns_Assessment_2.Controllers;
using K_Burns_Assessment_2.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using System.Web.Razor.Parser.SyntaxTree;
using System.Xml.Linq;

namespace K_Burns_Assessment_2.Controllers
{
    public class BlogsController : Controller
    {
        private MefistoDbContext context = new MefistoDbContext();
        // GET: Blogs
        public ActionResult Index()
        {

            //getting all the blogs
            var blogs = context.BlogPosts.Where(b => b.BlogPosted == true).ToList();

            foreach (var item in blogs)
            {
                context.Entry(item).Reload();//refresh entities
            }

            ViewBag.Categories = context.Categories.ToList();
            return View("Blogs", blogs);
        }

        public ActionResult Blogs(int? id)
        {
            //getting all the blogs that are in a specific category
            var blogs = context.BlogPosts.Where(b => b.CategoryID == id).Where(b => b.BlogPosted == true).ToList();

            //sending all the categories in a viewbag
            ViewBag.Categories = context.Categories.ToList();

            //get selected category
            string selectedCategory = blogs.FirstOrDefault().Category.Name;

            //Send all selected cateogry in a viewbag
            ViewBag.SelectedCategory = RemoveSpaces(selectedCategory);

            return View("Blogs", blogs);
        }

        public ActionResult Blog(int? id, string direction)
        {

            //if id is null return a bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get current logged-in user's ID
            string userId = User.Identity.GetUserId();

            //store blogs in a list
            List<BlogPost> blogs;

            //check if the session doesn't contain blogs
            if (Session["Blogs"] == null)
            {
                //fetch blogs from the db and store them in the session in order of blog post id
                blogs = context.BlogPosts
                    .Include(bp => bp.User)
                    .Include(bp => bp.Comments)
                    .OrderBy(bp => bp.BlogPostID)
                    .ToList();

                Session["Blogs"] = blogs;
                Session["CurrentIndex"] = 0; //set current index to 0
            }
            else
            {
                //set blog but not the session
                blogs = context.BlogPosts
                    .Include(bp => bp.User)
                    .Include(bp => bp.Comments)
                    .OrderBy(bp => bp.BlogPostID)
                    .ToList();
            }

            //get the current index from the session
            int currentIndex = (int)Session["CurrentIndex"];

            //update the current index based on the passed in direction (Previous, Next, or direct ID)
            if (direction == "Previous")
            {
                currentIndex = (currentIndex - 1 + blogs.Count) % blogs.Count;
            }
            else if (direction == "Next")
            {
                currentIndex = (currentIndex + 1) % blogs.Count;
            }
            else
            {
                currentIndex = blogs.FindIndex(bp => bp.BlogPostID == id.Value);
            }

            //get the current blog post based on the updated index
            BlogPost blogPost = blogs[currentIndex];

            //get comments for the current blog post
            List<Comment> comments = blogPost.Comments;

            //initialize a flag for user suspension
            bool isUserSuspended = false;

            //get current user from db
            User user = context.Users.SingleOrDefault(u => u.Id == userId);

            //if user exisit, we update the suspension flag
            if (user != null)
            {
                isUserSuspended = user.IsSuspended;
            }

            //create viewbags for the following: Categories, Comments, IsUser Suspended
            //& logged in user, this will be caught in the view
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Comments = comments;
            ViewBag.IsUserSuspended = isUserSuspended;
            ViewBag.LoggedInUser = User.Identity.Name;

            //Update the session with the current index
            Session["CurrentIndex"] = currentIndex;
            Session["CurrentBlogId"] = blogPost.BlogPostID;

            return View(blogPost);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Blog(int id,[Bind(Include = "BlogPostID,BlogPosted,DatePosted,NewCommentText")] BlogPost blogPost)
        {
            if (blogPost.BlogPostID > 0)
            {
                //declare new comment
                Comment newComment = new Comment();

                //pass data to properties
                newComment.DatePosted = DateTime.Now;
                newComment.UserId = User.Identity.GetUserId();
                newComment.BlogPostID = id;
                newComment.CommentText = blogPost.NewCommentText; //assign the new comment to comment text

                if(string.IsNullOrWhiteSpace(newComment.CommentText))
                {
                    TempData["EmptyComment"] = "Comment entered cannot be empty! ";
                }
                else
                {
                    // Add the comment to the database
                    context.Comments.Add(newComment);

                    //save changes in the database
                    context.SaveChanges();

                    ViewBag.Comments = blogPost.Comments;

                    TempData["AlertMessage"] = "Your comment has been posted... ";

                    //return View(blogPost);
                }
            }
            // Redirect to the blog post page regardless
            return RedirectToAction("Blog", id);
        }

        //other userful methods
        
        //remove spacing in strings
        static string RemoveSpaces(string input)
        {
            return input.Replace(" ", "");
        }
    }
}