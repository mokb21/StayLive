using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.IO;
using System.Net;
using StayLive.areas.Companies.Models.Company;
using StayLive.Controllers;
using StayLive.Helpers;

namespace StayLive.Areas.Companies.Controllers
{
    public class CompanyController : BaseController
    {
        #region Actions
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin)]
        public ActionResult Index()
        {
            return View();
        }

        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin)]
        public ActionResult Company(int Id = 0)
        {
            if (Id > 0 && !IsCompanyExist(Id))
                return this.NotFound();

            CompanyVM vm = new CompanyVM();
            vm = FillCompanyDetails(Id);
            return View(vm);
        }

        [HttpPost]
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin)]
        public ActionResult Company(CompanyVM vm, HttpPostedFileBase Logo)
        {
            int Id = 0;
            var company = new StayLive.Models.Company();
            if (vm.Id >= 0)
            {
                company.Name = vm.Name;
                company.Email = vm.Email;
                company.EmailAddress = vm.EmailAddress;
                company.EmailPassword = vm.Password;
                company.EnableSsl = vm.EnableSSL;
                company.Pop3Address = vm.Pop3Address;
                company.Pop3Port = (short)(vm.Pop3Port ?? 0);
                company.RegionId = int.Parse(vm.Region);
                company.SmtpAddress = vm.SmtpAdderss;
                company.SmtpPort = (short)(vm.SmtpPort ?? 0);
                if (Logo != null)
                {
                    string extension = System.IO.Path.GetExtension(Logo.FileName);
                    if (ControllersExtensions.IsValidImage(Logo))
                        company.Logo = ControllersExtensions.GetImageBytes(Logo);
                    else
                    {
                        this.MsgError(StayLive.Resources.General.SaveCompany, StayLive.Resources.Validations.InvalidImageFormat);
                        return View(vm);
                    }
                }
                //Create Company
                if (vm.Id == 0)
                    Id = AddNewCompany(company);
                //Edit Company
                else
                {
                    company.Id = vm.Id;
                    Id = EditCompany(company);
                }
            }

            if (Id == -1)
            {
                this.MsgError(StayLive.Resources.General.SaveCompany, StayLive.Resources.General.SomethingWentWorng);
                vm.RegionsList = FillRegionList();
                return View(vm);
            }
            else
            {
                SendEmail(Id);
                this.MsgSavedSuccessfuly();
                return RedirectToAction("Company", new { Id = Id });
            }
            
        }

        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin)]
        public ActionResult Delete(int Id)
        {
            if (!IsCompanyExist(Id))
                return View("~/Views/Shared/NotFound.chtml");
            var company = dbService.Companies.Find(Id);
            //Remove Users
            var usres = dbService.Users.Where(a => a.CompanyId == company.Id).ToList();
            foreach (var user in usres)
                dbService.Users.Remove(user);
            //Remove Levels
            var levels = dbService.Levels.Where(a => a.CompanyId == company.Id).FirstOrDefault();
            if (levels != null)
                dbService.Levels.Remove(levels);
            //Remove Company
            dbService.Companies.Remove(company);
            dbService.SaveChanges();
            this.MsgDeleteSuccessfuly();
            return RedirectToAction("Index");
        }

        #region password
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin)]
        public ActionResult ResetPassword(int Id)
        {
            var company = dbService.Companies.Find(Id);
            if (company == null)
                return this.NotFound();

            StayLive.Models.SaveModel vm = new Models.SaveModel()
            {
                title = StayLive.Resources.General.ResetPassword + " (" + company.Name + ")",
                action = "ResetPassword",
                controller = "Company",
                area = "Companies",
                isAjax = false,
                view = "~/Views/Shared/_resetPassword.cshtml",
                model = new Models.ResetPassword()
                {
                    Id = company.Id
                }
            };
            return Json(new { data = this.RenderPartialToString("~/Views/Shared/_SaveModal.cshtml", vm) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleFilter(Role = areas.Users.Models.UserRoles.SystemAdmin)]
        public ActionResult ResetPassword(Models.ResetPassword vm)
        {
            var company = dbService.Companies.Find(vm.Id);
            if (company == null)
                return this.NotFound();

            company.EmailPassword = vm.Password;
            dbService.SaveChanges();

            this.MsgSavedSuccessfuly();
            return RedirectToAction("Index");
        }
        #endregion
        #endregion

        #region Validation
        public JsonResult IsValidName(string Name, int Id)
        {
            try
            {
                var company = dbService.Companies.Where(a => a.Name.ToLower() == Name.ToLower()).FirstOrDefault();
                if (company != null && company.Id != Id)
                    return Json(StayLive.Resources.Validations.Unique, JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { }

            return Json(StayLive.Resources.Validations.InvalidName, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Getter Methodes
        [HttpPost]
        public ActionResult getCompaniesData()
        {
            List<CompanyRow> data = new List<CompanyRow>();
            data = getCompaniesRows();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methodes
        private List<CompanyRow> getCompaniesRows()
        {
            List<CompanyRow> companies = new List<CompanyRow>();
            var companiesLst = dbService.Companies.ToList();
            companies.AddRange(companiesLst.Select(a => new CompanyRow()
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                Region = a.Region.Name
            }));
            return companies;
        }

        private CompanyVM FillCompanyDetails(int Id)
        {
            CompanyVM vm = new CompanyVM();
            if (Id != 0)
            {
                var company = dbService.Companies.Find(Id);
                if (company != null)
                {
                    vm.Id = company.Id;
                    vm.Name = company.Name;
                    vm.Email = company.Email;
                    vm.EmailAddress = company.EmailAddress;
                    vm.EnableSSL = company.EnableSsl;
                    vm.Pop3Address = company.Pop3Address;
                    vm.Pop3Port = company.Pop3Port;
                    vm.Region = company.RegionId.ToString();
                    vm.SmtpAdderss = company.SmtpAddress;
                    vm.SmtpPort = company.SmtpPort;
                }
            }
            vm.RegionsList = FillRegionList();
            return vm;
        }

        private int AddNewCompany(StayLive.Models.Company company)
        {
            try
            {
                dbService.Companies.Add(company);

                //Adding Level
                StayLive.Models.Level level = new Models.Level();
                level.CompanyId = company.Id;
                level.FirstHours = 5;
                level.FirstName = "First";
                level.SecondHours = 8;
                level.SecondName = "Second";
                level.ThirdName = "Third";
                dbService.Levels.Add(level);

                dbService.SaveChanges();
                return company.Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
           
        }

        private int EditCompany (StayLive.Models.Company company)
        {
            try
            {
                var cmp = dbService.Companies.Find(company.Id);
                if (cmp != null)
                {
                    cmp.Name = company.Name;
                    cmp.Email = company.Email;
                    cmp.EmailAddress = company.EmailAddress;
                    cmp.EnableSsl = company.EnableSsl;
                    cmp.Pop3Port = company.Pop3Port;
                    cmp.Pop3Address = company.Pop3Address;
                    cmp.RegionId = company.RegionId;
                    cmp.SmtpPort = company.SmtpPort;
                    cmp.SmtpAddress = company.SmtpAddress;
                    if (company.Logo != null)
                        cmp.Logo = company.Logo;

                    dbService.SaveChanges();
                    return company.Id;
                }
                return -1;
            }
            catch (Exception ex)
            {

                return -1;
            } 
        }

        private bool IsCompanyExist(int Id)
        {
            if (dbService.Companies.Find(Id) != null)
                return true;
            else
                return false;
        }

        private List<SelectListItem> FillRegionList()
        {
            List<SelectListItem> RegionsList = new List<SelectListItem>();
            RegionsList.AddRange(dbService.Regions.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }));
            return RegionsList;
        }

        private bool SendEmail(int companyId)
        {
            var company = dbService.Companies.Find(companyId);
            if (company == null)
                return false;
            try
            {
                var email = new MailMessage();
                email.To.Add(new MailAddress(company.Email));
                email.From = new MailAddress("stayLive.emailservice@gmail.com");
                email.Subject = "Welcome to Stay Live";

                var body = "";
                using (var sr = new StreamReader(Server.MapPath("~/App_Data/Template/NewCompany.xml")))
                {
                    body = sr.ReadToEnd();
                }

                body = body.Replace("{Company}", company.Name);
                email.Body = body;
                email.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "stayLive.emailservice@gmail.com",
                        Password = "stay17471"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(email);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}