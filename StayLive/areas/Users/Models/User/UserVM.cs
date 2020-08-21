using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StayLive.Resources;

namespace StayLive.areas.Users.Models.User
{
    public class UserVM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Remote("IsValidUserName", "User", AdditionalFields = "Id")]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Email { get; set; }
        [RegularExpression(@"^(\+|00)?\(?([0-9]{1,3})\)?[ ]?([0-9]{1,3})[ ]?([0-9]{4})[ ]?([0-9]{3})$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PhoneNumber")]
        public string Mobile { get; set; }
        public string Role { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 6, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordLength")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*?[0-9])[A-Za-z0-9^<>.,?;:'()!~%\-_@#/*\$""]{2,}$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordWrongFormat")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }
        public string Company { get; set; }
        public string Level { get; set; }

        public List<SelectListItem> CompaniesList = new List<SelectListItem>();
        public List<SelectListItem> RolesList = new List<SelectListItem>();
        public List<SelectListItem> LevelsList = new List<SelectListItem>();

        public UserVM() { }

        public UserVM(UserRoles role)
        {
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
        }
    }
}