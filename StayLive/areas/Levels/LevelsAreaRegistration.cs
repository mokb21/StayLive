using System.Web.Mvc;

namespace StayLive.Areas.Levels
{
    public class LevelsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Levels";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Levels_default",
                "Levels/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
