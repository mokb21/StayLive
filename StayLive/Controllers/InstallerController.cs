using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StayLive.Models;
using StayLive.Helpers;

namespace StayLive.Controllers
{
    public class InstallerController : Controller
    {
        private Stopwatch stopwatch = new Stopwatch();
        protected StayLiveEntities dbService = new StayLiveEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            stopwatch.Start();
            base.OnActionExecuting(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            dbService.Dispose();
            base.Dispose(disposing);
            stopwatch.Stop();
        }

        public ActionResult Index()
        {
            if (dbService.Users.ToList().Count <= 0)
            {
                Models.User user = new Models.User
                {
                    CompanyId = null,
                    Name = "Admin",
                    UserName = "Admin",
                    Password = ControllersExtensions.HashPassword("staylive123"),
                    Email = "",
                    Role = (byte)StayLive.areas.Users.Models.UserRoles.SystemAdmin
                };

                dbService.Users.Add(user);
                dbService.SaveChanges();
            }
            return RedirectToAction("", "Home", new { area = "" });
        }
    }
}