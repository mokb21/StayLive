using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.Models
{
    public class _DualStringInt
    {
        public int value { get; set; }
        public string text { get; set; }

        public _DualStringInt() { }
        public _DualStringInt(int value, string text)
        {
            this.value = value;
            this.text = text;
        }
    }
}