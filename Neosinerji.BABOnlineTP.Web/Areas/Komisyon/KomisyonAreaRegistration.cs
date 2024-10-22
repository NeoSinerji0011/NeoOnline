using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon
{
    public class KomisyonAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Komisyon";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Komisyon_default",
                "Komisyon/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
