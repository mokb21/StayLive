using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using StayLive.Resources;

namespace StayLive.Models.Login
{
    public class LoginVM
    {
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }
        public string Back { get; set; }
        public bool RememberMe { get; set; }
    }
}