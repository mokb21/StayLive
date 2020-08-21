using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StayLiveServices.Model;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using OpenPop.Mime;

namespace StayLiveServices.Emails
{
    class Email
    {
        StayLiveEntities db = new StayLiveEntities();

        public void GetEmails(int uid, string cutOn, Company Company)
        {
            try
            {

                using (Pop3Client client = new Pop3Client())
                {
                    TicketReply ticketReply = new TicketReply();

                    client.Connect(Company.Pop3Address, Convert.ToInt32(Company.Pop3Port), (bool)Company.EnableSsl);
                    client.Authenticate(Company.EmailAddress, Company.EmailPassword);
                    MessageHeader headers = client.GetMessageHeaders(uid);
                    var ticket = db.Tickets.Where(a => "RE: Ticket " + a.Key + ": " + a.Subject == headers.Subject || a.Subject == headers.Subject).FirstOrDefault();
                    if (ticket != null)
                    {

                        ticketReply.TicketId = ticket.Id;
                        ticketReply.Status = 2;
                        ticketReply.IsInternal = false;
                        ticketReply.CreateByUserId = null;

                        Message message = client.GetMessage(uid);
                        var messagebody = message.FindFirstPlainTextVersion().GetBodyAsText();
                        var CutOn = messagebody.IndexOf(cutOn);
                        ticketReply.Message = messagebody.Substring(0, CutOn);
                        ticketReply.CreateDate = headers.DateSent.ToLocalTime();
                        foreach (MessagePart attachment in message.FindAllAttachments())
                        {
                            if (attachment.Body != null)
                            {
                                ticketReply.Attachment = attachment.Body;
                                ticketReply.AttachmentFileName = attachment.FileName;
                            }
                        }
                        db.TicketReplies.Add(ticketReply);
                        db.SaveChanges();
                        client.DeleteMessage(uid);
                    }

                }
            }
            catch (Exception)
            {
            }
        }
    }
}