using StayLive.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StayLive.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 6, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordLength")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*?[0-9])[A-Za-z0-9^<>.,?;:'()!~%\-_@#/*\$""]{2,}$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordWrongFormat")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 6, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordLength")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*?[0-9])[A-Za-z0-9^<>.,?;:'()!~%\-_@#/*\$""]{2,}$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordWrongFormat")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Compare("Password", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }
    }
}