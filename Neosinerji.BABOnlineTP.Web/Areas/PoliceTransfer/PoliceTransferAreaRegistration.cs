using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.PoliceTransfer
{
    public class PoliceTransferAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PoliceTransfer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PoliceTransfer_default",
                "PoliceTransfer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
