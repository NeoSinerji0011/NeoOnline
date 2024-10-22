using System;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Neosinerji.BABOnlineTP.Business.Pdf;
using Newtonsoft.Json;
using Neosinerji.BABOnlineTP.Business.Common.AEGON;
using System.Data;
using System.IO;
using System.Globalization;

namespace Neosinerji.BABOnlineTP.Business.AEGON
{
    public class AEGONEgitim : Teklif, IAEGONEgitim
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;

        string _RootPath;

        [InjectionConstructor]
        public AEGONEgitim(ICRService crService,
                                    ICRContext crContext,
                                    IMusteriService musteriService,
                                    IAracContext aracContext,
                                    IKonfigurasyonService konfigurasyonService,
                                    ITVMContext tvmContext,
                                    ILogService log,
                                    ITeklifService teklifService)
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
            _RootPath = System.Web.HttpContext.Current.Server.MapPath("/");
        }


        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.AEGON;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region REQUEST

                AegonEgitimRequest request = new AegonEgitimRequest();

                int odemeDonemiCarpani = 1;

                request.TeklifNo = teklif.TeklifNo;
                request.teklifTarihi = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                //request.musteriAdSoyad=teklif

                DateTime SigortaBaslangicTar = teklif.ReadSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                CultureInfo turkey = new CultureInfo("tr-TR");

                //Bu ürün için para birimi seçenekli değildir. Hesaplamalar ABD Doları üzerinden yapılmaktadır.
                request.parabirimi = "USD";

                TeklifSigortali teklifSigortali = teklif.Sigortalilar.FirstOrDefault();
                if (teklifSigortali != null)
                {
                    MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);

                    //Cinsiyeti
                    request.cinsiyet = sigortali.Cinsiyet;
                    request.musteriAdSoyad = String.Format("{0} {1}", sigortali.AdiUnvan, sigortali.SoyadiUnvan);

                    //pGVO
                    switch (sigortali.CiroBilgisi)
                    {
                        case "1": request.pGVO = 15.0; break;
                        case "2": request.pGVO = 20.0; break;
                        case "3": request.pGVO = 27.0; break;
                        case "4": request.pGVO = 35.0; break;
                        case "5": request.pGVO = 27.0; break;
                        default: request.pGVO = 27.0; break;
                    }

                    //Doğum Tarihi, Sigorta Başlangıç tarihi ve Yaş
                    if (sigortali.DogumTarihi.HasValue)
                    {
                        request.dogTar = sigortali.DogumTarihi.Value.ToString("dd.MM.yyyy");

                        request.sigBasTar = SigortaBaslangicTar.ToString("dd.MM.yyyy");

                        request.yas = AEGONTESabitPrimli.AegonYasHesapla(sigortali.DogumTarihi.Value, SigortaBaslangicTar);
                    }
                }

                //Sigorta Süresi
                string SigortaSuresi = teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(SigortaSuresi))
                    request.sigortaSure = Convert.ToInt32(SigortaSuresi);



                //Prim Odeme Donemi
                switch (teklif.ReadSoru(TESabitPrimliSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": request.odeDonem = "Aylık"; odemeDonemiCarpani = 1; break;
                    case "2": request.odeDonem = "3 Aylık"; odemeDonemiCarpani = 3; break;
                    case "3": request.odeDonem = "6 Aylık"; odemeDonemiCarpani = 6; break;
                    case "4": request.odeDonem = "Yıllık"; odemeDonemiCarpani = 12; break;
                }


                //pGeriOdeSure:
                string pGeriOdeSure = teklif.ReadSoru(EgitimSigortasiSorular.SigortaGeriOdemeSuresi, String.Empty);
                if (!String.IsNullOrEmpty(pGeriOdeSure))
                    request.pGeriOdeSure = Convert.ToInt32(pGeriOdeSure);


                //pGOYillikTutar
                request.pGOYillikTutar = Convert.ToDouble(teklif.ReadSoru(EgitimSigortasiSorular.GeriOdemelerdeAlinacakYillikTutar, decimal.Zero));

                #endregion

                #region RESPONSE | LOG

                this.BeginLog(request, typeof(AegonEgitimRequest), WebServisIstekTipleri.Teklif);

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.AEGON_ServiceURL);

                aegon.Service1 servis = new aegon.Service1();
                servis.Timeout = 150000;
                servis.Url = serviceURL;

                DataTable prim = servis.EgitimIcinHayat_ES01(request.TeklifNo, request.cinsiyet, request.dogTar, request.yas, request.parabirimi, request.sigBasTar,
                                                             request.sigortaSure, request.pGeriOdeSure, request.pGOYillikTutar, request.odeDonem, request.pGVO,
                                                             request.teklifTarihi, request.musteriAdSoyad, AegonCommon.FirmaKisaAdi);



                if (prim == null || prim.Rows[0]["HATA"] == null)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;

                    this.EndLog("Web Servise yanıt dönmedi", false);
                    this.AddHata("Web Servise yanıt dönmedi");
                    return;
                }


                DataRow row = prim.Rows[0];
                if (row["HATA"].ToString() != "HATA YOK")
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;

                    string hata = AegonHelper.AegonReplace(row["HATA"].ToString());
                    this.EndLog(hata, false);
                    this.AddHata(hata);
                    return;
                }
                else
                    this.EndLog(prim, true, prim.GetType());


                #endregion

                #region SUCCESS

                #region DataTable To Response

                AegonEgitimResponse response = new AegonEgitimResponse();

                //=====================ODEME BILGILERI========================//
                if (row["IB_AYLIK_PRIM"] != null)
                    response.IB_AYLIK_PRIM = row["IB_AYLIK_PRIM"].ToString();
                if (row["IB_YILLIK_PRIM"] != null)
                    response.IB_YILLIK_PRIM = row["IB_YILLIK_PRIM"].ToString();
                if (row["IB_TOPLAM_PRIM"] != null)
                    response.IB_TOPLAM_PRIM = row["IB_TOPLAM_PRIM"].ToString();
                if (row["IB_VERGI_AVANTAJI_TUTARI"] != null)
                    response.IB_VERGI_AVANTAJI_TUTARI = row["IB_VERGI_AVANTAJI_TUTARI"].ToString();
                if (row["IB_VER_AV_S_MALIYET"] != null)
                    response.IB_VER_AV_S_MALIYET = row["IB_VER_AV_S_MALIYET"].ToString();



                //=====================TEMINAT VE KESINTILER========================//
                if (row["IB_VEFAT_TEMINAT_TUTARI"] != null)
                    response.IB_VEFAT_TEMINAT_TUTARI = row["IB_VEFAT_TEMINAT_TUTARI"].ToString();
                if (row["IB_YILLIK_ORT_RISK_PRM_KES"] != null)
                    response.IB_YILLIK_ORT_RISK_PRM_KES = row["IB_YILLIK_ORT_RISK_PRM_KES"].ToString();
                if (row["IB_KAR_PAYI_DAG_ORAN"] != null)
                    response.IB_KAR_PAYI_DAG_ORAN = row["IB_KAR_PAYI_DAG_ORAN"].ToString();



                //===================GETIRI ORANLARI=============//
                if (row["IB_MIN_GETIRI_ORAN"] != null)
                    response.IB_MIN_GETIRI_ORAN = row["IB_MIN_GETIRI_ORAN"].ToString();
                if (row["IB_ORT_GETIRI_ORAN"] != null)
                    response.IB_ORT_GETIRI_ORAN = row["IB_ORT_GETIRI_ORAN"].ToString();
                if (row["IB_MAX_GETIRI_ORAN"] != null)
                    response.IB_MAX_GETIRI_ORAN = row["IB_MAX_GETIRI_ORAN"].ToString();



                //===================VERGI ONCESI GETIRI ORANLARI==================//
                if (row["IB_VO_MIN_GETIRI"] != null)
                    response.IB_VO_MIN_GETIRI = row["IB_VO_MIN_GETIRI"].ToString();
                if (row["IB_VO_ORT_GETIRI"] != null)
                    response.IB_VO_ORT_GETIRI = row["IB_VO_ORT_GETIRI"].ToString();
                if (row["IB_VO_MAX_GETIRI"] != null)
                    response.IB_VO_MAX_GETIRI = row["IB_VO_MAX_GETIRI"].ToString();



                //===================NET GETIRI ORANLARI==================//
                if (row["IB_NET_MIN"] != null)
                    response.IB_NET_MIN = row["IB_NET_MIN"].ToString();
                if (row["IB_NET_ORT"] != null)
                    response.IB_NET_ORT = row["IB_NET_ORT"].ToString();
                if (row["IB_NET_MAX"] != null)
                    response.IB_NET_MAX = row["IB_NET_MAX"].ToString();



                //===================EGITIM SURESI YILLIK ODEME==================//
                if (row["IB_EGT_SURE_YILLIK_ODE_MIN"] != null)
                    response.IB_EGT_SURE_YILLIK_ODE_MIN = row["IB_EGT_SURE_YILLIK_ODE_MIN"].ToString();
                if (row["IB_EGT_SURE_YILLIK_ODE_ORT"] != null)
                    response.IB_EGT_SURE_YILLIK_ODE_ORT = row["IB_EGT_SURE_YILLIK_ODE_ORT"].ToString();
                if (row["IB_EGT_SURE_YILLIK_ODE_MAX"] != null)
                    response.IB_EGT_SURE_YILLIK_ODE_MAX = row["IB_EGT_SURE_YILLIK_ODE_MAX"].ToString();



                //=====================DIGER BILGILER (SURUM - TETKIK - EGITIM ONCESI SURE)========================//
                if (row["SURUM_BILGI"] != null)
                    response.SURUM_BILGI = row["SURUM_BILGI"].ToString();
                if (row["TIBBI_TETKIK_SONUCU"] != null)
                    response.TIBBI_TETKIK_SONUCU = row["TIBBI_TETKIK_SONUCU"].ToString();
                if (row["IB_EGITIM_ONCESI_SURE"] != null)
                    response.IB_EGITIM_ONCESI_SURE = row["IB_EGITIM_ONCESI_SURE"].ToString();


                #endregion

                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = SigortaBaslangicTar;
                this.GenelBilgiler.BitisTarihi = SigortaBaslangicTar.AddYears(request.sigortaSure);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Now.AddDays(30);

                if (!String.IsNullOrEmpty(response.IB_YILLIK_PRIM))
                {
                    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(response.IB_YILLIK_PRIM, turkey);
                }

                if (!String.IsNullOrEmpty(response.IB_AYLIK_PRIM))
                {
                    //Aylık ödeme tutarı ödeme dönemi ile çarpılıyor ve dönemsel ödeme primi net prim bölümüne yazılıyor.
                    this.GenelBilgiler.NetPrim = Convert.ToDecimal(response.IB_AYLIK_PRIM, turkey) * odemeDonemiCarpani;
                }

                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.Doviz;
                this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = 0;
                this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                #region Teminatlar

                if (!String.IsNullOrEmpty(response.IB_VEFAT_TEMINAT_TUTARI))
                    this.AddTeminat(EgitimSigortasiTeminatlar.VefatTeminati, Convert.ToDecimal(response.IB_VEFAT_TEMINAT_TUTARI, turkey), 0, 0, 0, 0);

                #endregion

                #region Ödeme Planı

                this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));

                #endregion

                #region WebServiceResponse

                this.AddWebServisCevap(Common.WebServisCevaplar.SurumNo, response.SURUM_BILGI);

                //Boş Geldiğinde kaydetmiyor ve raporlama ekranında sorun yaratıyor.
                if (String.IsNullOrEmpty(response.TIBBI_TETKIK_SONUCU))
                {
                    response.TIBBI_TETKIK_SONUCU = " ";
                }

                //FORMATI DÜZELTİLİYOR.
                response.TIBBI_TETKIK_SONUCU = AegonHelper.AegonReplace(response.TIBBI_TETKIK_SONUCU);

                this.AddWebServisCevap(Common.WebServisCevaplar.TibbiTetkikSonucu, response.TIBBI_TETKIK_SONUCU);

                #endregion

                #region PDF

                CreateTeklifPDF(teklif, response, prim);

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

        public void CreateTeklifPDF(ITeklif anaTeklif, AegonEgitimResponse response, DataTable table)
        {
            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(_RootPath + "Content/templates/", PdfTemplates.EGITIM);

                pdf = new PDFHelper("Babonline", "Egitim Sigortası", "Eğitim Sigortası", 8, _RootPath + "Content/fonts/", PdfTemplates.SenticoSansDT_Regular);

                string surumNo = String.Empty;
                surumNo = response.SURUM_BILGI;
                //SAYFA NUMARASI YAZIYOR
                pdf.SetPageEventHelper(new PDFCustomEventHelperAEGON(surumNo));

                PDFParser parser = new PDFParser(template, pdf);

                #endregion

                #region Data Fill

                #region EGİTİM Poliçesi Bilgileri
                string imgpath = Path.Combine(_RootPath, "Content/img/Aegon/aegonlogo-pdf.jpg");
                string OB_FooterBorder = Path.Combine(_RootPath, "Content/img/Aegon/OB_FooterBorder.jpg");

                parser.SetVariable("$TVMLogo$", imgpath);
                parser.SetVariable("$E_FooterBorder$", OB_FooterBorder);

                #region SigortaAdayi Bilgileri

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(anaTeklif.SigortaEttiren.MusteriKodu);

                //Sigorta Adayı Bilgileri
                string AdiSoyadi = String.Empty;
                string DogumTarihi = String.Empty;
                string Cinsiyeti = String.Empty;
                string BaslangicYasi = String.Empty;
                string VergiIadeOran = String.Empty;

                DateTime SBTarihi = anaTeklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                if (musteri != null)
                {
                    AdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                    Cinsiyeti = musteri.Cinsiyet == "E" ? "Erkek" : "Kadın";
                    CultureInfo culture = new CultureInfo("tr-TR");
                    DogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty;

                    if (musteri.DogumTarihi.HasValue)
                        BaslangicYasi = AEGONTESabitPrimli.AegonYasHesapla(musteri.DogumTarihi.Value, SBTarihi).ToString();

                    switch (musteri.CiroBilgisi)
                    {
                        case "1": VergiIadeOran = "%15"; break;
                        case "2": VergiIadeOran = "%20"; break;
                        case "3": VergiIadeOran = "%27"; break;
                        case "4": VergiIadeOran = "%35"; break;
                        case "5": VergiIadeOran = "%27"; break;
                    }
                }

                parser.SetVariable("$AdiSoyadi$", AdiSoyadi);
                parser.SetVariable("$DogumTarihi$", DogumTarihi);
                parser.SetVariable("$BaslangicYasi$", BaslangicYasi);
                parser.SetVariable("$Cinsiyeti$", Cinsiyeti);

                #endregion

                #region Odeme Bilgileri

                string AylikPrimTutari = String.Empty;
                string YillikPrim = String.Empty;
                string PrimOdemeDonemi = String.Empty;
                string ToplamOdenecekPrim = String.Empty;
                string VergiAvantaji = String.Empty;
                string VergiAvantajiSonraMaliyet = String.Empty;


                if (!String.IsNullOrEmpty(response.IB_AYLIK_PRIM))
                    AylikPrimTutari = response.IB_AYLIK_PRIM;

                if (!String.IsNullOrEmpty(response.IB_YILLIK_PRIM))
                    YillikPrim = response.IB_YILLIK_PRIM;

                if (!String.IsNullOrEmpty(response.IB_TOPLAM_PRIM))
                    ToplamOdenecekPrim = response.IB_TOPLAM_PRIM;

                if (!String.IsNullOrEmpty(response.IB_VERGI_AVANTAJI_TUTARI))
                    VergiAvantaji = response.IB_VERGI_AVANTAJI_TUTARI;

                if (!String.IsNullOrEmpty(response.IB_VER_AV_S_MALIYET))
                    VergiAvantajiSonraMaliyet = response.IB_VER_AV_S_MALIYET;

                switch (anaTeklif.ReadSoru(OdulluBirikimSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": PrimOdemeDonemi = "Aylık"; break;
                    case "2": PrimOdemeDonemi = "3 Aylık"; break;
                    case "3": PrimOdemeDonemi = "6 Aylık"; break;
                    case "4": PrimOdemeDonemi = "Yıllık"; break;
                }


                parser.SetVariable("$AylikPrimTutari$", AylikPrimTutari);
                parser.SetVariable("$YillikPrim$", YillikPrim);
                parser.SetVariable("$PrimOdemeDonemi$", PrimOdemeDonemi);
                parser.SetVariable("$ToplamOdenecekPrim$", ToplamOdenecekPrim);
                parser.SetVariable("$VergiIadeOran$", VergiIadeOran);
                parser.SetVariable("$VergiAvantaji$", VergiAvantaji);
                parser.SetVariable("$VergiAvantajiSonraMaliyet$", VergiAvantajiSonraMaliyet);

                #endregion

                #region Teminat ve Kesinti Bilgileri

                string SigortaBaslangicTarihi = String.Empty;
                string ToplamSigortaSuresi = String.Empty;
                string EgitimOncesi = String.Empty;
                string EgitimSuresi = String.Empty;
                string VefatTeminatı = String.Empty;
                string YillikOrtalamaRiskPrimiKesintisi = String.Empty;
                string KarPayiDagitimOrani = String.Empty;

                SigortaBaslangicTarihi = SBTarihi.ToString("dd.MM.yyyy");
                ToplamSigortaSuresi = anaTeklif.ReadSoru(EgitimSigortasiSorular.SigortaSuresi, String.Empty);

                if (!String.IsNullOrEmpty(response.IB_EGITIM_ONCESI_SURE))
                    EgitimOncesi = response.IB_EGITIM_ONCESI_SURE;

                if (!String.IsNullOrEmpty(EgitimOncesi) && !String.IsNullOrEmpty(ToplamSigortaSuresi))
                {
                    int tplmsr;
                    int egtmOcs;
                    if (int.TryParse(EgitimOncesi, out egtmOcs) && int.TryParse(ToplamSigortaSuresi, out tplmsr))
                        EgitimSuresi = (tplmsr - egtmOcs).ToString();
                }

                if (!String.IsNullOrEmpty(response.IB_VEFAT_TEMINAT_TUTARI))
                    VefatTeminatı = response.IB_VEFAT_TEMINAT_TUTARI;

                if (!String.IsNullOrEmpty(response.IB_YILLIK_ORT_RISK_PRM_KES))
                    YillikOrtalamaRiskPrimiKesintisi = response.IB_YILLIK_ORT_RISK_PRM_KES;

                if (!String.IsNullOrEmpty(response.IB_KAR_PAYI_DAG_ORAN))
                    KarPayiDagitimOrani = response.IB_KAR_PAYI_DAG_ORAN;


                parser.SetVariable("$SigortaBaslangicTarihi$", SigortaBaslangicTarihi);
                parser.SetVariable("$ToplamSigortaSuresi$", ToplamSigortaSuresi);
                parser.SetVariable("$EgitimOncesi$", EgitimOncesi);
                parser.SetVariable("$EgitimSuresi$", EgitimSuresi);
                parser.SetVariable("$VefatTeminatı$", VefatTeminatı);
                parser.SetVariable("$YillikOrtalamaRiskPrimiKesintisi$", YillikOrtalamaRiskPrimiKesintisi);
                parser.SetVariable("$KarPayiDagitimOrani$", KarPayiDagitimOrani);

                #endregion

                #region Birikim Bilgileri (bkz.6)

                //Garanti Edilen (bkz not 1)
                string Garanti_VergiOncesi = String.Empty;
                string Garanti_Net = String.Empty;
                string Garanti_EgitimSuresi = String.Empty;

                //Varsayım 1 (Brüt Getiri %'si)
                string Varsayim1_VergiOncesi = String.Empty;
                string Varsayim1_Net = String.Empty;
                string Varsayim1_EgitimSuresi = String.Empty;

                //Varsayım 2 (Brüt Getiri %'si)
                string Varsayim2_VergiOncesi = String.Empty;
                string Varsayim2_Net = String.Empty;
                string Varsayim2_EgitimSuresi = String.Empty;

                //MAX MIN DEGERLER
                string IB_MIN_GETIRI_ORAN = String.Empty;
                string IB_ORT_GETIRI_ORAN = String.Empty;
                string IB_MAX_GETIRI_ORAN = String.Empty;


                //=========Garanti Edilen (bkz not 1)=============//
                if (!String.IsNullOrEmpty(response.IB_VO_MIN_GETIRI))
                    Garanti_VergiOncesi = response.IB_VO_MIN_GETIRI;
                if (!String.IsNullOrEmpty(response.IB_NET_MIN))
                    Garanti_Net = response.IB_NET_MIN;
                if (!String.IsNullOrEmpty(response.IB_EGT_SURE_YILLIK_ODE_MIN))
                    Garanti_EgitimSuresi = response.IB_EGT_SURE_YILLIK_ODE_MIN;


                //=========Varsayım 1 (Brüt Getiri %'si)=============//
                if (!String.IsNullOrEmpty(response.IB_VO_ORT_GETIRI))
                    Varsayim1_VergiOncesi = response.IB_VO_ORT_GETIRI;
                if (!String.IsNullOrEmpty(response.IB_NET_ORT))
                    Varsayim1_Net = response.IB_NET_ORT;
                if (!String.IsNullOrEmpty(response.IB_EGT_SURE_YILLIK_ODE_ORT))
                    Varsayim1_EgitimSuresi = response.IB_EGT_SURE_YILLIK_ODE_ORT;


                //=========Varsayım 2 (Brüt Getiri %'si)=============//
                if (!String.IsNullOrEmpty(response.IB_VO_MAX_GETIRI))
                    Varsayim2_VergiOncesi = response.IB_VO_MAX_GETIRI;
                if (!String.IsNullOrEmpty(response.IB_NET_MAX))
                    Varsayim2_Net = response.IB_NET_MAX;
                if (!String.IsNullOrEmpty(response.IB_EGT_SURE_YILLIK_ODE_MAX))
                    Varsayim2_EgitimSuresi = response.IB_EGT_SURE_YILLIK_ODE_MAX;


                //=========MAX MIN DEGERLER=============//
                if (!String.IsNullOrEmpty(response.IB_MIN_GETIRI_ORAN))
                    IB_MIN_GETIRI_ORAN = response.IB_MIN_GETIRI_ORAN;
                if (!String.IsNullOrEmpty(response.IB_ORT_GETIRI_ORAN))
                    IB_ORT_GETIRI_ORAN = response.IB_ORT_GETIRI_ORAN;
                if (!String.IsNullOrEmpty(response.IB_MAX_GETIRI_ORAN))
                    IB_MAX_GETIRI_ORAN = response.IB_MAX_GETIRI_ORAN;



                //Garanti Edilen (bkz not 1)
                parser.SetVariable("$Garanti_VergiOncesi$", Garanti_VergiOncesi);
                parser.SetVariable("$Garanti_Net$", Garanti_Net);
                parser.SetVariable("$Garanti_EgitimSuresi$", Garanti_EgitimSuresi);

                //Varsayım 1 (Brüt Getiri %'si)
                parser.SetVariable("$Varsayim1_VergiOncesi$", Varsayim1_VergiOncesi);
                parser.SetVariable("$Varsayim1_Net$", Varsayim1_Net);
                parser.SetVariable("$Varsayim1_EgitimSuresi$", Varsayim1_EgitimSuresi);

                //Varsayım 2 (Brüt Getiri %'si)
                parser.SetVariable("$Varsayim2_VergiOncesi$", Varsayim2_VergiOncesi);
                parser.SetVariable("$Varsayim2_Net$", Varsayim2_Net);
                parser.SetVariable("$Varsayim2_EgitimSuresi$", Varsayim2_EgitimSuresi);

                //MAX MIN DEGERLER
                parser.SetVariable("$IB_MIN_GETIRI_ORAN$", IB_MIN_GETIRI_ORAN);
                parser.SetVariable("$IB_ORT_GETIRI_ORAN$", IB_ORT_GETIRI_ORAN);
                parser.SetVariable("$IB_MAX_GETIRI_ORAN$", IB_MAX_GETIRI_ORAN);

                #endregion

                #region Footer AND Table

                string TeklifTarihi = String.Empty;
                string TeklifNo = String.Empty;
                string Tetkikler = String.Empty;

                TeklifTarihi = anaTeklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");
                TeklifNo = anaTeklif.TeklifNo.ToString();

                if (!String.IsNullOrEmpty(response.TIBBI_TETKIK_SONUCU) && response.TIBBI_TETKIK_SONUCU != "OK")
                    Tetkikler = response.TIBBI_TETKIK_SONUCU;

                #region TABLE

                List<AegonEgitimResponse> list = new List<AegonEgitimResponse>();

                foreach (DataRow row in table.Rows)
                {
                    AegonEgitimResponse listItem = new AegonEgitimResponse();

                    //===============Poliçe Yılı
                    if (row["ID_POLICE_YILI"] != null)
                        listItem.ID_POLICE_YILI = row["ID_POLICE_YILI"].ToString();

                    //===============Sigortalı Yaşı
                    if (row["ID_YAS"] != null)
                        listItem.ID_YAS = row["ID_YAS"].ToString();

                    //===============Yıllık Risk Kesintisi
                    if (row["ID_YILLIK_RISK_KESINTI"] != null && row["ID_YILLIK_RISK_KESINTI"].ToString() != ",00")
                        listItem.ID_YILLIK_RISK_KESINTI = row["ID_YILLIK_RISK_KESINTI"].ToString();

                    //===============Kümülatif Yıllık Prim
                    if (row["ID_KUM_YILLIK_PRIM"] != null && row["ID_KUM_YILLIK_PRIM"].ToString() != ",00")
                        listItem.ID_KUM_YILLIK_PRIM = row["ID_KUM_YILLIK_PRIM"].ToString();


                    //==============Vergi Öncesi Birikim 2%
                    if (row["ID_GE_VER_ONC_BIRIKIM"] != null && row["ID_GE_VER_ONC_BIRIKIM"].ToString() != ",00")
                        listItem.ID_GE_VER_ONC_BIRIKIM = row["ID_GE_VER_ONC_BIRIKIM"].ToString();
                    if (row["ID_GE_YIL_GER_ODE_IKR_TUT"] != null && row["ID_GE_YIL_GER_ODE_IKR_TUT"].ToString() != ",00")
                        listItem.ID_GE_YIL_GER_ODE_IKR_TUT = row["ID_GE_YIL_GER_ODE_IKR_TUT"].ToString();


                    //==============Vergi Öncesi Birikim 4%
                    if (row["ID_V1_VER_ONC_BIRIKIM"] != null && row["ID_V1_VER_ONC_BIRIKIM"].ToString() != ",00")
                        listItem.ID_V1_VER_ONC_BIRIKIM = row["ID_V1_VER_ONC_BIRIKIM"].ToString();
                    if (row["ID_V1_YIL_GER_ODE_IKR_TUT"] != null && row["ID_V1_YIL_GER_ODE_IKR_TUT"].ToString() != ",00")
                        listItem.ID_V1_YIL_GER_ODE_IKR_TUT = row["ID_V1_YIL_GER_ODE_IKR_TUT"].ToString();


                    //==============Vergi Öncesi Birikim 6%
                    if (row["ID_V2_GER_ODE_IKR_TUT"] != null && row["ID_V2_GER_ODE_IKR_TUT"].ToString() != ",00")
                        listItem.ID_V2_GER_ODE_IKR_TUT = row["ID_V2_GER_ODE_IKR_TUT"].ToString();
                    if (row["ID_V2_VER_ONC_BIRIKIM"] != null && row["ID_V2_VER_ONC_BIRIKIM"].ToString() != ",00")
                        listItem.ID_V2_VER_ONC_BIRIKIM = row["ID_V2_VER_ONC_BIRIKIM"].ToString();

                    if (!String.IsNullOrEmpty(listItem.ID_POLICE_YILI) && !String.IsNullOrEmpty(listItem.ID_YAS) && !String.IsNullOrEmpty(listItem.ID_YILLIK_RISK_KESINTI))
                        list.Add(listItem);
                }


                StringBuilder sb = new StringBuilder();

                foreach (AegonEgitimResponse item in list)
                {

                    sb.AppendLine("<tr><height=20><border=1><backgroundColor=clear>");
                    sb.AppendLine("<td><normal><center>" + item.ID_POLICE_YILI + "</td>");
                    sb.AppendLine("<td>" + item.ID_YAS + "</td>");
                    sb.AppendLine("<td>" + item.ID_YILLIK_RISK_KESINTI + "</td>");
                    sb.AppendLine("<td>" + item.ID_KUM_YILLIK_PRIM + "</td>");
                    sb.AppendLine("<td>" + item.ID_GE_VER_ONC_BIRIKIM + "</td>");
                    sb.AppendLine("<td>" + item.ID_GE_YIL_GER_ODE_IKR_TUT + "</td>");
                    sb.AppendLine("<td>" + item.ID_V1_VER_ONC_BIRIKIM + "</td>");
                    sb.AppendLine("<td>" + item.ID_V1_YIL_GER_ODE_IKR_TUT + "</td>");
                    sb.AppendLine("<td>" + item.ID_V2_VER_ONC_BIRIKIM + "</td>");
                    sb.AppendLine("<td>" + item.ID_V2_GER_ODE_IKR_TUT + "</td>");
                    sb.AppendLine("</tr>");

                }
                parser.ReplacePlaceHolder("PrimTable", sb.ToString());


                #endregion

                if (!String.IsNullOrEmpty(Tetkikler))
                {
                    int index = Tetkikler.IndexOf(".");
                    if (index != -1 && Tetkikler.Length > (index + 3))
                    {
                        string ilk = Tetkikler.Substring(0, index + 1);
                        string son = Tetkikler.Substring(index + 3);

                        Tetkikler = ilk + " \n " + son;
                    }
                }

                parser.SetVariable("$TeklifTarihi$", TeklifTarihi);
                parser.SetVariable("$TeklifNo$", TeklifNo);
                parser.SetVariable("$Tetkikler$", Tetkikler);

                #endregion

                #endregion

                #endregion

                #region 
                
            

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("egitim_icin_hayat_sigortasi{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("Egitim_icin_Hayat", fileName, fileData);

                anaTeklif.GenelBilgiler.PDFDosyasi = url;

                _TeklifService.UpdateGenelBilgiler(anaTeklif.GenelBilgiler);

                _Log.Info("PDF dokumanı oluşturuldu : {0}", url);

                #endregion
            }
            catch (Exception ex)
            {
                _Log.Error("PDF Oluşturlamadı.");
                _Log.Error(ex);
                throw;
            }
            finally
            {
                if (pdf != null)
                    pdf.Dispose();
            }
        }
    }
}
