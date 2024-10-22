using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UrunYonetimi, SekmeKodu = AltMenuSekmeler.VergiTanimlama)]
    public class VergiController : Controller
    {
        IVergiService _VergiService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        [Dependency]
        public IParametreContext _UnitOfWork { get; set; }

        public VergiController(IVergiService vergiService, IAktifKullaniciService aktifKullanici)
        {
            _VergiService = vergiService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                List<Vergi> vergiler = _VergiService.GetList().ToList<Vergi>();
                Mapper.CreateMap<Vergi, VergiModel>();
                VergiListModel model = new VergiListModel();
                model.Items = Mapper.Map<List<Vergi>, List<VergiModel>>(vergiler);

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
                    DataTableParameters<Vergi> vergiList = new DataTableParameters<Vergi>(Request, new Expression<Func<Vergi, object>>[]
                                                                                        {
                                                                                            t => t.VergiKodu,
                                                                                            t => t.VergiAdi
                                                                                        },
                                                                                            t => t.VergiKodu);
                    result = _VergiService.PagedList(vergiList);
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
                Vergi vergi = _VergiService.GetVergi(id);
                Mapper.CreateMap<Vergi, VergiModel>();
                VergiModel model = Mapper.Map<Vergi, VergiModel>(vergi);
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
                       SekmeKodu = AltMenuSekmeler.VergiTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                VergiModel model = new VergiModel();
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
                       SekmeKodu = AltMenuSekmeler.VergiTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(VergiModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Vergi vergi = new Vergi();
                    vergi.VergiAdi = model.VergiAdi;
                    vergi.VergiKodu = model.VergiKodu.Value;
                    vergi = _VergiService.CreateItem(vergi);

                    //Detay sayfasına gider...
                    return RedirectToAction("Detay", "Vergi", new { id = vergi.VergiKodu });
                }

                ModelState.AddModelError("", babonline.Message_RequiredValues);
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.UrunYonetimi,
                       SekmeKodu = AltMenuSekmeler.VergiTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                Vergi vergi = _VergiService.GetVergi(id);

                VergiModel model = new VergiModel();
                model.VergiKodu = vergi.VergiKodu;
                model.VergiAdi = vergi.VergiAdi;

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
                       SekmeKodu = AltMenuSekmeler.VergiTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(VergiModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Vergi vergi = _VergiService.GetVergi(model.VergiKodu.Value);
                    vergi.VergiAdi = model.VergiAdi;

                    _VergiService.UpdateItem(vergi);

                    //Guncelleme sonrası detay sayfasına gider..
                    return RedirectToAction("Detay", "Vergi", new { id = vergi.VergiKodu });
                }

                Vergi vergi2 = _VergiService.GetVergi(model.VergiKodu.Value);
                model.VergiKodu = vergi2.VergiKodu;
                model.VergiAdi = vergi2.VergiAdi;
                ModelState.AddModelError("", babonline.Message_RequiredValues);
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
