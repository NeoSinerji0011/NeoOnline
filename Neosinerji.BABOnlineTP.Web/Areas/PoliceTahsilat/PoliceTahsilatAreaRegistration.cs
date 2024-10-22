using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.PoliceTahsilat
{
    public class PoliceTahsilatAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PoliceTahsilat";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PoliceTahsilat_default",
                "PoliceTahsilat/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
