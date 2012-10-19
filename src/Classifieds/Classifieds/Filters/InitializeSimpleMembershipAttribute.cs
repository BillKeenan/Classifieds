using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using Classifieds.services.UserServices;
using WebMatrix.WebData;
using Classifieds.Models;

namespace Classifieds.Filters
{
        public class AuthorizationAttribute : ActionFilterAttribute, IActionFilter
        {
            public MyRole UserRole { get; set; }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
        }

        
    }
}
