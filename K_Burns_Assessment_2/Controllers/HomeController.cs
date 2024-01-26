using K_Burns_Assessment_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K_Burns_Assessment_2.Controllers
{
    public class HomeController : Controller
    {
        private MefistoDbContext context = new MefistoDbContext();
        public ActionResult Index()
        {
            //getting all the blogs
            var blogs = context.BlogPosts.ToList();

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.SelectedCategory = "Home";

            return View(blogs);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}