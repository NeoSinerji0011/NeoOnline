using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TaliAcente
{
    public class TaliAcenteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TaliAcente";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TaliAcente_default",
                "TaliAcente/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
