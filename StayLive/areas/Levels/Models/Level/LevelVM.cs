using StayLive.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StayLive.areas.Levels.Level.Models
{
    public class LevelVM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string First { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Second { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Third { get; set; }
        public int FirstTime { get; set; }
        public int SecondTime { get; set; }
        public int? ThirdTime { get; set; }
    }
}