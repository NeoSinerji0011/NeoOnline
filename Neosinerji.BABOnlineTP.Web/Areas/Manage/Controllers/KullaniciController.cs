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
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.KullaniciYonetimi, SekmeKodu = AltMenuSekmeler.KullaniciTanimlama)]
    public class KullaniciController : Controller
    {
        ITVMService _TVMService;
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _AktifKullanici;
        IYetkiService _YetkiService;
        ILogService _Log;

        public KullaniciController(ITVMService tvmService, IKullaniciService kullaniciService, IAktifKullaniciService aktifKullanici, IYetkiService yetkiService)
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
                KullaniciListeEkranModel model = new KullaniciListeEkranModel();
                model.TVMTipleri = TVMListProvider.TVMTipleriListe();
                model.DurumTipleri = KullaniciListeProvider.DurumTipleriListe();

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
                    KullaniciArama arama = new KullaniciArama(Request, new Expression<Func<KullaniciListeModel, object>>[]
                                                                    {
                                                                        t => t.TCKN,
                                                                        t => t.Adi,
                                                                        t => t.Soyadi,
                                                                        t => t.TVMUnvani,
                                                                        t => t.TVMTipiText,
                                                                        t => t.DepartmanAdi,
                                                                        t => t.Email,
                                                                        t => t.DurumText,
                                                                        t=>t.YetkiGrupAdi,
                                                                        t=>t.GoreviText,
                                                                        t=>t.TeknikPersonelKodu
                                                                    },
                                                                    t => t.KullaniciKodu,
                                                                    t => t.TCKN,
                                                                    "/Manage/Kullanici/Detay/",
                                                                    "/Manage/Kullanici/Guncelle/");

                    arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                    arama.TVMTipi = arama.TryParseParamShort("TVMTipi");
                    arama.Adi = arama.TryParseParamString("Adi");
                    arama.Soyadi = arama.TryParseParamString("Soyadi");
                    arama.Email = arama.TryParseParamString("Email");
                    arama.TCKN = arama.TryParseParamString("TCKN");
                    arama.Durum = arama.TryParseParamByte("Durum");
                    arama.TeknikPersonelKodu = arama.TryParseParamString("TeknikPersonelKodu");

                    int totalRowCount = 0;
                    List<KullaniciListeModel> list = _KullaniciService.PagedList(arama, out totalRowCount);

                    foreach (KullaniciListeModel item in list)
                    {
                        item.TVMTipiText = TVMListProvider.GetTVMTipiText(item.TVMTipi);

                        switch (item.Durum)
                        {
                            case KullaniciDurumTipleri.Aktif: item.DurumText = babonline.Active; break;
                            case KullaniciDurumTipleri.Pasif: item.DurumText = babonline.Pasive; break;
                            case KullaniciDurumTipleri.Dondurulmus: item.DurumText = babonline.Suspended; break;
                        }

                        switch (item.Gorevi)
                        {
                            case KullaniciGorevTipleri.Yok: item.GoreviText = "Yok"; break;
                            case KullaniciGorevTipleri.Yonetici: item.GoreviText = "Yönetici"; break;
                            case KullaniciGorevTipleri.YoneticiYardimcisi: item.GoreviText = "Yönetci Yardımcısı"; break;
                            case KullaniciGorevTipleri.Personel: item.GoreviText = "Personel"; break;
                        }
                    }


                    arama.AddFormatter(c => c.DurumText,
                                       c => c.Durum == KullaniciDurumTipleri.Aktif ? String.Format("<span class='label label-success'>{0}</span>", c.DurumText) :
                                                                                     String.Format("<span class='label label-important'>{0}</span>", c.DurumText));
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
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Gorme)]
        public ActionResult Detay(int id)
        {
            try
            {
                if (_KullaniciService.KullaniciYetkiKontrolu(id))
                {
                    TVMKullanicilar kullanici = _KullaniciService.GetKullanici(id);
                    KullaniciModel model = new KullaniciModel();

                    if (kullanici != null)
                        model = ModelConvertor(kullanici);

                    TVMDetay tvm = _TVMService.GetDetay(kullanici.TVMKodu);
                    model.ProjeKodu = tvm.ProjeKodu;

                    return View(model);
                }
                else return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
               AltMenuKodu = AltMenuler.KullaniciYonetimi,
               SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
               menuPermissionType = MenuPermissionType.ALtMenuSekme,
               menuPermission = MenuPermission.Gorme)]
        public ActionResult YetkiKontrol(int id)
        {
            try
            {
                if (_KullaniciService.KullaniciYetkiKontrolu(id))
                {
                    return Content("OK");
                }
                return Content("UNAUTHORIZED");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Content("ERROR");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle()
        {
            try
            {
                KullaniciModel model = new KullaniciModel();

                model.Durum = KullaniciDurumTipleri.Aktif;
                model.DepartmanKodu = 1;
                model.GorevTipleri = KullaniciListeProvider.GorevTipleri();
                model.TeklifPoliceUretimTipleri = KullaniciListeProvider.TeklifPoliceUretimTipleri();
                model.Departmanlar = new List<SelectListItem>();
                model.Departmanlar.Add(new SelectListItem() { Text = babonline.PleaseSelect, Value = "" });
                model.Yetkiler = new List<SelectListItem>();
                model.Yetkiler.Add(new SelectListItem() { Text = babonline.PleaseSelect, Value = "" });

                model.TeklifPoliceUretimi = 1;

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
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Ekleme)]
        public ActionResult Ekle(KullaniciModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // ==== Kullanıcı daha önce kaydedilmişse hata mesajı döndürülüyor ==== //
                    if (!_KullaniciService.KullaniciVarmi(model.TCKN, model.Email))
                    {
                        Mapper.CreateMap<KullaniciModel, TVMKullanicilar>();
                        TVMKullanicilar kullanici = Mapper.Map<TVMKullanicilar>(model);
                        kullanici.Durum = KullaniciDurumTipleri.Aktif;
                        kullanici.DepartmanKodu = model.DepartmanKodu;


                        kullanici = _KullaniciService.CreateKullanici(kullanici);

                        return RedirectToAction("Detay", "Kullanici", new { id = kullanici.KullaniciKodu });
                    }
                    ModelState.AddModelError("", "Bu kullanıcı daha önce kaydedildi");
                }

                // ==== Model valid değil bilgiler dolduruluyor ve hata mesajı giriliyor ==== //
                model.GorevTipleri = KullaniciListeProvider.GorevTipleri();
                model.TeklifPoliceUretimTipleri = KullaniciListeProvider.TeklifPoliceUretimTipleri();
                ModelState.AddModelError("", babonline.Message_TVMUserSaveError);

                // ==== TVM kodu girilmişse eski kayıtlar gönderiliyor ==== //
                if (model.TVMKodu.HasValue)
                {
                    model.Departmanlar = new SelectList(_TVMService.GetListDepartmanlar(model.TVMKodu.Value),
                                                        "DepartmanKodu", "Adi", model.DepartmanKodu).ListWithOptionLabel();
                    model.Yetkiler = new SelectList(_YetkiService.GetListYetkiGrup(model.TVMKodu.Value),
                                                    "YetkiGrupKodu", "YetkiGrupAdi", model.YetkiGrubu).ListWithOptionLabel();
                }
                else
                {
                    model.Departmanlar = new List<SelectListItem>();
                    model.Departmanlar.Add(new SelectListItem() { Text = babonline.PleaseSelect, Value = "" });
                    model.Yetkiler = new List<SelectListItem>();
                    model.Yetkiler.Add(new SelectListItem() { Text = babonline.PleaseSelect, Value = "" });
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
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                KullaniciGuncelleModel model = new KullaniciGuncelleModel();
                if (_KullaniciService.KullaniciYetkiKontrolu(id))
                {
                    TVMKullanicilar kullanici = _KullaniciService.GetKullanici(id);

                    if (kullanici != null)
                    {
                        TVMDetay tvm = _TVMService.GetDetay(kullanici.TVMKodu);
                        if (tvm != null)
                            model.TVMUnvani = tvm.Unvani;

                        if (kullanici.YoneticiKodu.HasValue)
                        {
                            TVMKullanicilar yonetici = _KullaniciService.GetKullanici(kullanici.YoneticiKodu.Value);
                            if (yonetici != null)
                                model.YoneticiAdi = yonetici.Adi + " " + yonetici.Soyadi;
                        }

                        model.KullaniciKodu = kullanici.KullaniciKodu;
                        model.Email = kullanici.Email;
                        model.TCKN = kullanici.TCKN;
                        model.Adi = kullanici.Adi;
                        model.Soyadi = kullanici.Soyadi;
                        model.DepartmanKodu = kullanici.DepartmanKodu ?? -1;
                        model.YetkiGrubu = kullanici.YetkiGrubu;
                        model.Gorevi = kullanici.Gorevi;
                        model.YoneticiKodu = kullanici.YoneticiKodu;
                        model.TeknikPersonelKodu = kullanici.TeknikPersonelKodu;
                        model.Telefon = kullanici.Telefon;
                        model.CepTelefon = kullanici.CepTelefon;
                        model.SkypeNumara = kullanici.SkypeNumara;
                        model.MTKodu = kullanici.MTKodu;
                        model.Durum = kullanici.Durum;
                        model.TeklifPoliceUretimi = kullanici.TeklifPoliceUretimi;
                        model.TVMKodu = kullanici.TVMKodu;
                        //model.APYmi = kullanici.AYPmi;


                        model.DurumTipleri = KullaniciListeProvider.DurumTipleri();
                        model.GorevTipleri = KullaniciListeProvider.GorevTipleri();
                        model.TeklifPoliceUretimTipleri = KullaniciListeProvider.TeklifPoliceUretimTipleri();

                        if (model.TVMKodu.HasValue)
                        {
                            model.Departmanlar = new SelectList(_TVMService.GetListDepartmanlar(model.TVMKodu.Value),
                                                               "DepartmanKodu", "Adi", model.DepartmanKodu).ListWithOptionLabel();
                            model.Yetkiler = new SelectList(_YetkiService.GetListYetkiGrup(_AktifKullanici.TVMKodu),
                                                            "YetkiGrupKodu", "YetkiGrupAdi", model.YetkiGrubu).ListWithOptionLabel();
                        }
                        else
                        {
                            model.Departmanlar = new List<SelectListItem>();
                            model.Yetkiler = new List<SelectListItem>();
                        }
                        return View(model);
                    }
                }
                return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/Errorpage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Guncelle(KullaniciGuncelleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.KullaniciKodu > 0 && _KullaniciService.KullaniciYetkiKontrolu(model.KullaniciKodu.Value))
                    {
                        TVMKullanicilar kullanici = _KullaniciService.GetKullanici(model.KullaniciKodu.Value);

                        if (kullanici != null)
                        {
                            kullanici.Adi = model.Adi;
                            kullanici.Soyadi = model.Soyadi;
                            kullanici.DepartmanKodu = model.DepartmanKodu;
                            kullanici.YetkiGrubu = model.YetkiGrubu;
                            kullanici.Gorevi = model.Gorevi;
                            kullanici.YoneticiKodu = model.YoneticiKodu;
                            kullanici.TeknikPersonelKodu = model.TeknikPersonelKodu;
                            kullanici.Telefon = model.Telefon;
                            kullanici.CepTelefon = model.CepTelefon;
                            kullanici.SkypeNumara = model.SkypeNumara;
                            string kullaniciEmail = kullanici.Email;
                            kullanici.Email = model.Email;
                            kullanici.TCKN = model.TCKN;
                            kullanici.MTKodu = model.MTKodu;
                            kullanici.Durum = model.Durum;
                            kullanici.TeklifPoliceUretimi = model.TeklifPoliceUretimi;
                            // kullanici.AYPmi = model.APYmi;

                            if (!_KullaniciService.KullaniciVarmi(model.TCKN, model.Email) || kullaniciEmail == model.Email)
                            {
                                _KullaniciService.UpdateKullanici(kullanici);

                                return RedirectToAction("Detay", "Kullanici", new { id = kullanici.KullaniciKodu });
                            }
                            ModelState.AddModelError("", "Bu email daha önce kaydedildi");

                        }
                        else return new RedirectResult("~/Error/ErrorPage/403");
                    }
                    else return new RedirectResult("~/Error/ErrorPage/403");
                }

                model.DurumTipleri = KullaniciListeProvider.DurumTipleri();
                model.GorevTipleri = KullaniciListeProvider.GorevTipleri();
                model.TeklifPoliceUretimTipleri = KullaniciListeProvider.TeklifPoliceUretimTipleri();

                if (model.TVMKodu.HasValue)
                {
                    model.Departmanlar = new SelectList(_TVMService.GetListDepartmanlar(model.TVMKodu.Value),
                                                        "DepartmanKodu", "Adi", model.DepartmanKodu).ListWithOptionLabel();
                    model.Yetkiler = new SelectList(_YetkiService.GetListYetkiGrup(model.TVMKodu.Value),
                                                    "YetkiGrupKodu", "YetkiGrupAdi", model.YetkiGrubu).ListWithOptionLabel();
                }
                else
                {
                    model.Departmanlar = new List<SelectListItem>();
                    model.Yetkiler = new List<SelectListItem>();
                }
                ModelState.AddModelError("", babonline.Message_TVMUserSaveError);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/Errorpage/500");
            }
        }

        [HttpPost]
        public ActionResult KullaniciListe(int tvmKodu)
        {
            List<KullaniciOzetModel> model = _KullaniciService.GetKullaniciOzet(tvmKodu);

            return PartialView("_KullaniciListe", model);
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public ActionResult Kilitle(int KullaniciKodu, string action)
        {
            try
            {
                if (_KullaniciService.KullaniciYetkiKontrolu(KullaniciKodu))
                {
                    KullaniciModel model = new KullaniciModel();
                    TVMKullanicilar kullanici = _KullaniciService.GetKullanici(KullaniciKodu);

                    if (kullanici != null)
                    {
                        if (action == "ac")
                            kullanici.Durum = 1;
                        else if (action == "kilitle")
                            kullanici.Durum = 2;

                        _KullaniciService.CreateKullaniciDurum(kullanici, kullanici.Durum);
                        _KullaniciService.UpdateKullanici(kullanici);

                        model = ModelConvertor(kullanici);

                        return PartialView("_DetayPartial", model);
                    }
                    else throw new HttpException(404, "Kullanıcı Bulunamadı"); ;
                }
                else throw new HttpException(403, "Yetkisiz Erişim");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw;
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public bool SifreSifirla(int KullaniciKodu)
        {
            try
            {
                if (KullaniciKodu > 0 && _KullaniciService.KullaniciYetkiKontrolu(KullaniciKodu))
                {
                    TVMKullanicilar kullanici = _KullaniciService.GetKullanici(KullaniciKodu);
                    if (kullanici != null)
                    {
                        return _KullaniciService.RecoverPassword(kullanici);
                    }
                    return false;
                }
                else return false;
            }
            catch (Exception er)
            {
                _Log.Error(er);
                return false;
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.KullaniciYonetimi,
                       SekmeKodu = AltMenuSekmeler.KullaniciTanimlama,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme,
                       menuPermission = MenuPermission.Guncelleme)]
        public bool SifreSifirlaMapfre(int KullaniciKodu)
        {
            try
            {
                if (KullaniciKodu > 0 && _KullaniciService.KullaniciYetkiKontrolu(KullaniciKodu))
                {
                    TVMKullanicilar kullanici = _KullaniciService.GetKullanici(KullaniciKodu);
                    if (kullanici != null)
                    {
                        return _KullaniciService.RecoverPasswordMapfre(kullanici);
                    }
                    return false;
                }
                else return false;
            }
            catch (Exception er)
            {
                _Log.Error(er);
                return false;
            }
        }

        [HttpPost]
        [AjaxException]
        public ActionResult GetPageMenu(int KullaniciKodu)
        {
            try
            {
                if (_KullaniciService.KullaniciYetkiKontrolu(KullaniciKodu))
                {
                    TVMKullanicilar kullanici = _KullaniciService.GetKullanici(KullaniciKodu);
                    if (kullanici != null)
                    {
                        KullaniciModel model = ModelConvertor(kullanici);

                        return PartialView("_PageMenuPartial", model);
                    }
                    else throw new HttpException(404, "Kullanıcı Bulunamadı");
                }
                else throw new HttpException(403, "Yetkisiz Erişim");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw;
            }
        }

        private KullaniciModel ModelConvertor(TVMKullanicilar kullanici)
        {
            KullaniciModel model = new KullaniciModel();

            if (kullanici != null)
            {
                Mapper.CreateMap<TVMKullanicilar, KullaniciModel>();
                model = Mapper.Map<KullaniciModel>(kullanici);

                TVMDetay tvm = _TVMService.GetDetay(kullanici.TVMKodu);
                if (tvm != null)
                    model.TVMUnvani = tvm.Unvani;

                if (kullanici.YoneticiKodu.HasValue)
                {
                    TVMKullanicilar yonetici = _KullaniciService.GetKullanici(kullanici.YoneticiKodu.Value);
                    if (yonetici != null)
                        model.YoneticiAdi = yonetici.Adi + " " + yonetici.Soyadi;
                }

                if (kullanici.DepartmanKodu.HasValue)
                {
                    TVMDepartmanlar departman = _TVMService.GetTVMDepartman(kullanici.DepartmanKodu.Value, kullanici.TVMKodu);
                    if (departman != null)
                        model.DepartmanAdi = departman.Adi;
                }
                //Kullanıcı Yetki Grup Adı Alınıyor...
                model.YetkiAdi = _YetkiService.GetYetkiGrupAdi(kullanici.YetkiGrubu);

                switch (kullanici.Gorevi)
                {
                    case KullaniciGorevTipleri.Yonetici: model.GorevAdi = babonline.Manager; break;
                    case KullaniciGorevTipleri.YoneticiYardimcisi: model.GorevAdi = babonline.AsistantManager; break;
                    case KullaniciGorevTipleri.Personel: model.GorevAdi = babonline.Personel; break;
                    default: model.GorevAdi = String.Empty; break;
                }

                model.Telefon = kullanici.Telefon;
                model.CepTelefon = kullanici.CepTelefon;
                model.SkypeNumara = kullanici.SkypeNumara;

                if (kullanici.TeklifPoliceUretimi == KullaniciTeklifPoliceUretimTipleri.Evet)
                    model.TeklifPoliceUretimiText = babonline.Yes;
                else
                    model.TeklifPoliceUretimiText = babonline.No;

            }

            return model;
        }
    }
}
