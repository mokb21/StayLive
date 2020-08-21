using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StayLive.Resources;

namespace StayLive.areas.Companies.Models.Company
{
    public class CompanyVM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Remote("IsValidName", "Company", AdditionalFields = "Id")]
        public string Name { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddress")]
        public string Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddress")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 6, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordLength")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*?[0-9])[A-Za-z0-9^<>.,?;:'()!~%\-_@#/*\$""]{2,}$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordWrongFormat")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int? SmtpPort { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string SmtpAdderss { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int? Pop3Port { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Pop3Address { get; set; }
        public bool EnableSSL { get; set; }
        public string Region { get; set; }

        public List<System.Web.Mvc.SelectListItem> RegionsList = new List<System.Web.Mvc.SelectListItem>();
    }
}