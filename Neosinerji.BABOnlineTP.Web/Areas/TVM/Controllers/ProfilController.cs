using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.TVM.Models;
using Neosinerji.BABOnlineTP.Web.Tools;

using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Controllers
{
    public class ProfilController : Controller
    {
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _Aktif;
        IFormsAuthenticationService _FormsAuthenticationService;
        IKullaniciFotografStorage _Storage;
        IYetkiService _YetkiService;
        ITVMService _TVMService;

        public ProfilController(IKullaniciService kullaniciService,
                                IAktifKullaniciService aktif,
                                IKullaniciFotografStorage storage,
                                IFormsAuthenticationService formService,
                                IYetkiService yetki,
                                ITVMService tvmService)
        {
            _KullaniciService = kullaniciService;
            _Aktif = aktif;
            _TVMService = tvmService;
            _Storage = storage;
            _YetkiService = yetki;
            _FormsAuthenticationService = formService;
        }


        [Authorization(AnaMenuKodu = 0)]
        public ActionResult Detay()
        {
            ProfilModel model = new ProfilModel();
            if (_Aktif != null)
            {
                ViewBag.SifreDegistirebilir = true;
                //if (_Aktif.ProjeKodu != TVMProjeKodlari.Aegon)
                //{
                //    ViewBag.SifreDegistirebilir = true;
                //}

                TVMKullanicilar kullanici = _KullaniciService.GetKullanici(_Aktif.KullaniciKodu);

                if (kullanici != null)
                {
                    TVMDetay tvm = _TVMService.GetDetay(_Aktif.TVMKodu);


                    model.TCKN = kullanici.TCKN;
                    model.KullaniciKodu = kullanici.KullaniciKodu;
                    model.Adi = kullanici.Adi;
                    model.Soyadi = kullanici.Soyadi;
                    model.Email = kullanici.Email;
                    model.KayitTarihi = kullanici.KayitTarihi.Value;
                    model.YetkiGrubu = _YetkiService.GetYetkiGrupAdi(kullanici.YetkiGrubu);
                    model.URL = kullanici.FotografURL;
                    model.TVMAdi = tvm.Unvani;
                    model.MTKodu = kullanici.MTKodu;

                    if (kullanici.DepartmanKodu.HasValue)
                    {
                        TVMDepartmanlar departman = _TVMService.GetTVMDepartman(kullanici.DepartmanKodu.Value, kullanici.TVMKodu);
                        if (departman != null)
                            model.Departman = departman.Adi;
                    }

                    switch (kullanici.Gorevi)
                    {
                        case KullaniciGorevTipleri.Yonetici: model.Gorevi = babonline.Manager; break;
                        case KullaniciGorevTipleri.YoneticiYardimcisi: model.Gorevi = babonline.AsistantManager; break;
                        case KullaniciGorevTipleri.Personel: model.Gorevi = babonline.Personel; break;
                        default: model.Gorevi = String.Empty; break;
                    }

                    if (kullanici.YoneticiKodu.HasValue)
                    {
                        TVMKullanicilar yonetici = _KullaniciService.GetKullanici(kullanici.YoneticiKodu.Value);
                        if (yonetici != null)
                            model.Yonetici = yonetici.Adi + " " + yonetici.Soyadi;
                    }


                    //Şifre Geçerlilik Suresi belirtiliyor..
                    model.SifreGecerlilikSuresi = tvm.SifreDegistirmeGunu;
                    short gecerlilikGunu = tvm.SifreDegistirmeGunu;
                    DateTime? SifreTarihi = kullanici.SifreTarihi;

                    if (SifreTarihi.HasValue)
                    {
                        TimeSpan fark = TurkeyDateTime.Now - SifreTarihi.Value;
                        model.SifreGecerlilikSuresi = gecerlilikGunu - (int)fark.TotalDays;
                    }
                }
            }
            else
                return new RedirectResult("~/Error/ErrorPage/403");

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SifreDegistir()
        {
            SifreDegistirModel model = new SifreDegistirModel();


            switch (_Aktif.ProjeKodu)
            {
                case TVMProjeKodlari.Aegon: return View("AegonSifreDegistir", model);
                case TVMProjeKodlari.Mapfre: return View("MapfreSifreDegistir", model);
                case TVMProjeKodlari.Lilyum: return View("LilyumSifreDegistir", model);
                default: return View(model);
            }
        }

        [HttpPost]
        // [AjaxException]
        [AllowAnonymous]
        public ActionResult SifreDegistir(SifreDegistirModel model)
        {
            if (ModelState.IsValid)
            {
                if (_Aktif != null)
                {
                    if (model.YeniSifre == model.YeniSifreTekrar)
                    {
                        if (model.YeniSifre == model.EskiSifre)
                        {
                            ModelState.AddModelError("", babonline.Message_Password_OldNew_Different);

                            switch (_Aktif.ProjeKodu)
                            {
                                case TVMProjeKodlari.Aegon: return View("~/Areas/TVM/Views/Shared/AegonSifreDegistir.cshtml", model);
                                case TVMProjeKodlari.Mapfre: return View("~/Areas/TVM/Views/Shared/MapfreSifreDegistir.cshtml", model);
                                case TVMProjeKodlari.Lilyum: return View("~/Areas/TVM/Views/Shared/LilyumSifreDegistir.cshtml", model);
                                default: return View(model);
                            }
                        }

                        TVMKullanicilar kullanici = _KullaniciService.GetKullanici(_Aktif.KullaniciKodu);

                        if (kullanici != null)
                        {
                            string gelenSifre = Encryption.HashPassword(model.EskiSifre);

                            if (kullanici.Sifre != gelenSifre)
                            {
                                ModelState.AddModelError("", babonline.Message_PasswordOld_Wrong);

                                switch (_Aktif.ProjeKodu)
                                {
                                    case TVMProjeKodlari.Aegon: return View("~/Areas/TVM/Views/Shared/AegonSifreDegistir.cshtml", model);
                                    case TVMProjeKodlari.Mapfre: return View("~/Areas/TVM/Views/Shared/MapfreSifreDegistir.cshtml", model);
                                    case TVMProjeKodlari.Lilyum: return View("~/Areas/TVM/Views/Shared/LilyumSifreDegistir.cshtml", model);
                                    default: return View(model);
                                }
                            }
                            else
                            {
                                kullanici.Sifre = Encryption.HashPassword(model.YeniSifre);
                                kullanici.SifreTarihi = TurkeyDateTime.Now;
                                kullanici.SifreDurumKodu = 0;
                                _KullaniciService.UpdateKullanici(kullanici);

                                //Kullanıcının Şifre Logları Tutuluyor...Kaydediliyor...
                                TVMKullaniciSifreTarihcesi sifreTarihcesi = new TVMKullaniciSifreTarihcesi();

                                sifreTarihcesi.DegistirmeTarihi = TurkeyDateTime.Now;
                                sifreTarihcesi.OncekiSifre = Encryption.HashPassword(model.EskiSifre);
                                sifreTarihcesi.YeniSifre = Encryption.HashPassword(model.YeniSifre);
                                sifreTarihcesi.TVMKodu = _Aktif.TVMKodu;
                                sifreTarihcesi.TVMKullaniciKodu = _Aktif.KullaniciKodu;
                                _KullaniciService.CreateSifreTarihcesi(sifreTarihcesi);

                                _Aktif.Logout();
                                _FormsAuthenticationService.SignOut();
                                if (kullanici.EmailOnayKodu != null)
                                {
                                    if (_Aktif.ProjeKodu == TVMProjeKodlari.Lilyum)
                                    {
                                        return RedirectToAction("LilyumKart", "Account", new { area = "" });
                                    }
                                    else
                                    {
                                        return RedirectToAction("SigortaliGiris", "Account", new { area = "" });
                                    }

                                }
                                return RedirectToAction("Detay", "Profil", new { Id = _Aktif.KullaniciKodu });
                            }
                        }
                        else
                        {
                            switch (_Aktif.ProjeKodu)
                            {
                                case TVMProjeKodlari.Aegon: return RedirectToAction("Login", "Aegon", new { area = "" });
                                case TVMProjeKodlari.Mapfre: return RedirectToAction("Login", "Mapfre", new { area = "" });
                                case TVMProjeKodlari.Lilyum: return RedirectToAction("LilyumKart", "Account", new { area = "" });
                                default: return RedirectToAction("Login", "Account", new { area = "" });
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Message_PasswordNew_Confirm);

                        switch (_Aktif.ProjeKodu)
                        {
                            case TVMProjeKodlari.Aegon: return View("~/Areas/TVM/Views/Shared/AegonSifreDegistir.cshtml", model);
                            case TVMProjeKodlari.Mapfre: return View("~/Areas/TVM/Views/Shared/MapfreSifreDegistir.cshtml", model);
                            case TVMProjeKodlari.Lilyum: return View("~/Areas/TVM/Views/Shared/LilyumSifreDegistir.cshtml", model);
                            default: return View(model);
                        }
                    }
                }
                model.YeniSifreTekrar = String.Empty;
                model.YeniSifre = String.Empty;
                model.EskiSifre = String.Empty;
            }
            switch (_Aktif.ProjeKodu)
            {
                case TVMProjeKodlari.Aegon: return View("~/Areas/TVM/Views/Shared/AegonSifreDegistir.cshtml", model);
                case TVMProjeKodlari.Mapfre: return View("~/Areas/TVM/Views/Shared/MapfreSifreDegistir.cshtml", model);
                case TVMProjeKodlari.Lilyum: return View("~/Areas/TVM/Views/Shared/LilyumSifreDegistir.cshtml", model);
                default: return View(model);
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim)]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPhoto(ProfilModel model, HttpPostedFileBase file)
        {
            string url = "";
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                string GUID = Guid.NewGuid().ToString();
                url = _Storage.UploadFile(GUID, "", file.InputStream);

                if (model.KullaniciKodu > 0 && _KullaniciService.KullaniciYetkiKontrolu(model.KullaniciKodu))
                {
                    TVMKullanicilar kullanici = _KullaniciService.GetKullanici(model.KullaniciKodu);
                    if (kullanici != null)
                    {
                        kullanici.FotografURL = url;

                        _KullaniciService.UpdateKullanici(kullanici);
                    }
                }
            }

            return RedirectToAction("Detay");
        }
    }
}
