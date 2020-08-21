using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.areas.Users.Models.User
{
    public class UserRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
    }
}