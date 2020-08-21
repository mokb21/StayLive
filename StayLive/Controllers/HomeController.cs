using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StayLive.Helpers;
using StayLive.Models;
using StayLive.Models.Home;

namespace StayLive.Controllers
{
    public class HomeController : BaseController
    {
        #region Actions
        public ActionResult Index()
        {
            HomeVM vm = new HomeVM();
            vm.Name = SessionHelper.AccountName;
            vm.Email = SessionHelper.AccountEmail;
            vm.LastUpdated = dbService.Tickets.Where(a => a.UpdateUserId == SessionHelper.AccountId).ToList().Count;
            vm.Solved = dbService.Tickets.Where(a => a.UpdateUserId == SessionHelper.AccountId
                && a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Completed).ToList().Count;
            vm.AssigedTo = dbService.Tickets.Where(a => a.AssignedUserId == SessionHelper.AccountId).ToList().Count;
            vm.Opened = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId
                && a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Opened).ToList().Count;
            vm.Completed = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId
                && a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Completed).ToList().Count;
            vm.Pending = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId
                && a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Pending).ToList().Count;
            vm.Spam = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId
                && a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Spam).ToList().Count;
            vm.Duplicated = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId
                && a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Duplicated).ToList().Count;
            vm.AllAssignedTickets = dbService.Tickets.Where(a => a.AssignedUserId == SessionHelper.AccountId
                 && (a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Opened
                     || a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Pending
                     || a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Completed)).ToList().Count;
            return View(vm);
        }

        public ActionResult PermissionDenied()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            StayLive.Models.SaveModel vm = new Models.SaveModel()
            {
                title = StayLive.Resources.General.ResetPassword,
                action = "ChangePassword",
                controller = "Home",
                area = "",
                isAjax = false,
                view = "~/Views/Shared/_changePassword.cshtml",
                model = new Models.ChangePassword()
            };
            return Json(new { data = this.RenderPartialToString("~/Views/Shared/_SaveModal.cshtml", vm) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword vm)
        {
            var user = dbService.Users.Find(SessionHelper.AccountId);
            if (user == null)
                return this.NotFound();

            if (ControllersExtensions.HashPassword(vm.CurrentPassword).SequenceEqual(user.Password))
            {
                user.Password = ControllersExtensions.HashPassword(vm.Password);
                dbService.SaveChanges();
                this.MsgSuccess(Resources.General.ChangePassword, Resources.General.PasswordUpdatedSuccessfully);
            }
            else
                this.MsgError(Resources.General.ChangePassword, Resources.General.SomethingWentWorng);

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult UpdateProfileImage()
        {
            if (Request.Files == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            HttpPostedFileBase file;
            file = Request.Files[0];

            if (file != null && file.ContentLength != 0)
                if (!file.IsValidImage())
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var user = dbService.Users.Find(SessionHelper.AccountId);
            if (user == null)
                return this.NotFound();

            user.ProfilePhoto = file.GetImageBytes();
            dbService.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetterMethodes
        [HttpPost]
        public ActionResult GetAssignedTickets()
        {
            List<DoughnutChartObject> data = new List<DoughnutChartObject>
            {
                GetDoughnutChartPart(areas.Tickets.Models.TicketInfo.TicketStatus.Pending),
                GetDoughnutChartPart(areas.Tickets.Models.TicketInfo.TicketStatus.Opened),
                GetDoughnutChartPart(areas.Tickets.Models.TicketInfo.TicketStatus.Completed),
                GetDoughnutChartPart(areas.Tickets.Models.TicketInfo.TicketStatus.Duplicated)
            };

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLevelsStatus()
        {
            int count = 0;
            List<DoughnutChartObject> data = new List<DoughnutChartObject>
            {
                new DoughnutChartObject()
                {
                    label = GetLevelNameAndTickets(1, out count),
                    value = count,
                    color = "#8155A2",
                    highlight = "#8155A2"
                },
                new DoughnutChartObject()
                {
                    label = GetLevelNameAndTickets(2, out count),
                    value = count,
                    color = "#FF4D4D",
                    highlight = "#FF4D4D"
                },
                new DoughnutChartObject()
                {
                    label = GetLevelNameAndTickets(3, out count),
                    value = count,
                    color = "#40ACDC",
                    highlight = "#40ACDC"
                }
            };
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTopUsers()
        {
            BarChartObject data = new BarChartObject();
            BarChartStroke strokeAssigned = new BarChartStroke()
            {
                fillColor = "#fc4b6c",
                highlightFill = "#fc4b6c",
                highlightStroke = "#fc4b6c",
                label = "Assigned tickets",
                strokeColor = "#fc4b6c"
            };
            BarChartStroke strokeLastUpdated = new BarChartStroke()
            {
                fillColor = "#1e88e5",
                highlightFill = "#1e88e5",
                highlightStroke = "#1e88e5",
                label = "Completed in last month",
                strokeColor = "#1e88e5"
            };

            Dictionary<int, int> tkAssigned = new Dictionary<int, int>();
            Dictionary<int, int> tkLastUpdated = new Dictionary<int, int>();
            var lastmonth = DateTime.Now.AddMonths(-1);
            foreach (var user in dbService.Users)
            {
                var AssignedCount = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId && a.User.Id == user.Id
                && (a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Opened
                    || a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Pending)).ToList().Count;

                var LastUpdatedCount = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId && a.User1.Id == user.Id
                    && a.Status == (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Completed
                    && a.UpdateDate > lastmonth).ToList().Count;

                if (AssignedCount != 0)
                {
                    tkAssigned[user.Id] = AssignedCount;
                    tkLastUpdated[user.Id] = LastUpdatedCount;
                }
            }

            foreach (var ticket in tkAssigned.OrderByDescending(a => a.Value).Take(10))
            {
                data.labels.Add(dbService.Users.Find(ticket.Key).Name);
                strokeAssigned.data.Add(ticket.Value);
                strokeLastUpdated.data.Add(tkLastUpdated[ticket.Key]);
            }

            data.datasets.Add(strokeAssigned);
            data.datasets.Add(strokeLastUpdated);

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PrivateMethodes
        private DoughnutChartObject GetDoughnutChartPart(StayLive.areas.Tickets.Models.TicketInfo.TicketStatus Status)
        {
            var count = dbService.Tickets.Where(a => a.AssignedUserId == SessionHelper.AccountId && a.Status == (byte)Status).ToList().Count;
            string label = "";
            string color = "";
            switch (Status)
            {
                case areas.Tickets.Models.TicketInfo.TicketStatus.Pending:
                    label = StayLive.Resources.Tickets.Pending;
                    color = areas.Tickets.Models.TicketInfo.TicketColor.Pending;
                    break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Opened:
                    label = StayLive.Resources.Tickets.Opened;
                    color = areas.Tickets.Models.TicketInfo.TicketColor.Opened;
                    break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Completed:
                    label = StayLive.Resources.Tickets.Completed;
                    color = areas.Tickets.Models.TicketInfo.TicketColor.Completed;
                    break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Spam:
                    label = StayLive.Resources.Tickets.Spam;
                    color = areas.Tickets.Models.TicketInfo.TicketColor.Spam;
                    break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Deleted:
                    label = StayLive.Resources.Tickets.Deleted;
                    color = areas.Tickets.Models.TicketInfo.TicketColor.Deleted;
                    break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Duplicated:
                    label = StayLive.Resources.Tickets.Duplicated;
                    color = areas.Tickets.Models.TicketInfo.TicketColor.Duplicated;
                    break;
                default:
                    break;
            }

            return new DoughnutChartObject() {
                label = label,
                value = count,
                color = color,
                highlight = color
            };
        }

        private string GetLevelNameAndTickets(byte level, out int TicketsCount)
        {
            TicketsCount = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId && a.Level == level 
            && a.Status != (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Completed
            && a.Status != (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Deleted
            && a.Status != (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Spam
            ).ToList().Count;

            var lvl = dbService.Levels.Where(a => a.CompanyId == SessionHelper.CompanyId).FirstOrDefault();
            if (lvl == null)
                return "";
            else
                switch (level)
                {
                    case 1:
                        return lvl.FirstName;
                    case 2:
                        return lvl.SecondName;
                    case 3:
                        return lvl.ThirdName;
                    default:
                        return "";
                }
        }
        #endregion
    }
}
