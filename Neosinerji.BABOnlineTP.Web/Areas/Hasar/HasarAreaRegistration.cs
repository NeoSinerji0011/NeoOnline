using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Hasar
{
    public class HasarAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Hasar";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Hasar_default",
                "Hasar/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
