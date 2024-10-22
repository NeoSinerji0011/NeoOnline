using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TaliAcenteTransfer
{
    public class TaliAcenteTransferAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TaliAcenteTransfer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TaliAcenteTransfer_default",
                "TaliAcenteTransfer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
