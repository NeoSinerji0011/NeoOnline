using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.GorevTakip
{
    public class GorevTakipAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GorevTakip";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "GorevTakip_default",
                "GorevTakip/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}