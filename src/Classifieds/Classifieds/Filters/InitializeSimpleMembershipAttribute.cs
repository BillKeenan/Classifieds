using System.Web.Mvc;
using Classifieds.services.UserServices;

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
