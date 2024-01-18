using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K_Burns_Assessment_2.Models
{
    using System;
    using System.Web.Mvc;

    public class RedirectUnauthorizedUsersFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            // Check if the requested path starts with "/Admin/" OR "/Member/
            var requestedPath = filterContext.HttpContext.Request.Path;

            // Check if the user is accessing the "/Admin/" route
            if (requestedPath.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) &&
            user.Identity.IsAuthenticated && !user.IsInRole("Admin"))
            {
                // Redirect authenticated users without the required role to the home page
                filterContext.Result = new RedirectResult("~/Home/Index");
            }

            // Check if the user is accessing the "/Member/" route
            if (requestedPath.StartsWith("/Member", StringComparison.OrdinalIgnoreCase) &&
            user.Identity.IsAuthenticated && !user.IsInRole("Member"))
            {
                // Redirect authenticated users without the required role to the home page
                filterContext.Result = new RedirectResult("~/Home/Index");
            }

            // Check if the user is accessing the "/Member/" route
            if (requestedPath.StartsWith("/Staff", StringComparison.OrdinalIgnoreCase) &&
            user.Identity.IsAuthenticated && !user.IsInRole("Staff"))
            {
                // Redirect authenticated users without the required role to the home page
                filterContext.Result = new RedirectResult("~/Home/Index");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}