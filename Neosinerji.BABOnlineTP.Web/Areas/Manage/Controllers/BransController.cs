using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UrunYonetimi, SekmeKodu = AltMenuSekmeler.BransTanimlama)]
    public class BransController : Controller
    {
        IBransService _BransService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        public BransController(IBransService bransService, IAktifKullaniciService aktifKullanici)
        {
            _BransService = bransService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                //List<Bran> bransList = _BransService.GetList(_AktifKullanici.TVMKodu);
                List<Bran> bransList = _BransService.GetList("1");

                BransListModel model = new BransListModel();
                Mapper.CreateMap<Bran, BransModel>();
                model.Items = Mapper.Map<List<Bran>, List<BransModel>>(bransList);

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
                Bran brans = _BransService.GetBrans(id);

                Mapper.CreateMap<Bran, BransModel>();
                BransModel model = Mapper.Map<Bran, BransModel>(brans);

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
                       SekmeKodu = AltMenuSekmeler.BransTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                BransModel model = new BransModel();
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", "0");

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
                       SekmeKodu = AltMenuSekmeler.BransTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(BransModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Bran bran = new Bran();
                    bran.BransAdi = model.BransAdi;
                    bran.BransKodu = model.BransKodu.Value;
                    bran.Durum = model.Durum;
                    bran = _BransService.CreateItem(bran);

                    //Detay sayfasına gider
                    return RedirectToAction("Detay", "Brans", new { id = bran.BransKodu });
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
                       SekmeKodu = AltMenuSekmeler.BransTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                Bran brans = _BransService.GetBrans(id);
                Mapper.CreateMap<Bran, BransModel>();
                BransModel model = Mapper.Map<Bran, BransModel>(brans);
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", brans.Durum);

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
                       SekmeKodu = AltMenuSekmeler.BransTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(BransModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Bran bran = _BransService.GetBrans(model.BransKodu.Value);
                    bran.BransAdi = model.BransAdi;
                    bran.Durum = model.Durum;

                    _BransService.UpdateItem(bran);

                    //Detay sayfasına gider
                    return RedirectToAction("Detay", "Brans", new { id = model.BransKodu });
                }
                // ModelState.AddModelError("", "değişecek");
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
