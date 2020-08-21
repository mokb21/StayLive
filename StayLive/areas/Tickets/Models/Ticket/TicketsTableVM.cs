using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StayLive.areas.Tickets.Models.Ticket
{
    public class TicketsTableVM
    {
        public string Level { get; set; }
        public string User { get; set; }
        public int Status{get;set;}
        public string Date { get; set; }

        public List<SelectListItem> UsersList = new List<SelectListItem>();
        public List<SelectListItem> LevelList = new List<SelectListItem>();
        public List<SelectListItem> DateList = new List<SelectListItem>();

        public TicketsTableVM()
        {
            DateList.Add(new SelectListItem() { Value = "0", Text = Resources.General.All });
            DateList.Add(new SelectListItem() { Value = "1", Text = Resources.General.Yesterday });
            DateList.Add(new SelectListItem() { Value = "2", Text = Resources.General.LastWeek });
            DateList.Add(new SelectListItem() { Value = "3", Text = Resources.General.LastMonth });
            DateList.Add(new SelectListItem() { Value = "3", Text = Resources.General.LastYear });
            Date = "0";

            LevelList.Add(new SelectListItem() { Value = "0", Text = Resources.General.All });
            Level = "0";

            Status = 0;

            UsersList.Add(new SelectListItem() { Value = "0", Text = Resources.General.All });
        }
    }
}