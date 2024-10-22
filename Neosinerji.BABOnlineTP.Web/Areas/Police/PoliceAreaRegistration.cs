using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Police
{
    public class PoliceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Police";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Police_default",
                "Police/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
