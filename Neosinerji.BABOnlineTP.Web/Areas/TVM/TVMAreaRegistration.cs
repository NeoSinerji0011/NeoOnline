using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM
{
    public class TVMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TVM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TVM_default",
                "TVM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
