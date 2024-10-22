using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif
{
    public class TeklifAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Teklif";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Teklif_default",
                "Teklif/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
