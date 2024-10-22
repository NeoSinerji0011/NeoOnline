using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Neosinerji.BABOnlineTP.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{WS}/{PoliceMuhasebe}.asmx/{*pathInfo}");
            routes.MapRoute(
              name: "online-acente",
              url: "online-acente",
              defaults: new { controller = "Home", action = "Index", id = 2 }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}