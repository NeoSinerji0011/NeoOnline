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
    public class AEGONOdulluBirikim : Teklif, IAEGONOdulluBirikim
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
        public AEGONOdulluBirikim(ICRService crService,
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

                AegonOBRequest request = new AegonOBRequest();

                request.TeklifNo = teklif.TeklifNo;
                request.teklifTarihi = TurkeyDateTime.Today.ToString("dd.MM.yyyy");

                DateTime SigortaBaslangicTar = teklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue);

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
                        case "1": request.pGVO = 15; break;
                        case "2": request.pGVO = 20; break;
                        case "3": request.pGVO = 27; break;
                        case "4": request.pGVO = 35; break;
                        default: request.pGVO = 27; break;
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
                string SigortaSuresi = teklif.ReadSoru(OdulluBirikimSorular.SigortaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(SigortaSuresi))
                    request.sigortaSure = Convert.ToInt32(SigortaSuresi);

                string tutar = String.Empty;

                switch (teklif.ReadSoru(OdulluBirikimSorular.HesaplamaSecenegi, String.Empty))
                {
                    case "1":
                        request.pHesapSecenek = 1;
                        request.tutar = (double)teklif.ReadSoru(OdulluBirikimSorular.YillikPrimTutari, decimal.Zero);
                        break;
                    case "2":
                        request.pHesapSecenek = 2;
                        request.tutar = (double)teklif.ReadSoru(OdulluBirikimSorular.SureSonuPrimIadesiTeminati, decimal.Zero);
                        break;
                }


                int bolen = 1;

                //Prim Odeme Donemi
                switch (teklif.ReadSoru(OdulluBirikimSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": bolen = 12; request.odeDonem = "Aylık"; break;
                    case "2": bolen = 4; request.odeDonem = "3 Aylık"; break;
                    case "3": bolen = 2; request.odeDonem = "6 Aylık"; break;
                    case "4": bolen = 1; request.odeDonem = "Yıllık"; break;
                }

                #endregion

                #region RESPONSE | LOG

                this.BeginLog(request, typeof(AegonOBRequest), WebServisIstekTipleri.Teklif);

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.AEGON_ServiceURL);

                aegon.Service1 servis = new aegon.Service1();
                servis.Timeout = 150000;
                servis.Url = serviceURL;

                DataTable prim = servis.OdulluBirikim_OB01(request.TeklifNo, request.cinsiyet, request.dogTar, request.yas, request.sigBasTar, request.odeDonem,
                                                           request.sigortaSure, request.pHesapSecenek, request.tutar, request.pGVO, request.teklifTarihi,
                                                           request.musteriAdSoyad, AegonCommon.FirmaKisaAdi);



                if (prim == null)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;

                    this.EndLog("Web Servise yanıt dönmedi", false);
                    this.AddHata("Web Servise yanıt dönmedi");
                    return;
                }


                DataRow row = prim.Rows[0];
                if (row["HATA"] != null && !String.IsNullOrEmpty(row["HATA"].ToString()))
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

                #region Genel bilgiler

                #region DataTable To Response

                CultureInfo turkey = new CultureInfo("tr-TR");

                AegonOBResponse response = new AegonOBResponse();

                if (row["IB_YILLIK_PRIM"] != null)
                    response.IB_YILLIK_PRIM = row["IB_YILLIK_PRIM"].ToString();
                if (row["IB_TOPLAM_PRIM"] != null)
                    response.IB_TOPLAM_PRIM = row["IB_TOPLAM_PRIM"].ToString();


                //=====================ODEME BILGILERI========================//
                if (row["IB_GELIR_VERGISI_ORANI"] != null)
                    response.IB_GELIR_VERGISI_ORANI = row["IB_GELIR_VERGISI_ORANI"].ToString();
                if (row["IB_VERGI_AVANTAJI"] != null)
                    response.IB_VERGI_AVANTAJI = row["IB_VERGI_AVANTAJI"].ToString();
                if (row["IB_VER_SON_PRIM_MALIYET"] != null)
                    response.IB_VER_SON_PRIM_MALIYET = row["IB_VER_SON_PRIM_MALIYET"].ToString();



                //=====================TEMINAT VE KESINTI BILGILERI========================//
                if (row["IB_VEFAT_TEMINATI"] != null)
                    response.IB_VEFAT_TEMINATI = row["IB_VEFAT_TEMINATI"].ToString();
                if (row["IB_PRIM_IADE_ODULU"] != null)
                    response.IB_PRIM_IADE_ODULU = row["IB_PRIM_IADE_ODULU"].ToString();
                if (row["IB_SURE_SONU_PRIM_IADE_ORANI"] != null)
                    response.IB_SURE_SONU_PRIM_IADE_ORANI = row["IB_SURE_SONU_PRIM_IADE_ORANI"].ToString();
                if (row["IB_ARA_DONEM_PRIM_IADE_ORANI"] != null)
                    response.IB_ARA_DONEM_PRIM_IADE_ORANI = row["IB_ARA_DONEM_PRIM_IADE_ORANI"].ToString();
                if (row["IB_RISK_PRIM_KESINTISI"] != null)
                    response.IB_RISK_PRIM_KESINTISI = row["IB_RISK_PRIM_KESINTISI"].ToString();
                if (row["IB_KAR_PAYI_DAG_ORANI"] != null)
                    response.IB_KAR_PAYI_DAG_ORANI = row["IB_KAR_PAYI_DAG_ORANI"].ToString();



                //=====================BIRIKIM BILGILERI========================//
                //Garanti Edilen
                if (row["IB_GARANTI_EDILEN"] != null)
                    response.IB_GARANTI_EDILEN = row["IB_GARANTI_EDILEN"].ToString();
                if (row["IB_GAR_EDILEN_ORAN"] != null)
                    response.IB_GAR_EDILEN_ORAN = row["IB_GAR_EDILEN_ORAN"].ToString();
                if (row["IB_GAR_EDILEN_VERGI_ONCESI"] != null)
                    response.IB_GAR_EDILEN_VERGI_ONCESI = row["IB_GAR_EDILEN_VERGI_ONCESI"].ToString();
                if (row["IB_GAR_EDILEN_NET"] != null)
                    response.IB_GAR_EDILEN_NET = row["IB_GAR_EDILEN_NET"].ToString();
                if (row["IB_GAR_EDILEN_ODUL_DAHIL"] != null)
                    response.IB_GAR_EDILEN_ODUL_DAHIL = row["IB_GAR_EDILEN_ODUL_DAHIL"].ToString();

                //Varsayılan 1
                if (row["IB_VARSAYIM_1_METIN"] != null)
                    response.IB_VARSAYIM_1_METIN = row["IB_VARSAYIM_1_METIN"].ToString();
                if (row["IB_VARSAYIM_1_ORAN"] != null)
                    response.IB_VARSAYIM_1_ORAN = row["IB_VARSAYIM_1_ORAN"].ToString();
                if (row["IB_VARSAYIM_1_VERGI_ONCESI"] != null)
                    response.IB_VARSAYIM_1_VERGI_ONCESI = row["IB_VARSAYIM_1_VERGI_ONCESI"].ToString();
                if (row["IB_VARSAYIM_1_NET"] != null)
                    response.IB_VARSAYIM_1_NET = row["IB_VARSAYIM_1_NET"].ToString();
                if (row["IB_VARSAYIM_1_ODUL_DAHIL"] != null)
                    response.IB_VARSAYIM_1_ODUL_DAHIL = row["IB_VARSAYIM_1_ODUL_DAHIL"].ToString();

                //Varsayılan 2
                if (row["IB_VARSAYIM_2_METIN"] != null)
                    response.IB_VARSAYIM_2_METIN = row["IB_VARSAYIM_2_METIN"].ToString();
                if (row["IB_VARSAYIM_2_ORAN"] != null)
                    response.IB_VARSAYIM_2_ORAN = row["IB_VARSAYIM_2_ORAN"].ToString();
                if (row["IB_VARSAYIM_2_VERGI_ONCESI"] != null)
                    response.IB_VARSAYIM_2_VERGI_ONCESI = row["IB_VARSAYIM_2_VERGI_ONCESI"].ToString();
                if (row["IB_VARSAYIM_2_NET"] != null)
                    response.IB_VARSAYIM_2_NET = row["IB_VARSAYIM_2_NET"].ToString();
                if (row["IB_VARSAYIM_2_ODUL_DAHIL"] != null)
                    response.IB_VARSAYIM_2_ODUL_DAHIL = row["IB_VARSAYIM_2_ODUL_DAHIL"].ToString();


                //=====================DIGER BILGILER (SURUM - TETKIK)========================//
                if (row["SURUM_BILGI"] != null)
                    response.SURUM_BILGI = row["SURUM_BILGI"].ToString();
                if (row["TIBBI_TETKIK_SONUCU"] != null)
                    response.TIBBI_TETKIK_SONUCU = row["TIBBI_TETKIK_SONUCU"].ToString();

                #endregion

                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = SigortaBaslangicTar;
                this.GenelBilgiler.BitisTarihi = SigortaBaslangicTar.AddYears(request.sigortaSure);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Now.AddDays(30);

                if (!String.IsNullOrEmpty(response.IB_YILLIK_PRIM))
                {
                    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(response.IB_YILLIK_PRIM, turkey);
                    this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim / bolen;
                }

                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = 0;
                this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                #endregion

                #region Teminatlar

                if (!String.IsNullOrEmpty(response.IB_VEFAT_TEMINATI))
                    this.AddTeminat(OdulluBirikimTeminatlar.Vefat, Convert.ToDecimal(response.IB_VEFAT_TEMINATI, turkey), 0, 0, 0, 0);

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

        public void CreateTeklifPDF(ITeklif anaTeklif, AegonOBResponse response, DataTable table)
        {
            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(_RootPath + "Content/templates/", PdfTemplates.ODULLU_BIRIKIM);

                pdf = new PDFHelper("Babonline", "Ödüllü Birikim Sigortası", "Ödüllü Birikim Sigortası", 8, _RootPath + "Content/fonts/",
                                    PdfTemplates.SenticoSansDT_Regular);

                string surumNo = String.Empty;
                surumNo = response.SURUM_BILGI;

                //SAYFA NUMARASI YAZIYOR
                pdf.SetPageEventHelper(new PDFCustomEventHelperAEGON(surumNo));


                PDFParser parser = new PDFParser(template, pdf);

                #endregion

                #region Data Fill

                #region Odullu Birikim Teklif Poliçesi Bilgileri

                string imgpath = Path.Combine(_RootPath, "Content/img/Aegon/aegonlogo-pdf.jpg");
                string OB_FooterBorder = Path.Combine(_RootPath, "Content/img/Aegon/OB_FooterBorder.jpg");

                parser.SetVariable("$TVMLogo$", imgpath);
                parser.SetVariable("$OB_FooterBorder$", OB_FooterBorder);


                #region SigortaAdayi Bilgileri

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(anaTeklif.SigortaEttiren.MusteriKodu);

                //Sigorta Adayı Bilgileri
                string AdiSoyadi = String.Empty;
                string DogumTarihi = String.Empty;
                string Cinsiyeti = String.Empty;
                string BaslangicYasi = String.Empty;

                DateTime SBTarihi = anaTeklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                if (musteri != null)
                {
                    AdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                    Cinsiyeti = musteri.Cinsiyet == "E" ? "Erkek" : "Kadın";
                    DogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty;
                    BaslangicYasi = AEGONTESabitPrimli.AegonYasHesapla(musteri.DogumTarihi.Value, SBTarihi).ToString();
                }

                parser.SetVariable("$AdiSoyadi$", AdiSoyadi);
                parser.SetVariable("$DogumTarihi$", DogumTarihi);
                parser.SetVariable("$BaslangicYasi$", BaslangicYasi);
                parser.SetVariable("$Cinsiyeti$", Cinsiyeti);

                #endregion

                #region Odeme Bilgileri

                string YillikDuzenliPrim = String.Empty;
                string ToplamDuzenliPrim = String.Empty;
                string TabiOlunanGelirVergisi = String.Empty;
                string DuzenliPrimVergiAvantaji = String.Empty;
                string VergiAvantajiSonrasiDuzenliPrimMaliyeti = String.Empty;

                YillikDuzenliPrim = response.IB_YILLIK_PRIM;
                TabiOlunanGelirVergisi = response.IB_GELIR_VERGISI_ORANI;
                DuzenliPrimVergiAvantaji = response.IB_VERGI_AVANTAJI;
                VergiAvantajiSonrasiDuzenliPrimMaliyeti = response.IB_VER_SON_PRIM_MALIYET;
                ToplamDuzenliPrim = response.IB_TOPLAM_PRIM;

                parser.SetVariable("$YillikDuzenliPrim$", YillikDuzenliPrim);
                parser.SetVariable("$ToplamDuzenliPrim$", ToplamDuzenliPrim);
                parser.SetVariable("$TabiOlunanGelirVergisi$", TabiOlunanGelirVergisi);
                parser.SetVariable("$DuzenliPrimVergiAvantaji$", DuzenliPrimVergiAvantaji);
                parser.SetVariable("$VergiAvantajiSonrasiDuzenliPrimMaliyeti$", VergiAvantajiSonrasiDuzenliPrimMaliyeti);

                #endregion

                #region Teminat ve Kesinti Bilgileri

                string SigortaBaslangicTarihi = String.Empty;
                string SigortaSuresiYil = String.Empty;
                string VefatTeminati = String.Empty;
                string PrimIadeliOdullu = String.Empty;
                string SureSonuPrimIadeliOrani = String.Empty;
                string AraDonemPrimIadeOrani = String.Empty;
                string RiskPrimKesintisi = String.Empty;
                string KarPayiDagitimOrani = String.Empty;

                SigortaBaslangicTarihi = SBTarihi.ToString("dd.MM.yyyy");
                SigortaSuresiYil = anaTeklif.ReadSoru(OdulluBirikimSorular.SigortaSuresi, String.Empty);

                VefatTeminati = response.IB_VEFAT_TEMINATI;
                PrimIadeliOdullu = response.IB_PRIM_IADE_ODULU;
                SureSonuPrimIadeliOrani = response.IB_SURE_SONU_PRIM_IADE_ORANI;
                AraDonemPrimIadeOrani = response.IB_ARA_DONEM_PRIM_IADE_ORANI;
                RiskPrimKesintisi = response.IB_RISK_PRIM_KESINTISI;
                KarPayiDagitimOrani = response.IB_KAR_PAYI_DAG_ORANI;

                parser.SetVariable("$SigortaBaslangicTarihi$", SigortaBaslangicTarihi);
                parser.SetVariable("$SigortaSuresiYil$", SigortaSuresiYil);
                parser.SetVariable("$VefatTeminati$", VefatTeminati);
                parser.SetVariable("$PrimIadeliOdullu$", PrimIadeliOdullu);
                parser.SetVariable("$SureSonuPrimIadeliOrani$", SureSonuPrimIadeliOrani);
                parser.SetVariable("$AraDonemPrimIadeOrani$", AraDonemPrimIadeOrani);
                parser.SetVariable("$RiskPrimKesintisi$", RiskPrimKesintisi);
                parser.SetVariable("$KarPayiDagitimOrani$", KarPayiDagitimOrani);

                #endregion

                #region Birikim Bilgileri (bkz.6)

                //Garanti Edilen (bkz not 1)
                string IB_GARANTI_EDILEN = "Garanti Edilen (bkz not 1)";
                string IB_GAR_EDILEN_ORAN = String.Empty;
                string IB_GAR_EDILEN_VERGI_ONCESI = String.Empty;
                string IB_GAR_EDILEN_NET = String.Empty;
                string IB_GAR_EDILEN_ODUL_DAHIL = String.Empty;

                //Varsayım 1 (Brüt Getiri %'si)
                string IB_VARSAYIM_1_METIN = "Varsayım 1 (Brüt Getiri %'si)";
                string IB_VARSAYIM_1_ORAN = String.Empty;
                string IB_VARSAYIM_1_VERGI_ONCESI = String.Empty;
                string IB_VARSAYIM_1_NET = String.Empty;
                string IB_VARSAYIM_1_ODUL_DAHIL = String.Empty;

                //Varsayım 1 (Brüt Getiri %'si)
                string IB_VARSAYIM_2_METIN = "Varsayım 2 (Brüt Getiri %'si)";
                string IB_VARSAYIM_2_ORAN = String.Empty;
                string IB_VARSAYIM_2_VERGI_ONCESI = String.Empty;
                string IB_VARSAYIM_2_NET = String.Empty;
                string IB_VARSAYIM_2_ODUL_DAHIL = String.Empty;




                //Garanti Edilen (bkz not 1)
                IB_GARANTI_EDILEN = response.IB_GARANTI_EDILEN;
                IB_GAR_EDILEN_ORAN = response.IB_GAR_EDILEN_ORAN;
                IB_GAR_EDILEN_VERGI_ONCESI = response.IB_GAR_EDILEN_VERGI_ONCESI;
                IB_GAR_EDILEN_NET = response.IB_GAR_EDILEN_NET;
                IB_GAR_EDILEN_ODUL_DAHIL = response.IB_GAR_EDILEN_ODUL_DAHIL;

                //Varsayım 1 (Brüt Getiri %'si)
                IB_VARSAYIM_1_METIN = response.IB_VARSAYIM_1_METIN;
                IB_VARSAYIM_1_ORAN = response.IB_VARSAYIM_1_ORAN;
                IB_VARSAYIM_1_VERGI_ONCESI = response.IB_VARSAYIM_1_VERGI_ONCESI;
                IB_VARSAYIM_1_NET = response.IB_VARSAYIM_1_NET;
                IB_VARSAYIM_1_ODUL_DAHIL = response.IB_VARSAYIM_1_ODUL_DAHIL;

                //Varsayım 2 (Brüt Getiri %'si)
                IB_VARSAYIM_2_METIN = response.IB_VARSAYIM_2_METIN;
                IB_VARSAYIM_2_ORAN = response.IB_VARSAYIM_2_ORAN;
                IB_VARSAYIM_2_VERGI_ONCESI = response.IB_VARSAYIM_2_VERGI_ONCESI;
                IB_VARSAYIM_2_NET = response.IB_VARSAYIM_2_NET;
                IB_VARSAYIM_2_ODUL_DAHIL = response.IB_VARSAYIM_2_ODUL_DAHIL;




                //Garanti Edilen (bkz not 1)
                parser.SetVariable("$GarantiEdilenOranText$", IB_GARANTI_EDILEN);
                parser.SetVariable("$GarantiEdilenOran$", IB_GAR_EDILEN_ORAN);
                parser.SetVariable("$GarantiEdilenVergiOncesi$", IB_GAR_EDILEN_VERGI_ONCESI);
                parser.SetVariable("$GarantiEdilenVergiOncesiNet$", IB_GAR_EDILEN_NET);
                parser.SetVariable("$GarantiEdilenOdulDahilNet$", IB_GAR_EDILEN_ODUL_DAHIL);

                //Varsayım 1 (Brüt Getiri %'si)
                parser.SetVariable("$Varsayim1OranText$", IB_VARSAYIM_1_METIN);
                parser.SetVariable("$Varsayim1Oran$", IB_VARSAYIM_1_ORAN);
                parser.SetVariable("$Varsayim1VergiOncesi$", IB_VARSAYIM_1_VERGI_ONCESI);
                parser.SetVariable("$Varsayim1OranVergiOncesiNet$", IB_VARSAYIM_1_NET);
                parser.SetVariable("$Varsayim1OranOdulDahilNet$", IB_VARSAYIM_1_ODUL_DAHIL);

                //Varsayım 2 (Brüt Getiri %'si)
                parser.SetVariable("$Varsayim2OranText$", IB_VARSAYIM_2_METIN);
                parser.SetVariable("$Varsayim2Oran$", IB_VARSAYIM_2_ORAN);
                parser.SetVariable("$Varsayim2VergiOncesi$", IB_VARSAYIM_2_VERGI_ONCESI);
                parser.SetVariable("$Varsayim2OranVergiOncesiNet$", IB_VARSAYIM_2_NET);
                parser.SetVariable("$Varsayim2OranOdulDahilNet$", IB_VARSAYIM_2_ODUL_DAHIL);

                #endregion

                #region Footer and Table

                string TeklifTarihi = String.Empty;
                string TeklifNo = String.Empty;
               
                string Tetkikler = response.TIBBI_TETKIK_SONUCU;

                TeklifTarihi = anaTeklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");
                TeklifNo = anaTeklif.TeklifNo.ToString();

               


                List<AegonOBResponse> list = new List<AegonOBResponse>();

                foreach (DataRow row in table.Rows)
                {

                    AegonOBResponse listItem = new AegonOBResponse();

                    if (row["ID_YIL"] != null)
                        listItem.ID_YIL = row["ID_YIL"].ToString();
                    if (row["ID_SIGORTALI_YASI"] != null)
                        listItem.ID_SIGORTALI_YASI = row["ID_SIGORTALI_YASI"].ToString();
                    if (row["ID_KUMULATIF_PRIM"] != null)
                        listItem.ID_KUMULATIF_PRIM = row["ID_KUMULATIF_PRIM"].ToString();
                    if (row["ID_PRIM_IADELI_ODUL_TUTARI"] != null)
                        listItem.ID_PRIM_IADELI_ODUL_TUTARI = row["ID_PRIM_IADELI_ODUL_TUTARI"].ToString();


                    //Varsayılan 1
                    if (row["ID_VARSAYIM_1_VO_MUH_BIRIKIM"] != null)
                        listItem.ID_VARSAYIM_1_VO_MUH_BIRIKIM = row["ID_VARSAYIM_1_VO_MUH_BIRIKIM"].ToString();
                    if (row["ID_VARSAYIM_1_NET_MUH_BIRIKIM"] != null)
                        listItem.ID_VARSAYIM_1_NET_MUH_BIRIKIM = row["ID_VARSAYIM_1_NET_MUH_BIRIKIM"].ToString();
                    if (row["ID_VARSAYIM_1_NET_ISTIRA_FESIH"] != null)
                        listItem.ID_VARSAYIM_1_NET_ISTIRA_FESIH = row["ID_VARSAYIM_1_NET_ISTIRA_FESIH"].ToString();


                    //Varsayılan 2
                    if (row["ID_VARSAYIM_2_VO_MUH_BIRIKIM"] != null)
                        listItem.ID_VARSAYIM_2_VO_MUH_BIRIKIM = row["ID_VARSAYIM_2_VO_MUH_BIRIKIM"].ToString();
                    if (row["ID_VARSAYIM_2_NET_MUH_BIRIKIM"] != null)
                        listItem.ID_VARSAYIM_2_NET_MUH_BIRIKIM = row["ID_VARSAYIM_2_NET_MUH_BIRIKIM"].ToString();
                    if (row["ID_VARSAYIM_2_NET_ISTIRA_FESIH"] != null)
                        listItem.ID_VARSAYIM_2_NET_ISTIRA_FESIH = row["ID_VARSAYIM_2_NET_ISTIRA_FESIH"].ToString();


                    if (!String.IsNullOrEmpty(listItem.ID_YIL) && !String.IsNullOrEmpty(listItem.ID_SIGORTALI_YASI) && !String.IsNullOrEmpty(listItem.ID_KUMULATIF_PRIM))
                        list.Add(listItem);
                }


                StringBuilder sb = new StringBuilder();

                foreach (AegonOBResponse item in list)
                {
                    #region 0 lar yazılmıyor.

                    //sb.AppendLine("<tr><height=15><border=1><backgroundColor=clear>");
                    //sb.AppendLine("<td><normal><center>" + item.ID_YIL + "</td>");
                    //sb.AppendLine("<td>" + (item.ID_SIGORTALI_YASI != "0" ? item.ID_SIGORTALI_YASI : " ") + "</td>");
                    //sb.AppendLine("<td>" + (item.ID_KUMULATIF_PRIM != "0" ? item.ID_KUMULATIF_PRIM : " ") + "</td>");
                    //sb.AppendLine("<td>" + (item.ID_PRIM_IADELI_ODUL_TUTARI != "0" ? item.ID_PRIM_IADELI_ODUL_TUTARI : " ") + "</td>");
                    //sb.AppendLine("<td><border=clear></td>");
                    //sb.AppendLine("<td><border=1>" + (item.ID_VARSAYIM_1_VO_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_1_VO_MUH_BIRIKIM : " ") + "</td>");
                    //sb.AppendLine("<td>" + (item.ID_VARSAYIM_1_NET_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_1_NET_MUH_BIRIKIM : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.ID_VARSAYIM_1_NET_ISTIRA_FESIH != "0" ? item.ID_VARSAYIM_1_NET_ISTIRA_FESIH : " ") + "</td>");
                    //sb.AppendLine("<td><border=clear></td>");
                    //sb.AppendLine("<td><border=1>" + (item.ID_VARSAYIM_2_VO_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_2_VO_MUH_BIRIKIM : " ") + "</td>");
                    //sb.AppendLine("<td>" + (item.ID_VARSAYIM_2_NET_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_2_NET_MUH_BIRIKIM : " ") + "</td>");
                    //sb.AppendLine("<td>" + (item.ID_VARSAYIM_2_NET_ISTIRA_FESIH != "0" ? item.ID_VARSAYIM_2_NET_ISTIRA_FESIH : " ") + "</td>");
                    //sb.AppendLine("</tr>");

                    #endregion

                    sb.AppendLine("<tr><height=15><border=1><backgroundColor=clear>");
                    sb.AppendLine("<td><normal><center>" + item.ID_YIL + "</td>");
                    sb.AppendLine("<td>" + (item.ID_SIGORTALI_YASI != "0" ? item.ID_SIGORTALI_YASI : " ") + "</td>");
                    sb.AppendLine("<td>" + (item.ID_KUMULATIF_PRIM != "0" ? item.ID_KUMULATIF_PRIM : " ") + "</td>");
                    sb.AppendLine("<td>" + (item.ID_PRIM_IADELI_ODUL_TUTARI != "0" ? item.ID_PRIM_IADELI_ODUL_TUTARI : " ") + "</td>");
                    sb.AppendLine("<td><border=clear></td>");
                    sb.AppendLine("<td><border=1>" + (item.ID_VARSAYIM_1_VO_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_1_VO_MUH_BIRIKIM : " ") + "</td>");
                    sb.AppendLine("<td>" + (item.ID_VARSAYIM_1_NET_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_1_NET_MUH_BIRIKIM : "") + "</td>");
                    sb.AppendLine("<td>" + (item.ID_VARSAYIM_1_NET_ISTIRA_FESIH != "0" ? item.ID_VARSAYIM_1_NET_ISTIRA_FESIH : " ") + "</td>");
                    sb.AppendLine("<td><border=clear></td>");
                    sb.AppendLine("<td><border=1>" + (item.ID_VARSAYIM_2_VO_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_2_VO_MUH_BIRIKIM : " ") + "</td>");
                    sb.AppendLine("<td>" + (item.ID_VARSAYIM_2_NET_MUH_BIRIKIM != "0" ? item.ID_VARSAYIM_2_NET_MUH_BIRIKIM : " ") + "</td>");
                    sb.AppendLine("<td>" + (item.ID_VARSAYIM_2_NET_ISTIRA_FESIH != "0" ? item.ID_VARSAYIM_2_NET_ISTIRA_FESIH : " ") + "</td>");
                    sb.AppendLine("</tr>");
                }

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


                parser.ReplacePlaceHolder("OB_FooterList", sb.ToString());
                parser.SetVariable("$TeklifTarihi$", TeklifTarihi);
                parser.SetVariable("$TeklifNo$", TeklifNo);
                parser.SetVariable("$Tetkikler$", Tetkikler);

                #endregion

                #endregion

                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("odullu_birikim_hayat_sigortasi{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("OdulluBirikim", fileName, fileData);

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
