using Neosinerji.BABOnlineTP.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business;

namespace Neosinerji.BABOnlineTP.Web.Controllers
{
    public class ErrorController : Controller
    {

        //
        // GET: /Error/
        public ActionResult ErrorPage(int? id)
        {
            ErrorModel model = new ErrorModel();
            model.HataKodu = 500;
            model.ReturnURL = "/";
            model.Mesaj = babonline.ThereWas_anError;

            if (id.HasValue)
            {
                switch (id)
                {
                    case 403: model.HataKodu = 403; model.Mesaj = babonline.Access_Is_Denied; break;
                    case 404: model.HataKodu = 404; model.Mesaj = babonline.ErrorPage_Description_404; break;
                }
            }

            IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            if (aktifKullanici != null)
            {
                if (aktifKullanici.IsAuthenticated)
                    model.ReturnURL = "/TVM/TVM/Index";
            }

            return View(model);
        }
    }
}
