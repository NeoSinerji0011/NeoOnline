using Neosinerji.BABOnlineTP.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Tools.Filters
{
    public class LifeLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                IKonfigurasyonService _KonfigurasyonService = DependencyResolver.Current.GetService<KonfigurasyonService>();
                bool aegonOnOff = Convert.ToBoolean(_KonfigurasyonService.GetKonfig("Aegon_OnOff").Deger);
                if (!aegonOnOff)
                {
                    filterContext.Result = new RedirectResult("~/");
                    return;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}