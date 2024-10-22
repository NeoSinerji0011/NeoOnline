using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Maps
{
    public class MapsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Maps";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Maps_default",
                "Maps/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
