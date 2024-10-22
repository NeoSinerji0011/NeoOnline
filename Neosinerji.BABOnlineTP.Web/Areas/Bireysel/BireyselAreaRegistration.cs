using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Bireysel
{
    public class BireyselAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Bireysel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Bireysel_default",
                "Bireysel/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}