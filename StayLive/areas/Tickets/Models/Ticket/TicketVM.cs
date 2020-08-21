using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StayLive.areas.Tickets.Models.TicketReply;
using System.Web.Mvc;

namespace StayLive.areas.Tickets.Models.Ticket
{
    public class TicketVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string TicketStatus { get; set; }
        public int TicketStatusValue { get; set; }
        public string CreatedDate { get; set; }
        public string LastUpdate { get; set; }
        public string AssignedTo { get; set; }
        public int AssignedToValue { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool isInternall { get; set; }
        public string Message { get; set; }
        public bool hasAttachment { get; set; }
        public List<SelectListItem> TicketStatusList = new List<SelectListItem>();
        public List<SelectListItem> UsersList = new List<SelectListItem>();
        public List<TicketReplyRow> Replies = new List<TicketReplyRow>();

        public TicketVM()
        {
            TicketStatusList.Add(new SelectListItem() { Value = TicketInfo.TicketStatus.Pending.GetHashCode().ToString(), Text = TicketInfo.TicketStatus.Pending.ToString() });
            TicketStatusList.Add(new SelectListItem() { Value = TicketInfo.TicketStatus.Opened.GetHashCode().ToString(), Text = TicketInfo.TicketStatus.Opened.ToString() });
            TicketStatusList.Add(new SelectListItem() { Value = TicketInfo.TicketStatus.Completed.GetHashCode().ToString(), Text = TicketInfo.TicketStatus.Completed.ToString() });
            TicketStatusList.Add(new SelectListItem() { Value = TicketInfo.TicketStatus.Duplicated.GetHashCode().ToString(), Text = TicketInfo.TicketStatus.Duplicated.ToString() });
        }
    }
}