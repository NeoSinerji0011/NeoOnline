using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.OdemeGirisi
{
    public class OdemeGirisiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OdemeGirisi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OdemeGirisi_default",
                "OdemeGirisi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
