//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StayLive.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Level
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public int FirstHours { get; set; }
        public string SecondName { get; set; }
        public int SecondHours { get; set; }
        public string ThirdName { get; set; }
        public int CompanyId { get; set; }
    
        public virtual Company Company { get; set; }
    }
}
