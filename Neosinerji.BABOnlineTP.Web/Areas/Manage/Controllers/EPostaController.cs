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
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.SistemYonetimi, SekmeKodu = AltMenuSekmeler.EPostaFormatlari)]
    public class EPostaController : Controller
    {
        IEPostaService _EPostaService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        public EPostaController(IEPostaService epostaService, IAktifKullaniciService aktifKullanici)
        {
            _EPostaService = epostaService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                List<EPostaFormatlari> list = _EPostaService.GetList().ToList<EPostaFormatlari>();
                Mapper.CreateMap<EPostaFormatlari, EPostaFormatModel>();
                List<EPostaFormatModel> model = Mapper.Map<List<EPostaFormatModel>>(list);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult Detay(int id)
        {
            try
            {
                EPostaFormatlari eposta = _EPostaService.GetEPosta(id);

                Mapper.CreateMap<EPostaFormatlari, EPostaFormatModel>();
                EPostaFormatModel model = Mapper.Map<EPostaFormatModel>(eposta);

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
                       SekmeKodu = AltMenuSekmeler.EPostaFormatlari,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                EPostaFormatModel model = new EPostaFormatModel();

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
                       SekmeKodu = AltMenuSekmeler.EPostaFormatlari,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(EPostaFormatModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<EPostaFormatModel, EPostaFormatlari>();
                    EPostaFormatlari format = Mapper.Map<EPostaFormatlari>(model);

                    format = _EPostaService.CreateItem(format);

                    return RedirectToAction("Detay", "EPosta", new { id = format.FormatId });
                }
                ModelState.AddModelError("", babonline.Message_RequiredValues);

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
                       SekmeKodu = AltMenuSekmeler.EPostaFormatlari,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                EPostaFormatlari eposta = _EPostaService.GetEPosta(id);

                Mapper.CreateMap<EPostaFormatlari, EPostaFormatModel>();
                EPostaFormatModel model = Mapper.Map<EPostaFormatlari, EPostaFormatModel>(eposta);

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
                       SekmeKodu = AltMenuSekmeler.EPostaFormatlari,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(EPostaFormatModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EPostaFormatlari format = _EPostaService.GetEPosta(model.FormatId.Value);

                    format.FormatAdi = model.FormatAdi;
                    format.Konu = model.Konu;

                    _EPostaService.UpdateItem(format);

                    return RedirectToAction("Detay", "EPosta", new { id = format.FormatId });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                     AltMenuKodu = AltMenuler.SistemYonetimi,
                     SekmeKodu = AltMenuSekmeler.EPostaFormatlari,
                     menuPermissionType = MenuPermissionType.ALtMenuSekme,
                     menuPermission = MenuPermission.Silme)]
        public ActionResult Sil(int id)
        {
            try
            {
                _EPostaService.DeleteFormat(id);
                return RedirectToAction("Liste", "EPosta");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }
    }
}
