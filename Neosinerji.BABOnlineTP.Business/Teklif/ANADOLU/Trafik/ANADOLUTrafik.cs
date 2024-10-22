using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Newtonsoft.Json;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ANADOLUTrafik : Teklif, IANADOLUTrafik
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IUlkeService _UlkeService;
        ITVMService _TVMService;
        [InjectionConstructor]
        public ANADOLUTrafik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
            : base()
        {
            _CRService = crService;
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            _TVMService = TVMService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.ANADOLU;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region Eski Versiyon

                #region Veri Hazırlama GENEL
                //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);

                //PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1 request = new PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1();
                #endregion

                #region Müşteri Bilgileri
                //MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                //if (MusteriTipleri.Ozel(sigortali.MusteriTipKodu))
                //{
                //    request.MUSTERI_TIPI = "O";
                //    request.TCKN = sigortali.KimlikNo;
                //    request.CINSIYET = sigortali.Cinsiyet;
                //    request.DOGUM_TARIHI = sigortali.DogumTarihi.HasValue ? sigortali.DogumTarihi.Value.ToString("yyyyMMdd") : null;
                //}
                //else
                //{
                //    request.MUSTERI_TIPI = "T";
                //    request.VKN = sigortali.KimlikNo;
                //    request.CINSIYET = null;
                //    request.DOGUM_TARIHI = null;
                //}

                //MusteriAdre adres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                //if (adres != null)
                //{
                //    CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                //                                                               f.IlKodu == adres.IlKodu &&
                //                                                               f.IlceKodu == adres.IlceKodu)
                //                                                    .SingleOrDefault<CR_IlIlce>();

                //    if (ililce != null)
                //    {
                //        request.MUSTERI_IL_KODU = ililce.CRIlKodu;
                //        request.MUSTERI_ILCE_KODU = ililce.CRIlceKodu;
                //    }
                //}
                #endregion

                #region Araç Bilgileri
                //string aracKodu = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                //request.ARAC_KODU = aracKodu.TrimStart('0');
                //request.ARAC_TESCIL_KODU = teklif.Arac.TescilSeriKod;
                //request.ARAC_TESCIL_SERI_NO = teklif.Arac.TescilSeriNo;
                //request.PLAKA_IL_KODU = teklif.Arac.PlakaKodu;

                //string plaka = teklif.Arac.PlakaNo;


                //if (plaka.Length == 5)
                //{
                //    string YeniPlaka = "";
                //    int sayac = 0;
                //    foreach (var item in plaka)
                //    {
                //        if (char.IsDigit(item) && sayac == 0)
                //        {
                //            YeniPlaka += " ";
                //            sayac++;
                //        }
                //        YeniPlaka += item;
                //    }
                //    plaka = YeniPlaka;
                //}
                //if (plaka == "YK" || plaka == "G 9999")
                //{
                //    request.PLAKA = "G 9999";
                //    request.PLAKA_TIPI = "Gecici";
                //}
                //else
                //{
                //    request.PLAKA = plaka;
                //    request.PLAKA_TIPI = "Yerli";
                //}

                //request.MODEL_YILI = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value.ToString() : String.Empty;

                //string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                //if (parts.Length == 2)
                //{
                //    string kullanimTarziKodu = parts[0];
                //    string kod2 = parts[1];
                //    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                //                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                //                                                                                  f.Kod2 == kod2)
                //                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                //    if (kullanimTarzi != null)
                //    {
                //        request.TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                //    }
                //}

                //DateTime baslangicTarihi = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                //request.BITIS_TARIHI = baslangicTarihi.AddYears(1).ToString("yyyyMMdd");

                //if (teklif.Arac.TrafikTescilTarihi.HasValue)
                //    request.TRAFIK_TESCIL_TARIHI = teklif.Arac.TrafikTescilTarihi.Value.ToString("yyyyMMdd");
                #endregion

                #region Önceki Poliçe
                //bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                //if (eskiPoliceVar)
                //{
                //    request.ONCEKI_SIRKET_KODU = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                //    request.ONCEKI_ACENTE_KODU = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                //    request.ONCEKI_POLICE_NO = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                //    request.ONCEKI_YENILEME_NO = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                //}
                #endregion

                #region Taşıyıcı Sorumluluk
                //bool tasiyiciSorumluluk = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, false);
                //if (tasiyiciSorumluluk)
                //{
                //    request.ZKTMS_SIRKET_KODU = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
                //    request.ZKTMS_ACENTE_KODU = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                //    request.ZKTMS_ESKI_POLICE_NO = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                //    request.ZKTMS_YENILEME_NO = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
                //}
                #endregion

                #region Servis call
                //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.ANADOLU });

                //request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                //request.WEB_GIRIS = servisKullanici.Sifre2;

                //ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2);
                //servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

                //this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Teklif);

                //PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1_Response>(request);

                //if (response.HATA_KODU != "0")
                //{
                //    this.EndLog(response, false, response.GetType());
                //    this.AddHata(response.HATA_TEXT);
                //}
                //else
                //    this.EndLog(response, true, response.GetType());
                #endregion

                #region Basarı Kontrol
                //if (!this.Basarili)
                //{
                //    this.Import(teklif);
                //    this.GenelBilgiler.Basarili = false;
                //    return;
                //}
                #endregion

                #region Teklif kaydı
                #region Genel bilgiler
                //this.Import(teklif);

                //this.GenelBilgiler.Basarili = true;
                //this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                //this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                //this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                //this.GenelBilgiler.BrutPrim = ANADOLUMessage.ToDecimal(response.SONUC);

                //decimal THGFonu = ANADOLUMessage.ToDecimal(response.THG_FON);
                //decimal GiderVergisi = ANADOLUMessage.ToDecimal(response.BSMV);
                //decimal GarantiFonu = ANADOLUMessage.ToDecimal(response.KTGS);

                //this.GenelBilgiler.ToplamVergi = THGFonu + GiderVergisi + GarantiFonu;
                //this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                //this.GenelBilgiler.TaksitSayisi = 1;
                //this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                //this.GenelBilgiler.GecikmeZammiYuzdesi = ANADOLUMessage.ToDecimal(response.GECIKME_SURPRIM_ORANI) * 100;

                //decimal hasarsizlik = ANADOLUMessage.ToDecimal(response.HASARSIZLIK_ORANI);
                //if (hasarsizlik > 0)
                //{
                //    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                //    this.GenelBilgiler.HasarSurprimYuzdesi = hasarsizlik * 100;
                //}
                //else if (hasarsizlik < 0)
                //{
                //    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Math.Abs(hasarsizlik) * 100;
                //    this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                //}
                //this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                //this.GenelBilgiler.ToplamKomisyon = 0;

                //// ==== Güncellenicek. ==== //
                ////this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                ////this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                ////Odeme Bilgileri
                //this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                //this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                //this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                #endregion

                #region Vergiler
                //this.AddVergi(TrafikVergiler.THGFonu, THGFonu);
                //this.AddVergi(TrafikVergiler.GiderVergisi, GiderVergisi);
                //this.AddVergi(TrafikVergiler.GarantiFonu, GarantiFonu);
                #endregion

                #region Teminatlar
                //var maxBaslamaTarih = _AracContext.AracTrafikTeminatRepository.All().Max(f => f.GecerlilikBaslamaTarihi);
                //var teminat = _AracContext.AracTrafikTeminatRepository.Filter(f => f.GecerlilikBaslamaTarihi <= this.GenelBilgiler.BaslamaTarihi &&
                //                                                                    f.GecerlilikBaslamaTarihi >= maxBaslamaTarih &&
                //                                                                    f.AracGrupKodu == request.TRAMER_TARIFE_KODU)
                //                                                        .FirstOrDefault();

                //if (teminat != null)
                //{
                //    this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, teminat.MaddiAracBasina.Value, 0, 0, 0, 0);
                //    this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, teminat.MaddiKazaBasina.Value, 0, 0, 0, 0);
                //    this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, teminat.TedaviKisiBasina.Value, 0, 0, 0, 0);
                //    this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, teminat.TedaviKazaBasina.Value, 0, 0, 0, 0);
                //    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, teminat.SakatlikKisiBasina.Value, 0, 0, 0, 0);
                //    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, teminat.SakatlikKazaBasina.Value, 0, 0, 0, 0);
                //}
                //this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kisi_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kaza_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kaza_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.Asistans, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Tedavi, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.Trafik, 0, 0, 0, 0, 0);
                #endregion

                #region Ödeme Planı
                //if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                //{
                //    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                //}
                #endregion

                #endregion

                #endregion

                #region Yeni Version
                #region Servis

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ANADOLU });
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);//"88.249.209.253"
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

                #endregion

                #region Müşteri
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                string musteriNo = _CRService.GetTUMMusteriKodu(this.TUMKodu, sigortali.MusteriKodu);

                if (String.IsNullOrEmpty(musteriNo))
                {
                    // musteriNo = this.MusteriKaydet(null, sigortali, teklif.GenelBilgiler.TVMKodu, konfig[Konfig.ANADOLU_ServiceUrl]);
                    musteriNo = this.MusteriKaydet(teklif, sigortali, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.ANADOLU_ServiceUrl]);

                    if (String.IsNullOrEmpty(musteriNo))
                    {
                        this.Hatalar.Add("ANADOLU SİGORTA sisteminde müşteri oluşturulamadı.");
                        this.GenelBilgiler.Basarili = false;
                        this.GenelBilgiler.WEBServisLogs = this.Log;
                        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                        return;
                    }
                }

                #endregion

                #region Prim Hesapla

                #endregion

                #region TeklifKaydet

                var request = new PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET();

                request.SESSION_ID = Guid.NewGuid().ToString();
                request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                request.WEB_SIFRE = servisKullanici.Sifre2;
                request.MUSTERI_NO = musteriNo;
                request.RETURN_BILGILENDIRME_FORMU_PDF = "";
                request.RETURN_GENEL_SART_PDF = "1";
                request.RETURN_TEKLIF_PDF = "1";
                request.SEND_EPOSTA = "0";
                request.E_POSTA = sigortali.EMail;

                DateTime baslangicTarihi = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                request.BASLANGIC_TARIHI = baslangicTarihi.ToString("yyyyMMdd");
                request.BITIS_TARIHI = baslangicTarihi.AddYears(1).ToString("yyyyMMdd");

                DateTime tescilTarihi = teklif.ReadSoru(TrafikSorular.Trafik_Tescil_Tarihi, TurkeyDateTime.Today);
                request.TESCIL_TARIHI = tescilTarihi.ToString("yyyyMMdd");

                DateTime trafigeCikisTarihi = teklif.ReadSoru(TrafikSorular.Trafige_Cikis_Tarihi, TurkeyDateTime.Today);
                request.TRAFIGE_CIKIS_TARIHI = trafigeCikisTarihi.ToString("yyyyMMdd");

                #region Araç

                string aracKodu = teklif.Arac.Marka + teklif.Arac.AracinTipi.PadLeft(3, '0');
                request.ARAC_KODU = aracKodu.TrimStart('0');
                request.PLAKA_IL_KODU = teklif.Arac.PlakaKodu;
                string plakaNo = teklif.Arac.PlakaNo.Length < 6 ? AnadoluPlaka(teklif.Arac.PlakaNo) : teklif.Arac.PlakaNo;
                request.PLAKA_TURU = "Yerli";
                request.PLAKA_NO = plakaNo;

                if (plakaNo == "YK" || plakaNo == "G 9999")
                {
                    request.PLAKA_NO = "YK 123";
                    request.PLAKA_TURU = "Gecici";
                }

                request.ARAC_TESCIL_KODU = !String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) ? teklif.Arac.TescilSeriKod : "";
                request.ARAC_TESCIL_SERI_NO = !String.IsNullOrEmpty(teklif.Arac.TescilSeriNo) ? teklif.Arac.TescilSeriNo : teklif.Arac.AsbisNo;
                request.MODEL_YILI = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value.ToString() : String.Empty;
                string TRAMER_TARIFE_KODU = string.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                    }
                }

                #region Anadolu Sigorta Marka Kodu Getir

                //Anadolu Sigorta, Kullanım Tipi metoduna Anadolu Sigortanın Marka kodunu istiyor. 
                var MarkaAdi = _AracContext.AracMarkaRepository.Find(s => s.MarkaKodu == teklif.Arac.Marka).MarkaAdi;
                if (teklif.Arac.Marka == "122")
                {
                    MarkaAdi = "RENAULT";
                }
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL requestMarka = new PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL();
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response responseMarkaList = servis.CallService<PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response>(requestMarka);
                string AnadoluMarkaKodu = String.Empty;
                if (responseMarkaList.HATA_KODU == "0" && responseMarkaList.MARKA_BILGISI != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var jsonArray = js.Deserialize<List<object>>(responseMarkaList.MARKA_BILGISI);
                    if (jsonArray != null)
                    {
                        var anadoluList = jsonArray.ConvertToMarka();
                        var markaVarmi = anadoluList.Where(m => m.Marka == MarkaAdi).FirstOrDefault();
                        if (markaVarmi != null)
                        {
                            AnadoluMarkaKodu = markaVarmi.Value;
                        }
                    }
                }

                #endregion

                string AnadoluKullanimTipi = teklif.ReadSoru(TrafikSorular.AnadoluKullanimTipi, "");
                string AnadoluKullanimSekli = teklif.ReadSoru(TrafikSorular.AnadoluKullanimSekli, "");

                if (!String.IsNullOrEmpty(AnadoluKullanimTipi))
                {
                    request.KULLANIM_TIPI_KODU = AnadoluKullanimTipi;
                }
                else
                {
                    request.KULLANIM_TIPI_KODU = AracKullanimTipi(request.MODEL_YILI, AnadoluMarkaKodu, TRAMER_TARIFE_KODU, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.ANADOLU_ServiceUrl], ANADOLUKullanimTipiUrunGrupAdi.Trafik);
                }
                if (!String.IsNullOrEmpty(AnadoluKullanimSekli))
                {
                    request.KULLANIM_SEKLI_KODU = AnadoluKullanimSekli;
                }
                else
                {
                    request.KULLANIM_SEKLI_KODU = AracKullanimSekli(request.MODEL_YILI, AnadoluMarkaKodu, TRAMER_TARIFE_KODU, request.KULLANIM_TIPI_KODU, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.ANADOLU_ServiceUrl], ANADOLUKullanimTipiUrunGrupAdi.Trafik);

                }

                switch (teklif.Arac.KullanimSekli)
                {
                    case "0": request.KULLANIM_AMACI = "Ticari"; break;
                    case "1": request.KULLANIM_AMACI = "Resmi"; break;
                    case "2": request.KULLANIM_AMACI = "Hususi"; break;
                }

                request.MOTOR_NO = teklif.Arac.MotorNo;
                request.SASI_NO = teklif.Arac.SasiNo;

                #endregion

                #region Önceki Poliçe

                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                if (eskiPoliceVar)
                {
                    request.ONCEKI_SIRKET_KODU = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, string.Empty);
                    request.ONCEKI_ACENTE_KODU = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, string.Empty);
                    request.ONCEKI_POLICE_NO = teklif.ReadSoru(TrafikSorular.Eski_Police_No, string.Empty);
                    request.ONCEKI_YENILEME_NO = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, string.Empty);
                }

                #endregion

                #region Taşıyıcı Sorumluluk

                bool tasiyiciSorumluluk = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, false);
                if (tasiyiciSorumluluk)
                {
                    request.ZKTMS_SIRKET_KODU = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, string.Empty);
                    request.ZKTMS_ACENTE_KODU = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, string.Empty);
                    request.ZKTMS_ESKI_POLICE_NO = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, string.Empty);
                    request.ZKTMS_YENILEME_NO = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, string.Empty);
                }

                #endregion

                #region Paket ve Ödeme
                CR_TrafikIMM CRBedel = new CR_TrafikIMM();
                bool sinirsizIMM = teklif.ReadSoru(TrafikSorular.Teminat_SinirsizIMM, false);
                if (sinirsizIMM)
                    request.TRAFIK_PAKET_ID = AnadoluTrafikPaketleri.SinirsizTrafikSigortasi;
                else
                {
                    string immKademe = teklif.ReadSoru(TrafikSorular.Teminat_IMM_Kademe, string.Empty);

                    var IMMBedel = _CRService.GetTrafikIMMBedel(Convert.ToInt32(string.IsNullOrEmpty(immKademe) ? "-1" : immKademe), parts[0], parts[1]);

                    if (IMMBedel != null)
                    {
                        CRBedel = _CRService.GetCRTrafikIMMBedel(TeklifUretimMerkezleri.ANADOLU, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                    }
                    else
                    {
                        CRBedel = null;
                    }
                    if (CRBedel != null)
                    {
                        if (CRBedel.Kombine == AnadoluTrafikKombineLimitleri.BirMilyon)
                            request.TRAFIK_PAKET_ID = AnadoluTrafikPaketleri.SuperTrafikSigortasi;
                        else if (CRBedel.Kombine == AnadoluTrafikKombineLimitleri.IkiYuzElliBin)
                            request.TRAFIK_PAKET_ID = AnadoluTrafikPaketleri.EkstraTrafikSigortasi;
                    }
                    else
                    {
                        request.TRAFIK_PAKET_ID = AnadoluTrafikPaketleri.TrafikSigortasi;
                    }
                }

                if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    if (teklif.GenelBilgiler.TaksitSayisi == 3)
                    {
                        request.ODEME_TIPI = AnadoluOdemeTipleri.KrediKartiBlokeliTaksit;
                        request.TAKSIT_SAYISI = AnadoluTaksitSayısı.PesinBlokeli3Taksit;
                    }
                    else
                    {
                        request.ODEME_TIPI = AnadoluOdemeTipleri.SanalPost;
                        request.TAKSIT_SAYISI = AnadoluTaksitSayısı.SanalPosPesin;
                    }
                }
                else
                {
                    request.ODEME_TIPI = AnadoluOdemeTipleri.Nakit;
                    request.TAKSIT_SAYISI = AnadoluTaksitSayısı.NakitPesin;
                }

                #endregion

                #region CallService

                this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Teklif);
                var response = new PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET_Response();
                if (teklif.Arac.Marka == "600")
                {
                    var res = new PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET_Response();
                    this.EndLog(res, false, res.GetType());
                    this.AddHata("Hata! Anadolu Sigorta, Markası Motosiklet olan araçlara Online Teklif düzenletmemektedir.");
                }
                else
                {
                    response = servis.CallService<PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET_Response>(request);
                    if (response.HATA_KODU != "0")
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.HATA_TEXT);
                    }
                    else
                    {
                        this.EndLog(response, true, response.GetType());
                    }
                }

                #endregion

                #region Basarı Kontrol

                if (!this.Basarili)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    return;
                }

                #endregion

                #region Teklif kaydı

                #region Genel bilgiler

                this.Import(teklif);

                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = ANADOLUMessage.ToDecimal(response.BRUT_PRIM);
                this.GenelBilgiler.TUMTeklifNo = response.TEKLIF_NO;

                decimal THGFonu = ANADOLUMessage.ToDecimal(response.THG_FON);
                decimal GiderVergisi = ANADOLUMessage.ToDecimal(response.BSMV);
                decimal GarantiFonu = ANADOLUMessage.ToDecimal(response.KTGS);

                this.GenelBilgiler.ToplamVergi = THGFonu + GiderVergisi + GarantiFonu;
                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.GecikmeZammiYuzdesi = ANADOLUMessage.ToDecimal(response.GECIKME_SURPRIM_ORANI) * 100;

                decimal hasarsizlik = ANADOLUMessage.ToDecimal(response.HASARSIZLIK_ORANI);
                if (hasarsizlik > 0)
                {
                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                    this.GenelBilgiler.HasarSurprimYuzdesi = hasarsizlik * 100;
                }
                else if (hasarsizlik < 0)
                {
                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Math.Abs(hasarsizlik) * 100;
                    this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                }
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = 0;
                //  this.GenelBilgiler.ToplamKomisyon = ANADOLUMessage.ToDecimal(response.KOMISYON_TUTARI); Ayın 27 sinde aktif olacak

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                if (!String.IsNullOrEmpty(response.TEKLIF_PDF))
                {
                    byte[] data = Convert.FromBase64String(response.TEKLIF_PDF);

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("Anadolu_Trafik_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("trafik", fileName, data);
                    this.GenelBilgiler.PDFDosyasi = url;

                    _Log.Info("Police_PDF url: {0}", url);
                }

                #endregion

                #region Vergiler

                this.AddVergi(TrafikVergiler.THGFonu, THGFonu);
                this.AddVergi(TrafikVergiler.GiderVergisi, GiderVergisi);
                this.AddVergi(TrafikVergiler.GarantiFonu, GarantiFonu);

                #endregion

                #region Teminatlar

                var maxBaslamaTarih = _AracContext.AracTrafikTeminatRepository.All().Max(f => f.GecerlilikBaslamaTarihi);
                var teminat = _AracContext.AracTrafikTeminatRepository.Filter(f => f.GecerlilikBaslamaTarihi <= this.GenelBilgiler.BaslamaTarihi &&
                                                                                    f.GecerlilikBaslamaTarihi >= maxBaslamaTarih &&
                                                                                    f.AracGrupKodu == TRAMER_TARIFE_KODU)
                                                                        .FirstOrDefault();

                if (teminat != null)
                {
                    this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, teminat.MaddiAracBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, teminat.MaddiKazaBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, teminat.TedaviKisiBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, teminat.TedaviKazaBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, teminat.SakatlikKisiBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, teminat.SakatlikKazaBasina.Value, 0, 0, 0, 0);
                }
                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Asistans, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Tedavi, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Trafik, 0, 0, 0, 0, 0);

                #endregion

                #region Ödeme Planı

                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }

                #endregion

                #endregion


                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            try
            {
                #region Eski Versiyon

                #region Veri Hazırlama GENEL
                //ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);

                //MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                #endregion

                #region Anadolu sigorta müşterisi
                //string MUSTERI_NO = _CRService.GetTUMMusteriKodu(this.TUMKodu, sigortali.MusteriKodu);

                //if (String.IsNullOrEmpty(MUSTERI_NO))
                //{
                //    MUSTERI_NO = this.MusteriKaydet(null, sigortali, teklif.GenelBilgiler.TVMKodu, konfig[Konfig.ANADOLU_ServiceUrl]);

                //    if (String.IsNullOrEmpty(MUSTERI_NO))
                //    {
                //        this.Hatalar.Add("ANADOLU SİGORTA sisteminde müşteri oluşturulamadı.");
                //        this.GenelBilgiler.Basarili = false;
                //        this.GenelBilgiler.WEBServisLogs = this.Log;
                //        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                //        return;
                //    }
                //}
                #endregion

                #region Araç Bilgileri
                //PR_SWEP_URT_OTO_WS_TRAFIK_KAYDET request = new PR_SWEP_URT_OTO_WS_TRAFIK_KAYDET();
                //request.MUSTERI_NO = MUSTERI_NO;
                //request.ACENTE_KODU = String.Empty;
                //request.ALT_ACENTE_KODU = String.Empty;

                //DateTime baslangicTarihi = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                //request.BASLANGIC_TARIHI = baslangicTarihi.ToString("yyyyMMdd");
                //request.BITIS_TARIHI = baslangicTarihi.AddYears(1).ToString("yyyyMMdd");

                //DateTime tescilTarihi = teklif.ReadSoru(TrafikSorular.Trafik_Tescil_Tarihi, TurkeyDateTime.Today);
                //request.TESCIL_TARIHI = tescilTarihi.ToString("yyyyMMdd");

                //DateTime trafigeCikisTarihi = teklif.ReadSoru(TrafikSorular.Trafige_Cikis_Tarihi, TurkeyDateTime.Today);
                //request.TRAFIGE_CIKIS_TARIHI = trafigeCikisTarihi.ToString("yyyyMMdd");

                //string aracKodu = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                //request.ARAC_KODU = aracKodu.TrimStart('0');
                //request.PLAKA_TURU = "Yerli";
                //request.PLAKA_IL_KODU = teklif.Arac.PlakaKodu;
                //request.PLAKA_NO = teklif.Arac.PlakaNo;
                //request.ARAC_TESCIL_KODU = teklif.Arac.TescilSeriKod;
                //request.ARAC_TESCIL_SERI_NO = teklif.Arac.TescilSeriNo;
                //request.MODEL_YILI = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value.ToString() : String.Empty;

                //string TRAMER_TARIFE_KODU = String.Empty;
                //string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                //if (parts.Length == 2)
                //{
                //    string kullanimTarziKodu = parts[0];
                //    string kod2 = parts[1];
                //    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                //                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                //                                                                                  f.Kod2 == kod2)
                //                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                //    if (kullanimTarzi != null)
                //    {
                //        TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                //    }
                //}

                //request.KULLANIM_TIPI_KODU = AracKullanimTipi(request.MODEL_YILI, request.ARAC_KODU, TRAMER_TARIFE_KODU, teklif.GenelBilgiler.TVMKodu, konfig[Konfig.ANADOLU_ServiceUrl]);
                //request.KULLANIM_SEKLI_KODU = AracKullanimSekli(request.MODEL_YILI, request.ARAC_KODU, TRAMER_TARIFE_KODU, request.KULLANIM_TIPI_KODU, teklif.GenelBilgiler.TVMKodu, konfig[Konfig.ANADOLU_ServiceUrl]);
                //switch (teklif.Arac.KullanimSekli)
                //{
                //    case "0": request.KULLANIM_AMACI = "Ticari"; break;
                //    case "1": request.KULLANIM_AMACI = "Resmi"; break;
                //    case "2": request.KULLANIM_AMACI = "Hususi"; break;
                //}

                //request.KILOMETRE = String.Empty;
                //request.ARAC_RENGI = String.Empty;
                //request.ARAC_ALT_RENGI = String.Empty;

                //request.MOTOR_NO = teklif.Arac.MotorNo;
                //request.SASI_NO = teklif.Arac.SasiNo;
                //request.MUAYENE_GECERLILIK_TARIHI = String.Empty;
                //request.TAKOMETRE = String.Empty;
                #endregion

                #region Önceki Poliçe
                //bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                //if (eskiPoliceVar)
                //{
                //    request.ONCEKI_SIRKET_KODU = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                //    request.ONCEKI_ACENTE_KODU = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                //    request.ONCEKI_POLICE_NO = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                //    request.ONCEKI_YENILEME_NO = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                //}
                #endregion

                #region Taşıyıcı Sorumluluk
                //bool tasiyiciSorumluluk = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, false);
                //if (tasiyiciSorumluluk)
                //{
                //    request.ZKTMS_SIRKET_KODU = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
                //    request.ZKTMS_ACENTE_KODU = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                //    request.ZKTMS_ESKI_POLICE_NO = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                //    request.ZKTMS_YENILEME_NO = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
                //}
                #endregion

                #region Ödeme Bilgileri
                //request.NET_PRIM = ANADOLUMessage.ToDecimalString(this.GenelBilgiler.NetPrim.Value);

                //byte odemetipi = this.GenelBilgiler.OdemeTipi ?? OdemeTipleri.Yok;
                //byte odemesekli = this.GenelBilgiler.OdemeSekli ?? OdemeSekilleri.Pesin;
                //byte taksitsayisi = odeme.TaksitSayisi;

                ///*
                // ödeme tipi
                //    1: Kredi Kartı Blokeli Taksit
                //    2: Sanal Pos(peşinat) & Kredi kartı talimatı
                // */
                //request.ODEME_TIPI = odemesekli == OdemeSekilleri.Vadeli ? "1" : "2";

                ///*
                //Taksit Sayısı girilir
                //    Seçilen ödeme tipi “1” ise: ”3, 4, 5, 6, 7, 8, 9, 10, 11, 12” taksit seçeneğinden biri;
                //    Seçilen ödeme tipi “2” ise: ”0, 1” taksit seçeneğinden biri gönderilmelidir.
                // */
                //request.TAKSIT_SAYISI = taksitsayisi.ToString();
                //request.PESIN_BLOKELI_TAKSIT = String.Empty;


                ////request.ODEME_TIPI = "2";
                ////request.TAKSIT_SAYISI = "1";

                //MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();
                //if (telefon != null)
                //{
                //    string[] telParts = telefon.Numara.Split('-');

                //    if (telParts.Length == 3)
                //    {
                //        request.KART_TEL_ALAN = telParts[1];
                //        request.KART_TEL_NO = telParts[2];
                //    }
                //}
                #endregion

                #region service call
                //request.E_POSTA = sigortali.EMail;
                //request.SEND_EPOSTA = "0";
                //request.RETURN_GENEL_SART_PDF = "1";
                //request.RETURN_POLICE_PDF = "1";

                //TEst
                //request.KREDI_KART_NO = "5126515200012347";
                //request.SON_KULLANMA_TARIHI = "012014";
                //request.CVV2 = "820";
                //request.KART_TEL_ALAN = "545";
                //request.KART_TEL_NO = "5568775";

                //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.ANADOLU });

                //request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                //request.WEB_SIFRE = servisKullanici.Sifre2;

                //ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2);
                //servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

                //this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Police);
                ////Kredi kartı bilgileri Loglanmıyor.
                //if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                //{
                //    request.KREDI_KART_NO = odeme.KrediKarti.KartNo;
                //    request.SON_KULLANMA_TARIHI = odeme.KrediKarti.SKY + odeme.KrediKarti.SKA;
                //    request.CVV2 = odeme.KrediKarti.CVC;

                //    string[] adparts = odeme.KrediKarti.KartSahibi.Split(' ');
                //    if (adparts.Length == 1)
                //    {
                //        request.KART_SAHIBI_ADI = odeme.KrediKarti.KartSahibi;
                //    }
                //    else if (adparts.Length == 2)
                //    {
                //        request.KART_SAHIBI_ADI = adparts[0];
                //        request.KART_SAHIBI_SOYADI = adparts[1];
                //    }
                //    else if (adparts.Length == 3)
                //    {
                //        request.KART_SAHIBI_ADI = adparts[0] + " " + adparts[1];
                //        request.KART_SAHIBI_SOYADI = adparts[2];
                //    }
                //    else
                //    {
                //        request.KART_SAHIBI_ADI = odeme.KrediKarti.KartSahibi;
                //    }
                //}
                //else
                //{
                //    request.KREDI_KART_NO = "";
                //    request.SON_KULLANMA_TARIHI = "";
                //    request.CVV2 = "";
                //    request.KART_SAHIBI_ADI = "";
                //}
                //PR_SWEP_URT_OTO_WS_TRAFIK_KAYDET_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_TRAFIK_KAYDET_Response>(request);

                #endregion

                #region Hata Kontrol Ve Kayıt
                //if (response.HATA_KODU != "0")
                //{
                //    this.EndLog(response, false, response.GetType());
                //    this.AddHata(response.HATA_TEXT);
                //}
                //else
                //    this.EndLog(response, true, response.GetType());


                //if (this.Hatalar.Count == 0)
                //{
                //    this.GenelBilgiler.TUMPoliceNo = response.POLICE_NO;
                //    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                //    //Muhasebe aktarımı
                //    //this.SendMuhasebe();

                //    // ==== POLICE_PDF	String	PDF dosyasının şifreli hali Base64
                //    if (!String.IsNullOrEmpty(response.POLICE_PDF))
                //    {
                //        byte[] data = Convert.FromBase64String(response.POLICE_PDF);

                //        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                //        string fileName = String.Format("Anadolu_Trafik_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                //        string url = storage.UploadFile("kasko", fileName, data);
                //        this.GenelBilgiler.PDFPolice = url;

                //        _Log.Info("Police_PDF url: {0}", url);
                //    }

                //    // ==== GENEL_SART_PDF	String	PDF dosyasının şifreli hali Base64
                //    if (!String.IsNullOrEmpty(response.GENEL_SART_PDF))
                //    {
                //        byte[] data = Convert.FromBase64String(response.GENEL_SART_PDF);

                //        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                //        string fileName = String.Format("Anadolu_Trafik_GenelSartlar_{0}.pdf", System.Guid.NewGuid().ToString());
                //        string url = storage.UploadFile("kasko", fileName, data);
                //        this.GenelBilgiler.PDFGenelSartlari = url;

                //        _Log.Info("PDF_Genel şartlar url: {0}", url);
                //    }
                //}

                //this.GenelBilgiler.WEBServisLogs = this.Log;
                //_TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                #endregion

                #endregion

                #region Yeni Versiyon

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ANADOLU });
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                var request = new PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR();
                request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                request.WEB_SIFRE = servisKullanici.Sifre2;
                request.SESSION_ID = "LOGIN";
                request.REVIZYON_NO = string.Empty;
                request.OTORIZASYON_KODU = string.Empty;
                request.REFERANS_ID = string.Empty;
                request.RETURN_POLICE_PDF = "1";
                request.RETURN_GENEL_SART_PDF = "1";
                request.RETURN_BILGILENDIRME_FORMU_PDF = "1";

                MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();
                if (telefon != null)
                {
                    string[] telParts = telefon.Numara.Split('-');
                    if (telParts.Length == 3)
                    {
                        request.KART_TEL_ALAN = telParts[1];
                        request.KART_TEL_NO = telParts[2];
                    }
                }

                if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    if (teklif.GenelBilgiler.TaksitSayisi == 3)
                    {
                        request.SECILEN_ODEME_TIPI = AnadoluOdemeTipleri.KrediKartiBlokeliTaksit;
                        request.TAKSIT_SAYISI = AnadoluTaksitSayısı.PesinBlokeli3Taksit;
                    }
                    else
                    {
                        request.SECILEN_ODEME_TIPI = AnadoluOdemeTipleri.SanalPost;
                        request.TAKSIT_SAYISI = AnadoluTaksitSayısı.SanalPosPesin;
                    }
                }
                else
                {
                    request.SECILEN_ODEME_TIPI = AnadoluOdemeTipleri.Nakit;
                    request.TAKSIT_SAYISI = AnadoluTaksitSayısı.NakitPesin;
                }

                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    request.KREDI_KART_NO = odeme.KrediKarti.KartNo;
                    request.SON_KULLANMA_TARIHI = odeme.KrediKarti.SKY + odeme.KrediKarti.SKA;
                    request.CVV2 = odeme.KrediKarti.CVC;

                    string[] adparts = odeme.KrediKarti.KartSahibi.Split(' ');
                    if (adparts.Length == 1)
                    {
                        request.KART_SAHIBI_ADI = odeme.KrediKarti.KartSahibi;
                    }
                    else if (adparts.Length == 2)
                    {
                        request.KART_SAHIBI_ADI = adparts[0];
                        request.KART_SAHIBI_SOYADI = adparts[1];
                    }
                    else if (adparts.Length == 3)
                    {
                        request.KART_SAHIBI_ADI = adparts[0] + " " + adparts[1];
                        request.KART_SAHIBI_SOYADI = adparts[2];
                    }
                    else
                    {
                        request.KART_SAHIBI_ADI = odeme.KrediKarti.KartSahibi;
                    }
                }
                else
                {
                    request.KREDI_KART_NO = "";
                    request.SON_KULLANMA_TARIHI = "";
                    request.CVV2 = "";
                    request.KART_SAHIBI_ADI = "";
                }

                request.TEKLIF_NO = this.GenelBilgiler.TUMTeklifNo;

                #region Call Service

                this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Police);

                var response = servis.CallService<PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR_Response>(request);

                if (response.HATA_KODU != "0")
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.HATA_TEXT);
                }
                else
                    this.EndLog(response, true, response.GetType());

                #endregion

                #region Kayıt

                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.POLICE_NO;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    if (!String.IsNullOrEmpty(response.POLICE_PDF))
                    {
                        byte[] data = Convert.FromBase64String(response.POLICE_PDF);

                        IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Format("Anadolu_Trafik_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                        string url = storage.UploadFile("trafik", fileName, data);
                        this.GenelBilgiler.PDFPolice = url;

                        _Log.Info("Police_PDF url: {0}", url);
                    }

                    if (!String.IsNullOrEmpty(response.GENEL_SART_PDF))
                    {
                        byte[] data = Convert.FromBase64String(response.GENEL_SART_PDF);

                        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                        string fileName = String.Format("Anadolu_Trafik_GenelSartlar_{0}.pdf", System.Guid.NewGuid().ToString());
                        string url = storage.UploadFile("kasko", fileName, data);
                        this.GenelBilgiler.PDFGenelSartlari = url;

                        _Log.Info("PDF_Genel şartlar url: {0}", url);
                    }
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }

        public string MusteriKaydet(ITeklif teklif, MusteriGenelBilgiler sigortali, int tvmKodu, string servisUrl)
        {
            string MUSTERI_NO = String.Empty;
            try
            {
                #region Ana bilgiler
                PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET request = new PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET();
                switch (sigortali.MusteriTipKodu)
                {
                    case MusteriTipleri.TCMusteri: request.MUSTERI_TIPI = "tcozel"; break;
                    case MusteriTipleri.TuzelMusteri: request.MUSTERI_TIPI = "tctuzel"; break;
                    case MusteriTipleri.SahisFirmasi: request.MUSTERI_TIPI = "tctuzel"; break;
                    case MusteriTipleri.YabanciMusteri: request.MUSTERI_TIPI = "yozel"; break;
                    default: request.MUSTERI_TIPI = String.Empty; break;
                }

                if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                {
                    request.TCKN = sigortali.KimlikNo;
                    request.MUSTERI_ADI = sigortali.AdiUnvan;
                    request.MUSTERI_SOYADI = sigortali.SoyadiUnvan;
                    if (sigortali.DogumTarihi.HasValue)
                        request.DOGUM_TARIHI = sigortali.DogumTarihi.Value.ToString("yyyyMMdd");
                    request.CINSIYET = sigortali.Cinsiyet;
                }
                else if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri || sigortali.MusteriTipKodu == MusteriTipleri.SahisFirmasi)
                {
                    request.VKN = sigortali.KimlikNo;
                    request.UNVAN = String.Format("{0} {1}", sigortali.AdiUnvan, sigortali.SoyadiUnvan);
                }
                else if (sigortali.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                {
                    request.YABANCI_KIMLIK_NO = sigortali.KimlikNo;
                    request.ULKE_KODU = String.Empty;
                }

                request.MUSTERI_EPOSTA = sigortali.EMail;
                #endregion

                #region Adres ve telefon
                MusteriAdre adres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                if (adres != null)
                {
                    CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                  f.IlKodu == adres.IlKodu &&
                                                                                  f.IlceKodu == adres.IlceKodu)
                                                                    .SingleOrDefault<CR_IlIlce>();

                    if (ililce != null)
                    {
                        switch (adres.AdresTipi)
                        {
                            case AdresTipleri.Ev: request.ADRES_TIPI = "Ev"; break;
                            case AdresTipleri.Is: request.ADRES_TIPI = "İş"; break;
                            case AdresTipleri.Diger: request.ADRES_TIPI = "Diğer"; break;
                            default: request.ADRES_TIPI = "Diğer"; break;
                        }

                        request.ADRES_IL_KODU = ililce.CRIlKodu;
                        request.ILCE_ADI = ililce.CRIlceAdi;
                        request.ILCE_KODU = ililce.CRIlceKodu;
                    }

                    request.MUSTERI_BINA_ADI = String.Empty;
                    request.MUSTERI_MAHALLE_KOY = adres.Mahalle;
                    request.MUSTERI_CADDE = adres.Cadde;
                    request.MUSTERI_SOKAK = adres.Sokak;
                    request.MUSTERI_KAPI_NO = adres.BinaNo;
                    request.MUSTERI_DAIRE_NO = adres.DaireNo;
                    request.MUSTERI_SERBESTADRES = adres.Adres;
                }

                string SE_NumaraTipi = teklif.ReadSoru(TrafikSorular.SENumaratipi, "");
                short numaratipi = IletisimNumaraTipleri.Cep;

                MusteriTelefon telefon = new MusteriTelefon();

                if (!String.IsNullOrEmpty(SE_NumaraTipi))
                {
                    numaratipi = Convert.ToInt16(SE_NumaraTipi);

                }
                telefon = sigortali.MusteriTelefons.FirstOrDefault(s => s.IletisimNumaraTipi == numaratipi);

                if (telefon != null)
                {
                    string[] parts = telefon.Numara.Split('-');

                    if (parts.Length == 3)
                    {
                        switch (telefon.IletisimNumaraTipi)
                        {
                            case IletisimNumaraTipleri.Ev: request.MUSTERI_TEL_TIP = "Ev"; break;
                            case IletisimNumaraTipleri.Is: request.MUSTERI_TEL_TIP = "İş"; break;
                            case IletisimNumaraTipleri.Cep: request.MUSTERI_TEL_TIP = "Cep"; break;
                            case IletisimNumaraTipleri.Fax: request.MUSTERI_TEL_TIP = "Faks"; break;
                            default: request.MUSTERI_TEL_TIP = "Ev"; break;
                        }

                        request.MUSTERI_TEL_ALAN = parts[1];
                        request.MUSTERI_TEL_NO = parts[2];
                    }
                }
                #endregion

                #region service call
                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });

                request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                request.WEB_SIFRE = servisKullanici.Sifre2;

                request.ANA_SEKTOR_KODU = "5200";
                request.ALT_SEKTOR_KODU = "5201";
                request.FAALIYET_OLCEGI = "DGR0010653";
                request.CIRO_ARALIGI = "DGR0021620";
                request.SABIT_VARLIK_ARALIGI = "DGR0022113";

                //Sonradan Eklendi
                request.MESLEK_GRUBU_KODU = "1300";
                request.MESLEK_TIPI_KODU = "1399";

                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = servisUrl;
                teklif = null;
                //HEADER ile LOGLANLANIYOR
                if (teklif != null)
                    teklif.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.MusteriKayit);
                else
                    this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.MusteriKayit);


                PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET_Response response = servis.CallService<PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET_Response>(request);

                MUSTERI_NO = response.MUSTERI_NO;
                #endregion

                #region Hata Kontrol
                if (response.HATA_KODU != "0")
                {
                    if (teklif != null)
                    {
                        teklif.EndLog(response, false, response.GetType());
                        teklif.AddHata(response.HATA_TEXT);
                    }
                    else
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.HATA_TEXT);
                    }
                }
                else
                    if (teklif != null) teklif.EndLog(response, true, response.GetType());
                else this.EndLog(response, true, response.GetType());


                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                if (teklif != null)
                {
                    teklif.EndLog(ex.Message, false);
                    teklif.AddHata(ex.Message);
                }
                else
                {
                    this.EndLog(ex.Message, false);
                    this.AddHata(ex.Message);
                }



                #endregion
            }
            return MUSTERI_NO;
        }

        //public string AracKullanimTipi(string modelYili, string aracKodu, string tarifeKodu, int tvmKodu, string servisUrl)
        //{
        //    string kullanimTipi = String.Empty;

        //    try
        //    {
        //        PR_SWEP_URT_OTO_WS_SET_KULLANIM_TIPI_FOR_INTERNET_SATIS request = new PR_SWEP_URT_OTO_WS_SET_KULLANIM_TIPI_FOR_INTERNET_SATIS();

        //        request.ARAC_KODU = aracKodu;
        //        request.MODEL_YILI = modelYili;
        //        request.TRAMER_TARIFE_KODU = tarifeKodu;

        //        TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ANADOLU });

        //        KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);

        //        ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
        //        servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

        //        PR_SWEP_URT_OTO_WS_SET_KULLANIM_TIPI_FOR_INTERNET_SATIS_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_SET_KULLANIM_TIPI_FOR_INTERNET_SATIS_Response>(request);


        //        System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
        //        var jsonArray = js.Deserialize<List<object>>(response.KULLANIM_TIPI);

        //        object[] values = (object[])jsonArray.FirstOrDefault();

        //        if (values != null && values.Length > 1)
        //        {
        //            kullanimTipi = values[0].ToString();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _Log.Error(ex);
        //    }

        //    return kullanimTipi;
        //}

        //public string AracKullanimSekli(string modelYili, string aracKodu, string tarifeKodu, string kullanimTipi, int tvmKodu, string servisUrl, string grupUrunAdi)
        //{
        //    string kullanimSekli = String.Empty;

        //    try
        //    {
        //        PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS request = new PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS();

        //        request.ARAC_KODU = aracKodu;
        //        request.MODEL_YILI = modelYili;
        //        request.TRAMER_TARIFE_KODU = tarifeKodu;

        //        TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ANADOLU });

        //        KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);

        //        ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
        //        servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

        //        PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS_Response>(request);

        //        System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
        //        var jsonArray = js.Deserialize<List<object>>(response.KULLANIM_SEKLI);

        //        object[] values = (object[])jsonArray.FirstOrDefault();

        //        if (values != null && values.Length > 1)
        //        {
        //            kullanimSekli = values[0].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _Log.Error(ex);
        //    }

        //    return kullanimSekli;
        //}

        //Anadolu Sigorta Kullanım Tipi ve Kullanım Şekli metodlarında GET olan Anadolu metotlar kullanılıyor

        public string AracKullanimTipi(string modelYili, string aracKodu, string tarifeKodu, int tvmKodu, string servisUrl, string grupUrunAdi)
        {
            string kullanimTipi = String.Empty;

            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);
                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI request = new PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI();

                request.MARKA_KODU = aracKodu;
                request.MODEL_YILI = modelYili;
                request.TRAMER_TARIFE_KODU = tarifeKodu;
                request.GRUP_URUN_ADI = grupUrunAdi;

                this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Teklif);

                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI_Response>(request);
                this.EndLog(response, false, response.GetType());

                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jsonArray = js.Deserialize<List<object>>(response.KULLANIM_TIPI);

                object[] values = (object[])jsonArray.FirstOrDefault();

                if (values != null && values.Length > 1)
                {

                    kullanimTipi = values[0].ToString();
                }

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return kullanimTipi;
        }

        public AnadoluReturnModel AracKullanimTipiList(string modelYili, string aracKodu, string tarifeKodu, int tvmKodu, string grupUrunAdi)
        {
            string kullanimTipi = String.Empty;
            AnadoluReturnModel model = new AnadoluReturnModel();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);
                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });

                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                string AnadoluMarkaKodu = String.Empty;
                ListModel KullanimTip = new ListModel();

                #region Anadolu Sigorta Marka Kodu Getir

                //Anadolu Sigorta, Kullanım Tipi metoduna Anadolu Sigortanın Marka kodunu istiyor. 
                var MarkaAdi = _AracContext.AracMarkaRepository.Find(s => s.MarkaKodu == aracKodu).MarkaAdi;
                if (aracKodu == "122")
                {
                    MarkaAdi = "RENAULT";
                }
                if (MarkaAdi.Contains("OTOYOL") && MarkaAdi.Contains("FIAT"))
                {
                    MarkaAdi = "OTOYOL\\FIAT";
                }
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL requestMarka = new PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL();
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response responseMarkaList = servis.CallService<PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response>(requestMarka);

                if (responseMarkaList.HATA_KODU == "0" && responseMarkaList.MARKA_BILGISI != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer js1 = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var jsonArray1 = js1.Deserialize<List<object>>(responseMarkaList.MARKA_BILGISI);
                    if (jsonArray1 != null)
                    {
                        var anadoluList = jsonArray1.ConvertToMarka();
                        var markaVarmi = anadoluList.Where(m => m.Marka == MarkaAdi).FirstOrDefault();
                        if (markaVarmi != null)
                        {
                            AnadoluMarkaKodu = markaVarmi.Value;
                        }
                    }
                }
                else
                {
                    model.list = new List<ListModel>();
                    model.hata = responseMarkaList.HATA_MESAJI;
                    return model;
                }

                #endregion


                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI request = new PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI();

                request.MARKA_KODU = AnadoluMarkaKodu;
                request.MODEL_YILI = modelYili;
                request.TRAMER_TARIFE_KODU = tarifeKodu;
                request.GRUP_URUN_ADI = grupUrunAdi;

                KonfigTable konfig2 = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);
                servis.Url = konfig2[Konfig.ANADOLU_ServiceUrl];

                this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Teklif);

                ANADOLUService servisKullanimTipi = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servisKullanimTipi.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI_Response response = servisKullanimTipi.CallService<PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_TIPI_LISTESI_Response>(request);
                this.EndLog(response, false, response.GetType());

                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jsonArray = js.Deserialize<List<object>>(response.KULLANIM_TIPI);

                if (jsonArray != null)
                {
                    object[] values = (object[])jsonArray.FirstOrDefault();

                    model.list = new List<ListModel>();
                    if (values != null && values.Length > 1)
                    {
                        var tipList = jsonArray.ConvertToTip();
                        foreach (var item in tipList)
                        {
                            KullanimTip = new ListModel();
                            KullanimTip.key = item.key;
                            KullanimTip.value = item.value;
                            model.list.Add(KullanimTip);
                        }
                    }
                    model.anadoluMarkaKodu = AnadoluMarkaKodu;
                }
                else
                {
                    model.list = new List<ListModel>();
                    model.hata = "Hata! Kullanım tipi/tipleri bulunamadı.";
                    model.anadoluMarkaKodu = "";
                }

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                model.list = new List<ListModel>();

                if (ex.Message.IndexOf("(500) Internal Server Error.") != -1)
                {
                    model.hata = "Hata! Anadolu Sigorta Şirketi sistemi ile bağlantı kurulamadı! Lüften, işleminizi 15-30 saniye sonra tekrar deneyiniz.";
                }
                else
                {
                    model.hata = "Hata! " + ex.Message;
                }
            }

            return model;
        }

        public string AracKullanimSekli(string modelYili, string aracKodu, string tarifeKodu, string kullanimTipi, int tvmKodu, string servisUrl, string grupUrunAdi)
        {
            string kullanimSekli = String.Empty;

            try
            {
                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI request = new PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI();

                request.MARKA_KODU = aracKodu;
                request.MODEL_YILI = modelYili;
                request.TRAMER_TARIFE_KODU = tarifeKodu;
                request.GRUP_URUN_ADI = grupUrunAdi;
                request.KULLANIM_TIPI_KODU = kullanimTipi;

                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);

                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI_Response>(request);

                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jsonArray = js.Deserialize<List<object>>(response.KULLANIM_SEKLI);

                object[] values = (object[])jsonArray.FirstOrDefault();

                if (values != null && values.Length > 1)
                {
                    kullanimSekli = values[0].ToString();
                }
                kullanimSekli = "DGR0000130";

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return kullanimSekli;
        }

        public AnadoluReturnModel AracKullanimSekliList(string anadoluMarkaKodu, string modelYili, string aracKodu, string tarifeKodu, string kullanimTipi, int tvmKodu)
        {
            string kullanimSekli = String.Empty;
            AnadoluReturnModel model = new AnadoluReturnModel();
            try
            {

                #region Anadolu Sigorta Marka Kodu Getir
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);

                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

                if (String.IsNullOrEmpty(anadoluMarkaKodu))
                {
                    //Anadolu Sigorta, Kullanım Tipi metoduna Anadolu Sigortanın Marka kodunu istiyor. 
                    var MarkaAdi = _AracContext.AracMarkaRepository.Find(s => s.MarkaKodu == aracKodu).MarkaAdi;
                    if (aracKodu == "122")
                    {
                        MarkaAdi = "RENAULT";
                    }

                    if (MarkaAdi.Contains("OTOYOL") && MarkaAdi.Contains("FIAT"))
                    {
                        MarkaAdi = "OTOYOL\\FIAT";
                    }
                    PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL requestMarka = new PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL();
                    PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response responseMarkaList = servis.CallService<PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response>(requestMarka);

                    if (responseMarkaList.HATA_KODU == "0" && responseMarkaList.MARKA_BILGISI != null)
                    {
                        System.Web.Script.Serialization.JavaScriptSerializer js1 = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var jsonArray1 = js1.Deserialize<List<object>>(responseMarkaList.MARKA_BILGISI);
                        if (jsonArray1 != null)
                        {
                            var anadoluList = jsonArray1.ConvertToMarka();
                            var markaVarmi = anadoluList.Where(m => m.Marka == MarkaAdi).FirstOrDefault();
                            if (markaVarmi != null)
                            {
                                anadoluMarkaKodu = markaVarmi.Value;
                            }
                        }
                    }
                    else
                    {
                        model.list = new List<ListModel>();
                        model.hata = responseMarkaList.HATA_MESAJI;
                        return model;
                    }
                }

                #endregion

                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI request = new PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI();
                ListModel KullanimSekli = new ListModel();

                request.MARKA_KODU = anadoluMarkaKodu;
                request.MODEL_YILI = modelYili;
                request.TRAMER_TARIFE_KODU = tarifeKodu;
                request.GRUP_URUN_ADI = ANADOLUKullanimTipiUrunGrupAdi.Kasko;
                request.KULLANIM_TIPI_KODU = kullanimTipi;

                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

                PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI_Response>(request);

                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jsonArray = js.Deserialize<List<object>>(response.KULLANIM_SEKLI);
                if (jsonArray != null)
                {
                    object[] values = (object[])jsonArray.FirstOrDefault();
                    model.list = new List<ListModel>();
                    if (values != null && values.Length > 1)
                    {
                        var tipList = jsonArray.ConvertToSekil();
                        foreach (var item in tipList)
                        {
                            KullanimSekli = new ListModel();
                            KullanimSekli.key = item.key;
                            KullanimSekli.value = item.value;
                            model.list.Add(KullanimSekli);
                        }
                    }
                }
                else
                {
                    model.list = new List<ListModel>();
                    model.hata = "Hata! Kullanım şekli/şekilleri bulunamadı.";
                }

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                model.list = new List<ListModel>();
                if (ex.Message.IndexOf("(500) Internal Server Error.") != -1)
                {
                    model.hata = "Hata! Anadolu Sigorta Şirketi sistemi ile bağlantı kurulamadı! Lüften, işleminizi 15-30 saniye sonra tekrar deneyiniz.";
                }
                else
                {
                    model.hata = "Hata! " + ex.Message;
                }
            }

            return model;
        }

        private string AnadoluPlaka(string plakaNo)
        {
            int no = -1;
            string nPlakaNo = string.Empty;
            if (plakaNo.Length < 6)
                foreach (var item in plakaNo.ToCharArray())
                {
                    if (int.TryParse(item.ToString(), out no))
                    {
                        for (int i = 0; i < 6 - plakaNo.Length; i++)
                            nPlakaNo += " ";
                        nPlakaNo += plakaNo.Substring(plakaNo.IndexOf(item));
                        break;
                    }
                    else
                        nPlakaNo += item;
                }
            return nPlakaNo;
        }

        public AnadoluReturnModel AracMarka(string aracKodu, int tvmKodu)
        {
            string kullanimTipi = String.Empty;
            AnadoluReturnModel model = new AnadoluReturnModel();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUTrafik);
                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });
               
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                string AnadoluMarkaKodu = String.Empty;
                ListModel KullanimTip = new ListModel();

                #region Anadolu Sigorta Marka Kodu Getir

                //Anadolu Sigorta, Kullanım Tipi metoduna Anadolu Sigortanın Marka kodunu istiyor. 
                var MarkaAdi = _AracContext.AracMarkaRepository.Find(s => s.MarkaKodu == aracKodu).MarkaAdi;
                if (aracKodu == "122")
                {
                    MarkaAdi = "RENAULT";
                }
                if (MarkaAdi.Contains("OTOYOL") && MarkaAdi.Contains("FIAT"))
                {
                    MarkaAdi = "OTOYOL\\FIAT";
                }
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL requestMarka = new PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL();
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response responseMarkaList = servis.CallService<PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response>(requestMarka);

                if (responseMarkaList.HATA_KODU == "0" && responseMarkaList.MARKA_BILGISI != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer js1 = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var jsonArray1 = js1.Deserialize<List<object>>(responseMarkaList.MARKA_BILGISI);
                    if (jsonArray1 != null)
                    {
                        var anadoluList = jsonArray1.ConvertToMarka();
                        var markaVarmi = anadoluList.Where(m => m.Marka == MarkaAdi).FirstOrDefault();
                        if (markaVarmi != null)
                        {
                            model.anadoluMarkaKodu = markaVarmi.Value;
                        }
                    }
                }
                else
                {
                    model.list = new List<ListModel>();
                    model.hata = responseMarkaList.HATA_MESAJI;
                    return model;
                }

                #endregion

            }
            catch (Exception ex)
            {
                _Log.Error(ex);

                if (ex.Message.IndexOf("(500) Internal Server Error.") != -1)
                {
                    model.hata = "Hata! Anadolu Sigorta Şirketi sistemi ile bağlantı kurulamadı! Lüften, işleminizi 15-30 saniye sonra tekrar deneyiniz.";
                }
                else
                {
                    model.hata = "Hata! " + ex.Message;
                }
            }
            model.list = new List<ListModel>();
            return model;
        }

    }

    //Anadolu Sigortanın WS'inden Markaları liste haline çevirmek için kullanılıyor.
    public static class Extension
    {
        public static List<AnadoluMarka> ConvertToMarka(this List<object> list)
        {
            var _list = new List<AnadoluMarka>();
            for (int i = 0; i < list.Count; i++)
            {
                object[] values = (object[])list[i];
                _list.Add(new AnadoluMarka() { Marka = values[1].ToString(), Value = values[0].ToString() });
            }
            return _list;
        }
        public class AnadoluMarka
        {
            public string Marka { get; set; }
            public string Value { get; set; }

        }
    }

    //Anadolu Sigortanın WS'inden Kullanım Tiplerini liste haline çevirmek için kullanılıyor.
    public static class Extensions
    {
        public static List<KullanimTip> ConvertToTip(this List<object> list)
        {
            var _list = new List<KullanimTip>();
            for (int i = 0; i < list.Count; i++)
            {
                object[] values = (object[])list[i];
                _list.Add(new KullanimTip() { key = values[0].ToString(), value = values[1].ToString() });
            }
            return _list;
        }
        public class KullanimTip
        {
            public string key { get; set; }
            public string value { get; set; }
        }
    }

    //Anadolu Sigortanın WS'inden Kullanım Şeklini liste haline çevirmek için kullanılıyor.
    public static class ExtensionSekill
    {
        public static List<KullanimSekil> ConvertToSekil(this List<object> list)
        {
            var _list = new List<KullanimSekil>();
            for (int i = 0; i < list.Count; i++)
            {
                object[] values = (object[])list[i];
                _list.Add(new KullanimSekil() { key = values[0].ToString(), value = values[1].ToString() });
            }
            return _list;
        }
        public class KullanimSekil
        {
            public string key { get; set; }
            public string value { get; set; }
        }
    }



    public class AnadoluReturnModel
    {
        public List<ListModel> list { get; set; }
        public string hata { get; set; }
        public string anadoluMarkaKodu { get; set; }
    }

}
