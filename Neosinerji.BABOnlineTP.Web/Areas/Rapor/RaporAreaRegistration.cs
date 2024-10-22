using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor
{
    public class RaporAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Rapor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Rapor_default",
                "Rapor/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
