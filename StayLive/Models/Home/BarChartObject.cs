using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.Models.Home
{
    public class BarChartObject
    {
        public List<BarChartStroke> datasets = new List<BarChartStroke>();
        public List<string> labels = new List<string>();
    }

    public class BarChartStroke
    {
        public List<int> data = new List<int>();
        public string fillColor { get; set; }
        public string highlightFill { get; set; }
        public string strokeColor { get; set; }
        public string highlightStroke { get; set; }
        public string label { get; set; }
    }
}