using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Areas.TVM.Models;
using System.Globalization;
using System.Text;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer;
using Neosinerji.BABOnlineTP.Business.TaliPolice;
using SmsApi;
using SmsApi.Types;


namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Controllers
{
    [Authorization(AnaMenuKodu = 0)]
    public class TVMController : Controller
    {
        IYetkiService _YetkiService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullanici;
        IMenuService _MenuService;
        ITVMService _TVMService;
        IKullaniciService _KullaniciService;
        IMusteriService _MusteriService;
        ITeklifService _TeklifService;
        IDuyuruService _DuyuruService;
        ITVMContext _TVMContext;
        ILogService _LogService;
        ITaliAcenteTransferService _TaliAcenteTransferService;
        IDenemeTVMService _DenemeTVMService;
        ITaliPoliceService _TaliPoliceService;
        ITVMKullanicilarService _TVMKullanicilarService;
        IBransService _BransService;
        ICommonService _CommonService;
        public TVMController(IYetkiService yetkiService,
                             IFormsAuthenticationService formsAuthenticationService,
                             IAktifKullaniciService aktifkullanici,
                             ITVMService tvmService,
                             IMenuService menuService,
                             IKullaniciService kullaniciService,
                             IMusteriService musteriService,
                             ITeklifService teklifService,
                             IDuyuruService duyuruService,
                             ITVMContext tvmContext,
                             ITaliAcenteTransferService taliAcenteTransferService,
                             IDenemeTVMService denemeTVMService,
                             ILogService logService,
                             ITaliPoliceService taliPoliceService, IBransService bransService,
              ICommonService commonService
            )
        {
            _YetkiService = yetkiService;
            _FormsAuthenticationService = formsAuthenticationService;
            _AktifKullanici = aktifkullanici;
            _MenuService = menuService;
            _TVMService = tvmService;
            _KullaniciService = kullaniciService;
            _MusteriService = musteriService;
            _TeklifService = teklifService;
            _DuyuruService = duyuruService;
            _TVMContext = tvmContext;
            _LogService = logService;
            _TaliAcenteTransferService = taliAcenteTransferService;
            _DenemeTVMService = denemeTVMService;
            _TaliPoliceService = taliPoliceService;
            _BransService = bransService;
            _CommonService = commonService;
        }

        public ActionResult Index(string baslamaT, string bitisT)
        {
            if(_AktifKullanici.TVMKodu== 153008)
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }
            _TVMKullanicilarService = DependencyResolver.Current.GetService<TVMKullanicilarService>();
            var tvm = _AktifKullanici.TVMKodu;
            var kulKodu = _AktifKullanici.KullaniciKodu;
            var tvmKulannici = _TVMKullanicilarService.GetTVMKullanici(kulKodu);
            var getTVMDetay = _TVMService.GetDetay(tvm);

            YonetimModel model = new YonetimModel();
            model.KullaniciNotlari = new List<KullaniciNotListeleModel>();
            model.Duyurular = new List<DuyuruProcedureModel>();

            model.NeoOnlineKokpitMenuYekiliMi = false;
            try
            {
                DateTime baslamaTarihi = TurkeyDateTime.Now.AddDays(-6);
                DateTime bitisTarihi = TurkeyDateTime.Now;

                if (_AktifKullanici != null)
                {
                    if (_AktifKullanici.ProjeKodu == TVMProjeKodlari.Mapfre)
                    {
                        baslamaTarihi = TurkeyDateTime.Today;
                        bitisTarihi = TurkeyDateTime.Now;
                    }

                    //Kullanıcı Notları
                    KullaniciNotlariGetir(model.KullaniciNotlari);

                    if (!String.IsNullOrEmpty(baslamaT) && !String.IsNullOrEmpty(bitisT) && baslamaT.Length > 24 && bitisT.Length > 24)
                    {
                        baslamaTarihi = DateTime.ParseExact(baslamaT.Substring(1, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        bitisTarihi = DateTime.ParseExact(bitisT.Substring(1, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }

                    model.BaslangicTarihi = baslamaTarihi;
                    model.BitisTarihi = bitisTarihi;

                    model.Duyurular = _DuyuruService.GetListDuyuruByTvmId(_AktifKullanici.TVMKodu);
                    model.DuyuruId = 0;
                    if (model.Duyurular != null && model.Duyurular.Count > 0)
                    {
                        var SonDuyuruId = model.Duyurular.Max(s => s.DuyuruId);
                        var SonDuyuruBt = model.Duyurular.Select(s => s.BaslangisTarihi).Max(s => s.Date);
                        var fark = _CommonService.GunFarkikBul(TurkeyDateTime.Today, SonDuyuruBt);
                        if (SonDuyuruId != null)
                        {
                            if (fark == 0 || fark == 1)
                            {
                                model.DuyuruId = SonDuyuruId;
                            }
                        }
                    }

                    Performans Performans = _TVMContext.Performansim(_AktifKullanici.TVMKodu, _AktifKullanici.KullaniciKodu, baslamaTarihi, bitisTarihi);

                    PerformansChart chartModel = new PerformansChart(Performans);

                    model.ToplamMusteri = chartModel.ToplamMusteri;
                    model.ToplamTeklif = chartModel.ToplamTeklif;
                    model.ToplamPolice = chartModel.ToplamPolice;
                    model.PolicelesmeOrani = chartModel.PolicelesmeOrani;

                    foreach (var item in _AktifKullanici.Yetkiler)
                    {
                        if (item.MenuKodu == AltMenuler.OfflineUretimPerformansi && item.Gorme == 1)
                        {
                            model.NeoOnlineKokpitMenuYekiliMi = true;
                        }
                    }

                    ViewBag.JScript = chartModel.ALLJScript;
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }

            if (_AktifKullanici.ProjeKodu == TVMProjeKodlari.Aegon)
                return View("Aegon", model);

            if (getTVMDetay.MobilDogrulama == true && _AktifKullanici.KullaniciKodu != 11191 && _AktifKullanici.BagliOlduguTvmKodu == -9999)
            {
                if (!String.IsNullOrEmpty(tvmKulannici.MobilDogrulamaOnaylandiMi))
                {
                    if (tvmKulannici.MobilDogrulamaOnaylandiMi.Trim() == "NULL")
                    {
                        return new RedirectResult("~/Account/Login");
                    }
                }
                _TVMService.MobilOnayDogrulandi(tvm, kulKodu);
            }


            return View(model);
        }

        public ActionResult NotEkle()
        {
            KullaniciNotEkleModel model = new KullaniciNotEkleModel();

            model.Oncelikler = new SelectList(TVMListProvider.NotOncelikTipleri(), "Value", "Text", "1");

            return PartialView("_NotEklePartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NotEkle(KullaniciNotEkleModel model)
        {
            if (ModelState.IsValid)
            {
                TVMKullaniciNotlar not = new TVMKullaniciNotlar();

                not.Konu = model.Konu;
                not.Aciklama = model.Aciklama;
                not.Oncelik = model.Oncelik;

                _TVMService.CreateKullaniciNot(not);

                List<KullaniciNotListeleModel> notModel = new List<KullaniciNotListeleModel>();
                KullaniciNotlariGetir(notModel);

                return PartialView("_KullaniciNotlarPartial", notModel);
            }
            return null;
        }

        public PartialViewResult NotSil(int NotId)
        {
            if (NotId > 0)
            {
                _TVMService.DeleteKullaniciNot(NotId);
                List<KullaniciNotListeleModel> notModel = new List<KullaniciNotListeleModel>();
                KullaniciNotlariGetir(notModel);

                return PartialView("_KullaniciNotlarPartial", notModel);
            }
            else
                return null;
        }

        private void KullaniciNotlariGetir(List<KullaniciNotListeleModel> notModel)
        {
            //Kullanıcı Notları
            List<TVMKullaniciNotlar> notlar = _TVMService.GetListKullaniciNotlar();
            foreach (var item in notlar)
            {
                KullaniciNotListeleModel mdl = new KullaniciNotListeleModel();

                mdl.NotId = item.KullaniciNotId;
                mdl.Aciklama = item.Aciklama;
                mdl.Konu = item.Konu;
                mdl.Oncelik = TVMListProvider.GetNotOnceligiText(item.Oncelik);
                mdl.BitisTarihi = item.BitisTarihi;

                notModel.Add(mdl);
            }
        }

        public PartialViewResult NotDetay(int NotId)
        {
            KullaniciNotDetayModel model = new KullaniciNotDetayModel();
            TVMKullaniciNotlar not = new TVMKullaniciNotlar();
            not = _TVMService.GetKullaniciNot(NotId);
            if (not != null)
            {
                model.Aciklama = not.Aciklama;
                model.Konu = not.Konu;
                model.EklemeTarihi = not.EklemeTarihi.ToString("dd.MM.yyyy");
                model.NotId = not.KullaniciNotId;

                return PartialView("_NotDetayPartial", model);
            }
            return null;
        }

        [HttpPost]
        public PartialViewResult DuyuruDetay(int DuyuruId)
        {
            if (DuyuruId > 0)
            {
                Duyurular duyuru = _DuyuruService.GetDuyuruAnaSayfa(DuyuruId);

                if (duyuru != null)
                {
                    KullaniciDuyuruDetayModel model = new KullaniciDuyuruDetayModel();
                    model.DuyuruId = duyuru.DuyuruId;
                    model.Konu = duyuru.Konu;
                    model.Aciklama = duyuru.Aciklama;
                    model.BaslamaTarihi = duyuru.BaslangisTarihi.ToString("dd.MM.yyyy");
                    model.BitisTarihi = duyuru.BitisTarihi.ToString("dd.MM.yyyy");

                    return PartialView("_DuyuruDetayPartial", model);
                }
                return null;
            }
            return null;
        }

        public ActionResult TVMDenemeSurum()
        {
            TVMPDenemeEkranModel model = new TVMPDenemeEkranModel();


            return View(model);
        }

        [HttpPost]
        public ActionResult TVMDenemeSurum(TVMPDenemeEkranModel model)
        {
            TVMDetay detay;
            detay = _TaliAcenteTransferService.GetSonTvm(100);

            //int denemeTvmKoduSayac = 99990001;
            int denemeTvmKoduSayac = detay.Kodu;
            do
            {
                denemeTvmKoduSayac++;
            }
            while (_TaliAcenteTransferService.TVMKoduVarMi(denemeTvmKoduSayac));

            TVMDenemeParametreler tvmDenemeParametreModel = new TVMDenemeParametreler();
            tvmDenemeParametreModel.tvmDetay = _TaliAcenteTransferService.GetDetay(NeosinerjiTVM.NeosinerjiTVMKodu);
            tvmDenemeParametreModel.yetkiGrup = _TaliAcenteTransferService.GetTVYetkiGrup(575);// denemesürüm yetkigrupkodu
            tvmDenemeParametreModel.yetkiGrupYetkileri = _TaliAcenteTransferService.GetListTVYetkiGrupYetkileri(tvmDenemeParametreModel.yetkiGrup.YetkiGrupKodu);
            //tvmParametreModel.departmanlar = _TaliAcenteTransferService.GetListDepartmanlar(tvmKod);
            tvmDenemeParametreModel.servisKullanicilar = _TaliAcenteTransferService.GetListTVMWebServisKullanicilari(NeosinerjiTVM.NeosinerjiTVMKodu);
            tvmDenemeParametreModel.urunYetkileri = _TaliAcenteTransferService.GetListTVMUrunYetkileri(NeosinerjiTVM.NeosinerjiTVMKodu);

            TVMDenemeSurum denemTvmModel = new TVMDenemeSurum();
            #region TVM Deneme detay

            denemTvmModel.TVMDetay = new TVMDetay();
            //excelden okunanlar
            denemTvmModel.TVMDetay.Unvani = model.TvmUnvan;
            denemTvmModel.TVMDetay.Email = model.Email;
            //denemTvmModel.TVMDetay.Telefon = item.Telefon;
            //denemTvmModel.TVMDetay.Fax = item.Faks;
            //denemTvmModel.TVMDetay.Adres = item.AcikAdres;
            denemTvmModel.TVMDetay.Kodu = denemeTvmKoduSayac;
            denemTvmModel.TVMDetay.Tipi = 2;
            denemTvmModel.TVMDetay.AcentSuvbeVar = 0;
            denemTvmModel.TVMDetay.Profili = 0;
            denemTvmModel.TVMDetay.BaglantiSiniri = tvmDenemeParametreModel.tvmDetay.BaglantiSiniri;
            denemTvmModel.TVMDetay.BagliOlduguTVMKodu = tvmDenemeParametreModel.tvmDetay.Kodu;
            denemTvmModel.TVMDetay.Banner = tvmDenemeParametreModel.tvmDetay.Banner;
            denemTvmModel.TVMDetay.BolgeKodu = tvmDenemeParametreModel.tvmDetay.BolgeKodu;
            denemTvmModel.TVMDetay.Durum = tvmDenemeParametreModel.tvmDetay.Durum;
            denemTvmModel.TVMDetay.DurumGuncallemeTarihi = tvmDenemeParametreModel.tvmDetay.DurumGuncallemeTarihi;
            //denemTvmModel.TVMDetay.DuyuruTVMs = tvmDenemeParametreModel.tvmDetay.DuyuruTVMs;
            denemTvmModel.TVMDetay.GrupKodu = tvmDenemeParametreModel.tvmDetay.GrupKodu;
            denemTvmModel.TVMDetay.IlceKodu = tvmDenemeParametreModel.tvmDetay.IlceKodu;
            denemTvmModel.TVMDetay.IlKodu = tvmDenemeParametreModel.tvmDetay.IlKodu;
            denemTvmModel.TVMDetay.KayitNo = model.LevhaNo;
            denemTvmModel.TVMDetay.Latitude = tvmDenemeParametreModel.tvmDetay.Latitude;
            denemTvmModel.TVMDetay.Logo = tvmDenemeParametreModel.tvmDetay.Logo;
            denemTvmModel.TVMDetay.Longitude = tvmDenemeParametreModel.tvmDetay.Longitude;
            denemTvmModel.TVMDetay.MuhasebeEntegrasyon = tvmDenemeParametreModel.tvmDetay.MuhasebeEntegrasyon;
            denemTvmModel.TVMDetay.Notlar = tvmDenemeParametreModel.tvmDetay.Notlar;
            denemTvmModel.TVMDetay.ProjeKodu = tvmDenemeParametreModel.tvmDetay.ProjeKodu;
            denemTvmModel.TVMDetay.Semt = tvmDenemeParametreModel.tvmDetay.Semt;
            denemTvmModel.TVMDetay.SifreDegistirmeGunu = tvmDenemeParametreModel.tvmDetay.SifreDegistirmeGunu;
            denemTvmModel.TVMDetay.SifreIkazGunu = tvmDenemeParametreModel.tvmDetay.SifreIkazGunu;
            denemTvmModel.TVMDetay.SifreKontralSayisi = tvmDenemeParametreModel.tvmDetay.SifreKontralSayisi;
            denemTvmModel.TVMDetay.SozlesmeBaslamaTarihi = tvmDenemeParametreModel.tvmDetay.SozlesmeBaslamaTarihi;
            denemTvmModel.TVMDetay.SozlesmeDondurmaTarihi = tvmDenemeParametreModel.tvmDetay.SozlesmeDondurmaTarihi;

            denemTvmModel.TVMDetay.TCKN = tvmDenemeParametreModel.tvmDetay.TCKN;
            denemTvmModel.TVMDetay.UcretlendirmeKodu = tvmDenemeParametreModel.tvmDetay.UcretlendirmeKodu;
            denemTvmModel.TVMDetay.UlkeKodu = tvmDenemeParametreModel.tvmDetay.UlkeKodu;
            denemTvmModel.TVMDetay.VergiDairesi = tvmDenemeParametreModel.tvmDetay.VergiDairesi;
            denemTvmModel.TVMDetay.VergiNumarasi = tvmDenemeParametreModel.tvmDetay.VergiNumarasi;
            denemTvmModel.TVMDetay.WebAdresi = tvmDenemeParametreModel.tvmDetay.WebAdresi;

            #endregion
            #region TVM Yetki Grupları Add

            denemTvmModel.DenemeSurumYetkiGrup = new TVMYetkiGruplari();
            denemTvmModel.DenemeSurumYetkiGrup.TVMKodu = denemeTvmKoduSayac;
            denemTvmModel.DenemeSurumYetkiGrup.YetkiGrupAdi = tvmDenemeParametreModel.yetkiGrup.YetkiGrupAdi;
            denemTvmModel.DenemeSurumYetkiGrup.YetkiSeviyesi = tvmDenemeParametreModel.yetkiGrup.YetkiSeviyesi;
            denemTvmModel.TVMDetay.TVMYetkiGruplaris.Add(denemTvmModel.DenemeSurumYetkiGrup);

            #endregion
            #region tali için urunyetkileri ekleniyor.
            foreach (var urunYetkileri in tvmDenemeParametreModel.urunYetkileri)
            {
                denemTvmModel.DenemeSurumUrunYetki = new TVMUrunYetkileri();
                denemTvmModel.DenemeSurumUrunYetki.AcikHesapTahsilatGercek = urunYetkileri.AcikHesapTahsilatGercek;
                denemTvmModel.DenemeSurumUrunYetki.AcikHesapTahsilatTuzel = urunYetkileri.AcikHesapTahsilatTuzel;
                denemTvmModel.DenemeSurumUrunYetki.BABOnlineUrunKodu = urunYetkileri.BABOnlineUrunKodu;
                denemTvmModel.DenemeSurumUrunYetki.HavaleEntegrasyon = urunYetkileri.HavaleEntegrasyon;
                denemTvmModel.DenemeSurumUrunYetki.KrediKartiTahsilat = urunYetkileri.KrediKartiTahsilat;
                denemTvmModel.DenemeSurumUrunYetki.ManuelHavale = urunYetkileri.ManuelHavale;
                denemTvmModel.DenemeSurumUrunYetki.Police = urunYetkileri.Police;
                denemTvmModel.DenemeSurumUrunYetki.Rapor = urunYetkileri.Rapor;
                denemTvmModel.DenemeSurumUrunYetki.Teklif = urunYetkileri.Teklif;
                denemTvmModel.DenemeSurumUrunYetki.TUMKodu = urunYetkileri.TUMKodu;
                denemTvmModel.DenemeSurumUrunYetki.TUMUrunKodu = urunYetkileri.TUMUrunKodu;
                denemTvmModel.DenemeSurumUrunYetki.TVMKodu = denemeTvmKoduSayac;
                denemTvmModel.TVMDetay.TVMUrunYetkileris.Add(denemTvmModel.DenemeSurumUrunYetki);
            }

            #endregion

            bool kayit = _DenemeTVMService.AddDenemeTVM(denemTvmModel);
            if (kayit == false)
            {
                ViewBag.mesaj = "Kayit Eklenemedi Lütfen Bilgilerinizi Kontrol Ediniz.";
                return View(model);
            }

            #region tali için yetkigrupyetkileri kaydediliyor.


            List<TVMYetkiGrupYetkileri> denemeTVMYetkiList = new List<TVMYetkiGrupYetkileri>();
            foreach (var itemGrupYetkileri in tvmDenemeParametreModel.yetkiGrupYetkileri)
            {
                TVMYetkiGrupYetkileri denemeTVMYetkiler = new TVMYetkiGrupYetkileri();
                //acenteModel.taliYetkiGrupYetkileri = new TVMYetkiGrupYetkileri();
                denemeTVMYetkiler.AltMenuKodu = itemGrupYetkileri.AltMenuKodu;
                denemeTVMYetkiler.AnaMenuKodu = itemGrupYetkileri.AnaMenuKodu;
                denemeTVMYetkiler.Degistirme = itemGrupYetkileri.Degistirme;
                denemeTVMYetkiler.Gorme = itemGrupYetkileri.Gorme;
                denemeTVMYetkiler.SekmeKodu = itemGrupYetkileri.SekmeKodu;
                denemeTVMYetkiler.Silme = itemGrupYetkileri.Silme;
                //taliYetkiler.TVMYetkiGruplari = itemGrupYetkileri.TVMYetkiGruplari;
                denemeTVMYetkiler.YeniKayit = itemGrupYetkileri.YeniKayit;
                denemeTVMYetkiler.YetkiGrupKodu = denemTvmModel.DenemeSurumYetkiGrup.YetkiGrupKodu;
                denemeTVMYetkiList.Add(denemeTVMYetkiler);

            }
            #endregion
            _TaliAcenteTransferService.taliYetkiAdd(denemeTVMYetkiList);
            #region servisKullanıcıları Ekle
            denemTvmModel.tvmWebServisKullanicilari = new List<TVMWebServisKullanicilari>();

            foreach (var servisKullanici in tvmDenemeParametreModel.servisKullanicilar)
            {
                denemTvmModel.DenemeSurumServisKullanici = new TVMWebServisKullanicilari();
                denemTvmModel.DenemeSurumServisKullanici.CompanyId = servisKullanici.CompanyId;
                denemTvmModel.DenemeSurumServisKullanici.KullaniciAdi = servisKullanici.KullaniciAdi;
                denemTvmModel.DenemeSurumServisKullanici.KullaniciAdi2 = servisKullanici.KullaniciAdi2;
                denemTvmModel.DenemeSurumServisKullanici.PartajNo_ = servisKullanici.PartajNo_;
                denemTvmModel.DenemeSurumServisKullanici.Sifre = servisKullanici.Sifre;
                denemTvmModel.DenemeSurumServisKullanici.Sifre2 = servisKullanici.Sifre2;
                denemTvmModel.DenemeSurumServisKullanici.SourceId = servisKullanici.SourceId;
                denemTvmModel.DenemeSurumServisKullanici.SubAgencyCode = servisKullanici.SubAgencyCode;
                denemTvmModel.DenemeSurumServisKullanici.TUMKodu = servisKullanici.TUMKodu;
                denemTvmModel.DenemeSurumServisKullanici.TVMKodu = denemeTvmKoduSayac;
                denemTvmModel.DenemeSurumServisKullanici.Sifre = servisKullanici.Sifre;

                denemTvmModel.tvmWebServisKullanicilari.Add(denemTvmModel.DenemeSurumServisKullanici);
            }
            _TaliAcenteTransferService.taliWebServisKullaniciAdd(denemTvmModel.tvmWebServisKullanicilari);

            #endregion
            #region tali için Kullanıcı oluştur

            string newTck;
            bool kayitVarMi = false;
            do
            {
                newTck = tckUret();
                kayitVarMi = _TaliAcenteTransferService.tckVarMi(newTck);
            }
            while (!kayitVarMi);

            denemTvmModel.tvmKullanici = new TVMKullanicilar()
            {
                Adi = model.TeknikPersonelAd,
                DepartmanKodu = 0,
                Durum = 1,
                Email = denemTvmModel.TVMDetay.Email,
                FotografURL = "https://neoonlinestrg.blob.core.windows.net/musteri-dokuman/346/avatar.png",
                Gorevi = 1,
                HataliSifreGirisSayisi = 0,
                KayitTarihi = TurkeyDateTime.Today,
                Sifre = Encryption.HashPassword(model.LevhaNo),
                SifreDurumKodu = 0,
                SifreTarihi = TurkeyDateTime.Today,
                Soyadi = denemTvmModel.TVMDetay.Unvani.Length > 15 ? denemTvmModel.TVMDetay.Unvani.Substring(15) : ".",
                TCKN = newTck,
                TeklifPoliceUretimi = 1,
                TeknikPersonelKodu = denemeTvmKoduSayac.ToString(),
                Telefon = denemTvmModel.TVMDetay.Telefon,
                TVMKodu = Convert.ToInt32(denemeTvmKoduSayac),
                YetkiGrubu = denemTvmModel.DenemeSurumYetkiGrup.YetkiGrupKodu,
            };

            _TaliAcenteTransferService.taliKullaniciAdd(denemTvmModel.tvmKullanici);

            #endregion

            ViewBag.mesaj = "Kayit Eklendi.";
            return View(model);
        }

        public string tckUret()
        {
            Random r = new Random();
            string randomTc = string.Empty;
            for (int i = 0; i < 11; i++)
            {
                randomTc += r.Next(0, 9).ToString();

            }

            return randomTc;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.OfflineUretimPerformansi, SekmeKodu = 0)]
        public ActionResult OfflineUretimPerformans()
        {
            OfflinePerformansModel model = new OfflinePerformansModel();

            // var tvmler = _TaliPoliceService.GetYetkiliTVM(_AktifKullanici.TVMKodu).OrderBy(s => s.Unvani);
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);

            model.MerkezAcenteMi = false;
            TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                model.tvmKodu = _AktifKullanici.TVMKodu;
                model.MerkezAcenteMi = true;
            }
            else
            {
                model.tvmKodu = tvmDetay.BagliOlduguTVMKodu;
            }
            if (tvmDetay != null && tvmDetay.BolgeYetkilisiMi == 1)
            {
                model.BolgeYetkilisiMi = true;
            }

            model.tvmler = new MultiSelectList(tvmler.OrderBy(s => s.Unvani), "Kodu", "Unvani");

            List<int> yillar = new List<int>();
            for (int i = 2010; i < 2030; i++)
            {
                yillar.Add(i);
            }

            model.donemYil = TurkeyDateTime.Today.Year;
            model.donemler = new SelectList(yillar);

            List<string> TVMListe = new List<string>();
            TVMListe.Add(_AktifKullanici.TVMKodu.ToString());
            OfflineUretimPerformans Performans = _TVMContext.OfflineUretim(model.tvmKodu, model.donemYil, _AktifKullanici.TVMKodu.ToString());

            OfflineUretimChart chartModel = new OfflineUretimChart(Performans);
            model.tvmKodu = _AktifKullanici.TVMKodu;
            model.taliTvmKodu = model.tvmKodu;
            model.donemYil = model.donemYil;
            model.list = chartModel.list;
            foreach (var item in model.list)
            {
                if (item.policeAdetOcak > 0 && item.policeAdetSubat > 0)
                {
                    model.uretimSatir1 = true;
                }
            }
            ViewBag.JScript = chartModel.ALLJScript;


            return View(model);
        }
        public ActionResult Excel()
        {
            return View();
        }
            public ActionResult OfflineUretimPerformansKullanici()
        {
            OfflinePerformansModelKullanici model = new OfflinePerformansModelKullanici();
            List<KullaniciModelForList> kullanicilar = null;
            // var tvmler = _TaliPoliceService.GetYetkiliTVM(_AktifKullanici.TVMKodu).OrderBy(s => s.Unvani);


            IQueryable<TVMDetay> YetkiliTVMler = null;
            model.MerkezAcenteMi = false;
            TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                kullanicilar = _KullaniciService.GetListAktifTVMKullanicilari(_AktifKullanici.TVMKodu);
                YetkiliTVMler = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu);
                if (YetkiliTVMler != null)
                {
                    foreach (var item in YetkiliTVMler)
                    {
                        foreach (var kullanici in _KullaniciService.GetListAktifTVMKullanicilari(item.Kodu))
                        {
                            kullanicilar.Add(kullanici);
                        } 
                    }
                }

                
                model.tvmKodu = _AktifKullanici.TVMKodu;
                model.MerkezAcenteMi = true;
            }
            else
            {
                model.tvmKodu = tvmDetay.BagliOlduguTVMKodu;
                kullanicilar = _KullaniciService.GetListAktifTVMKullanicilari(_AktifKullanici.TVMKodu);
            }
            if (tvmDetay != null && tvmDetay.BolgeYetkilisiMi == 1)
            {

                model.BolgeYetkilisiMi = true;
            }

            model.kullanicilar = new MultiSelectList(kullanicilar.OrderBy(s => s.AdiSoyadi), "KullaniciKodu", "AdiSoyadi");

            List<int> yillar = new List<int>();
            for (int i = 2010; i < 2030; i++)
            {
                yillar.Add(i);
            }

            model.donemYil = TurkeyDateTime.Today.Year;
            model.donemler = new SelectList(yillar);

            List<string> TVMListe = new List<string>();
            TVMListe.Add(_AktifKullanici.TVMKodu.ToString());
            OfflineUretimPerformansKullanici Performans = _TVMContext.OfflineUretimKullanici(model.tvmKodu, model.donemYil, _AktifKullanici.KullaniciKodu.ToString());

            OfflineUretimKullaniciChart chartModel = new OfflineUretimKullaniciChart(Performans);
            model.tvmKodu = _AktifKullanici.TVMKodu;
            model.kullaniciKodu = model.kullaniciKodu;
            model.donemYil = model.donemYil;
            model.list = chartModel.list;
            foreach (var item in model.list)
            {
                if (item.policeAdetOcak > 0 && item.policeAdetSubat > 0)
                {
                    model.uretimSatir1 = true;
                }
            }
            ViewBag.JScript = chartModel.ALLJScript;


            return View(model);
        }
      
        [HttpPost]

        public ActionResult OfflineUretimPerformansKullanici(OfflinePerformansModelKullanici model)
        {
            model.MerkezAcenteMi = false;
            if (model.kullaniciList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.kullaniciList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.KullaniciListe = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.KullaniciListe = model.KullaniciListe + liste[i] + ",";
                    else model.KullaniciListe = model.KullaniciListe + liste[i];
                }
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.KullaniciListe = String.Empty;
            }
            List<KullaniciModelForList> kullanicilar = null;
            IQueryable<TVMDetay> YetkiliTVMler = null;
            TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                kullanicilar = _KullaniciService.GetListAktifTVMKullanicilari(_AktifKullanici.TVMKodu);
                YetkiliTVMler = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu);
                if (YetkiliTVMler != null)
                {
                    foreach (var item in YetkiliTVMler)
                    {
                        foreach (var kullanici in _KullaniciService.GetListAktifTVMKullanicilari(item.Kodu))
                        {
                            kullanicilar.Add(kullanici);
                        }
                    }
                }


                model.tvmKodu = _AktifKullanici.TVMKodu;
                model.MerkezAcenteMi = true;
            }
            else
            {
                model.tvmKodu = tvmDetay.BagliOlduguTVMKodu;
                kullanicilar = _KullaniciService.GetListAktifTVMKullanicilari(_AktifKullanici.TVMKodu);
            }



            if (tvmDetay != null && tvmDetay.BolgeYetkilisiMi == 1)
            {
                model.BolgeYetkilisiMi = true;
            }
            //var tvmler = _TaliPoliceService.GetYetkiliTVM(_AktifKullanici.TVMKodu).OrderBy(s => s.Unvani);
         
            model.kullanicilar = new MultiSelectList(kullanicilar, "KullaniciKodu", "AdiSoyadi");

            List<int> yillar = new List<int>();
            for (int i = 2010; i < 2030; i++)
            {
                yillar.Add(i);
            }
            model.donemler = new SelectList(yillar);

            OfflineUretimPerformansKullanici Performans = _TVMContext.OfflineUretimKullanici(model.tvmKodu, model.donemYil,model.KullaniciListe);
         
            OfflineUretimKullaniciChart chartModel = new OfflineUretimKullaniciChart(Performans);
            
            model.tvmKodu = _AktifKullanici.TVMKodu;
            model.kullaniciKodu = model.kullaniciKodu;
            model.donemYil = model.donemYil;

            model.list = chartModel.list;
            foreach (var item in model.list)
            {
                if (item.policeAdetOcak > 0 && item.policeAdetSubat > 0)
                {
                    model.uretimSatir1 = true;
                }
            }

            if (model.kullaniciList != null)
            {
                var b= _BransService.GetList("1");
                KullaniciKokpitViewModel rapor = new KullaniciKokpitViewModel();
                List<TVMKullanicilar> kul = new List<TVMKullanicilar>();
                for (int i = 0; i < model.kullaniciList.Count(); i++)
                {
                   
                    if (model.kullaniciList[i]!="multiselect-all")
                    {
                        TVMKullanicilar t = _KullaniciService.GetKullanici(Convert.ToInt32(model.kullaniciList[i]));
                        kul.Add(t);
                    }
                   
                }
                rapor.kullanicilar = kul;

                rapor.performansList = Performans;
                rapor.donemYil = model.donemYil;
                ViewBag.performans = rapor;
                ViewBag.brans = b;
               
            }
          
           

          



            ViewBag.JScript = chartModel.ALLJScript;

            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.OfflineUretimPerformansi, SekmeKodu = 0)]
        public ActionResult OfflineUretimPerformans(OfflinePerformansModel model)
        {
            model.MerkezAcenteMi = false;
            if (model.tvmList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.tvmList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.TVMListe = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.TVMListe = model.TVMListe + liste[i] + ",";
                    else model.TVMListe = model.TVMListe + liste[i];
                }
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.TVMListe = String.Empty;
            }
            TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                model.tvmKodu = _AktifKullanici.TVMKodu;
                model.MerkezAcenteMi = true;
            }
            else
            {
                model.tvmKodu = tvmDetay.BagliOlduguTVMKodu;
            }
            if (tvmDetay != null && tvmDetay.BolgeYetkilisiMi == 1)
            {
                model.BolgeYetkilisiMi = true;
            }
            //var tvmler = _TaliPoliceService.GetYetkiliTVM(_AktifKullanici.TVMKodu).OrderBy(s => s.Unvani);
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.tvmler = new MultiSelectList(tvmler, "Kodu", "Unvani");

            List<int> yillar = new List<int>();
            for (int i = 2010; i < 2030; i++)
            {
                yillar.Add(i);
            }
            model.donemler = new SelectList(yillar);

            OfflineUretimPerformans Performans = _TVMContext.OfflineUretim(model.tvmKodu, model.donemYil, model.TVMListe);

            OfflineUretimChart chartModel = new OfflineUretimChart(Performans);
            model.tvmKodu = _AktifKullanici.TVMKodu;
            model.taliTvmKodu = _AktifKullanici.TVMKodu;
            model.donemYil = model.donemYil;

            model.list = chartModel.list;
            foreach (var item in model.list)
            {
                if (item.policeAdetOcak > 0 && item.policeAdetSubat > 0)
                {
                    model.uretimSatir1 = true;
                }
            }
            ViewBag.JScript = chartModel.ALLJScript;

            return View(model);
        }

        [HttpPost]
        public ActionResult KokpitGuncelle(string donemYil)
        {
            bool result = false;
            List<Bran> branslar = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            string bransListesi = "";
            string tvmListesi = "";
            int merkezAcentekodu = _AktifKullanici.TVMKodu;
            var tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
            if (tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                List<TVMOzetModel> tvmler = _TVMService.GetSatisKanallariListesi(_AktifKullanici.TVMKodu);
              
                for (int i = 0; i < tvmler.Count; i++)
                {
                    if (i != tvmler.Count - 1)
                        tvmListesi += tvmler[i].Kodu + ",";
                    else
                    {
                        tvmListesi += tvmler[i].Kodu;
                    }
                }
            }
            else
            {
                tvmListesi += _AktifKullanici.TVMKodu;
                merkezAcentekodu = tvmDetay.BagliOlduguTVMKodu;
            }
            for (int i = 0; i < branslar.Count; i++)
            {
                if (i != branslar.Count - 1)
                    bransListesi += branslar[i].BransKodu + ",";
                else
                {
                    bransListesi += branslar[i].BransKodu;
                }
            }
            result = _TVMService.KokpitGuncelle(merkezAcentekodu, Convert.ToInt32(donemYil), tvmListesi, bransListesi);

            return Json(new { Success = result });
        }
        [HttpPost]
        public ActionResult KokpitGuncelleKullanici(string donemYil)
        {
            bool result = false;
            List<Bran> branslar = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            string bransListesi = "";
            string tvmListesi = "";
            int merkezAcentekodu = _AktifKullanici.TVMKodu;
            var tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
            //if (tvmDetay.BagliOlduguTVMKodu == -9999)
            //{
            //    List<TVMOzetModel> tvmler = _TVMService.GetSatisKanallariListesi(_AktifKullanici.TVMKodu);

            //    for (int i = 0; i < tvmler.Count; i++)
            //    {
            //        if (i != tvmler.Count - 1)
            //            tvmListesi += tvmler[i].Kodu + ",";
            //        else
            //        {
            //            tvmListesi += tvmler[i].Kodu;
            //        }
            //    }
            //}
            //else
            //{
                tvmListesi += _AktifKullanici.KullaniciKodu;
                merkezAcentekodu = _AktifKullanici.TVMKodu;
            //}
            for (int i = 0; i < branslar.Count; i++)
            {
                if (i != branslar.Count - 1)
                    bransListesi += branslar[i].BransKodu + ",";
                else
                {
                    bransListesi += branslar[i].BransKodu;
                }
            }
            result = _TVMService.KokpitKullaniciGuncelle(merkezAcentekodu, Convert.ToInt32(donemYil), tvmListesi, bransListesi);

            return Json(new { Success = result });
        }
        public ActionResult MobilOnayKodu(MobilOnayKoduModel model)
        {
            model.TvmKodu = _AktifKullanici.TVMKodu;
            model.KullaniciKodu = _AktifKullanici.KullaniciKodu;
            var tel = _KullaniciService.GetKullanici(model.KullaniciKodu);
            model.cepTelefonu = tel.CepTelefon;

            return View(model);
        }

        //[HttpPost]
        public ActionResult SMSGonder()
        {
            string Message = String.Empty;
            var tvm = _AktifKullanici.TVMKodu;
            var tel = _AktifKullanici.KullaniciKodu;
            var telefon = _KullaniciService.GetKullanici(tel);
            string cepTel = "";
            cepTel = telefon.CepTelefon.Substring(0, 2);
            cepTel += telefon.CepTelefon.Substring(3, 3);
            cepTel += telefon.CepTelefon.Substring(7, 7);
            //  var getTVMDetay = _TVMService.GetDetay(tvm);
            var smsKullaniciBilgileri = _TVMService.GetSmsKullaniciBilgileri(tvm);

            var dogrulamaKodu = this.SendSMS(smsKullaniciBilgileri.KullaniciAdi, smsKullaniciBilgileri.Sifre, smsKullaniciBilgileri.Gonderen, smsKullaniciBilgileri.KullaniciAdi, cepTel, smsKullaniciBilgileri.SmsSuresiDK);
            var kullaniciKoduEkle = _TVMService.KullaniciMobilOnayKoduEkle(tvm, _AktifKullanici.KullaniciKodu, dogrulamaKodu);
            //if (kullaniciKoduEkle)
            //{
            //    Message = "Lütfen, sistemde kayıtlı cep telefonunuza gelen \n doğrulama kodunu 3 dakika içerisinde giriniz. ";
            //}
            //else
            //{
            //    Message = "Hata: Mobil onay doğrulama kodu gönderilirken \n bir hata oluştu. Lütfen tekrar deneyiniz!";
            //}

            //return Json(new { Success = "True", Message = Message });
            return RedirectToAction("MobilOnayKodu", "TVM", new { area = "TVM" });
        }

        [HttpPost]
        public ActionResult SMSGonderOkButonu(string smsKodu)
        {
            string Message = String.Empty;
            var tvm = _AktifKullanici.TVMKodu;
            var kullaniciKodu = _AktifKullanici.KullaniciKodu;
            var telefon = _KullaniciService.GetKullanici(kullaniciKodu);

            var kodDogrula = _TVMService.KullaniciMobilOnayKoduDogrula(tvm, kullaniciKodu, smsKodu);
            if (kodDogrula)
            {
                var kodSifirla = _TVMService.KullaniciMobilOnayKoduSifirla(tvm, kullaniciKodu, smsKodu);
                if (kodSifirla)
                {
                    //   return RedirectToAction("Index", "TVM", new { area = "TVM" });

                }
                else
                {
                    Message = "Mobil onay kodunuzu doğrulama sırasında bir hata oluştu. Lütfen işleminizi tekrar deneyiniz.";
                }
            }
            else
            {
                Message = "Mobil onay kodunuzu hatalı veya eksik girdiniz. Lütfen tekrar deneyiniz!";
                return Json(new { Success = "False", Message = Message });
            }
            return Json(new { Success = "True", Message = Message });
            //return View(model);
        }

        public string SendSMS(string smsKullaniciAdi, string smsSifre, string smsGonderen, string KullaniciAdi, string cepTelNo, short smsSuresi)
        {
            // SUBMIT
            //6 haneli rastgele sayı üretimi
            Random rnd = new Random();
            string dogrulamaKodu = "";
            for (int i = 0; i < 6; i++)
            {
                dogrulamaKodu += rnd.Next(0, 9).ToString();
            }

            // messenger objesi
            var messenger = new Messenger(smsKullaniciAdi, smsSifre);

            // gönderilecek mesaj içeriği
            // [ZORUNLU]

            var mesaj = "Sn. " + KullaniciAdi + " " + dogrulamaKodu + " NeoConnect Mobil Onay Kodunuz.";

            // telefon numaraları
            // [ZORUNLU]
            var list = new List<string>();
            list.Add(cepTelNo);

            // başlık, ileri tarihli gönderim, geçerlilik süresi
            var header = new Header
            {
                //Gönderen Adı / Başlık 
                //[ZORUNLU]
                From = smsGonderen,

                //İleri tarihli gönderim yapılmak isteniyorsa girilmelidir. 
                //[OPSİYONEL]
                ScheduledDeliveryTime = new DateTime(),

                //mesaj gönderimi başarısız ise 1440 dk boyunca tekrar denenecek ve bu süre sonunda artık gönderim denenmeyecektir. 
                //Bu alan gönderilmez veya 0 gönderilirse default değeri 1440 olarak alır.
                //[OPSİYONEL]
                ValidityPeriod = smsSuresi
            };


            // Mesajı gönderip değişkene atayalım.
            var msgObj = messenger.Submit(mesaj, list, header, DataCoding.Default);

            /////////////////////////////////////////
            // gelen cevaplar ve anlamları.
            /////////////////////////////////////////

            // Status.
            // Code ve Desc alanları yer alır. Mesajın sisteme başarılı şekilde gönderilip gönderilmediği bilgisini verir.
            // Code = 200 ve Description = OK ise mesaj sisteme başarılı şekilde ulaşmıştır. Bu mesajın iletim durumu değildir.
            // Alabileceği değerler için dokümana bakınız.
            var statusCode = msgObj.Response.Status.Code;
            var statusDesc = msgObj.Response.Status.Description;

            // MessageId
            // Sistemden rapor alınması için geri dönen mesaj numarasıdır.
            var msgId = msgObj.Response.MessageId;
            if (statusCode != 200)
            {
                string hata = "Hata: " + statusDesc + "Mesaj Numarası: " + msgId;
                //      hataMesaji.Text = hata;
                return hata;
            }
            else
            {
                return dogrulamaKodu;
            }
        }


    }
}
