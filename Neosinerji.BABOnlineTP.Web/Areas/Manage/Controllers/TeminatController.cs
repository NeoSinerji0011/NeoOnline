using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UrunYonetimi, SekmeKodu = AltMenuSekmeler.TeminatTanimlama)]
    public class TeminatController : Controller
    {
        ITeminatService _TeminatService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        public TeminatController(ITeminatService teminatService, IAktifKullaniciService aktifKullanici)
        {
            _TeminatService = teminatService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                List<Teminat> Teminats = _TeminatService.GetList().ToList<Teminat>();
                Mapper.CreateMap<Teminat, TeminatModel>();
                TeminatListModel model = new TeminatListModel();
                model.Items = Mapper.Map<List<Teminat>, List<TeminatModel>>(Teminats);

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
                    DataTableParameters<Teminat> teminatList = new DataTableParameters<Teminat>(Request, new Expression<Func<Teminat, object>>[]
                                                                                        {
                                                                                            t => t.TeminatKodu,
                                                                                            t => t.TeminatAdi
                                                                                        },
                                                                                            t => t.TeminatKodu);


                    result = _TeminatService.PagedList(teminatList);
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
                Teminat teminat = _TeminatService.GetTeminat(id);
                Mapper.CreateMap<Teminat, TeminatModel>();
                TeminatModel model = Mapper.Map<Teminat, TeminatModel>(teminat);

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
                       SekmeKodu = AltMenuSekmeler.TeminatTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                TeminatModel model = new TeminatModel();
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
                       SekmeKodu = AltMenuSekmeler.TeminatTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(TeminatModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Teminat teminat = new Teminat();
                    teminat.TeminatAdi = model.TeminatAdi;
                    teminat.TeminatKodu = model.TeminatKodu.Value;

                    teminat = _TeminatService.CreateItem(teminat);
                    //Eklemeden sonra detaya gider....!!!!
                    return RedirectToAction("Detay", "Teminat", new { id = teminat.TeminatKodu });
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
                       AltMenuKodu = AltMenuler.UrunYonetimi,
                       SekmeKodu = AltMenuSekmeler.TeminatTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                Teminat teminat = _TeminatService.GetTeminat(id);
                TeminatModel model = new TeminatModel();
                model.TeminatKodu = teminat.TeminatKodu;
                model.TeminatAdi = teminat.TeminatAdi;

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
                       SekmeKodu = AltMenuSekmeler.TeminatTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Guncelle(TeminatModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Teminat teminat = _TeminatService.GetTeminat(model.TeminatKodu.Value);
                    teminat.TeminatAdi = model.TeminatAdi;

                    _TeminatService.UpdateItem(teminat);
                    return RedirectToAction("Detay", "Teminat", new { id = teminat.TeminatKodu });
                }
                Teminat teminat2 = _TeminatService.GetTeminat(model.TeminatKodu.Value);
                model.TeminatKodu = teminat2.TeminatKodu;
                model.TeminatAdi = teminat2.TeminatAdi;
                ModelState.AddModelError("", "Lütfen zorunlu alanları doldurunuz");
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }
    }
}
