using System.Web.Mvc;

namespace StayLive.Areas.Tickets
{
    public class TicketsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Tickets";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Tickets_default",
                "Tickets/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
