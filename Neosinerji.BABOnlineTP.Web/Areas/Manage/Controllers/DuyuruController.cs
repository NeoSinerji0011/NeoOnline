using AutoMapper;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.SistemYonetimi, SekmeKodu = AltMenuSekmeler.DuyuruYayinlama)]
    public class DuyuruController : Controller
    {
        IDuyuruService _DuyuruService;
        ITVMService _TVMService;
        IKullaniciService _KullaniciService;
        IDuyuruDokumanStorage _Storage;
        IAktifKullaniciService _AktifKullanici;
        ILogService _log;

        public DuyuruController(IDuyuruService duyuruService, ITVMService tvmService, IKullaniciService kullaniciService, IDuyuruDokumanStorage storage)
        {
            _DuyuruService = duyuruService;
            _TVMService = tvmService;
            _KullaniciService = kullaniciService;
            _Storage = storage;
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            DuyuruListeModel model = new DuyuruListeModel();

            try
            {
                if (_AktifKullanici != null)
                {
                    List<Duyurular> duyurular = _DuyuruService.GetAllDuyuru();

                    model.Items = new List<DuyuruDetayModel>();

                    foreach (var item in duyurular)
                    {
                        DuyuruDetayModel duyurudetay = new DuyuruDetayModel();

                        duyurudetay.DuyuruId = item.DuyuruId;
                        duyurudetay.Aciklama = item.Aciklama;
                        duyurudetay.Konu = item.Konu;
                        duyurudetay.BaslangisTarihi = item.BaslangisTarihi;
                        duyurudetay.BitisTarihi = item.BitisTarihi;
                        duyurudetay.Oncelik = TVMListProvider.GetNotOnceligiText(item.Oncelik);

                        model.Items.Add(duyurudetay);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.SistemYonetimi,
                       SekmeKodu = AltMenuSekmeler.DuyuruYayinlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Gorme)]
        public ActionResult Detay(int Id)
        {
            try
            {
                DuyuruDetayModel model = new DuyuruDetayModel();

                if (_AktifKullanici != null && Id > 0)
                {
                    Duyurular duyuru = _DuyuruService.GetDuyuru(Id);

                    if (duyuru != null)
                    {
                        Mapper.CreateMap<Duyurular, DuyuruDetayModel>();
                        model = Mapper.Map<DuyuruDetayModel>(duyuru);

                        model.Oncelik = TVMListProvider.GetNotOnceligiText(duyuru.Oncelik);

                        TVMKullanicilar kullanici = _KullaniciService.GetKullaniciPublic(duyuru.EkleyenKullanici);
                        if (kullanici != null)
                            model.Ekleyen = kullanici.Adi + " " + kullanici.Soyadi;

                        model.EkliTVMler = _DuyuruService.GetEkliTVMName(duyuru.DuyuruId);
                    }
                    else
                        return new RedirectResult("~/Error/ErrorPage/403");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.SistemYonetimi,
                       SekmeKodu = AltMenuSekmeler.DuyuruYayinlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                DuyuruEkleModel model = new DuyuruEkleModel();

                if (_AktifKullanici != null)
                {
                    List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();

                    if (tvmlist != null)
                        model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani", "");

                    model.Oncelikler = new SelectList(TVMListProvider.NotOncelikTipleri(), "Value", "Text", "1");
                    model.BaslangisTarihi = TurkeyDateTime.Now;
                    model.BitisTarihi = TurkeyDateTime.Now.AddDays(10);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.SistemYonetimi,
                       SekmeKodu = AltMenuSekmeler.DuyuruYayinlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(DuyuruEkleModel model)
        {
            try
            {
                if (_AktifKullanici != null)
                {
                    if (ModelState.IsValid)
                    {
                        Duyurular duyuru = new Duyurular();
                        duyuru.Konu = model.Konu;
                        duyuru.Aciklama = model.Aciklama;
                        duyuru.BaslangisTarihi = model.BaslangisTarihi;
                        duyuru.BitisTarihi = model.BitisTarihi;
                        duyuru.Oncelik = model.Oncelik;

                        duyuru = _DuyuruService.CreateDuyuru(duyuru, model.TVMLerSelectList, null);

                        return RedirectToAction("Detay", "Duyuru", new { Id = duyuru.DuyuruId });
                    }

                    List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
                    model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani", model.TVMLerSelectList);
                    model.Oncelikler = new SelectList(TVMListProvider.NotOncelikTipleri(), "Value", "Text", "1");
                    ModelState.AddModelError("", "lütfen girdiğiniz bilgilerin doğru ve eksiksiz oluşunu kontrol ediniz");

                    return View(model);
                }

                ModelState.AddModelError("", "Bir hata oluuştu");

                return View(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.SistemYonetimi,
                       SekmeKodu = AltMenuSekmeler.DuyuruYayinlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int Id)
        {
            try
            {
                Duyurular duyuru = new Duyurular();

                if (_AktifKullanici != null)
                {
                    duyuru = _DuyuruService.GetDuyuru(Id);
                    if (duyuru != null)
                    {
                        Mapper.CreateMap<Duyurular, DuyuruEkleModel>();
                        DuyuruEkleModel model = Mapper.Map<DuyuruEkleModel>(duyuru);
                        model.Oncelikler = new SelectList(TVMListProvider.NotOncelikTipleri(), "Value", "Text", model.Oncelik);

                        List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();

                        var selectedItemIds = _DuyuruService.GetAddedTVMList(duyuru.DuyuruId);

                        model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani", selectedItemIds);

                        return View(model);
                    }
                    else return new RedirectResult("~/Error/ErrorPage/403");
                }
                else return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.SistemYonetimi,
                       SekmeKodu = AltMenuSekmeler.DuyuruYayinlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(DuyuruEkleModel model)
        {
            try
            {
                if (_AktifKullanici != null)
                {
                    if (ModelState.IsValid)
                    {
                        Duyurular duyuru = _DuyuruService.GetDuyuru(model.DuyuruId);

                        if (duyuru != null && duyuru.DuyuruId > 0)
                        {
                            duyuru.Konu = model.Konu;
                            duyuru.Oncelik = model.Oncelik;
                            duyuru.Aciklama = model.Aciklama;
                            duyuru.BaslangisTarihi = model.BaslangisTarihi;
                            duyuru.BitisTarihi = model.BitisTarihi;

                            _DuyuruService.UpdateDuyuru(duyuru, model.TVMLerSelectList);
                            return RedirectToAction("Detay", "Duyuru", new { Id = duyuru.DuyuruId });
                        }
                        else return new RedirectResult("~/Error/ErrorPage/403");
                    }

                    List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();

                    model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani", model.TVMLerSelectList);
                    model.Oncelikler = new SelectList(TVMListProvider.NotOncelikTipleri(), "Value", "Text", "1");

                    ModelState.AddModelError("", "Lütfen girdiğiniz bilgileri kontrol ediniz");

                    return View(model);
                }
                else return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.SistemYonetimi,
                       SekmeKodu = AltMenuSekmeler.DuyuruYayinlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Silme)]
        public ActionResult DuyuruSil(int Id)
        {
            try
            {
                if (_AktifKullanici != null && Id > 0)
                {
                    Duyurular duyuru = _DuyuruService.GetDuyuru(Id);

                    if (duyuru != null)
                    {
                        _DuyuruService.DeleteDuyuru(Id);
                        return RedirectToAction("Liste", "Duyuru");
                    }
                    else return new RedirectResult("~/Error/ErrorPage/403");
                }
                else return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }
    }
}
