using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.areas.Tickets.Models.TicketReply
{
    public class TicketReplyRow
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public string CreatedDate { get; set; }
        public int? UserId { get; set; }
        public bool isReverse { get; set; }
        public bool isInternal { get; set; }
        public bool isTicket { get; set; }
        public bool hasAttachment { get; set; }
        public string AttachmentFileName { get; set; }
    }
}