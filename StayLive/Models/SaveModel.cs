using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.Models
{
    public class SaveModel
    {
        public string title { get; set; }
        public string action { get; set; }
        public string controller { get; set; }
        public string area { get; set; }
        public int? id { get; set; }
        public int? id1 { get; set; }
        public string view { get; set; }
        public object model { get; set; }
        public bool isAjax { get; set; }
        public string onCompleteFunction { get; set; }
        public string onSuccessFunction { get; set; }
        public string onFailureFunction { get; set; }
        public bool isLarge { get; set; }
    }
}