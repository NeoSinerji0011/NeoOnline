using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.PoliceUretimHedefPlanlanan
{
    public class PoliceUretimHedefPlanlananAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PoliceUretimHedefPlanlanan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PoliceUretimHedefPlanlanan_default",
                "PoliceUretimHedefPlanlanan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
