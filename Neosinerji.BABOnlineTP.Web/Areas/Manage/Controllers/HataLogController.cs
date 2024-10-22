using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.Service;


namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.SistemYonetimi, SekmeKodu = AltMenuSekmeler.HataLog)]
    public class HataLogController : Controller
    {
        IHataLogService _hata;
        ITVMService _TvmService;
        ILogService _Log;
        IAktifKullaniciService _AktifKullanici;
        IKullaniciService _KullaniciService;

        public HataLogController()
        {
            _hata = DependencyResolver.Current.GetService<IHataLogService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                HataLogModel model = new HataLogModel();

                model.Tarih = TurkeyDateTime.Today;
                model.Saat = TurkeyDateTime.Now.AddMinutes(-10).ToString("HH:mm:ss");

                model.LogTypeTipleri = new SelectList(HataLogListProvider.HataLogTipleri(), "Value", "Text", model.LogType).ToList<SelectListItem>();

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Liste(HataLogModel model)
        {
            try
            {
                model.LogTypeTipleri = new SelectList(HataLogListProvider.HataLogTipleri(), "Value", "Text", model.LogType).ToList<SelectListItem>();

                model.HataList = _hata.GetHataList(model.TVMKullaniciAdi, model.Tarih, model.Saat, model.LogType);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult TVM(string query)
        {
            var tvmler = _TvmService.GetTVMListe(query);

            return Json(new { options = tvmler.Select(s => s.Unvani).ToArray() }, JsonRequestBehavior.AllowGet);
        }
    }
}
