using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.Common;
using System.IO;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = 1)]
    public class AracController : Controller
    {
        //
        // GET: /Manage/Arac/
        IAracDegerService _AracDegerService;
        IAracService _AracService;
        ILogService _Log;
        IAktifKullaniciService _AktifKullanici;


        public AracController(IAracDegerService aracDegerService, IAracService aracService)
        {
            _AracDegerService = aracDegerService;
            _AracService = aracService;
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
        }

        public ActionResult Guncelle()
        {
            try
            {
                if (_AktifKullanici != null && _AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                    return View();

                return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }



        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = 1)]
        public void UploadZip(HttpPostedFileBase file)
        {
            try
            {
                if (_AktifKullanici != null && _AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = Path.Combine(Server.MapPath("~/Files"), file.FileName);
                        file.SaveAs(path);

                        _AracDegerService.AracDegerlistesiAktar(path);
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = 1)]
        public bool HDIAracTipAktar()
        {
            _AracDegerService.HDIAracListesiAktar();
            return true;
        }
    }
}
