using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Business;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Service;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using System.Xml;
using Neosinerji.BABOnlineTP.Web.Areas.TaliAcenteTransfer.Models;
using Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer;

namespace Neosinerji.BABOnlineTP.Web.Areas.TaliAcenteTransfer.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = 0, SekmeKodu = 0)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class TaliAcenteTransferController : Controller
    {
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _AktifKullanici;
        ITaliAcenteTransferService _TaliAcenteTransferService;
        ILogService _Log;
        public TaliAcenteTransferController(
                                 IKullaniciService kullanici,
                                 IAktifKullaniciService aktifKullanici,
                                 ITaliAcenteTransferService taliAcenteTransferService,
                                 ILogService log)
        {
            _KullaniciService = kullanici;
            _AktifKullanici = aktifKullanici;
            _TaliAcenteTransferService = taliAcenteTransferService;
            _Log = log;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TaliAcente()
        {
            TaliAcenteTransferModel model = new TaliAcenteTransferModel();
            model.BagliOlduguTVMKodu = -9999;

            return View(model);
        }

        [HttpPost]
        public ActionResult TaliAcente(TaliTransferKayitModel model)
        {
            int tvmKod = Convert.ToInt32(model.tvmKodu);

            List<Business.TaliAcenteTransfer.TaliAcente> acenteModelList = new List<Business.TaliAcenteTransfer.TaliAcente>();
            Business.TaliAcenteTransfer.TaliAcente acenteModel;


            List<Business.TaliAcenteTransfer.TaliAcenteExcel> TaliListExcel = null;
            TaliListExcel = _TaliAcenteTransferService.getTaliler(model.Path, tvmKod);

            #region tvm tabloları okunuyor.
            TVMParametreler tvmParametreModel = new TVMParametreler();
            tvmParametreModel.tvmDetay = _TaliAcenteTransferService.GetDetay(tvmKod);
            tvmParametreModel.yetkiGruplari = _TaliAcenteTransferService.GetListTVYetkiGruplari(tvmKod);
            tvmParametreModel.yetkiGrupYetkileri = _TaliAcenteTransferService.GetListTVYetkiGrupYetkileri(tvmParametreModel.yetkiGruplari);
            // tvmParametreModel.kullanicilar = _TaliAcenteTransferService.GetListKullanicilar(model.BagliOlduguTVMKodu.Value);
            //tvmParametreModel.departmanlar = _TaliAcenteTransferService.GetListDepartmanlar(tvmKod);
            //tvmParametreModel.bolgeler = _TaliAcenteTransferService.GetListBolgeler(tvmKod);
            tvmParametreModel.servisKullanicilar = _TaliAcenteTransferService.GetListTVMWebServisKullanicilari(tvmKod);
            tvmParametreModel.urunYetkileri = _TaliAcenteTransferService.GetListTVMUrunYetkileri(tvmKod);
            #endregion

            TaliTransferKayitModel taliModel = new TaliTransferKayitModel();

            #region Acente Bilgileri
            int tvmKoduSayac = _TaliAcenteTransferService.GetSonTvmKodu(tvmKod);

            foreach (var item in TaliListExcel)
            {
                if (item.Email == null)
                {
                    continue;
                }
                do
                {
                    tvmKoduSayac++;
                }
                while (_TaliAcenteTransferService.TVMKoduVarMi(tvmKoduSayac));

                acenteModel = new Business.TaliAcenteTransfer.TaliAcente();
                acenteModel.taliAcenteExcelModel = new TaliAcenteExcel();

                #region tali detay

                acenteModel.TVMDetay = new TVMDetay();
                //excelden okunanlar
                acenteModel.TVMDetay.Unvani = item.Unvan;
                acenteModel.TVMDetay.Email = item.Email;
                acenteModel.TVMDetay.Telefon = item.Telefon;
                acenteModel.TVMDetay.Fax = item.Faks;
                acenteModel.TVMDetay.Adres = item.AcikAdres;

                acenteModel.TVMDetay.Kodu = tvmKoduSayac;
                acenteModel.TVMDetay.Tipi = 2;
                acenteModel.TVMDetay.AcentSuvbeVar = 0;
                acenteModel.TVMDetay.Profili = 0;
                acenteModel.TVMDetay.BaglantiSiniri = tvmParametreModel.tvmDetay.BaglantiSiniri;
                acenteModel.TVMDetay.BagliOlduguTVMKodu = tvmParametreModel.tvmDetay.Kodu;
                acenteModel.TVMDetay.Banner = tvmParametreModel.tvmDetay.Banner;
                acenteModel.TVMDetay.BolgeKodu = tvmParametreModel.tvmDetay.BolgeKodu;
                acenteModel.TVMDetay.Durum = tvmParametreModel.tvmDetay.Durum;
                acenteModel.TVMDetay.DurumGuncallemeTarihi = tvmParametreModel.tvmDetay.DurumGuncallemeTarihi;
                acenteModel.TVMDetay.DuyuruTVMs = tvmParametreModel.tvmDetay.DuyuruTVMs;
                acenteModel.TVMDetay.GrupKodu = tvmParametreModel.tvmDetay.GrupKodu;
                acenteModel.TVMDetay.IlceKodu = tvmParametreModel.tvmDetay.IlceKodu;
                acenteModel.TVMDetay.IlKodu = tvmParametreModel.tvmDetay.IlKodu;
                acenteModel.TVMDetay.KayitNo = tvmParametreModel.tvmDetay.KayitNo;
                acenteModel.TVMDetay.Latitude = tvmParametreModel.tvmDetay.Latitude;
                acenteModel.TVMDetay.Logo = tvmParametreModel.tvmDetay.Logo;
                acenteModel.TVMDetay.Longitude = tvmParametreModel.tvmDetay.Longitude;
                acenteModel.TVMDetay.MuhasebeEntegrasyon = tvmParametreModel.tvmDetay.MuhasebeEntegrasyon;
                acenteModel.TVMDetay.Notlar = tvmParametreModel.tvmDetay.Notlar;
                acenteModel.TVMDetay.ProjeKodu = tvmParametreModel.tvmDetay.ProjeKodu;
                acenteModel.TVMDetay.Semt = tvmParametreModel.tvmDetay.Semt;
                acenteModel.TVMDetay.SifreDegistirmeGunu = tvmParametreModel.tvmDetay.SifreDegistirmeGunu;
                acenteModel.TVMDetay.SifreIkazGunu = tvmParametreModel.tvmDetay.SifreIkazGunu;
                acenteModel.TVMDetay.SifreKontralSayisi = tvmParametreModel.tvmDetay.SifreKontralSayisi;
                acenteModel.TVMDetay.SozlesmeBaslamaTarihi = tvmParametreModel.tvmDetay.SozlesmeBaslamaTarihi;
                acenteModel.TVMDetay.SozlesmeDondurmaTarihi = tvmParametreModel.tvmDetay.SozlesmeDondurmaTarihi;

                acenteModel.TVMDetay.TCKN = tvmParametreModel.tvmDetay.TCKN;
                acenteModel.TVMDetay.UcretlendirmeKodu = tvmParametreModel.tvmDetay.UcretlendirmeKodu;
                acenteModel.TVMDetay.UlkeKodu = tvmParametreModel.tvmDetay.UlkeKodu;
                acenteModel.TVMDetay.VergiDairesi = tvmParametreModel.tvmDetay.VergiDairesi;
                acenteModel.TVMDetay.VergiNumarasi = tvmParametreModel.tvmDetay.VergiNumarasi;
                acenteModel.TVMDetay.WebAdresi = tvmParametreModel.tvmDetay.WebAdresi;

                #endregion
                #region TVM Yetki Grupları Add

                acenteModel.yetkiGruplari = new List<TVMYetkiGruplari>();
                foreach (var yetkiGruplari in tvmParametreModel.yetkiGruplari)
                {
                    acenteModel.taliYetkiGruplari = new TVMYetkiGruplari();
                    acenteModel.taliYetkiGruplari.TVMKodu = tvmKoduSayac;
                    acenteModel.taliYetkiGruplari.YetkiGrupAdi = yetkiGruplari.YetkiGrupAdi;
                    acenteModel.taliYetkiGruplari.YetkiSeviyesi = yetkiGruplari.YetkiSeviyesi;
                    acenteModel.TVMDetay.TVMYetkiGruplaris.Add(acenteModel.taliYetkiGruplari);

                }

                #endregion
                #region tali için urunyetkileri ekleniyor.
                //acenteModel.tvmUrunYetkileri = new List<TVMUrunYetkileri>();
                foreach (var urunYetkileri in tvmParametreModel.urunYetkileri)
                {
                    acenteModel.tvmUrunYetki = new TVMUrunYetkileri();
                    acenteModel.tvmUrunYetki.AcikHesapTahsilatGercek = urunYetkileri.AcikHesapTahsilatGercek;
                    acenteModel.tvmUrunYetki.AcikHesapTahsilatTuzel = urunYetkileri.AcikHesapTahsilatTuzel;
                    acenteModel.tvmUrunYetki.BABOnlineUrunKodu = urunYetkileri.BABOnlineUrunKodu;
                    acenteModel.tvmUrunYetki.HavaleEntegrasyon = urunYetkileri.HavaleEntegrasyon;
                    acenteModel.tvmUrunYetki.KrediKartiTahsilat = urunYetkileri.KrediKartiTahsilat;
                    acenteModel.tvmUrunYetki.ManuelHavale = urunYetkileri.ManuelHavale;
                    acenteModel.tvmUrunYetki.Police = urunYetkileri.Police;
                    acenteModel.tvmUrunYetki.Rapor = urunYetkileri.Rapor;
                    acenteModel.tvmUrunYetki.Teklif = urunYetkileri.Teklif;
                    acenteModel.tvmUrunYetki.TUMKodu = urunYetkileri.TUMKodu;
                    acenteModel.tvmUrunYetki.TUMUrunKodu = urunYetkileri.TUMUrunKodu;
                    acenteModel.tvmUrunYetki.TVMKodu = tvmKoduSayac;
                    acenteModel.TVMDetay.TVMUrunYetkileris.Add(acenteModel.tvmUrunYetki);
                }

                #endregion

                acenteModelList.Add(acenteModel);

                //talinin tvmdetay ı yetki grupkodlarını yeni id lerle okumak için yetkigrupları tablosuyla ekleniyor.
                _TaliAcenteTransferService.Add(acenteModelList);

                var yeniTvmEklendiMi = _TaliAcenteTransferService.GetDetay(tvmKoduSayac);
                if (yeniTvmEklendiMi == null)
                {
                    continue;
                }

                acenteModel.yetkiGruplari = new List<TVMYetkiGruplari>();
                //buraad yeni eklenen talinin yetkigrup tablosundaki kayıtları okunuyor.
                acenteModel.yetkiGruplari = _TaliAcenteTransferService.GetListTVYetkiGruplari(tvmKoduSayac);

                #region tali için yetkigrupyetkileri kaydediliyor.

                int yetkiToplam = acenteModel.yetkiGruplari.Count();
                int yetkiGrupToplam = tvmParametreModel.yetkiGrupYetkileri.Count();
                int sayac = tvmParametreModel.yetkiGrupYetkileri.Count() / acenteModel.yetkiGruplari.Count();
                int say1 = 0;
                List<TVMYetkiGrupYetkileri> taliYetkiList = new List<TVMYetkiGrupYetkileri>();
                foreach (var itemNewYetkiGrup in acenteModel.yetkiGruplari)
                {
                    foreach (var itemGrupYetkileri in tvmParametreModel.yetkiGrupYetkileri.Skip(say1).Take(sayac))
                    {
                        TVMYetkiGrupYetkileri taliYetkiler = new TVMYetkiGrupYetkileri();
                        //acenteModel.taliYetkiGrupYetkileri = new TVMYetkiGrupYetkileri();
                        taliYetkiler.AltMenuKodu = itemGrupYetkileri.AltMenuKodu;
                        taliYetkiler.AnaMenuKodu = itemGrupYetkileri.AnaMenuKodu;
                        taliYetkiler.Degistirme = itemGrupYetkileri.Degistirme;
                        taliYetkiler.Gorme = itemGrupYetkileri.Gorme;
                        taliYetkiler.SekmeKodu = itemGrupYetkileri.SekmeKodu;
                        taliYetkiler.Silme = itemGrupYetkileri.Silme;
                        //taliYetkiler.TVMYetkiGruplari = itemGrupYetkileri.TVMYetkiGruplari;
                        taliYetkiler.YeniKayit = itemGrupYetkileri.YeniKayit;
                        taliYetkiler.YetkiGrupKodu = itemNewYetkiGrup.YetkiGrupKodu;
                        taliYetkiList.Add(taliYetkiler);

                    }
                    say1 = say1 + sayac;
                }

                _TaliAcenteTransferService.taliYetkiAdd(taliYetkiList);
                #endregion
                #region servisKullanıcıları Ekle
                acenteModel.tvmWebServisKullanicilari = tvmParametreModel.servisKullanicilar;
                acenteModel.tvmWebServisKullanicilari = new List<TVMWebServisKullanicilari>();

                foreach (var servisKullanici in tvmParametreModel.servisKullanicilar)
                {
                    acenteModel.servisKullanici = new TVMWebServisKullanicilari();
                    acenteModel.servisKullanici.CompanyId = servisKullanici.CompanyId;
                    acenteModel.servisKullanici.KullaniciAdi = servisKullanici.KullaniciAdi;
                    acenteModel.servisKullanici.KullaniciAdi2 = servisKullanici.KullaniciAdi2;
                    acenteModel.servisKullanici.PartajNo_ = servisKullanici.PartajNo_;
                    acenteModel.servisKullanici.Sifre = servisKullanici.Sifre;
                    acenteModel.servisKullanici.Sifre2 = servisKullanici.Sifre2;
                    acenteModel.servisKullanici.SourceId = servisKullanici.SourceId;
                    acenteModel.servisKullanici.SubAgencyCode = servisKullanici.SubAgencyCode;
                    acenteModel.servisKullanici.TUMKodu = servisKullanici.TUMKodu;
                    acenteModel.servisKullanici.TVMKodu = tvmKoduSayac;
                    acenteModel.servisKullanici.Sifre = servisKullanici.Sifre;

                    acenteModel.tvmWebServisKullanicilari.Add(acenteModel.servisKullanici);
                }
                _TaliAcenteTransferService.taliWebServisKullaniciAdd(acenteModel.tvmWebServisKullanicilari);

                #endregion

                //departman ve bölgeler kaldırıldı.
                #region talidepartman ekle
                //acenteModel.tvmDepartmanlar = tvmParametreModel.departmanlar;
                //acenteModel.tvmDepartmanlar = new List<TVMDepartmanlar>();
                //foreach (var itemDepartman in tvmParametreModel.departmanlar)
                //{
                //    acenteModel.taliDepartman = new TVMDepartmanlar();
                //    acenteModel.taliDepartman.Adi = itemDepartman.Adi;
                //    acenteModel.taliDepartman.BolgeKodu = itemDepartman.BolgeKodu;
                //    acenteModel.taliDepartman.DepartmanKodu = itemDepartman.DepartmanKodu;
                //    acenteModel.taliDepartman.Durum = itemDepartman.Durum;
                //    acenteModel.taliDepartman.MerkezYetkisi = itemDepartman.MerkezYetkisi;
                //    acenteModel.taliDepartman.TVMKodu = tvmKoduSayac;
                //    acenteModel.tvmDepartmanlar.Add(acenteModel.taliDepartman);

                //}
                //_TaliAcenteTransferService.taliDepartmanlarAdd(acenteModel.tvmDepartmanlar);
                #endregion
                #region talibölgeler ekle
                //acenteModel.tvmBolgeleri = tvmParametreModel.bolgeler;
                //acenteModel.tvmBolgeleri = new List<TVMBolgeleri>();
                //foreach (var itemBolge in tvmParametreModel.bolgeler)
                //{
                //    acenteModel.taliBolge = new TVMBolgeleri();
                //    acenteModel.taliBolge.Aciklama = itemBolge.Aciklama;
                //    acenteModel.taliBolge.BolgeAdi = itemBolge.BolgeAdi;
                //    acenteModel.taliBolge.Durum = itemBolge.Durum;
                //    acenteModel.taliBolge.TVMBolgeKodu = itemBolge.TVMBolgeKodu;
                //    acenteModel.taliBolge.TVMKodu = tvmKoduSayac;
                //    acenteModel.tvmBolgeleri.Add(acenteModel.taliBolge);

                //}
                //_TaliAcenteTransferService.taliBolgelerAdd(acenteModel.tvmBolgeleri);
                #endregion

                #region tali için Kullanıcı oluştur

                int yetkigrupKoduFirst = acenteModel.yetkiGruplari.FirstOrDefault().YetkiGrupKodu;
                string newTck;
                bool kayitVarMi = false;
                do
                {
                    newTck = tckUret();
                    kayitVarMi = _TaliAcenteTransferService.tckVarMi(newTck);
                }
                while (!kayitVarMi);

                acenteModel.tvmKullanici = new TVMKullanicilar()
                {
                    Adi = acenteModel.TVMDetay.Unvani.Length > 15 ? acenteModel.TVMDetay.Unvani.Substring(0, 15) : acenteModel.TVMDetay.Unvani,
                    DepartmanKodu = 0,
                    Durum = 1,
                    Email = acenteModel.TVMDetay.Email,
                    FotografURL = "https://neoonlinestrg.blob.core.windows.net/musteri-dokuman/346/avatar.png",
                    Gorevi = 1,
                    HataliSifreGirisSayisi = 0,
                    KayitTarihi = TurkeyDateTime.Today,
                    Sifre = Encryption.HashPassword("neo123"),
                    SifreDurumKodu = 0,
                    SifreTarihi = TurkeyDateTime.Today,
                    Soyadi = acenteModel.TVMDetay.Unvani.Length > 15 ? acenteModel.TVMDetay.Unvani.Substring(15) : ".",
                    TCKN = newTck,
                    TeklifPoliceUretimi = 1,
                    TeknikPersonelKodu = tvmKoduSayac.ToString(),
                    Telefon = acenteModel.TVMDetay.Telefon,
                    TVMKodu = Convert.ToInt32(tvmKoduSayac),
                    YetkiGrubu = yetkigrupKoduFirst,
                };

                _TaliAcenteTransferService.taliKullaniciAdd(acenteModel.tvmKullanici);

                #endregion
            }
            #endregion

            taliModel.basariliKayitlar = _TaliAcenteTransferService.getBasariliKayitlar();
            taliModel.basarisizKayitlar = _TaliAcenteTransferService.getBasarisizKayitlar();
            return Json(new { Success = true, BasariliKayit = taliModel.basariliKayitlar, BasarisizKayit = taliModel.basarisizKayitlar });


        }


        [HttpPost]
        [AjaxException]
        public ActionResult TaliUpload(TaliAcenteTransferModel model, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid && file != null && file.ContentLength > 0 && !String.IsNullOrEmpty(model.BagliOlduguTVMKodu.ToString()))
                {
                    string path = String.Empty;

                    if (file != null)
                    {
                        path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);
                        file.SaveAs(path);
                    }

                    List<Business.TaliAcenteTransfer.TaliAcenteExcel> TaliListExcel = null;
                    TaliListExcel = _TaliAcenteTransferService.getTaliler(path, model.BagliOlduguTVMKodu.Value);

                    if (TaliListExcel != null)
                    {
                        TaliTransferKayitModel tali = new TaliTransferKayitModel();
                        tali.taliCount = TaliListExcel.Count();
                        tali.Path = path;
                        tali.tvmKodu = model.BagliOlduguTVMKodu.ToString();

                        return PartialView("_TaliUpload", tali);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
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

    }
}

