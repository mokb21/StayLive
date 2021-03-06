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
    
    public partial class TicketReply
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Message { get; set; }
        public byte Status { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentFileName { get; set; }
        public bool IsInternal { get; set; }
        public Nullable<int> CreateByUserId { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual Ticket Ticket { get; set; }
        public virtual User User { get; set; }
    }
}
