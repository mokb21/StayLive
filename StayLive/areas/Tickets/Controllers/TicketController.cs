using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.IO;
using System.Net.Mail;
using System.Net;
using StayLive.Controllers;
using StayLive.Helpers;
using StayLive.Models;
using StayLive.areas.Tickets.Models.Ticket;
using StayLive.areas.Tickets.Models.TicketReply;


namespace StayLive.Areas.Tickets.Controllers
{
    public class TicketController : BaseController
    {
        #region Actions
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin, Role2 = areas.Users.Models.UserRoles.Supervisor, Role3 = areas.Users.Models.UserRoles.Agent)]
        public ActionResult Index()
        {
            TicketsTableVM vm = new TicketsTableVM();
            vm = FillTicketFilter();
            TicketsTableFilter filter = null;
            var tmpObj = Session["TicketListFilter"];
            if (tmpObj != null)
            {
                filter = tmpObj as TicketsTableFilter;
                vm.Date = filter.Date.ToString();
                vm.Level = filter.Level.ToString();
                vm.User = filter.User.ToString();
                vm.Status = filter.Status;
            }
            return View(vm);
        }

        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin, Role2 = areas.Users.Models.UserRoles.Supervisor, Role3 = areas.Users.Models.UserRoles.Agent)]
        public ActionResult Ticket(string Key)
        {
            var ticket = dbService.Tickets.Where(a => a.Key == Key).FirstOrDefault();
            if (ticket == null)
                return this.NotFound();

            TicketVM vm = new TicketVM
            {
                Id = ticket.Id,
                Code = ticket.Key,
                Subject = ticket.Subject,
                AssignedTo = (ticket.User == null ? "No one" : ticket.User.Name),
                AssignedToValue = ticket.Id,
                CreatedDate = ticket.CreateDate.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                LastUpdate = (ticket.UpdateDate.HasValue ? ticket.UpdateDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : ""),
                TicketStatus = ((StayLive.areas.Tickets.Models.TicketInfo.TicketStatus)Enum.ToObject(typeof(StayLive.areas.Tickets.Models.TicketInfo.TicketStatus), ticket.Status)).ToString(),
                TicketStatusValue = ticket.Status,
                UserEmail = ticket.Email,
                UserName = ticket.Name,
                Replies = getTicketReplies(ticket.Id),
                UsersList = getUsers((ticket.AssignedUserId == null ? 0 : ticket.AssignedUserId.Value)),
            };
            return View(vm);
        }

        [HttpPost]
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin, Role2 = areas.Users.Models.UserRoles.Supervisor, Role3 = areas.Users.Models.UserRoles.Agent)]
        public ActionResult Ticket(TicketVM vm, HttpPostedFileBase Attachment)
        {
            TicketReply reply = new TicketReply()
            {
                Status = (byte)vm.TicketStatusValue,
                Message = vm.Message,
                IsInternal = vm.isInternall,
                TicketId = vm.Id,
                CreateDate = DateTime.Now,
                CreateByUserId = SessionHelper.AccountId,
                Attachment = (Attachment != null ? ControllersExtensions.GetImageBytes(Attachment) : null),
                AttachmentFileName = (Attachment != null ? Attachment.FileName : null)
            };
            
            Ticket ticket = dbService.Tickets.Find(vm.Id);
            if (ticket == null)
                return this.NotFound();

            ticket.UpdateDate = DateTime.Now;
            ticket.UpdateUserId = SessionHelper.AccountId;
            ticket.Status = (byte)vm.TicketStatusValue;
            ticket.AssignedUserId = vm.AssignedToValue;
            if (!reply.IsInternal)
            {
                if (!SendEmail(ticket, reply))
                {
                    this.MsgError(StayLive.Resources.Tickets.SendReply, StayLive.Resources.General.SomethingWentWorng);
                    return RedirectToAction("Ticket", "Ticket", new { area = "Tickets", Key = ticket.Key });
                }
            }
            dbService.TicketReplies.Add(reply);
            dbService.SaveChanges();
            this.MsgSuccess(StayLive.Resources.Tickets.SendReply, StayLive.Resources.Tickets.ReplySentSuccessfully);
            return RedirectToAction("Ticket", "Ticket", new { area = "Tickets", Key = ticket.Key });
        }

        [HttpPost]
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin, Role2 = areas.Users.Models.UserRoles.Supervisor, Role3 = areas.Users.Models.UserRoles.Agent)]
        public ActionResult Filter(TicketsTableVM vm)
        {
            TicketsTableFilter filter = new TicketsTableFilter()
            {
                User = int.Parse(vm.User),
                Level = int.Parse(vm.Level),
                Status = vm.Status,
                Date = int.Parse(vm.Date)
            };
            Session["TicketListFilter"] = filter;
            return RedirectToAction("Index", "Ticket", new { area = "Tickets" });
        }

        [HttpGet]
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin, Role2 = areas.Users.Models.UserRoles.Supervisor, Role3 = areas.Users.Models.UserRoles.Agent)]
        public ActionResult TicketsByStatus(int Status)
        {
            TicketsTableFilter filter = new TicketsTableFilter();
            filter.Status = Status;
            Session["TicketListFilter"] = filter;
            return RedirectToAction("Index", "Ticket", new { area = "Tickets" });
        }

        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin, Role2 = areas.Users.Models.UserRoles.Supervisor, Role3 = areas.Users.Models.UserRoles.Agent)]
        public ActionResult Delete(int Id)
        {
            var ticket = dbService.Tickets.Find(Id);
            if (ticket != null)
            {
                if (SessionHelper.AccountRole == (byte)areas.Users.Models.UserRoles.Admin)
                    ticket.Status = (byte)areas.Tickets.Models.TicketInfo.TicketStatus.Deleted;
                else
                    ticket.Status = (byte)areas.Tickets.Models.TicketInfo.TicketStatus.Spam;
            }
            dbService.SaveChanges();
            this.MsgDeleteSuccessfuly();
            return RedirectToAction("Index");
        }
        #endregion

        #region Getter Methodes
        [HttpPost]
        public ActionResult getTicketsData()
        {
            List<TicketRow> data;
            int TotalCount = 0;

            var draw = Request.Form.GetValues("draw").FirstOrDefault();//page number
            int start = int.Parse(Request.Form.GetValues("start").First());//use it to skip
            int length = int.Parse(Request.Form.GetValues("length").First());//count to show

            string sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            string sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();//sort direction 
            string searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


            Expression<Func<TicketRow, bool>> filter = (a) => (a.ClientName.ToLower().Contains(searchValue) || (a.CreateDate.ToLower()).Contains(searchValue) ||
                (a.Level.ToLower()).Contains(searchValue) || (a.Status).Contains(searchValue));

            List<TicketRow> lstTickets = getTicketsRow().AsQueryable().Where(filter).ToList();

            TotalCount = lstTickets.Count;
            data = lstTickets.AsQueryable().Select(a => a).OrderBy(sortColumn + " " + sortColumnDir)
            .Skip(start)
            .Take(length)
            .ToList();

            return Json(new { draw = draw, recordsFiltered = TotalCount, recordsTotal = TotalCount, data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin, Role2 = areas.Users.Models.UserRoles.Supervisor, Role3 = areas.Users.Models.UserRoles.Agent)]
        public ActionResult Download(int Id, string Type)
        {
            if (Type == "ticket")
            {
                var ticket = dbService.Tickets.Find(Id);
                if (ticket != null)
                {
                    byte[] attachment = ticket.Attachment;
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = ticket.AttachmentFileName,
                        Inline = false
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    return File(attachment, ticket.AttachmentFileName);
                }
            }
            else if (Type == "reply")
            {
                var reply = dbService.TicketReplies.Find(Id);
                if (reply != null)
                {
                    byte[] attachment = reply.Attachment;
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = reply.AttachmentFileName,
                        Inline = false
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    return File(attachment, reply.AttachmentFileName);
                }
            }
            return null;
        }
        #endregion

        #region Private Methodes
        private List<TicketRow> getTicketsRow()
        {
            List<TicketRow> tickets = new List<TicketRow>();
            var tkts = dbService.Tickets.Where(a => a.CompanyId == SessionHelper.CompanyId).ToList();

            if (SessionHelper.AccountRole == (byte)areas.Users.Models.UserRoles.Supervisor || SessionHelper.AccountRole == (byte)areas.Users.Models.UserRoles.Supervisor)
            {
                var user = dbService.Users.Find(SessionHelper.AccountId);
                tkts = tkts.Where(a => a.Level == user.Level.Value).ToList();
            }

            TicketsTableFilter filter = null;
            var tmpObj = Session["TicketListFilter"];
            if (tmpObj != null)
            {
                filter = tmpObj as TicketsTableFilter;
                if (filter.Status > 0)
                    tkts = tkts.Where(a => a.Status == filter.Status).ToList();
                if (filter.User > 0)
                    tkts = tkts.Where(a => a.AssignedUserId == filter.User).ToList();
                if (filter.Level > 0)
                    tkts = tkts.Where(a => a.Level == filter.Level).ToList();
                if (filter.Date > 0)
                {
                    switch (filter.Date)
                    {
                        case 1:
                            tkts = tkts.Where(a => a.CreateDate >= DateTime.Now.AddDays(-1)).ToList();
                            break;
                        case 2:
                            tkts = tkts.Where(a => a.CreateDate >= DateTime.Now.AddDays(-7)).ToList();
                            break;
                        case 3:
                            var today = DateTime.Today;
                            var month = new DateTime(today.Year, today.Month, 1);
                            var first = month.AddMonths(-1);
                            var last = month.AddDays(-1);
                            tkts = tkts.Where(a => a.CreateDate >= first && a.CreateDate <= last).ToList();
                            break;
                        case 4:
                            tkts = tkts.Where(a => a.CreateDate >= DateTime.Now.AddYears(-1)).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }

            if (tkts != null)
                tickets.AddRange(tkts.Select(a => new TicketRow()
                {
                    Id = a.Id,
                    Key = a.Key,
                    Subject = a.Subject,
                    ClientName = a.Name,
                    Level = getLevelName((StayLive.areas.Levels.Models.LevelInfo.LevelOrder)Enum.ToObject(typeof(StayLive.areas.Levels.Models.LevelInfo.LevelOrder), a.Level)),
                    CreateDate = a.CreateDate.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    Status = ((StayLive.areas.Tickets.Models.TicketInfo.TicketStatus)Enum.ToObject(typeof(StayLive.areas.Tickets.Models.TicketInfo.TicketStatus), a.Status)).ToString()
                }));
            return tickets;
        }

        private List<TicketReplyRow> getTicketReplies(int Id)
        {
            List<TicketReplyRow> replies = new List<TicketReplyRow>();
            var ticket = dbService.Tickets.Find(Id);
            if (ticket == null)
                return null;

            //The Main Message
            replies.Add(new TicketReplyRow()
            {
                Id = ticket.Id,
                Status = StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Pending.ToString(),
                hasAttachment = (ticket.Attachment != null),
                CreatedDate = ticket.CreateDate.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                isReverse = false,
                User = ticket.Name,
                UserId = 0,
                Message = (ticket.Message == null ? "" : ticket.Message.Replace("\r\n", "<br />")),
                AttachmentFileName = ticket.AttachmentFileName,
                isTicket = true
            });

            if (ticket.TicketReplies != null)
                replies.AddRange(ticket.TicketReplies.OrderBy(a => a.CreateDate).Select(a => new TicketReplyRow()
                {
                    Id = a.Id,
                    Status = ((StayLive.areas.Tickets.Models.TicketInfo.TicketStatus)Enum.ToObject(typeof(StayLive.areas.Tickets.Models.TicketInfo.TicketStatus), a.Status)).ToString(),
                    hasAttachment = (a.Attachment != null),
                    CreatedDate = a.CreateDate.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    isReverse = (a.CreateByUserId.HasValue ? (a.CreateByUserId.Value == SessionHelper.AccountId) : false),
                    User = (a.CreateByUserId.HasValue ? a.User.Name : ticket.Name),
                    UserId = a.CreateByUserId,
                    Message = a.Message.Replace("\r\n", "<br />"),
                    isInternal = a.IsInternal,
                    AttachmentFileName = a.AttachmentFileName
                }));

            return replies;
        }

        private List<SelectListItem> getUsers(int AssignedUserId)
        {
            List<SelectListItem> users = new List<SelectListItem>();
            var usr = dbService.Users.Where(a => a.CompanyId == SessionHelper.CompanyId).ToList();
            if (usr != null)
                users.AddRange(usr.Select(a => new SelectListItem()
                {
                    Text = a.Name,
                    Value = a.Id.ToString(),
                    Selected = (AssignedUserId == a.Id)
                }));
            return users;
        }

        private string getLevelName(StayLive.areas.Levels.Models.LevelInfo.LevelOrder levelType)
        {
            var level = dbService.Levels.Where(a => a.CompanyId == SessionHelper.CompanyId).FirstOrDefault();
            if (level != null)
            {
                switch (levelType)
                {
                    case StayLive.areas.Levels.Models.LevelInfo.LevelOrder.First:
                        return level.FirstName;
                    case StayLive.areas.Levels.Models.LevelInfo.LevelOrder.Second:
                        return level.SecondName;
                    default:
                        return level.ThirdName;
                }
            }
            return "";
        }

        private bool SendEmail(Ticket ticket, TicketReply reply)
        {
            var company = dbService.Companies.Find(SessionHelper.CompanyId);
            if (company == null)
                return false;
            try
            {
                var email = new MailMessage();
                email.To.Add(new MailAddress(ticket.Email));
                email.From = new MailAddress(SessionHelper.AccountEmail);
                email.Subject = "Ticket " + ticket.Key + ": " + ticket.Subject;
                email.Body = CreateEmailBody(ticket, reply);

                if (reply.Attachment != null)
                {
                    MemoryStream Attachment = new MemoryStream(reply.Attachment);
                    email.Attachments.Add(new Attachment(Attachment, reply.AttachmentFileName));
                }
                email.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = company.EmailAddress,
                        Password = company.EmailPassword
                    };
                    smtp.Credentials = credential;
                    smtp.Host = company.SmtpAddress;
                    smtp.Port = company.SmtpPort;
                    smtp.EnableSsl = company.EnableSsl;
                    smtp.Send(email);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string CreateEmailBody(Ticket ticket, TicketReply reply)
        {
            string body;

            string oldreplies = "";
            foreach (var item in ticket.TicketReplies.OrderBy(a => a.Id))
            {
                if (item.User != null)
                {
                    oldreplies = oldreplies + string.Format("<strong style='font-size:15px'>{0}:</strong><span style='font-size: 11.3px;color: #1e88e5'> on {1}</span><br/>", item.User.Name, item.CreateDate.Value.ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    oldreplies = oldreplies + string.Format("<strong style='font-size:15px'>{0}:</strong><span style='font-size: 11.3px;color: #1e88e5'> on {1}</span><br/>", "You", item.CreateDate.Value.ToString("yyyy/MM/dd HH:mm:ss"));
                }

                oldreplies = oldreplies + item.Message.Replace("\r\n", "<br />") + "<br/>";
                oldreplies = oldreplies + "<div style='margin-top:2px'>";
                switch ((StayLive.areas.Tickets.Models.TicketInfo.TicketStatus)Enum.ToObject(typeof(StayLive.areas.Tickets.Models.TicketInfo.TicketStatus), item.Status))
                {
                    case areas.Tickets.Models.TicketInfo.TicketStatus.Opened:
                        oldreplies = oldreplies + string.Format("<span style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#40ACDA;'>Opened</span>"); break;
                    case areas.Tickets.Models.TicketInfo.TicketStatus.Pending:
                        oldreplies = oldreplies + string.Format("<span style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#EEA733;'>Pending</span>"); break;
                    case areas.Tickets.Models.TicketInfo.TicketStatus.Completed:
                        oldreplies = oldreplies + string.Format("<span style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#1CA650;'>Completed</span>"); break;
                    case areas.Tickets.Models.TicketInfo.TicketStatus.Spam:
                        oldreplies = oldreplies + string.Format("<span style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#FF4D4D;'>Spam</span>"); break;
                    case areas.Tickets.Models.TicketInfo.TicketStatus.Deleted:
                        oldreplies = oldreplies + string.Format("<span style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#D9283A;'>Deleted</span>"); break;
                    case areas.Tickets.Models.TicketInfo.TicketStatus.Duplicated:
                        oldreplies = oldreplies + string.Format("<span style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#8155A2;'>Duplicated</span>"); break;
                    default:
                        oldreplies = oldreplies + string.Format("<span style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#808080;'>N/A</span>"); break;
                }
                if (item.Attachment != null)
                {
                    oldreplies = oldreplies + string.Format("<span data-toggle='tooltip'title='Attachment' style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#EE7A53;margin-left:3px'>{0}</span>", item.AttachmentFileName);
                }
                oldreplies = oldreplies + "<hr style='border: 0;height: 1px;background-image: -webkit-linear-gradient(left, #f0f0f0, #8c8b8b, #f0f0f0);background-image: -moz-linear-gradient(left, #f0f0f0, #8c8b8b, #f0f0f0);background-image: -ms-linear-gradient(left, #f0f0f0, #8c8b8b, #f0f0f0);background-image: -o-linear-gradient(left, #f0f0f0, #8c8b8b, #f0f0f0);'></hr>";
                oldreplies = oldreplies + "</div>";
                oldreplies = oldreplies + "<br/>";
            }

            using (var sr = new StreamReader(Server.MapPath("~/App_Data/Template/BodyTemplate.xml")))
            {
                body = sr.ReadToEnd();
            }
            body = body.Replace("{Subject}", ticket.Subject);
            body = body.Replace("{Message}", ticket.Message.Replace("\r\n", "<br />"));
            body = body.Replace("{User}", SessionHelper.AccountName);
            body = body.Replace("{CreateDate}", reply.CreateDate.Value.ToString("yyyy/MM/dd HH:mm:ss"));
            body = body.Replace("{ReplyMessage}", reply.Message.Replace("\r\n", "<br />"));
            body = body.Replace("{OldReplies}", oldreplies);

            string status = "<div style='margin-top:2px'>";
            switch ((StayLive.areas.Tickets.Models.TicketInfo.TicketStatus)Enum.ToObject(typeof(StayLive.areas.Tickets.Models.TicketInfo.TicketStatus), ticket.Status))
            {
                case areas.Tickets.Models.TicketInfo.TicketStatus.Opened:
                    status = status + string.Format("<span style='background-color:#40ACDA;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Opened</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Pending:
                    status = status + string.Format("<span style='background-color:#EEA733;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Pending</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Completed:
                    status = status + string.Format("<span style='background-color:#1CA650;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Completed</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Spam:
                    status = status + string.Format("<span style='background-color:#FF4D4D;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Spam</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Deleted:
                    status = status + string.Format("<span style='background-color:#D9283A;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Deleted</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Duplicated:
                    status = status + string.Format("<span style='background-color:#8155A2;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Duplicated</span>"); break;
                default:
                    status = status + string.Format("<span style='background-color:#808080;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>N/A</span>"); break;
            }
            if (ticket.Attachment != null)
            {
                status = status + string.Format("<span data-toggle='tooltip'title='Attachment' style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#EE7A53;margin-left:3px'>{0}</span>", ticket.AttachmentFileName);
            }
            status = status + "</div>";

            body = body.Replace("{Status}", status);

            string statusreply = "<div style='margin-top:2px'>";
            switch ((StayLive.areas.Tickets.Models.TicketInfo.TicketStatus)Enum.ToObject(typeof(StayLive.areas.Tickets.Models.TicketInfo.TicketStatus), reply.Status))
            {
                case areas.Tickets.Models.TicketInfo.TicketStatus.Opened:
                    statusreply = statusreply + string.Format("<span style='background-color:#40ACDA;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Opened</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Pending:
                    statusreply = statusreply + string.Format("<span style='background-color:#EEA733;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Pending</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Completed:
                    statusreply = statusreply + string.Format("<span style='background-color:#1CA650;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Completed</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Spam:
                    statusreply = statusreply + string.Format("<span style='background-color:#FF4D4D;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Spam</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Deleted:
                    statusreply = statusreply + string.Format("<span style='background-color:#D9283A;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Deleted</span>"); break;
                case areas.Tickets.Models.TicketInfo.TicketStatus.Duplicated:
                    statusreply = statusreply + string.Format("<span style='background-color:#8155A2;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>Duplicated</span>"); break;
                default:
                    statusreply = statusreply + string.Format("<span style='background-color:#808080;color:white;border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;'>N/A</span>"); break;
            }
            if (reply.Attachment != null)
            {
                statusreply = statusreply + string.Format("<span data-toggle='tooltip'title='Attachment' style='border-radius: .25rem; text-shadow: none; font-size: 11px; font-weight: normal; padding: 3px 5px 3px;color:white;background-color:#EE7A53;margin-left:3px'>{0}</span>", reply.AttachmentFileName);
            }
            statusreply = statusreply + "</div>";
            body = body.Replace("{StatusReply}", statusreply);

            return body;
        }

        private TicketsTableVM FillTicketFilter()
        {
            TicketsTableVM vm = new TicketsTableVM();
            vm.User = SessionHelper.AccountId.ToString();
            var users = dbService.Users.Where(a => a.CompanyId == SessionHelper.CompanyId).ToList();
            vm.UsersList.AddRange(users.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }));
            var level = dbService.Levels.Where(a => a.CompanyId == SessionHelper.CompanyId).FirstOrDefault();
            vm.LevelList.Add(new SelectListItem()
            {
                Value = "1",
                Text = level.FirstName
            });
            vm.LevelList.Add(new SelectListItem()
            {
                Value = "2",
                Text = level.SecondName
            });
            vm.LevelList.Add(new SelectListItem()
            {
                Value = "3",
                Text = level.ThirdName
            });
            return vm;
        }
        #endregion

        #region temp
        public ActionResult NewTicket()
        {
            StayLive.Models.Ticket ticket = new Models.Ticket()
            {
                Key = "33G355",
                Name = "Moahamed Kabbani",
                Email = "kabbani13666@gmail.com",
                AssignedUserId = 4,
                CompanyId = 1,
                Status = 1,
                Level = 1,
                Message = "",
                Subject = "Fixing Style Issue",
                Mobile = "+936999999999",
                CreateDate = DateTime.Now
            };
            dbService.Tickets.Add(ticket);
            dbService.SaveChanges();
            return RedirectToAction("", "Ticket", new { area = "Tickets" });
        }
        #endregion
    }
}
