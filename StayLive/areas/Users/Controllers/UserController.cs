using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using StayLive.Controllers;
using StayLive.Helpers;
using StayLive.areas.Users.Models;
using StayLive.areas.Users.Models.User;

namespace StayLive.Areas.Users.Controllers
{
    public class UserController : BaseController
    {
        #region Actions
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin, Role2 = areas.Users.Models.UserRoles.Admin)]
        public ActionResult Index()
        {
            return View();
        }

        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin, Role2 = areas.Users.Models.UserRoles.Admin)]
        public ActionResult UserProfile(int Id = 0)
        {
            if (Id > 0 && !IsUserExist(Id))
                return this.NotFound();

            UserVM vm = new UserVM();
            vm = FillUserDetails(Id);
            return View(vm);
        }

        [HttpPost]
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin, Role2 = areas.Users.Models.UserRoles.Admin)]
        public ActionResult UserProfile(UserVM vm, HttpPostedFileBase Photo)
        {
            int Id = 0;
            var user = new StayLive.Models.User();
            if (vm.Id >= 0)
            {
                user.Name = vm.Name;
                user.Email = vm.Email;
                user.UserName = vm.UserName;
                user.Password = ControllersExtensions.HashPassword(vm.Password);
                user.Role = byte.Parse(vm.Role);
                user.Mobile = vm.Mobile;

                if (vm.Level != null && (byte.Parse(vm.Level) != (byte)StayLive.areas.Users.Models.UserRoles.Admin 
                    || byte.Parse(vm.Level) != (byte)StayLive.areas.Users.Models.UserRoles.SystemAdmin))
                    user.Level = byte.Parse(vm.Level);
                else
                    user.Level = null;

                if (SessionHelper.AccountRole == (byte)StayLive.areas.Users.Models.UserRoles.Admin)
                    user.CompanyId = SessionHelper.CompanyId.Value;
                else
                    user.CompanyId = int.Parse(vm.Company);

                if (Photo != null)
                {
                    string extension = System.IO.Path.GetExtension(Photo.FileName);
                    if (ControllersExtensions.IsValidImage(Photo))
                        user.ProfilePhoto = ControllersExtensions.GetImageBytes(Photo);
                    else
                    {
                        this.MsgError(StayLive.Resources.General.SaveUser, StayLive.Resources.Validations.InvalidImageFormat);
                        return View(vm);
                    }
                }
                //Create User
                if (vm.Id == 0)
                    Id = AddNewUser(user);
                //Edit User
                else
                {
                    user.Id = vm.Id;
                    Id = EditUser(user);
                }
            }

            if (Id == -1)
            {
                this.MsgError(StayLive.Resources.General.SaveUser, StayLive.Resources.General.SomethingWentWorng);
                vm.CompaniesList = FillCompaniesList();
                vm.RolesList = FillRolesList((UserRoles)SessionHelper.AccountRole);
                vm.LevelsList = FillLevelsList();
                return View(vm);
            }
            else
            {
                this.MsgSavedSuccessfuly();
                return RedirectToAction("UserProfile", new { Id = Id });
            }

        }

        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin, Role2 = areas.Users.Models.UserRoles.Admin)]
        public ActionResult Delete(int Id)
        {
            if (!IsUserExist(Id))
                return View("~/Views/Shared/NotFound.chtml");
            var user = dbService.Users.Find(Id);
            var tickets = dbService.Tickets.Where(a => a.UpdateUserId == user.Id || a.AssignedUserId == user.Id).ToList();
            foreach (var ticket in tickets)
            {
                if (ticket.AssignedUserId == user.Id)
                {
                    ticket.AssignedUserId = null;
                }
                else if (ticket.UpdateUserId == user.Id)
                {
                    ticket.UpdateUserId = null;
                }
            }
            dbService.Users.Remove(user);
            dbService.SaveChanges();
            this.MsgDeleteSuccessfuly();
            return RedirectToAction("Index");
        }

        #region password
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin, Role2 = areas.Users.Models.UserRoles.Admin)]
        public ActionResult ResetPassword(int Id)
        {
            var user = dbService.Users.Find(Id);
            if (user == null)
                return this.NotFound();

            StayLive.Models.SaveModel vm = new Models.SaveModel()
            {
                title = StayLive.Resources.General.ResetPassword + " (" + user.Name + ")",
                action = "ResetPassword",
                controller = "User",
                area = "Users",
                isAjax = false,
                view = "~/Views/Shared/_resetPassword.cshtml",
                model = new Models.ResetPassword()
                {
                    Id = user.Id
                }
            };
            return Json(new { data = this.RenderPartialToString("~/Views/Shared/_SaveModal.cshtml", vm) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin, Role2 = areas.Users.Models.UserRoles.Admin)]
        public ActionResult ResetPassword(Models.ResetPassword vm)
        {
            var user = dbService.Users.Find(vm.Id);
            if (user == null)
                return this.NotFound();

            user.Password = ControllersExtensions.HashPassword(vm.Password);
            dbService.SaveChanges();

            this.MsgSavedSuccessfuly();
            return RedirectToAction("Index");
        }
        #endregion

        #endregion

        #region Getter Methodes
        [HttpPost]
        public ActionResult getUsersData()
        {
            List<UserRow> data;
            int TotalCount = 0;

            var draw = Request.Form.GetValues("draw").FirstOrDefault();//page number
            int start = int.Parse(Request.Form.GetValues("start").First());//use it to skip
            int length = int.Parse(Request.Form.GetValues("length").First());//count to show

            string sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            string sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();//sort direction 
            string searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


            Expression<Func<UserRow, bool>> filter = (a) => (a.Name.ToLower().Contains(searchValue) || (a.Company.ToLower()).Contains(searchValue) ||
                (a.Email.ToLower()).Contains(searchValue) || (a.Mobile).Contains(searchValue) || (a.Role.ToLower()).Contains(searchValue));

            List<UserRow> lstUsers = getUsersRow().AsQueryable().Where(filter).ToList();

            TotalCount = lstUsers.Count;
            data = lstUsers.AsQueryable().Select(a => a).OrderBy(sortColumn + " " + sortColumnDir)
            .Skip(start)
            .Take(length)
            .ToList();

            return Json(new { draw = draw, recordsFiltered = TotalCount, recordsTotal = TotalCount, data = data }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Validation
        public JsonResult IsValidUserName(string UserName, int Id)
        {
            try
            {
                var user = dbService.Users.Where(a => a.UserName.ToLower() == UserName.ToLower()).FirstOrDefault();
                if (user != null && user.Id != Id)
                    return Json(StayLive.Resources.Validations.Unique, JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { }

            return Json(StayLive.Resources.Validations.InvalidUserName, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methodes
        private bool IsUserExist(int Id)
        {
            if (dbService.Users.Find(Id) != null)
                return true;
            else
                return false;
        }

        private List<UserRow> getUsersRow()
        {
            List<UserRow> users = new List<UserRow>();
            List<StayLive.Models.User> userslst = new List<Models.User>();
            if (SessionHelper.AccountRole == (byte)StayLive.areas.Users.Models.UserRoles.SystemAdmin)
            {
                userslst = dbService.Users.Where(a => a.Role == (byte)UserRoles.SystemAdmin
                || a.Role == (byte)UserRoles.Admin).ToList();
            }
            else
            {
                userslst = dbService.Users.Where(a => a.CompanyId == SessionHelper.CompanyId).ToList();
            }

            if (userslst != null)
            {
                users.AddRange(userslst.Select(a => new UserRow()
                {
                    Id = a.Id,
                    Email = a.Email,
                    Name = a.Name,
                    Role = ((UserRoles)a.Role).ToString(),
                    Mobile = a.Mobile,
                    Company = (a.CompanyId.HasValue ? a.Company.Name : "")
                }));
            }
            
            return users;
        }

        private UserVM FillUserDetails(int Id)
        {
            UserVM vm = new UserVM((UserRoles)SessionHelper.AccountRole.Value);
            if (Id != 0)
            {
                var user = dbService.Users.Find(Id);
                if (user != null)
                {
                    vm.Id = user.Id;
                    vm.Name = user.Name;
                    vm.UserName = user.UserName;
                    vm.Email = user.Email;
                    vm.Company = user.CompanyId.ToString();
                    vm.Level = user.Level.ToString();
                    vm.Mobile = user.Mobile;
                    vm.Role = user.Role.ToString();
                }
            }
            vm.LevelsList = FillLevelsList();
            vm.CompaniesList = FillCompaniesList();
            return vm;
        }

        private List<SelectListItem> FillLevelsList()
        {
            List<SelectListItem> levels = new List<SelectListItem>();
            var level = dbService.Levels.Where(a => a.CompanyId == SessionHelper.CompanyId.Value).FirstOrDefault();
            if (level == null)
                return null;
            levels.Add(new SelectListItem() { Value = ((byte)StayLive.areas.Levels.Models.LevelInfo.LevelOrder.First).ToString(), Text = level.FirstName });
            levels.Add(new SelectListItem() { Value = ((byte)StayLive.areas.Levels.Models.LevelInfo.LevelOrder.Second).ToString(), Text = level.SecondName });
            levels.Add(new SelectListItem() { Value = ((byte)StayLive.areas.Levels.Models.LevelInfo.LevelOrder.Third).ToString(), Text = level.ThirdName });
            return levels;
        }

        private List<SelectListItem> FillCompaniesList()
        {
            List<SelectListItem> companieslst = new List<SelectListItem>();
            var companies = dbService.Companies.ToList();
            if (companies != null)
            {
                companieslst.AddRange(companies.Select(a => new SelectListItem()
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }));
            }
            return companieslst;
        }

        private List<SelectListItem> FillRolesList(UserRoles role)
        {
            List<SelectListItem> RolesList = new List<SelectListItem>();
            if (role == UserRoles.SystemAdmin)
            {
                RolesList.Add(new SelectListItem()
                {
                    Value = ((int)UserRoles.SystemAdmin).ToString(),
                    Text = (UserRoles.SystemAdmin).ToString()
                });
            }
            RolesList.Add(new SelectListItem()
            {
                Value = ((int)UserRoles.Admin).ToString(),
                Text = (UserRoles.Admin).ToString()
            });
            RolesList.Add(new SelectListItem()
            {
                Value = ((int)UserRoles.Supervisor).ToString(),
                Text = (UserRoles.Supervisor).ToString()
            });
            RolesList.Add(new SelectListItem()
            {
                Value = ((int)UserRoles.Agent).ToString(),
                Text = (UserRoles.Agent).ToString()
            });
            return RolesList;
        }

        private int AddNewUser(StayLive.Models.User user)
        {
            try
            {
                dbService.Users.Add(user);
                dbService.SaveChanges();
                return user.Id;
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        private int EditUser(StayLive.Models.User user)
        {
            try
            {
                var usr = dbService.Users.Find(user.Id);
                if (usr != null)
                {
                    usr.Name = user.Name;
                    usr.UserName = user.UserName;
                    usr.Email = user.Email;
                    usr.CompanyId = user.CompanyId;
                    usr.Mobile = user.Mobile;
                    usr.Role = user.Role;
                    usr.Level = user.Level;
                    if (user.ProfilePhoto != null)
                        usr.ProfilePhoto = user.ProfilePhoto;

                    dbService.SaveChanges();
                    return user.Id;
                }
                return -1;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }
        #endregion
    }
}