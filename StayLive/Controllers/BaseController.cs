using System.Web.Mvc;
using System.Diagnostics;
using System.Web.Routing;
using StayLive.Helpers;
using StayLive.Models;
using System;

namespace StayLive.Controllers
{
    public class BaseController : Controller
    {
        private Stopwatch stopwatch = new Stopwatch();
        protected StayLiveEntities dbService = new StayLiveEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            stopwatch.Start();
            base.OnActionExecuting(filterContext);

            if (String.IsNullOrWhiteSpace(SessionHelper.AccountName))//To Login Change it to false for now
            {
                //message errorAccountId
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "Index",
                    controller = "Login",
                    area = ""
                }));
                return;
            }

            //=> Authenticated user
            //if (SessionHelper.AccountId == null)
            //{
                //SessionHelper.AccountId = 1;
                //SessionHelper.AccountName = "mokb";
                //SessionHelper.AccountEmail= "mkabbani@fms-tech.com";
                //SessionHelper.AccountRole = 1;
                //SessionHelper.CompanyId = 1;
                //SessionHelper.CompanyName = "FMS";
            //}

            //if (!SessionHelper.IsSessionInitialized)
            //    SessionHelper.InitializeSession();
        }

        protected override void Dispose(bool disposing)
        {
            dbService.Dispose();
            base.Dispose(disposing);
            stopwatch.Stop();
        }

    }
}
