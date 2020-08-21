using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.Models.Home
{
    public class HomeVM
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int RoleValue { get; set; }
        public int AssigedTo { get; set; }
        public int LastUpdated { get; set; }
        public int Solved { get; set; }
        public int All { get; set; }
        public int Opened { get; set; }
        public int Pending { get; set; }
        public int Completed { get; set; }
        public int Duplicated { get; set; }
        public int Spam { get; set; }
        public int AllAssignedTickets { get; set; }
    }
}