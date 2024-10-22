using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Neosinerji.BABOnlineTP.Web {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            MvcHandler.DisableMvcResponseHeader = true; 
            //Model binders
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            //ModelBinders.Binders.Add(typeof(Nullable<DateTime>), new Tools.Helpers.NullableDateTimeModelBinder());

            Bootstrapper.Initialise();
        }

        protected void Application_BeginRequest()
        {

            HttpCookie language = Request.Cookies["lang"];
            if (language != null)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language.Value);
                var tempTr = new System.Globalization.CultureInfo("tr-TR");
                Thread.CurrentThread.CurrentCulture.DateTimeFormat = tempTr.DateTimeFormat;
                Thread.CurrentThread.CurrentCulture.NumberFormat = tempTr.NumberFormat;
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //if (Server != null)
            //{
            //    var exception = Server.GetLastError(); //Oluşan hatayı değişkene atadık.

            //    //Hata Kaydediliyor.
            //    ILogService log = DependencyResolver.Current.GetService<ILogService>();
            //    Exception ex = exception as Exception;
            //    log.Error(ex);

            //    var httpException = exception as HttpException;

            //    //Sunucudaki hatayı temizledik.
            //    Response.Clear();
            //    Server.ClearError();

            //    //IIS'in tipik hata sayfalarını görmezden geldik.
            //    Response.TrySkipIisCustomErrors = true;

            //    if (httpException != null)
            //    {
            //        Response.StatusCode = httpException.GetHttpCode();

            //        switch (Response.StatusCode)
            //        {
            //            case 403: //Eğer 403 hatası meydana gelmiş ise devreye girecek.
            //                Response.Redirect("/Error/ErrorPage/" + 403);
            //                break;
            //            case 404: //Eğer 404 hatası meydana gelmiş ise devreye girecek.
            //                Response.Redirect("/Error/ErrorPage/" + 404);
            //                break;
            //            default: //Eğer 500 veya başka bir hata meydana gelmiş ise devreye girecek.
            //                Response.Redirect("/Error/ErrorPage/" + 500);
            //                break;
            //        }
            //    }
            //    else if (!Response.IsRequestBeingRedirected)
            //    {
            //        Response.Redirect("/Error/ErrorPage/" + 500);
            //    }
            //}
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Response.Headers.Remove("X-Powered-By");
                HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
                HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
                HttpContext.Current.Response.Headers.Remove("Server");
            }
            catch (Exception)
            {
            }
        }
    }
}
