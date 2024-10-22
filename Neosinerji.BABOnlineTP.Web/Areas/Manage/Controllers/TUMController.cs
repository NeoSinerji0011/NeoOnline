using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Tools;
using System.IO;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.TUMyonetimi, SekmeKodu = AltMenuSekmeler.TeklifUretmeMerkeziTUM)]
    public class TUMController : Controller
    {

        ITUMService _TUMService;
        IUlkeService _UlkeService;
        IBransService _BransService;
        IUrunService _UrunService;
        IKullaniciFotografStorage _LogoStorage;
        ITVMDokumanStorage _Storage;
        IAktifKullaniciService _AktifKullaniciService;
        ILogService _Log;
        IBankaSubeleriService _BankaSubeleri;

        public TUMController(ITUMService tumService, IUlkeService ulkeService, IAktifKullaniciService aktifKullanici, BransService bransService, UrunService urunService, IKullaniciFotografStorage logostorage, ITVMDokumanStorage storage)
        {
            _TUMService = tumService;
            _UlkeService = ulkeService;
            _BransService = bransService;
            _UrunService = urunService;
            _AktifKullaniciService = aktifKullanici;
            _LogoStorage = logostorage;
            _Storage = storage;
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _BankaSubeleri = DependencyResolver.Current.GetService<IBankaSubeleriService>();
        }

        #region TUM Genel Metodlar
        public ActionResult Liste()
        {
            TUMListeModel model = new TUMListeModel();

            return View(model);
        }

        public ActionResult Detay(int id)
        {
            TUMDetay tum = _TUMService.GetDetay(id);

            Mapper.CreateMap<TUMDetay, TUMDetayModel>();
            TUMDetayModel model = Mapper.Map<TUMDetayModel>(tum);

            model.BaglantiSiniriText = TUMListProvider.GetVarYok(tum.BaglantiSiniri);

            Ulke ulke = _UlkeService.GetUlke(tum.UlkeKodu);
            model.UlkeAdi = (ulke == null ? string.Empty : ulke.UlkeAdi);

            Il il = _UlkeService.GetIl(tum.UlkeKodu, tum.IlKodu);
            model.IlAdi = (il == null ? string.Empty : il.IlAdi);

            if (tum.IlceKodu != null)
            {
                Ilce ilce = _UlkeService.GetIlce(tum.IlceKodu.Value);
                model.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
            }
            else
            {
                model.IlceAdi = string.Empty;
            }


            model.BankaHesaplariList = BankaHesaplariList(id);
            model.DokumanlariList = DokumanList(id);
            model.IletisimYetkilileriList = IletisimYetkilileriList(id);
            model.IPBaglantilariList = BaglantiList(id);
            model.NotlarList = NotlarList(id);
            model.UrunleriList = UrunList(id);

            TUMLogoModel logomodel = new TUMLogoModel();
            logomodel.Kodu = tum.Kodu;
            logomodel.Alt = tum.Unvani;
            logomodel.Src = tum.Logo;
            model.LogoModel = logomodel;

            return View(model);
        }

        public ActionResult ListePager()
        {
            if (Request["sEcho"] != null)
            {
                TUMListe tumListe = new TUMListe(Request, new Expression<Func<TUMDetay, object>>[]
                {
                                                    t => t.Kodu,
                                                    t => t.Unvani
                                                },

                                                t => t.Kodu,
                                                t => t.Unvani,
                                                "/Manage/TUM/Detay/",
                                                "/Manage/TUM/Guncelle/");
                tumListe.Kodu = tumListe.TryParseParamInt("Kodu");
                tumListe.Unvani = tumListe.TryParseParamString("Unvani");

                DataTableList result = _TUMService.PagedList(tumListe);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.TUMyonetimi,
                       SekmeKodu = AltMenuSekmeler.TeklifUretmeMerkeziTUM,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle()
        {
            try
            {
                TUMEkleModel model = new TUMEkleModel();
                model.TUMBaslangicTarihi = TurkeyDateTime.Now;
                model.TUMBitisTarihi = TurkeyDateTime.Now.AddYears(50);

                SetProperties(model);

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
                       AltMenuKodu = AltMenuler.TUMyonetimi,
                       SekmeKodu = AltMenuSekmeler.TeklifUretmeMerkeziTUM,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle(TUMEkleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TUMDetay tum = new TUMDetay();

                    //Genel Bilgiler
                    tum.Durum = 1;
                    tum.DurumGuncellemeTarihi = TurkeyDateTime.Now;
                    tum.TUMBaslangicTarihi = TurkeyDateTime.Now;

                    tum.Kodu = model.Kodu ?? 0;
                    tum.Unvani = model.Unvani;
                    tum.BirlikKodu = model.BirlikKodu;
                    tum.VergiDairesi = model.VergiDairesi;
                    tum.VergiNumarasi = model.VergiNumarasi;
                    tum.UcretlendirmeKodu = model.UcretlendirmeKodu;
                    tum.TUMBitisTarihi = model.TUMBitisTarihi;

                    //İletişim Bİlgileri
                    tum.Telefon = model.Telefon;
                    tum.Fax = model.Fax;
                    tum.Email = model.Email;
                    tum.WebAdresi = model.WebAdresi;

                    //Adres Bilgileri
                    tum.UlkeKodu = model.UlkeKodu;
                    tum.IlKodu = model.IlKodu;
                    tum.IlceKodu = model.IlceKodu;
                    tum.Semt = model.Semt;
                    tum.Adres = model.Adres;

                    //Güvenlik Bilgileri
                    tum.BaglantiSiniri = model.BaglantiSiniri;

                    if (_TUMService.GetListTUMDetay().Count() != 0)
                        tum.Kodu = _TUMService.GetListTUMDetay().Select(s => s.Kodu).Max() + 1;

                    _TUMService.CreateDetay(tum);

                    return RedirectToAction("Detay", "TUM", new { id = tum.Kodu });
                }

                ModelState.AddModelError("", babonline.Message_TVMSaveError);
                SetProperties(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.TUMyonetimi,
                       SekmeKodu = AltMenuSekmeler.TeklifUretmeMerkeziTUM,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                TUMDetay tum = _TUMService.GetDetay(id);
                Mapper.CreateMap<TUMDetay, TUMGuncelleModel>();
                TUMGuncelleModel model = Mapper.Map<TUMDetay, TUMGuncelleModel>(tum);

                model.TUMBaslangicTarihi = tum.TUMBaslangicTarihi.HasValue ? tum.TUMBaslangicTarihi : TurkeyDateTime.Now;
                model.TUMBitisTarihi = tum.TUMBitisTarihi.HasValue ? tum.TUMBitisTarihi : TurkeyDateTime.Now.AddYears(50);

                SetProperties(model);
                TUMLogoModel logomodel = new TUMLogoModel();
                logomodel.Kodu = tum.Kodu;
                logomodel.Alt = tum.Unvani;
                logomodel.Src = tum.Logo;

                model.LogoModel = logomodel;

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
                       AltMenuKodu = AltMenuler.TUMyonetimi,
                       SekmeKodu = AltMenuSekmeler.TeklifUretmeMerkeziTUM,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Guncelle(TUMGuncelleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TUMDetay tum = _TUMService.GetDetay(model.Kodu.Value);

                    tum.DurumGuncellemeTarihi = TurkeyDateTime.Now;
                    tum.Unvani = model.Unvani;
                    tum.BirlikKodu = model.BirlikKodu;
                    tum.VergiDairesi = model.VergiDairesi;
                    tum.VergiNumarasi = model.VergiNumarasi;
                    tum.UcretlendirmeKodu = model.UcretlendirmeKodu;
                    tum.Durum = model.Durum;
                    tum.TUMBaslangicTarihi = model.TUMBaslangicTarihi;
                    tum.TUMBitisTarihi = model.TUMBitisTarihi;
                    tum.UlkeKodu = model.UlkeKodu;
                    tum.IlKodu = model.IlKodu;
                    tum.IlceKodu = model.IlceKodu;
                    tum.Semt = model.Semt;
                    tum.Adres = model.Adres;
                    tum.Telefon = model.Telefon;
                    tum.Fax = model.Fax;
                    tum.Email = model.Email;
                    tum.WebAdresi = model.WebAdresi;
                    tum.BaglantiSiniri = model.BaglantiSiniri;

                    _TUMService.UpdateDetay(tum);

                    return RedirectToAction("Detay", "TUM", new { id = tum.Kodu });
                }

                ModelState.AddModelError("", babonline.Message_TVMSaveError);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        #endregion

        #region List Metodları
        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult BaglantiView(int TUMKodu)
        {
            TUMIPBaglantiListModel model = BaglantiList(TUMKodu);

            return PartialView("_Baglantilar", model);
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public TUMIPBaglantiListModel BaglantiList(int TUMKodu)
        {
            List<TUMIPBaglanti> baglantilar = _TUMService.GetListIPBaglanti(TUMKodu);

            TUMIPBaglantiListModel model = new TUMIPBaglantiListModel();
            Mapper.CreateMap<TUMIPBaglanti, TUMIPBaglantiModel>();
            model.Items = Mapper.Map<List<TUMIPBaglanti>, List<TUMIPBaglantiModel>>(baglantilar);

            return model;

        }

        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult NotView(int TUMKodu)
        {
            return PartialView("_Notlar", NotlarList(TUMKodu));
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public TUMNotlarListModel NotlarList(int TUMKodu)
        {
            List<TUMNotlar> notlar = _TUMService.GetListNotlar(TUMKodu);

            TUMNotlarListModel model = new TUMNotlarListModel();
            Mapper.CreateMap<TUMNotlar, TUMNotlarModel>();
            model.Items = Mapper.Map<List<TUMNotlar>, List<TUMNotlarModel>>(notlar);

            return model;
        }

        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult DokumanView(int TUMKodu)
        {
            return PartialView("_Dokumanlar", DokumanList(TUMKodu));
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public TUMDokumanlarListModel DokumanList(int TUMKodu)
        {
            List<TUMDokumanlar> dokumanlar = _TUMService.GetListDokumanlar(TUMKodu).ToList<TUMDokumanlar>();

            TUMDokumanlarListModel model = new TUMDokumanlarListModel();
            Mapper.CreateMap<TUMDokumanlar, TUMDokumanlarModel>();
            model.Items = Mapper.Map<List<TUMDokumanlar>, List<TUMDokumanlarModel>>(dokumanlar);

            return model;

        }

        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult AcenteView(int TUMKodu)
        {
            return PartialView("_Urunler", UrunList(TUMKodu));
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public TUMUrunleriListModel UrunList(int TUMKodu)
        {
            List<TUMUrunleri> urunler = _TUMService.GetListUrunler(TUMKodu);

            TUMUrunleriListModel model = new TUMUrunleriListModel();
            Mapper.CreateMap<TUMUrunleri, TUMUrunleriModel>();
            model.Items = Mapper.Map<List<TUMUrunleri>, List<TUMUrunleriModel>>(urunler);

            return model;
        }
        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult UrunView(int TUMKodu)
        {
            TUMUrunleriListModel model = new TUMUrunleriListModel();
            model = UrunList(TUMKodu);
            return PartialView("_Urunler", model);
        }

        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult BankaHesapView(int TUMKodu)
        {
            return PartialView("_BankaHesaplari", BankaHesaplariList(TUMKodu));
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public TUMBankaHesaplariListModel BankaHesaplariList(int TUMKodu)
        {
            List<TUMBankaHesaplari> bankahesaplari = _TUMService.GetListTUMBankaHesaplari(TUMKodu);

            TUMBankaHesaplariListModel model = new TUMBankaHesaplariListModel();
            Mapper.CreateMap<TUMBankaHesaplari, TUMBankaHesaplariModel>();
            model.Items = Mapper.Map<List<TUMBankaHesaplari>, List<TUMBankaHesaplariModel>>(bankahesaplari);

            return model;
        }

        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult IletisimYetkiView(int TUMKodu)
        {
            return PartialView("_IletisimYetkilileri", IletisimYetkilileriList(TUMKodu));
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public TUMIletisimYetkilileriListModel IletisimYetkilileriList(int TUMKodu)
        {
            List<TUMIletisimYetkilileri> iletisimYetkilileri = _TUMService.GetListTUMIletisimYetkilileri(TUMKodu);

            TUMIletisimYetkilileriListModel model = new TUMIletisimYetkilileriListModel();
            Mapper.CreateMap<TUMIletisimYetkilileri, TUMIletisimYetkilileriModel>();
            model.Items = Mapper.Map<List<TUMIletisimYetkilileri>, List<TUMIletisimYetkilileriModel>>(iletisimYetkilileri);

            foreach (TUMIletisimYetkilileriModel telefon in model.Items)
            {
                telefon.TelefonTipText = TelefonTipler.TelefonTipleri().Where(w => w.Value == telefon.TelefonTipi.ToString()).First().Text;
            }

            return model;
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public List<Bran> BranslarList()
        {
            return _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
        }
        #endregion

        #region Bağlantı Ekleme
        public ActionResult BaglantiEkle(int TUMKodu)
        {
            TUMIPBaglantiModel model = new TUMIPBaglantiModel();
            model.TUMKodu = TUMKodu;
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_BaglantiEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BaglantiEkle(TUMIPBaglanti model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TUMIPBaglantiModel, TUMIPBaglanti>();
                TUMIPBaglanti baglanti = Mapper.Map<TUMIPBaglanti>(model);

                baglanti.KayitTarihi = TurkeyDateTime.Now;

                if (_TUMService.GetListIPBaglanti(model.TUMKodu).Count() != 0)
                    baglanti.SiraNo = _TUMService.GetListIPBaglanti(model.TUMKodu).Select(s => s.SiraNo).Max() + 1;

                _TUMService.CreateIPBaglanti(baglanti);
                return null;
            }


            return PartialView("_BaglantiEkle", model);
        }

        public ActionResult BaglantiGuncelle(int SiraNo, int tumKodu)
        {
            TUMIPBaglanti baglanti = _TUMService.GetTUMIPBaglanti(SiraNo, tumKodu);

            Mapper.CreateMap<TUMIPBaglanti, TUMIPBaglantiModel>();
            TUMIPBaglantiModel model = Mapper.Map<TUMIPBaglanti, TUMIPBaglantiModel>(baglanti);

            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_BaglantiEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BaglantiGuncelle(TUMIPBaglanti model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TUMIPBaglantiModel, TUMIPBaglanti>();
                TUMIPBaglanti baglanti = Mapper.Map<TUMIPBaglanti>(model);

                baglanti.KayitTarihi = TurkeyDateTime.Now;

                _TUMService.UpdateItem(baglanti);
                return null;
            }


            return PartialView("_BaglantiEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult BaglantiSil(int baglantiKodu, int tumKodu)
        {
            _TUMService.DeleteBaglanti(baglantiKodu, tumKodu);
            TUMIPBaglantiListModel model = BaglantiList(tumKodu);

            return PartialView("_Baglantilar", model);
        }
        #endregion

        #region Not Ekleme
        public ActionResult NotEkle(int TUMKodu)
        {
            TUMNotlarModel model = new TUMNotlarModel();
            model.TUMKodu = TUMKodu;

            return PartialView("_NotEkle", model);
        }

        public ActionResult NotOku(int SiraNo, int tumKodu)
        {
            TUMNotlar not = _TUMService.GetTUMNot(SiraNo, tumKodu);

            Mapper.CreateMap<TUMNotlar, TUMNotlarModel>();
            TUMNotlarModel model = Mapper.Map<TUMNotlar, TUMNotlarModel>(not);

            return PartialView("_NotEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NotEkle(TUMNotlarModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TUMNotlarModel, TUMNotlar>();
                TUMNotlar not = Mapper.Map<TUMNotlar>(model);

                not.EkleyenPersonelKodu = 1;
                not.EklemeTarihi = TurkeyDateTime.Now;

                if (_TUMService.GetListNotlar(model.TUMKodu).Count() != 0)
                    not.SiraNo = _TUMService.GetListNotlar(model.TUMKodu).Select(s => s.SiraNo).Max() + 1;

                _TUMService.CreateNot(not);
                return null;
            }


            return PartialView("_NotEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult NotSil(int notKodu, int tumKodu)
        {
            _TUMService.DeleteNot(notKodu, tumKodu);
            TUMNotlarListModel model = NotlarList(tumKodu);

            return PartialView("_Notlar", model);
        }
        #endregion

        #region Dokuman Ekleme
        public ActionResult Dokuman(int TUMKodu)
        {
            TUMDokumanlarModel model = new TUMDokumanlarModel();
            model.TUMKodu = TUMKodu;

            return PartialView("_DokumanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult Dokuman(TUMDokumanlarModel model, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid && file != null && file.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);

                    if (_TUMService.CheckedFileName(fileName))
                    {
                        string url = _Storage.UploadFile(model.TUMKodu.ToString(), fileName, file.InputStream);
                        TUMDokumanlar dokuman = new TUMDokumanlar();
                        dokuman.TUMKodu = model.TUMKodu;
                        //Tvm ile ilgili bililer otomatik gelicek....
                        dokuman.EkleyenPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                        dokuman.EklemeTarihi = TurkeyDateTime.Now;

                        // dokuman.DokumanAdi = fileName;
                        dokuman.DokumanTuru = model.DokumanTuru;
                        dokuman.Dokuman = url;
                        if (_TUMService.GetListDokumanlar(model.TUMKodu).Count() != 0)
                            dokuman.SiraNo = _TUMService.GetListDokumanlar(model.TUMKodu).Select(s => s.SiraNo).Max() + 1;

                        _TUMService.CreateDokuman(dokuman);
                        //Kayıt Başarılı ise detay sayfasına gönderiliyor...
                        return null;
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
                        return PartialView("_DokumanEkle", model);
                    }
                }
                //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
                return PartialView("_DokumanEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
                return PartialView("_DokumanEkle", model);
            }
        }

        #region Dokuman Guncelle Iptal



        //public ActionResult DokumanGuncelle(int SiraNo, int tumKodu)
        //{
        //    TUMDokumanlar dokuman = _TUMService.GetTUMDokuman(SiraNo, tumKodu);

        //    Mapper.CreateMap<TUMDokumanlar, TUMDokumanlarModel>();
        //    TUMDokumanlarModel model = Mapper.Map<TUMDokumanlar, TUMDokumanlarModel>(dokuman);

        //    return PartialView("_DokumanEkle", model);
        //}

        //[HttpPost]
        //[AjaxException]
        //public ActionResult DokumanGuncelle(TUMDokumanlarModel model, HttpPostedFileBase file)
        //{
        //    if (ModelState.IsValid && file.ContentLength > 0)
        //    {
        //        string fileName = System.IO.Path.GetFileName(file.FileName);

        //        if (_TUMService.CheckedFileName(fileName))
        //        {
        //            string url = "";// _Storage.UploadFile(model.TUMKodu.ToString(), fileName, file.InputStream);
        //            TUMDokumanlar dokuman = new TUMDokumanlar();
        //            dokuman.TUMKodu = model.TUMKodu;
        //            //Tvm ile ilgili bililer otomatik gelicek....
        //            dokuman.EkleyenPersonelKodu = 1;
        //            dokuman.EklemeTarihi = TurkeyDateTime.Now;

        //            //dokuman.DokumanAdi = fileName;
        //            dokuman.DokumanTuru = model.DokumanTuru;
        //            dokuman.Dokuman = url;
        //            dokuman.SiraNo = model.SiraNo;

        //            _TUMService.UpdateItem(dokuman);
        //            //Kayıt Başarılı ise detay sayfasına gönderiliyor...
        //            return null;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
        //            return View(model);
        //        }
        //    }
        //    //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
        //    ModelState.AddModelError("", babonline.Message_DocumentSaveError);
        //    return View(model);
        //}

        #endregion

        [HttpPost]
        [AjaxException]
        public ActionResult DokumanSil(int dokumanKodu, int tumKodu)
        {
            _TUMService.DeleteDokuman(dokumanKodu, tumKodu);

            TUMDokumanlarListModel model = DokumanList(tumKodu);
            return PartialView("_Dokumanlar", model);
        }
        #endregion

        #region BankaHesaplari Ekleme
        public ActionResult BankaHesaplariEkle(int TUMKodu)
        {
            TUMBankaHesaplariModel model = new TUMBankaHesaplariModel();
            model.TUMKodu = TUMKodu;


            List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
            List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

            model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
            model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();



            return PartialView("_BankaHesaplariEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BankaHesaplariEkle(TUMBankaHesaplariModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TUMBankaHesaplariModel, TUMBankaHesaplari>();
                TUMBankaHesaplari bankahesap = Mapper.Map<TUMBankaHesaplari>(model);

                if (_TUMService.GetListTUMBankaHesaplari(model.TUMKodu).Count() != 0)
                    bankahesap.SiraNo = _TUMService.GetListTUMBankaHesaplari(model.TUMKodu).Select(s => s.SiraNo).Max() + 1;

                _TUMService.CreateTUMBankaHesap(bankahesap);
                return null;
            }


            return PartialView("_BankaHesaplariEkle", model);
        }

        public ActionResult BankaHesaplariGuncelle(int SiraNo, int tumKodu)
        {
            TUMBankaHesaplari bankahesap = _TUMService.GetTUMBankaHesap(SiraNo, tumKodu);

            Mapper.CreateMap<TUMBankaHesaplari, TUMBankaHesaplariModel>();
            TUMBankaHesaplariModel model = Mapper.Map<TUMBankaHesaplari, TUMBankaHesaplariModel>(bankahesap);


            model.TUMKodu = tumKodu;
            List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
            List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

            model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
            model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();


            return PartialView("_BankaHesaplariEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BankaHesaplariGuncelle(TUMBankaHesaplariModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TUMBankaHesaplariModel, TUMBankaHesaplari>();
                TUMBankaHesaplari bankahesap = Mapper.Map<TUMBankaHesaplari>(model);

                _TUMService.UpdateBankaHesap(bankahesap);
                return null;
            }


            return PartialView("_AcentelikEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult BankaHesaplariSil(int SiraNo, int tumKodu)
        {
            _TUMService.DeleteTUMBankaHesap(SiraNo, tumKodu);
            TUMBankaHesaplariListModel model = BankaHesaplariList(tumKodu);

            return PartialView("_BankaHesaplari", model);
        }
        #endregion

        #region IletisimYetkilileri Ekleme
        public ActionResult IletisimYetkilileriEkle(int TUMKodu)
        {
            TUMIletisimYetkilileriModel model = new TUMIletisimYetkilileriModel();
            model.TUMKodu = TUMKodu;
            model.TelefonTipleri = new SelectList(TelefonTipler.TelefonTipleri(), "Value", "Text", model.TelefonTipi);

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult IletisimYetkilileriEkle(TUMIletisimYetkilileriModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TUMIletisimYetkilileriModel, TUMIletisimYetkilileri>();
                TUMIletisimYetkilileri iletisimYetkili = Mapper.Map<TUMIletisimYetkilileri>(model);

                if (_TUMService.GetListTUMIletisimYetkilileri(model.TUMKodu).Count() != 0)
                    iletisimYetkili.SiraNo = _TUMService.GetListTUMIletisimYetkilileri(model.TUMKodu).Select(s => s.SiraNo).Max() + 1;

                _TUMService.CreateTUMIletisimYetkili(iletisimYetkili);
                return null;
            }


            return PartialView("_IletisimYetkilileriEkle", model);
        }

        public ActionResult IletisimYetkilileriGuncelle(int SiraNo, int tumKodu)
        {
            TUMIletisimYetkilileri iletisimYetkili = _TUMService.GetTUMIletisimYetkili(SiraNo, tumKodu);

            Mapper.CreateMap<TUMIletisimYetkilileri, TUMIletisimYetkilileriModel>();
            TUMIletisimYetkilileriModel model = Mapper.Map<TUMIletisimYetkilileri, TUMIletisimYetkilileriModel>(iletisimYetkili);

            model.TelefonTipleri = new SelectList(TelefonTipler.TelefonTipleri(), "Value", "Text", model.TelefonTipi);

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult IletisimYetkilileriGuncelle(TUMIletisimYetkilileriModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TUMIletisimYetkilileriModel, TUMIletisimYetkilileri>();
                TUMIletisimYetkilileri iletisimYetkili = Mapper.Map<TUMIletisimYetkilileri>(model);

                _TUMService.UpdateIletisimYetkili(iletisimYetkili);
                return null;
            }


            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult IletisimYetkilileriSil(int SiraNo, int tumKodu)
        {
            _TUMService.DeleteTUMIletisimYetkili(SiraNo, tumKodu);
            TUMIletisimYetkilileriListModel model = IletisimYetkilileriList(tumKodu);

            return PartialView("_IletisimYetkilileri", model);
        }
        #endregion

        #region Urun Ekleme
        public JsonResult UrunBul(int UrunKodu)
        {
            Urun urun = _UrunService.GetUrun(UrunKodu);
            JsonResult jresult = new JsonResult();
            jresult.Data = urun;

            return Json(jresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UrunEkle(int TUMKodu)
        {
            TUMUrunleriModel model = new TUMUrunleriModel();
            model.TUMKodu = TUMKodu;

            model.Urunler = new SelectList(_UrunService.GetListUrun(), "UrunKodu", "UrunAdi");
            return PartialView("_UrunEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult UrunEkle(TUMUrunleriModel model)
        {
            TUMUrunleriModel modellist = new TUMUrunleriModel();
            if (ModelState.IsValid)
            {
                TUMUrunleri urun = _TUMService.GetTUMUrun(model.TUMUrunKodu, model.TUMKodu);
                if (urun == null)
                {
                    TUMUrunleri yeniurun = new TUMUrunleri();
                    yeniurun.TUMKodu = model.TUMKodu;
                    yeniurun.BABOnlineUrunKodu = model.BABOnlineUrunKodu;
                    yeniurun.TUMBransKodu = model.TUMBransKodu;
                    yeniurun.TUMBransAdi = model.TUMBransAdi;
                    yeniurun.TUMUrunKodu = model.TUMUrunKodu;
                    yeniurun.TUMUrunAdi = model.TUMUrunAdi;

                    _TUMService.CreateUrun(yeniurun);
                    return null;
                }
                else
                {
                    modellist.TUMKodu = model.TUMKodu;
                    modellist.Urunler = new SelectList(_UrunService.GetListUrun(), "UrunKodu", "UrunAdi");
                    ModelState.AddModelError("", "Bu urun daha önce eklenmiş");
                    return PartialView("_UrunEkle", modellist);
                }
            }
            modellist = new TUMUrunleriModel();
            modellist.TUMKodu = model.TUMKodu;
            modellist.Urunler = new SelectList(_UrunService.GetListUrun(), "UrunKodu", "UrunAdi");
            ModelState.AddModelError("", babonline.Message_RequiredValues);
            return PartialView("_UrunEkle", modellist);
        }

        public ActionResult UrunGuncelle(string TUMUrunKodu, int TUMKodu, int BabOnlineUrunKodu)
        {
            TUMUrunleriModel model = new TUMUrunleriModel();
            if (!String.IsNullOrEmpty(TUMUrunKodu) && TUMKodu > 0 && BabOnlineUrunKodu > 0)
            {
                TUMUrunleri urun = _TUMService.GetTUMUrun(TUMUrunKodu, TUMKodu, BabOnlineUrunKodu);

                model.TUMKodu = TUMKodu;
                model.BABOnlineUrunKodu = urun.BABOnlineUrunKodu;
                model.TUMBransKodu = urun.TUMBransKodu;
                model.TUMBransAdi = urun.TUMBransAdi;
                model.TUMUrunKodu = urun.TUMUrunKodu;
                model.TUMUrunAdi = urun.TUMUrunAdi;
                model.BabOnlineUrunAdi = _UrunService.GetUrun(urun.BABOnlineUrunKodu).UrunAdi;
            }

            return PartialView("_UrunEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult UrunGuncelle(TUMUrunleriModel model)
        {
            if (ModelState.IsValid)
            {
                TUMUrunleri urun = _TUMService.GetTUMUrun(model.TUMUrunKodu, model.TUMKodu, model.BABOnlineUrunKodu);
                urun.TUMBransAdi = model.TUMBransAdi;
                urun.TUMBransKodu = model.TUMBransKodu;
                urun.TUMUrunAdi = model.TUMUrunAdi;
                _TUMService.UpdateItem(urun);
                return null;
            }
            ModelState.AddModelError("", babonline.Message_RequiredValues);
            return PartialView("_BaglantiEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public bool UrunSil(string TUMUrunKodu, int TUMKodu, int BabOnlineUrunKodu)
        {
            _TUMService.DeleteUrun(TUMUrunKodu, TUMKodu, BabOnlineUrunKodu);
            return true;
        }
        #endregion

        #region Logo Yukleme

        [HttpGet]
        [AjaxException]
        public ActionResult LogoEkle(int TUMKodu)
        {
            if (TUMKodu > 0)
            {
                TUMLogoModel model = new TUMLogoModel();
                model.Kodu = TUMKodu;
                return PartialView("_LogoEkle", model);
            }
            return null;
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult LogoEkle(TUMLogoModel model, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && ModelState.IsValid && model.Kodu > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileExtension = Path.GetExtension(fileName);
                if ((fileExtension == ".jpg") || (fileExtension == ".gif") || (fileExtension == ".png"))
                {
                    string guid = Guid.NewGuid().ToString();
                    string url = _LogoStorage.UploadFile(guid + "." + fileExtension, file.InputStream);

                    TUMDetay tum = _TUMService.GetDetay(model.Kodu);
                    tum.Logo = url;
                    _TUMService.UpdateDetay(tum);

                    return null;
                }
                ModelState.AddModelError("", "Yanlızca (jpg,gif,png) türündeki dosyaları ekleyebilirsiniz.");
                return PartialView("_LogoEkle", model);
            }
            ModelState.AddModelError("", "Lütfen uygun formatta bir dosya giriniz.");
            return null;
        }

        public ActionResult LogoView(int TUMKodu)
        {
            if (TUMKodu > 0)
            {
                TUMLogoModel model = new TUMLogoModel();
                TUMDetay tvm = _TUMService.GetDetay(TUMKodu);
                if (tvm != null)
                {
                    model.Kodu = TUMKodu;
                    model.Alt = tvm.Unvani;
                    model.Src = tvm.Logo;
                    return PartialView("_Logo", model);
                }
                return null;
            }
            return null;
        }
        #endregion

        private void SetProperties(TUMEkleModel model)
        {
            List<Ulke> ulkeler = _UlkeService.GetUlkeList();
            List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu);
            List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu);

            model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
            model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
            model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", "0").ListWithOptionLabel();
            model.BaglantiSiniriVarYok = new SelectList(TUMListProvider.VarYokTipleri(), "Value", "Text", model.BaglantiSiniri);
            //model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
        }

        private void SetProperties(TUMGuncelleModel model)
        {
            List<Ulke> ulkeler = _UlkeService.GetUlkeList();
            List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu);
            List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu);

            model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
            model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
            model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", "0").ListWithOptionLabel();
            model.BaglantiSiniriVarYok = new SelectList(TUMListProvider.VarYokTipleri(), "Value", "Text", model.BaglantiSiniri);
            model.Durumlar = new SelectList(DurumListesiAktifPasif.TUMDurumTipleri(), "Value", "Text", model.Durum);

            model.IPBaglantilariList = BaglantiList(model.Kodu.Value);
            model.NotlarList = NotlarList(model.Kodu.Value);
            model.DokumanlariList = DokumanList(model.Kodu.Value);
            model.BankaHesaplariList = BankaHesaplariList(model.Kodu.Value);
            model.IletisimYetkilileriList = IletisimYetkilileriList(model.Kodu.Value);
            model.UrunleriList = UrunList(model.Kodu.Value);
        }
    }
}
