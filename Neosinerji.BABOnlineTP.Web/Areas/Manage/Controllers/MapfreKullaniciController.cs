using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Tools;


namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.KullaniciYonetimi, SekmeKodu = AltMenuSekmeler.MapfreKullaniciTanimlama)]
    public class MapfreKullaniciController : Controller
    {
        ITVMService _TVMService;
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _AktifKullanici;
        IYetkiService _YetkiService;
        ILogService _Log;
        public MapfreKullaniciController(ITVMService tvmService, IKullaniciService kullaniciService, IAktifKullaniciService aktifKullanici, IYetkiService yetkiService)
        {
            _TVMService = tvmService;
            _KullaniciService = kullaniciService;
            _AktifKullanici = aktifKullanici;
            _YetkiService = yetkiService;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                MapfreKullaniciListeEkranModel model = new MapfreKullaniciListeEkranModel();
                model.Olusturuldu = true;
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
            try
            {
                if (Request["sEcho"] != null)
                {
                    MapfreKullaniciArama arama = new MapfreKullaniciArama(Request, new Expression<Func<MapfreKullaniciListeModel, object>>[]
                                                                    {
                                                                        t => t.Bolge,
                                                                        t => t.TVMUnvan,
                                                                        t => t.AnaPartaj,
                                                                        t => t.TaliPartaj,
                                                                        t => t.KullaniciAdi,
                                                                        t => t.Email,
                                                                        t => t.OlusturulduText
                                                                    });

                    arama.KullaniciAdi = arama.TryParseParamString("KullaniciAdi");
                    arama.Partaj = arama.TryParseParamString("Partaj");


                    int totalRowCount = 0;
                    List<MapfreKullaniciListeModel> list = _KullaniciService.MapfrePagedList(arama, out totalRowCount);

                    arama.AddFormatter(c => c.OlusturulduText, c => c.Olusturuldu ? String.Format("<span class='label label-success'>{0}</span>", babonline.Yes) : String.Format("<span class='label label-success'>{0}</span>", babonline.No));

                    DataTableList result = arama.Prepare(list, totalRowCount);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return null;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.MapfreKullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            MapfreKullaniciModel model = new MapfreKullaniciModel();
            List<TVMBolgeleri> bolgeler = _TVMService.GetListBolgeler(NeosinerjiTVM.MapfreeTVMKodu);
            model.Bolgeler = new SelectList(bolgeler, "TVMBolgeKodu", "BolgeAdi", "").ListWithOptionLabel(false);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.MapfreKullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(MapfreKullaniciModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_KullaniciService.MapfreKullaniciEkleTest(model.KullaniciAdi))
                    {

                        MapfreKullanici kullanici = new MapfreKullanici();
                        kullanici.Bolge = model.Bolge.ToString();
                        kullanici.TVMUnvan = model.TVMUnvan.Trim();
                        kullanici.AnaPartaj = model.AnaPartaj.Trim();
                        kullanici.TaliPartaj = model.TaliPartaj.Trim();
                        kullanici.KullaniciAdi = model.KullaniciAdi.Trim();
                        kullanici.EMail = model.Email;
                        kullanici.Olusturuldu = false;
                        kullanici = _KullaniciService.CreateMapfreKullanici(kullanici);

                        _KullaniciService.MapfreKulaniciOlustur(kullanici.MapfeKullaniciId, String.Empty);

                        return RedirectToAction("Liste", "MapfreKullanici");
                    }
                    TVMDetay detay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);

                    List<TVMBolgeleri> bolgeler;
                    if (String.IsNullOrEmpty(model.Bolge) || model.Bolge == "0")
                        bolgeler = new List<TVMBolgeleri>();
                    else
                        bolgeler = _TVMService.GetListBolgeler(detay.BagliOlduguTVMKodu);


                    model.Bolgeler = new SelectList(bolgeler, "TVMBolgeKodu", "BolgeAdi", model.Bolge).ListWithOptionLabel(false);
                    ModelState.AddModelError("", "Bu kullanıcı daha önce kaydedildi");

                }
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
