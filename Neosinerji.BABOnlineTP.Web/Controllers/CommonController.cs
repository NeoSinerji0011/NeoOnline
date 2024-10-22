using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Web.Areas.TaliAcente.Models;
using System.Security.Cryptography;
using static Neosinerji.BABOnlineTP.Business.Common.KORU.KORUCommon;
//using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;

namespace Neosinerji.BABOnlineTP.Web.Controllers
{
    public class CommonController : Controller
    {


        public CommonController()
        {

        }

        #region İl ve İlçeler

        public ActionResult IlleriGetir(string UlkeKodu)
        {
            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            List<Il> iller = _UlkeService.GetIlList(UlkeKodu).ToList<Il>();
            IlListModel model = new IlListModel();
            Mapper.CreateMap<Il, IlModel>();
            model.Items = Mapper.Map<List<Il>, List<IlModel>>(iller);

            return Json(new SelectList(model.Items, "IlKodu", "IlAdi").ListWithOptionLabelIller(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult IlceleriGetir(string UlkeKodu, string IlKodu)
        {
            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            List<Ilce> ilceler = _UlkeService.GetIlceList(UlkeKodu, IlKodu).ToList<Ilce>();
            IlceListModel model = new IlceListModel();
            Mapper.CreateMap<Ilce, IlceModel>();
            model.Items = Mapper.Map<List<Ilce>, List<IlceModel>>(ilceler);

            return Json(new SelectList(model.Items, "IlceKodu", "IlceAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Meslek
        public ActionResult MeslekGetir(string query)
        {
            ITanimService _TanimService = DependencyResolver.Current.GetService<ITanimService>();
            List<Meslek> meslekler = _TanimService.GetListMeslek(query);

            JsonOptions options = JsonHelper.GetOptions(meslekler, "MeslekAdi");

            return Json(options, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Araç Marka ve Tip

        public ActionResult AracTarzGetir(string KullanimSekliKodu)
        {
            short kullanimSekli = Convert.ToInt16(KullanimSekliKodu);
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();
            List<AracKullanimTarziServisModel> tarzlar = _AracService.GetAracKullanimTarziTeklif(kullanimSekli);

            return Json(new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AracMarkaGetir(string KullanimTarziKodu)
        {
            string[] parts = KullanimTarziKodu.Split('-');
            KullanimTarziKodu = parts[0];
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();
            List<AracMarka> markalar = _AracService.GetAracMarkaList(KullanimTarziKodu);

            return Json(new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AracTipiGetir(string KullanimTarziKodu, string MarkaKodu, string Model)
        {
            string[] parts = KullanimTarziKodu.Split('-');
            int model = Convert.ToInt32(Model);
            KullanimTarziKodu = parts[0];
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();
            List<AracTip> tipler = _AracService.GetAracTipList(KullanimTarziKodu, MarkaKodu, model);

            return Json(new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModelleriGetir(string MarkaKodu)
        {
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();
            List<AracTip> modeller = _AracService.GetAracTipList(MarkaKodu);

            return Json(new SelectList(modeller, "TipKodu", "TipAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Trafik IMM ve FK
        public ActionResult TrafikIMM(string KullanimTarzi)
        {
            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            List<SelectListItem> model = new List<SelectListItem>();
            string[] aracKullanim = KullanimTarzi.Split('-');
            model = new SelectList(_CRService.GetTrafikIMMListe(aracKullanim[0], aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TrafikFK(string KullanimTarzi)
        {
            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            List<SelectListItem> model = new List<SelectListItem>();
            string[] aracKullanim = KullanimTarzi.Split('-');
            model = new SelectList(_CRService.GetTrafikFKListe(aracKullanim[0], aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Kasko IMM ve FK
        public ActionResult KaskoIMM(string KullanimTarzi)
        {
            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            List<SelectListItem> model = new List<SelectListItem>();
            string[] aracKullanim = KullanimTarzi.Split('-');
            model = new SelectList(_CRService.GetKaskoIMMListe(aracKullanim[0], aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult KaskoFK(string KullanimTarzi)
        {
            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            List<SelectListItem> model = new List<SelectListItem>();
            string[] aracKullanim = KullanimTarzi.Split('-');
            model = new SelectList(_CRService.GetKaskoFKListe(aracKullanim[0], aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //#region Kasko IMM ve FK
        //public ActionResult KaskoIMM(string KullanimTarzi)
        //{
        //    ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
        //    List<SelectListItem> model = new List<SelectListItem>();
        //    string[] aracKullanim = KullanimTarzi.Split('-');
        //    model = new SelectList(_CRService.GetKaskoIMMList(TeklifUretimMerkezleri.HDI, aracKullanim[0], aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult KaskoFK(string KullanimTarzi)
        //{
        //    ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
        //    List<SelectListItem> model = new List<SelectListItem>();
        //    string[] aracKullanim = KullanimTarzi.Split('-');
        //    model = new SelectList(_CRService.GetKaskoFKList(TeklifUretimMerkezleri.HDI, aracKullanim[0], aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}
        //#endregion

        #region Tescil ilçeler
        public ActionResult TescilIlceleriGetir(string IlKodu)
        {
            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();

            return Json(new SelectList(_CRService.GetTescilIlceList(IlKodu), "Key", "Value").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Tuzel Musteri Sektor

        public ActionResult SektorleriGetir(string AnaSektor)
        {
            ITanimService _MeslekVeTanimService = DependencyResolver.Current.GetService<ITanimService>();

            List<GenelTanimlar> faaliyetAltSektor = _MeslekVeTanimService.GetListAltSektor(AnaSektor);

            return Json(new SelectList(faaliyetAltSektor, "TanimId", "Aciklama", "0").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Harita

        public ActionResult GetTVMLocation()
        {
            ITVMService _tvmService = DependencyResolver.Current.GetService<ITVMService>();

            return Json(new { model = _tvmService.GetListTVMHarita() });
        }

        public bool SendIletisimFormu(HaritaIletisimFormModel model)
        {
            IEMailService _EmailService = DependencyResolver.Current.GetService<IEMailService>();
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();

            if (ModelState.IsValid)
            {
                if (_EmailService.SendHaritaIletisimForm(model.TVMKodu, model.AdSoyad, model.EMail, model.Telefon))
                    return true;
                else
                {
                    ModelState.AddModelError("", "Girdiğiniz bilgileri kontrol ediniz.");
                    return false;
                }
            }
            ModelState.AddModelError("", "Girdiğiniz bilgileri kontrol ediniz.");
            return false;
        }

        #endregion

        public ActionResult KimlikSorgula(string kimlikNo, int tvmKodu)
        {
            TaliKimlikModel model = new TaliKimlikModel();
            if (String.IsNullOrEmpty(kimlikNo))
            {
                model.SorgulamaHata("Kimlik numarası boş bırakılamaz");
            }
            else if (kimlikNo.Length == 11 || kimlikNo.Length == 10)
            {
                IMusteriService _MusteriService = DependencyResolver.Current.GetService<IMusteriService>(); 
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, tvmKodu);
                if (musteri == null)
                {
                    IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                    KimlikSorguResponse kimlik = mapfreSorgu.KimlikSorgu(NeosinerjiTVM.NeosinerjiTVMKodu, kimlikNo);

                    if (kimlik != null && kimlikNo.Length == 11)
                    {
                        model.Ad = kimlik.vrgAd;
                        model.Soyad = kimlik.vrgSoyAd;
                        model.SorgulamaSonuc = true;
                    }
                    if (kimlik != null && kimlikNo.Length == 10)
                    {
                        model.Ad = kimlik.vrgnvan;
                        model.Soyad = ".";
                        model.SorgulamaSonuc = true;
                    }
                    if (kimlik == null)
                    {

                        model.SorgulamaHata("Kimlik bilgileri alınamadı.");
                        return Json(model);
                    }
                    else if (kimlik != null && !String.IsNullOrEmpty(kimlik.hata))
                    {
                        model.SorgulamaHata(kimlik.hata);
                        return Json(model);
                    }
                    else if (kimlik != null)
                    {
                        if (!String.IsNullOrEmpty(kimlik.vrgTCKimlikNo) && kimlik.vrgTCKimlikNo.Length == 11)
                        {
                            if (!String.IsNullOrEmpty(kimlik.vrgAd) && kimlik.vrgAd.Length > 150)
                            {
                                model.Ad = kimlik.vrgAd.Substring(0, 150);
                            }
                            else
                            {
                                model.Ad = kimlik.vrgAd;
                            }
                            if (!String.IsNullOrEmpty(kimlik.vrgSoyAd) && kimlik.vrgSoyAd.Length > 50)
                            {
                                model.Soyad = kimlik.vrgSoyAd.Substring(0, 50);
                            }
                            else
                            {
                                model.Soyad = kimlik.vrgSoyAd;
                            }
                        }
                        else if (!String.IsNullOrEmpty(kimlik.vrgVergiNo) && kimlikNo.Length == 10)
                        {
                            if (!String.IsNullOrEmpty(kimlik.vrgnvan) && kimlik.vrgnvan.Length > 150)
                            {
                                model.Ad = kimlik.vrgnvan.Substring(0, 150);
                            }
                            else
                            {
                                model.Ad = kimlik.vrgnvan;
                            }
                            model.Soyad = ".";
                        }
                    }
                }
                else
                {
                    model.Ad = musteri.AdiUnvan;
                    model.Soyad = musteri.SoyadiUnvan;
                    model.SorgulamaSonuc = true;
                }
            }
            return Json(model);
        }

        public ActionResult InternetKullaniciAktivasyon(int TVMKodu, string tcVkn, string adSoyad, string email, string telefon)
        {
            KullaniciKayitModel model = new KullaniciKayitModel();

            IKullaniciService _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();

            var KullaniciEmailVarMi = _KullaniciService.GetKullaniciByEmail(email);
            if (KullaniciEmailVarMi == null)
            {

                ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
                IYetkiService _YetkiService = DependencyResolver.Current.GetService<IYetkiService>();


                var tvmdetay = _TVMService.GetListTVMDetayInternet(TVMKodu);
                if (tvmdetay != null && tvmdetay.Tipi == TVMTipleri.Internet)
                {
                    var tvmdepartmanlar = _TVMService.GetListDepartmanlar(tvmdetay.Kodu);
                    var tvmYetkiler = _YetkiService.GetListYetkiGrupByTVMKodu(tvmdetay.Kodu);
                    int tvmYetkiKod = 0;
                    if (tvmYetkiler != null)
                    {
                        var yetkis = tvmYetkiler.Where(s => s.YetkiGrupAdi == "İnternet").FirstOrDefault();
                        if (yetkis != null)
                        {
                            tvmYetkiKod = yetkis.YetkiGrupKodu;
                        }
                    }
                    if (tvmdepartmanlar != null)
                    {
                        if (_KullaniciService.KullaniciEkleTest(tcVkn))
                        {
                            TVMKullanicilar kullanici = new TVMKullanicilar();
                            string[] parts = adSoyad.Split(' ');
                            kullanici.TVMKodu = tvmdetay.Kodu;
                            kullanici.TCKN = tcVkn;
                            kullanici.Adi = parts[0];
                            kullanici.Soyadi = parts[1];
                            kullanici.Durum = KullaniciDurumTipleri.Aktif;
                            kullanici.DepartmanKodu = tvmdepartmanlar.FirstOrDefault().DepartmanKodu;
                            kullanici.CepTelefon = telefon;
                            kullanici.Email = email;
                            kullanici.TeklifPoliceUretimi = 1;
                            kullanici.Durum = KullaniciDurumTipleri.Pasif;
                            kullanici.Gorevi = KullaniciGorevTipleri.Personel;
                            kullanici.YetkiGrubu = tvmYetkiKod;

                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] veri = new byte[5];
                            rng.GetBytes(veri);

                            string aktivasyonKodu = veri[0].ToString() + veri[1].ToString() + veri[2].ToString() + veri[3].ToString() + veri[4].ToString();

                            kullanici.EmailOnayKodu = aktivasyonKodu;
                            kullanici = _KullaniciService.CreateKullanici(kullanici);
                            model.KayitYapildiMi = true;
                            model.KayitMesaj =
                            "Sn. " + adSoyad + ", NeoOnline sisteminden teklif ve poliçe düzenleyebilmeniz için sisteme kullanıcı kaydınız yapılmıştır." +
                            "NeoOnline sistemine bağlanıp işlem yapabilmeniz için E-Posta Adresinize Kullanıcı Bilgileriniz gönderilmiştir.E-posta Adresinizde" +
                            "Gelen veya SPAM(Gereksiz E-Posta) mesaj kutularınızı kontrol ederek kullanıcı adı ve şifreniz ile sisteme giriş yapabilirsiniz.";
                            return Json(model);
                        }
                        model.KayitYapildiMi = false;
                        model.KayitMesaj =
                        "Sn. " + adSoyad + ", Bu kullanıcı daha önce kaydedildi";
                        return Json(model);
                    }
                    else
                    {
                        model.KayitYapildiMi = false;
                        model.KayitMesaj =
                        "Sn. " + adSoyad + ", seçmiş olduğunuz acente bilgilerinde eksiklik olduğundan dolayı işleminiz iptal edilmiştir. Harita üzerindeki acentelerden herhangi birini seçerek işleminize devam edebilir veya seçmiş olduğunuz acente ile iletişim kurabilmek için ilgili acenteyi seçerek Acente Benimle İletişime Geçsin butonunu kullanabilirsiniz.";
                        return Json(model);
                    }
                }
                else
                {
                    model.KayitYapildiMi = false;
                    model.KayitMesaj =
                    "Sn. " + adSoyad + ", seçmiş olduğunuz acente Teklif/Poliçe işlemine izin vermemektedir. Harita üzerindeki acentelerden herhangi birini seçerek işleminize devam edebilir veya seçmiş olduğunuz acente ile iletişim kurabilmek için ilgili acenteyi seçerek Acente Benimle İletişime Geçsin butonunu kullanabilirsiniz.";
                    return Json(model);
                }
            }
            else
            {
                model.KayitYapildiMi = false;
                model.KayitMesaj =
                "Sn. " + adSoyad + ", Girmiş olduğunuz mail adresi başka kullanıcı tarafından sistemde kullanılmaktadır. Lütfen başka mail adresi giriniz.";
                return Json(model);
            }
        }

        public ActionResult Verify(string EmailKontrolKodu)
        {
            try
            {
                IKullaniciService _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();
                ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                var kullanici = _KullaniciService.GetKullaniciEmailKontorl(EmailKontrolKodu);
                kullanici.Durum = KullaniciDurumTipleri.Aktif;

                _KullaniciService.UpdateKullanici(kullanici);

                var tvmDetay = _TvmService.GetDetay(kullanici.TVMKodu);
                if (tvmDetay != null)
                {
                    if (tvmDetay.ProjeKodu == TVMProjeKodlari.Lilyum)
                    {
                        return RedirectToAction("LilyumKart", "Account");
                    }
                }

                return RedirectToAction("SigortaliGiris", "Account");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Bir hata oluştu.");
                throw;
            }
        }
        [HttpPost]
        public ActionResult LilyumInternetMusteriKayit(string tcVkn, string adSoyad, string email, string telefon)
        {
            KullaniciKayitModel model = new KullaniciKayitModel();
            if (!String.IsNullOrEmpty(tcVkn) && !String.IsNullOrEmpty(adSoyad) && !String.IsNullOrEmpty(email))
            {
                IKullaniciService _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();

                var KullaniciVarMi = _KullaniciService.KullaniciVarmi(tcVkn, email);
                if (!KullaniciVarMi)
                {
                    ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
                    IYetkiService _YetkiService = DependencyResolver.Current.GetService<IYetkiService>();

                    //var tvmdetay = NeosinerjiTVM.LilyumInternetSatisTVMKodu;
                    //var tvmKodu = NeosinerjiTVM.LilyumInternetSatisNeoTVMKodu;
                    //var departmanKodu = 0; //İNTERNET DEPARTMANI
                    //int tvmYetkiKod = 1185; //Lilyum Acentesi İnternet Yetki Grup Kodu Sabit Kullanılacak

                    //Canlı da bunlar açılacak
                    var departmanKodu = LilyumKullaniciTanimlari.LilyumInternetDepartmanKodu;
                    var tvmKodu = LilyumKullaniciTanimlari.LilyumInternetTvmKodu;
                    int tvmYetkiKod = LilyumKullaniciTanimlari.LilyumInternetYetkiKodu;

                    TVMKullanicilar kullanici = new TVMKullanicilar();
                    string[] parts = adSoyad.Split(' ');
                    kullanici.TVMKodu = tvmKodu;
                    kullanici.TCKN = tcVkn;
                    if (parts.Count() == 2)
                    {
                        kullanici.Adi = parts[0];
                        kullanici.Soyadi = parts[1];
                    }
                    else if (parts.Count() == 3)
                    {
                        kullanici.Adi = parts[0] + " " + parts[1];
                        kullanici.Soyadi = parts[2];
                    }
                    else
                    {
                        kullanici.Adi = parts[0];
                        kullanici.Soyadi = "";
                    }

                    kullanici.Durum = KullaniciDurumTipleri.Aktif;
                    kullanici.DepartmanKodu = departmanKodu;
                    kullanici.CepTelefon = telefon;
                    kullanici.Email = email;
                    kullanici.TeklifPoliceUretimi = 1;
                    kullanici.Gorevi = KullaniciGorevTipleri.Personel;
                    kullanici.YetkiGrubu = tvmYetkiKod;

                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] veri = new byte[5];
                    rng.GetBytes(veri);

                    string aktivasyonKodu = veri[0].ToString() + veri[1].ToString() + veri[2].ToString() + veri[3].ToString() + veri[4].ToString();

                    kullanici.EmailOnayKodu = aktivasyonKodu;
                    kullanici = _KullaniciService.CreateKullanici(kullanici);
                    model.KayitYapildiMi = true;
                    model.KayitMesaj =
                    "Sn. " + adSoyad + ", kullanıcı kaydınız yapılmıştır. Lütfen e-posta adresinize gelen aktivasyon işlemini onaylayınız.(E-posta adresinizde gelen veya spam(gereksiz e-posta) mesaj kutunuzu kontrol ediniz.)";
                    return Json(model);
                }
                else
                {
                    model.KayitYapildiMi = false;
                    model.KayitMesaj =
                    "Sn. " + adSoyad + ", Girmiş olduğunuz bilgiler başka kullanıcı tarafından sistemde kullanılmaktadır. Lütfen girdiğiniz bilgileri kontrol ediniz.";
                    return Json(model);
                }
            }
            else
            {
                model.KayitYapildiMi = false;
                model.KayitMesaj =
                "Lütfen zorunlu alanları doldurunuz.";
                return Json(model);
            }
        }

    }
}