using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UrunYonetimi, SekmeKodu = AltMenuSekmeler.UrunTanimlama)]
    public class UrunController : Controller
    {
        IUrunService _UrunService;
        IBransService _BransService;
        ITeminatService _TeminatService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        public UrunController(IUrunService urunService, IBransService bransService, ITeminatService teminatService, IAktifKullaniciService aktifKullanici)
        {
            _TeminatService = teminatService;
            _UrunService = urunService;
            _BransService = bransService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                List<UrunServiceModel> urunler = _UrunService.GetList();
                UrunListModel model = new UrunListModel();

                Mapper.CreateMap<UrunServiceModel, UrunModel>();
                model.Items = Mapper.Map<List<UrunModel>>(urunler);

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
                Urun urun = _UrunService.GetUrun(id);
                Mapper.CreateMap<Urun, UrunModel>();
                UrunModel model = Mapper.Map<Urun, UrunModel>(urun);
                model.Teminatlari = UrunTeminatlari(urun.UrunKodu);
                model.Vergileri = UrunVergileri(urun.UrunKodu);
                model.Sorulari = UrunSorulari(urun.UrunKodu);

                model.BransAdi = urun.Bran.BransAdi;

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
                       SekmeKodu = AltMenuSekmeler.UrunTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                UrunModel model = new UrunModel();
                model.Durum = UrunDurumlari.Aktif;

                BransListModel Bransmodel = new BransListModel();
                List<Bran> bransList = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                Mapper.CreateMap<Bran, BransModel>();
                Bransmodel.Items = Mapper.Map<List<BransModel>>(bransList);

                model.BransList = new SelectList(Bransmodel.Items, "BransKodu", "BransAdi", "").ListWithOptionLabel();
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", "1");

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
                       SekmeKodu = AltMenuSekmeler.UrunTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(UrunModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Urun urun = new Urun();
                    urun.UrunKodu = model.UrunKodu.Value;
                    urun.UrunAdi = model.UrunAdi;
                    urun.BransKodu = model.BransKodu;
                    urun.Durum = model.Durum;

                    _UrunService.CreateItem(urun);

                    //Detay sayfasına gidiyor 
                    return RedirectToAction("Detay", "Urun", new { id = urun.UrunKodu });
                }

                BransListModel Bransmodel = new BransListModel();
                List<Bran> bransList = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                Mapper.CreateMap<Bran, BransModel>();
                Bransmodel.Items = Mapper.Map<List<BransModel>>(bransList);

                model.BransList = new SelectList(Bransmodel.Items, "BransKodu", "BransKodu").ListWithOptionLabel();
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

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
                       SekmeKodu = AltMenuSekmeler.UrunTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                UrunModel model = new UrunModel();

                //Urun Modeli oluşturuluyor
                Urun urun = _UrunService.GetUrun(id);

                //Model Mapleniyor....
                Mapper.CreateMap<Urun, UrunModel>();
                model = Mapper.Map<Urun, UrunModel>(urun);
                model.BransAdi = urun.Bran.BransAdi;

                model.Teminatlari = UrunTeminatlari(urun.UrunKodu);
                model.Vergileri = UrunVergileri(urun.UrunKodu);
                model.Sorulari = UrunSorulari(urun.UrunKodu);

                //Dropdown list
                BransListModel Bransmodel = new BransListModel();
                List<Bran> bransList = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                Mapper.CreateMap<Bran, BransModel>();
                Bransmodel.Items = Mapper.Map<List<BransModel>>(bransList);

                model.BransList = new SelectList(Bransmodel.Items, "BransKodu", "BransAdi").ListWithOptionLabel();
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

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
                       SekmeKodu = AltMenuSekmeler.UrunTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(UrunModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Urun urun = _UrunService.GetUrun(model.UrunKodu.Value);
                    urun.UrunAdi = model.UrunAdi;
                    urun.BransKodu = model.BransKodu;
                    urun.Durum = model.Durum;

                    _UrunService.UpdateItem(urun);

                    //Detay sayfasına gider   (Güncelleme sonrası)
                    return RedirectToAction("Detay", "Urun", new { id = urun.UrunKodu });
                }

                model.Teminatlari = UrunTeminatlari(model.UrunKodu.Value);
                model.Vergileri = UrunVergileri(model.UrunKodu.Value);
                model.Sorulari = UrunSorulari(model.UrunKodu.Value);

                //Dropdown list
                BransListModel Bransmodel = new BransListModel();
                List<Bran> bransList = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                Mapper.CreateMap<Bran, BransModel>();
                Bransmodel.Items = Mapper.Map<List<BransModel>>(bransList);

                model.BransList = new SelectList(Bransmodel.Items, "BransKodu", "BransAdi").ListWithOptionLabel();
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

                ModelState.AddModelError("", babonline.Message_TVMSaveError);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }


        [HttpPost]
        [AjaxException]
        public ActionResult TeminatEkle(int urunKodu, int[] teminatKodu)
        {
            foreach (int tKodu in teminatKodu)
            {
                _UrunService.AddTeminat(urunKodu, tKodu);
            }

            UrunModel model = new UrunModel();
            model.UrunKodu = urunKodu;
            model.Teminatlari = UrunTeminatlari(urunKodu);

            return PartialView("_Teminatlar", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult TeminatSil(int urunKodu, int teminatKodu)
        {
            _UrunService.DeleteTeminat(urunKodu, teminatKodu);

            UrunModel model = new UrunModel();
            model.UrunKodu = urunKodu;
            model.Teminatlari = UrunTeminatlari(urunKodu);

            return PartialView("_Teminatlar", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult VergiEkle(int urunKodu, int[] vergiKodu)
        {
            foreach (int vKodu in vergiKodu)
            {
                _UrunService.AddVergi(urunKodu, vKodu);
            }

            UrunModel model = new UrunModel();
            model.UrunKodu = urunKodu;
            model.Vergileri = UrunVergileri(urunKodu);

            return PartialView("_Vergiler", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult VergiSil(int urunKodu, int vergiKodu)
        {
            _UrunService.DeleteVergi(urunKodu, vergiKodu);

            UrunModel model = new UrunModel();
            model.UrunKodu = urunKodu;
            model.Vergileri = UrunVergileri(urunKodu);

            return PartialView("_Vergiler", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult SoruEkle(int urunKodu, int[] soruKodu)
        {
            foreach (int sKodu in soruKodu)
            {
                _UrunService.AddSoru(urunKodu, sKodu);
            }

            UrunModel model = new UrunModel();
            model.UrunKodu = urunKodu;
            model.Sorulari = UrunSorulari(urunKodu);

            return PartialView("_Sorular", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult SoruSil(int urunKodu, int soruKodu)
        {
            _UrunService.DeleteSoru(urunKodu, soruKodu);

            UrunModel model = new UrunModel();
            model.UrunKodu = urunKodu;
            model.Sorulari = UrunSorulari(urunKodu);

            return PartialView("_Sorular", model);
        }

        private List<UrunTeminatModel> UrunTeminatlari(int urunKodu)
        {
            List<UrunTeminatServiceModel> teminatlar = _UrunService.GetUrunTeminatlari(urunKodu);

            Mapper.CreateMap<UrunTeminatServiceModel, UrunTeminatModel>();
            List<UrunTeminatModel> model = Mapper.Map<List<UrunTeminatModel>>(teminatlar);

            return model;
        }

        private List<UrunVergiModel> UrunVergileri(int urunKodu)
        {
            List<UrunVergiServiceModel> vergiler = _UrunService.GetUrunVergileri(urunKodu);

            Mapper.CreateMap<UrunVergiServiceModel, UrunVergiModel>();
            List<UrunVergiModel> model = Mapper.Map<List<UrunVergiModel>>(vergiler);

            return model;
        }

        private List<UrunSoruModel> UrunSorulari(int urunKodu)
        {
            List<UrunSoruServiceModel> sorular = _UrunService.GetUrunSorulari(urunKodu);

            Mapper.CreateMap<UrunSoruServiceModel, UrunSoruModel>();
            List<UrunSoruModel> model = Mapper.Map<List<UrunSoruModel>>(sorular);

            return model;
        }
    }
}
