using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.areas.Tickets.Models.Ticket
{
    public class TicketsTableFilter
    {
        public int Status { get; set; }
        public int User { get; set; }
        public int Level { get; set; }
        public int Date { get; set; }
    }
}