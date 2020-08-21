using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StayLive.Helpers
{
    public class RoleFilter : ActionFilterAttribute
    {
        public StayLive.areas.Users.Models.UserRoles Role { get; set; }
        public StayLive.areas.Users.Models.UserRoles Role2 { get; set; }
        public StayLive.areas.Users.Models.UserRoles Role3 { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((byte)Role == SessionHelper.AccountRole 
                || (byte)Role2 == SessionHelper.AccountRole 
                || (byte)Role3 == SessionHelper.AccountRole)
                return;
            else
            {
                //throw new PermessionDeniedException();
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "PermissionDenied",
                    controller = "Home",
                    area = ""
                }));
                return;
            }
        }

    }
}