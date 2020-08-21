using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using StayLive.Models.Login;
using StayLive.Helpers;
using StayLive.Models;

namespace StayLive.Controllers
{
    public class LoginController : Controller
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
            LoginVM vm = new LoginVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(LoginVM vm)
        {
          
        var user = dbService.Users.Where(a => a.UserName.ToLower() == vm.UserName.ToLower()).FirstOrDefault();
            if (user == null)
            {
                this.MsgError(StayLive.Resources.General.Login, StayLive.Resources.Validations.InvalidUsernameTryAgain);
                return View(vm);
            }

            if (ControllersExtensions.HashPassword(vm.Password).SequenceEqual(user.Password)) 
            {
                if (!SessionHelper.IsSessionInitialized)
                    SessionHelper.InitializeSession();

                SessionHelper.AccountId = user.Id;
                SessionHelper.AccountName = user.Name;
                SessionHelper.AccountEmail = user.Email;
                SessionHelper.AccountRole = user.Role;
                SessionHelper.CompanyId = (user.CompanyId == null ? 0 : user.Company.Id);
                SessionHelper.CompanyName = (user.CompanyId == null ? null : user.Company.Name);

                //Check and Change
                SessionHelper.Language = "en";

                this.MsgSuccess("Stay Live", "Welcome to Stay Live");
                return RedirectToAction("", "Home", new { area = "" });
            }

            this.MsgError(StayLive.Resources.General.Login, StayLive.Resources.Validations.InvalidPasswordTryAgain);
            return View(vm);
        }

        public ActionResult Logout()
        {
            SessionHelper.Destroy();
            return Redirect("~/Login");
        }
    }
}
