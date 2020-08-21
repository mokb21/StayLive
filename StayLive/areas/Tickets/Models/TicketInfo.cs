using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.areas.Tickets.Models
{
    public class TicketInfo
    {
        public enum TicketStatus
        {
            Pending = 1,
            Opened = 2,
            Completed = 3,
            Spam = 4,
            Deleted = 5,
            Duplicated = 6
        }

        public class TicketColor
        {

            private TicketColor() { }

            public const string Pending = "#EEA733";
            public const string Opened = "#40ACDA";
            public const string Completed = "#1CA650";
            public const string Spam = "#FF4D4D";
            public const string Deleted = "#D9283A";
            public const string Duplicated = "#8155A2";
        }


    }
}