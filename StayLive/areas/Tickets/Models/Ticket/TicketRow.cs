using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.areas.Tickets.Models.Ticket
{
    public class TicketRow
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Subject { get; set; }
        public string ClientName { get; set; }
        public string CreateDate { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
    }
}