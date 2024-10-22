using System;
using System.Linq;
using System.Web;
using AutoMapper;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.SistemYonetimi, SekmeKodu = AltMenuSekmeler.Konfigurasyon)]
    public class KonfigurasyonController : Controller
    {
        IKonfigurasyonService _KonfigurasyonService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        public KonfigurasyonController(IKonfigurasyonService konfigurasyonService, IAktifKullaniciService aktifKullanici)
        {
            _KonfigurasyonService = konfigurasyonService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                List<Konfigurasyon> list = _KonfigurasyonService.GetList().ToList<Konfigurasyon>();

                Mapper.CreateMap<Konfigurasyon, KonfigurasyonModel>();
                List<KonfigurasyonModel> model = Mapper.Map<List<KonfigurasyonModel>>(list);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult Detay(string id)
        {
            try
            {
                Konfigurasyon konfig = _KonfigurasyonService.GetKonfig(id);

                Mapper.CreateMap<Konfigurasyon, KonfigurasyonModel>();
                KonfigurasyonModel model = Mapper.Map<KonfigurasyonModel>(konfig);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                      AltMenuKodu = AltMenuler.SistemYonetimi,
                      SekmeKodu = AltMenuSekmeler.Konfigurasyon,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme,
                      menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                KonfigurasyonModel model = new KonfigurasyonModel();

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
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                      AltMenuKodu = AltMenuler.SistemYonetimi,
                      SekmeKodu = AltMenuSekmeler.Konfigurasyon,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme,
                      menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(KonfigurasyonModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<KonfigurasyonModel, Konfigurasyon>();
                    Konfigurasyon konfig = Mapper.Map<Konfigurasyon>(model);

                    konfig = _KonfigurasyonService.CreateItem(konfig);

                    return RedirectToAction("Detay", "Konfigurasyon", new { id = konfig.Kod });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                      AltMenuKodu = AltMenuler.SistemYonetimi,
                      SekmeKodu = AltMenuSekmeler.Konfigurasyon,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme,
                      menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(string id)
        {
            try
            {
                Konfigurasyon konfig = _KonfigurasyonService.GetKonfig(id);

                Mapper.CreateMap<Konfigurasyon, KonfigurasyonModel>();
                KonfigurasyonModel model = Mapper.Map<Konfigurasyon, KonfigurasyonModel>(konfig);

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
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                      AltMenuKodu = AltMenuler.SistemYonetimi,
                      SekmeKodu = AltMenuSekmeler.Konfigurasyon,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme,
                      menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(KonfigurasyonModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<KonfigurasyonModel, Konfigurasyon>();
                    Konfigurasyon konfig = Mapper.Map<Konfigurasyon>(model);

                    _KonfigurasyonService.UpdateItem(konfig);

                    return RedirectToAction("Detay", "Konfigurasyon", new { id = konfig.Kod });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.SistemYonetimi,
                       SekmeKodu = AltMenuSekmeler.Konfigurasyon,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Silme)]
        public ActionResult Sil(string id)
        {
            try
            {
                if (_AktifKullanici != null && _AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                {
                    _KonfigurasyonService.DeleteKonfig(id);

                    return RedirectToAction("Liste", "Konfigurasyon");
                }
                return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }
    }
}
