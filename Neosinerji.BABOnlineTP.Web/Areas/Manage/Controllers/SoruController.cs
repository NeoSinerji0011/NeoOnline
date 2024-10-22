using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using AutoMapper;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UrunYonetimi, SekmeKodu = AltMenuSekmeler.SoruTanimlama)]
    public class SoruController : Controller
    {
        ISoruService _SoruService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        public SoruController(ISoruService soruService, IAktifKullaniciService aktifKullanici)
        {
            _SoruService = soruService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                List<Soru> liste = _SoruService.GetList().ToList<Soru>();

                Mapper.CreateMap<Soru, SoruModel>();
                List<SoruModel> model = Mapper.Map<List<SoruModel>>(liste);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult ListePager()
        {
            DataTableList result = new DataTableList();

            try
            {

                if (Request["sEcho"] != null)
                {
                    DataTableParameters<Soru> soruList = new DataTableParameters<Soru>(Request, new Expression<Func<Soru, object>>[]
                                                                                        {
                                                                                            t => t.SoruKodu,
                                                                                            t => t.SoruAdi
                                                                                        },
                                                                                            t => t.SoruKodu);

                    result = _SoruService.PagedList(soruList);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detay(int id)
        {
            try
            {
                Soru soru = _SoruService.GetSoru(id);
                SoruModel model = new SoruModel();

                Mapper.CreateMap<Soru, SoruModel>();
                model = Mapper.Map<Soru, SoruModel>(soru);

                model.SoruCevapTipiAdi = SoruTipProvider.SoruCevapTipiText(model.SoruCevapTipi.Value);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.UrunYonetimi,
                       SekmeKodu = AltMenuSekmeler.SoruTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                SoruModel model = new SoruModel();
                model.SoruCevapTipLeri = SoruTipProvider.SoruCevapTipleriList();

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
                       AltMenuKodu = AltMenuler.UrunYonetimi,
                       SekmeKodu = AltMenuSekmeler.SoruTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(SoruModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Soru soru = new Soru();

                    soru.SoruKodu = model.SoruKodu.Value;
                    soru.SoruAdi = model.SoruAdi;
                    soru.SoruCevapTipi = model.SoruCevapTipi.Value;
                    soru.SoruCevapUzunlugu = model.SoruCevapUzunlugu.Value;

                    soru = _SoruService.CreateItem(soru);

                    return RedirectToAction("Detay", "Soru", new { id = soru.SoruKodu });
                }

                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
            }
            catch (DbUpdateException ex)
            {
                string message = ExceptionTools.GetMessage(ex);
                ModelState.AddModelError("", message);
            }

            model.SoruCevapTipLeri = SoruTipProvider.SoruCevapTipleriList();
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.UrunYonetimi,
                       SekmeKodu = AltMenuSekmeler.SoruTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                Soru soru = _SoruService.GetSoru(id);
                Mapper.CreateMap<Soru, SoruModel>();
                SoruModel model = Mapper.Map<Soru, SoruModel>(soru);

                model.SoruCevapTipLeri = SoruTipProvider.SoruCevapTipleriList();

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
                       AltMenuKodu = AltMenuler.UrunYonetimi,
                       SekmeKodu = AltMenuSekmeler.SoruTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int id, SoruModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Soru soru = _SoruService.GetSoru(id);
                    soru.SoruAdi = model.SoruAdi;
                    soru.SoruCevapTipi = model.SoruCevapTipi.Value;

                    _SoruService.UpdateItem(soru);

                    return RedirectToAction("Detay", "Soru", new { id = soru.SoruKodu });
                }

                model.SoruCevapTipLeri = SoruTipProvider.SoruCevapTipleriList();

                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }
    }
}
