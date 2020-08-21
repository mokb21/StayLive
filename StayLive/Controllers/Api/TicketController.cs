using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StayLive.Models;


namespace StayLive.Controllers.Api
{
    public class TicketController : ApiController
    {
        private StayLiveEntities dbService = new StayLiveEntities();
        private static Random random = new Random();
        private static Random randomnumber = new Random();

        [HttpPost]
        public Task<IHttpActionResult> AddNewTicket([FromBody]object ticket)
        {
            if (ticket != null)
            {
                return Task.Run<IHttpActionResult>(() =>
                {
                    try
                    {
                        var item = JsonConvert.SerializeObject(ticket, typeof(Ticket),
                            new JsonSerializerSettings());
                        JObject jItem = JObject.Parse(item.ToLower());

                        var newTicket = (Ticket)JsonConvert.DeserializeObject(item, typeof(Ticket));
                        newTicket.Key = RandomKey();
                        newTicket.Status = (byte)StayLive.areas.Tickets.Models.TicketInfo.TicketStatus.Pending;
                        newTicket.Level = 1;
                        newTicket.CreateDate = DateTime.Now;
                        dbService.Tickets.Add(newTicket);
                        
                        return Content(HttpStatusCode.OK, newTicket.Key);
                    }
                    catch (Exception ex)
                    {
                        return new BadRequestResult(this);
                    }
                });
            }
            else
            {
                return null;
            }
        }

        private static string RandomKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = randomnumber.Next(6, 11);
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                dbService.SaveChanges();
                dbService.Dispose();
            }
            catch (Exception ex)
            {
            }

        }
    }
}