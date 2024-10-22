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
using System.Data;
using System.Globalization;
using System.IO;
using Neosinerji.BABOnlineTP.Business.Common.AEGON;


namespace Neosinerji.BABOnlineTP.Business.AEGON
{
    public class AEGONKorunanGelecek : Teklif, IAEGONKorunanGelecek
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
        public AEGONKorunanGelecek(ICRService crService,
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
                CultureInfo turkey = new CultureInfo("tr-TR");

                #region REQUEST

                AegonKGRequest request = new AegonKGRequest();

                request.pTeklifNo = teklif.TeklifNo;
                request.teklifTarihi = TurkeyDateTime.Today.ToString("dd.MM.yyyy");

                DateTime SigortaBaslangicTar = teklif.ReadSoru(KorunanGelecekSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                //Para birimi
                request.parabirimi = KorunanGelecekParaBirimi.ParaBirimiText(teklif.ReadSoru(KorunanGelecekSorular.ParaBirimi, String.Empty));
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
                        case "1": request.gvo = 15; break;
                        case "2": request.gvo = 20; break;
                        case "3": request.gvo = 27; break;
                        case "4": request.gvo = 35; break;
                        case "5": request.gvo = 27; break;
                        default: request.gvo = 27; break;
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
                string SigortaSuresi = teklif.ReadSoru(KorunanGelecekSorular.SigortaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(SigortaSuresi))
                    request.sigortaSure = Convert.ToInt32(SigortaSuresi);

                //request.primOdemeSuresi = Convert.ToInt32(Math.Round((request.sigortaSure * 0.75), 0, MidpointRounding.AwayFromZero));


                //Prim Odeme Donemi
                switch (teklif.ReadSoru(KorunanGelecekSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": request.odeDonem = "Aylık"; break;
                    case "2": request.odeDonem = "3 Aylık"; break;
                    case "3": request.odeDonem = "6 Aylık"; break;
                    case "4": request.odeDonem = "Yıllık"; break;
                }

                string AnaTeminatKodu = teklif.ReadSoru(KorunanGelecekSorular.VefatTeminati, String.Empty);

                if (AnaTeminatKodu != null && !String.IsNullOrEmpty(AnaTeminatKodu))
                {
                    TeklifTeminat vefat = teklif.Teminatlar.Where(s => s.TeminatKodu == KorunanGelecekTeminatlar.Vefat).FirstOrDefault();
                    request.anaTeminat = "VEF";
                    request.anaTeminatTutar = (double)vefat.TeminatBedeli.Value;
                }
                string EkTeminat = teklif.ReadSoru(KorunanGelecekSorular.MaluliyetYillikDestek, String.Empty);

                if (EkTeminat != null && !String.IsNullOrEmpty(EkTeminat))
                {
                    request.ekTeminat = "MALYD";
                }
                else request.ekTeminat = "";


                #endregion

                #region RESSPONSE LOG
                this.BeginLog(request, typeof(AegonKGRequest), WebServisIstekTipleri.Teklif);

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.AEGON_ServiceURL);

                aegon.Service1 servis = new aegon.Service1();
                servis.Timeout = 150000;
                servis.Url = serviceURL;


                DataTable prim = servis.KorunanGelecek_KG01(request.pTeklifNo, request.cinsiyet, request.dogTar, request.yas, request.sigBasTar,
                                                            request.sigortaSure, request.odeDonem, request.parabirimi, request.anaTeminat, request.anaTeminatTutar,
                                                            request.ekTeminat, request.gvo, request.teklifTarihi, request.musteriAdSoyad, AegonCommon.FirmaKisaAdi);


                if (prim == null || prim.Rows.Count == 0 || prim.Rows[0]["HATA"] == null)
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
                    this.AddHata(row["HATA"].ToString());
                    return;
                }
                else
                    this.EndLog(prim, true, prim.GetType());


                //if (row["HATA"] != null && !String.IsNullOrEmpty(row["HATA"].ToString()) && row["HATA"].ToString() != "HATA YOK")
                //{
                //    this.Import(teklif);
                //    this.GenelBilgiler.Basarili = false;

                //    string hata = AegonHelper.AegonReplace(row["HATA"].ToString());
                //    this.EndLog(hata, false);
                //    this.AddHata(hata);
                //    return;
                //}
                //else
                //    this.EndLog(prim, true, prim.GetType());

                #endregion

                #region SUCCESS

                #region Genel Bilgiler
                AegonKGResponse response = new AegonKGResponse();

                #region DataTableToResponse

                if (row["HATA"] != null)
                    response.HATA = row["HATA"].ToString();
                if (row["SURUM_BILGI"] != null)
                    response.SURUM_BILGI = row["SURUM_BILGI"].ToString();
                if (row["TIBBI_TETKIK_SONUCU"] != null)
                    response.TIBBI_TETKIK_SONUCU = row["TIBBI_TETKIK_SONUCU"].ToString();
                if (row["YIL"] != null)
                    response.YIL = row["YIL"].ToString();
                if (row["YAS"] != null)
                    response.YAS = row["YAS"].ToString();
                if (row["PRIM_ODEME_SURESI"] != null)
                    response.PRIM_ODEME_SURESI = row["PRIM_ODEME_SURESI"].ToString();
                if (row["PRIMDEN_VERGI_AVANTAJI"] != null)
                    response.PRIMDEN_VERGI_AVANTAJI = row["PRIMDEN_VERGI_AVANTAJI"].ToString();
                if (row["VAS_PRIM_MALIYETI"] != null)
                    response.VAS_PRIM_MALIYETI = row["VAS_PRIM_MALIYETI"].ToString();


                //Teminatlar
                if (row["AT_SIGORTA_TUTARI"] != null)
                    response.AT_SIGORTA_TUTARI = row["AT_SIGORTA_TUTARI"].ToString();
                if (row["TOPLAM_YILLIK_PRIM"] != null)
                    response.TOPLAM_YILLIK_PRIM = row["TOPLAM_YILLIK_PRIM"].ToString();
                if (row["DONEM_PRIMI"] != null)
                    response.DONEM_PRIMI = row["DONEM_PRIMI"].ToString();
                if (row["SURE_SONU_TOP_PRIM"] != null)
                    response.SURE_SONU_TOP_PRIM = row["SURE_SONU_TOP_PRIM"].ToString();
                if (row["MALYD_TEM_TUTAR"] != null)
                    response.MALYD_TEM_TUTAR = row["MALYD_TEM_TUTAR"].ToString();


                #endregion

                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = SigortaBaslangicTar;
                this.GenelBilgiler.BitisTarihi = SigortaBaslangicTar.AddYears(request.sigortaSure);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Now.AddDays(30);

                if (!String.IsNullOrEmpty(response.TOPLAM_YILLIK_PRIM))
                    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(response.TOPLAM_YILLIK_PRIM, turkey);

                if (!String.IsNullOrEmpty(response.DONEM_PRIMI))
                    this.GenelBilgiler.NetPrim = Convert.ToDecimal(response.DONEM_PRIMI, turkey);

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

                #endregion

                #region Teminatlar

                TeklifTeminat vefatTeminati = teklif.Teminatlar.Where(s => s.TeminatKodu == KorunanGelecekTeminatlar.Vefat).FirstOrDefault();
                if (vefatTeminati != null && !String.IsNullOrEmpty(response.AT_SIGORTA_TUTARI) && !String.IsNullOrEmpty(response.DONEM_PRIMI) && response.DONEM_PRIMI != "0"
                && !String.IsNullOrEmpty(response.TOPLAM_YILLIK_PRIM) && response.TOPLAM_YILLIK_PRIM != "0")
                    this.AddTeminat(KorunanGelecekTeminatlar.Vefat, Convert.ToDecimal(response.AT_SIGORTA_TUTARI, turkey), 0,
                                                Convert.ToDecimal(response.DONEM_PRIMI, turkey),
                                                Convert.ToDecimal(response.TOPLAM_YILLIK_PRIM, turkey), 0);

                TeklifTeminat maluliyet = teklif.Teminatlar.Where(s => s.TeminatKodu == KorunanGelecekTeminatlar.MaluliyetYillikDestek).FirstOrDefault();
                if (maluliyet != null && !String.IsNullOrEmpty(response.MALYD_TEM_TUTAR) && !String.IsNullOrEmpty(response.DONEM_PRIMI) && response.DONEM_PRIMI != "0"
                && !String.IsNullOrEmpty(response.TOPLAM_YILLIK_PRIM) && response.TOPLAM_YILLIK_PRIM != "0")
                    this.AddTeminat(KorunanGelecekTeminatlar.MaluliyetYillikDestek, Convert.ToDecimal(response.AT_SIGORTA_TUTARI, turkey), 0,
                                                Convert.ToDecimal(response.DONEM_PRIMI, turkey),
                                                Convert.ToDecimal(response.TOPLAM_YILLIK_PRIM, turkey), 0);
                #endregion

                #region Ödeme Planı
                this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                #endregion

                #region Web Servis Response

                this.AddWebServisCevap(Common.WebServisCevaplar.SurumNo, response.SURUM_BILGI);

                //Web servisten gelen uyarı mesajı
                if (!String.IsNullOrEmpty(response.HATA))
                    this.AddWebServisCevap(Common.WebServisCevaplar.Uyari, response.HATA.Replace("\n", "|"));

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

        public void CreateTeklifPDF(ITeklif anaTeklif, AegonKGResponse response, DataTable table)
        {
            PDFHelper pdf = null;
            CultureInfo culture = new CultureInfo("tr-TR");
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(_RootPath + "Content/templates/", PdfTemplates.KORUNAN_GEECEK);

                pdf = new PDFHelper("Babonline", "Korunan Gelecek Hayat Sigortası", "Korunan Gelecek Hayat Sigortası", 8, _RootPath + "Content/fonts/",
                                    PdfTemplates.SenticoSansDT_Regular);

                //SAYFA NUMARASI YAZIYOR
                pdf.SetPageEventHelper(new PDFCustomEventHelperAEGON(response.SURUM_BILGI));


                PDFParser parser = new PDFParser(template, pdf);

                #endregion

                #region Data Fill

                #region Korunan Gelecek Teklif Poliçesi Bilgileri

                #region IMG
                string imgpath = Path.Combine(_RootPath, "Content/img/Aegon/aegonlogo-pdf.jpg");
                string paraBirimiTuru = String.Empty;
                string imgfooter = Path.Combine(_RootPath, "Content/img/Aegon/OB_FooterBorder.jpg");


                string paraBirimiImg = KorunanGelecekParaBirimi.ParaBirimiText(anaTeklif.ReadSoru(KorunanGelecekSorular.ParaBirimi, String.Empty));

                if (paraBirimiImg == "USD")
                    paraBirimiTuru = "ABD Doları";
                else paraBirimiTuru = "EURO";

                parser.SetVariable("$TVMLogo$", imgpath);
                parser.SetVariable("$ParaBirimiTuru$", paraBirimiTuru);
                parser.SetVariable("$KG_FooterBorder$", imgfooter);

                #endregion

                #region SigortaAdayi Bilgileri

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(anaTeklif.SigortaEttiren.MusteriKodu);

                //Sigorta Adayı Bilgileri
                string AdiSoyadi = String.Empty;
                string DogumTarihi = String.Empty;
                string Cinsiyeti = String.Empty;
                string GirisYasi = String.Empty;

                //Vergi ile İlgili Bilgiler
                string BeyanEdilenGelirVergisiOrani = String.Empty;
                string PrimdenVergiAvantaji = String.Empty;
                string VergiAvantajiSonrasiPrimMaliyeti = String.Empty;

                DateTime SBTarihi = anaTeklif.ReadSoru(KorunanGelecekSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                if (musteri != null)
                {
                    AdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                    Cinsiyeti = musteri.Cinsiyet == "E" ? "Erkek" : "Kadın";

                    DogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty;


                    GirisYasi = AEGONTESabitPrimli.AegonYasHesapla(musteri.DogumTarihi.Value, SBTarihi).ToString();

                    switch (musteri.CiroBilgisi)
                    {
                        case "1": BeyanEdilenGelirVergisiOrani = "%15"; break;
                        case "2": BeyanEdilenGelirVergisiOrani = "%20"; break;
                        case "3": BeyanEdilenGelirVergisiOrani = "%27"; break;
                        case "4": BeyanEdilenGelirVergisiOrani = "%35"; break;
                        case "5": BeyanEdilenGelirVergisiOrani = "%27"; break;
                    }
                }

                parser.SetVariable("$AdiSoyadi$", AdiSoyadi);
                parser.SetVariable("$DogumTarihi$", DogumTarihi);
                parser.SetVariable("$GirisYasi$", GirisYasi);
                parser.SetVariable("$Cinsiyeti$", Cinsiyeti);


                PrimdenVergiAvantaji = response.PRIMDEN_VERGI_AVANTAJI;
                VergiAvantajiSonrasiPrimMaliyeti = response.VAS_PRIM_MALIYETI;

                //Vergi Bilgileri
                parser.SetVariable("$BEGVO$", BeyanEdilenGelirVergisiOrani);
                parser.SetVariable("$PrimdenVergiAvantaji$", PrimdenVergiAvantaji);
                parser.SetVariable("$VASPM$", VergiAvantajiSonrasiPrimMaliyeti);

                #endregion

                #region Sigorta ile İlgili Bilgiler

                string AnaTeminatBedel = String.Empty;
                string EkTeminatBedel = String.Empty;
                string SigortaSuresi = String.Empty;
                string PrimOdemeSuresi = String.Empty;
                string BaslangicTarihi = String.Empty;
                string ParaBirimi = String.Empty;
                string PrimOdeme = String.Empty;
                string SureBoyuToplamPrim = String.Empty;

                if (anaTeklif.ReadSoru(KorunanGelecekSorular.VefatTeminati, false))
                {
                    TeklifTeminat vefat = anaTeklif.Teminatlar.Where(s => s.TeminatKodu == KorunanGelecekTeminatlar.Vefat).FirstOrDefault();

                    if (vefat != null)
                        AnaTeminatBedel = vefat.TeminatBedeli.Value.ToString("N2", culture);
                }

                if (anaTeklif.ReadSoru(KorunanGelecekSorular.MaluliyetYillikDestek, false))
                {
                    TeklifTeminat maluliyet = anaTeklif.Teminatlar.Where(s => s.TeminatKodu == KorunanGelecekTeminatlar.MaluliyetYillikDestek).FirstOrDefault();
                    if (maluliyet != null && !String.IsNullOrEmpty(response.MALYD_TEM_TUTAR))
                        EkTeminatBedel = response.MALYD_TEM_TUTAR;
                }
                else EkTeminatBedel = "Yok";

                SigortaSuresi = anaTeklif.ReadSoru(KorunanGelecekSorular.SigortaSuresi, String.Empty);

                BaslangicTarihi = SBTarihi.ToString("dd.MM.yyyy");
                // int sigortaSuresi = Convert.ToInt32(SigortaSuresi);
                PrimOdemeSuresi = response.PRIM_ODEME_SURESI.ToString();

                switch (anaTeklif.ReadSoru(KorunanGelecekSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": PrimOdeme = "Aylık"; break;
                    case "2": PrimOdeme = "3 Aylık"; break;
                    case "3": PrimOdeme = "6 Aylık"; break;
                    case "4": PrimOdeme = "Yıllık"; break;
                }
                string paraBirimi = KorunanGelecekParaBirimi.ParaBirimiText(anaTeklif.ReadSoru(KorunanGelecekSorular.ParaBirimi, String.Empty));

                if (paraBirimi == "USD")
                    ParaBirimi = "ABD Doları";
                else ParaBirimi = "Euro";

                if (response.SURE_SONU_TOP_PRIM.ToString() != null && response.SURE_SONU_TOP_PRIM.ToString() != "" && response.SURE_SONU_TOP_PRIM.ToString() != "0")
                    SureBoyuToplamPrim = response.SURE_SONU_TOP_PRIM;

                parser.SetVariable("$AnaTeminatBedel$", AnaTeminatBedel);
                parser.SetVariable("$EkTeminatBedel$", EkTeminatBedel);
                parser.SetVariable("$SigortaSuresi$", SigortaSuresi);
                parser.SetVariable("$PrimOdemeSuresi$", PrimOdemeSuresi.ToString());
                parser.SetVariable("$BaslangicTarihi$", BaslangicTarihi);
                parser.SetVariable("$ParaBirimi$", ParaBirimi);
                parser.SetVariable("$PrimOdemeSikligi$", PrimOdeme);
                parser.SetVariable("$SureBoyuToplamPrim$", SureBoyuToplamPrim);

                #endregion

                #region Table
                List<AegonKGResponse> list = new List<AegonKGResponse>();


                foreach (DataRow row in table.Rows)
                {
                    AegonKGResponse listItem = new AegonKGResponse();
                    if (row["HATA"] != null && row["HATA"].ToString() == "HATA YOK")
                    {
                        if (row["YIL"] != null)
                            listItem.YIL = row["YIL"].ToString();
                        if (row["YAS"] != null)
                            listItem.YAS = row["YAS"].ToString();

                        if (row["AT_SIGORTA_TUTARI"] != null)
                            listItem.AT_SIGORTA_TUTARI = row["AT_SIGORTA_TUTARI"].ToString();

                        if (row["TOPLAM_YILLIK_PRIM"] != null)
                            listItem.TOPLAM_YILLIK_PRIM = row["TOPLAM_YILLIK_PRIM"].ToString();

                        if (row["DONEM_PRIMI"] != null)
                            listItem.DONEM_PRIMI = row["DONEM_PRIMI"].ToString();

                        if (!String.IsNullOrEmpty(listItem.YIL) && !String.IsNullOrEmpty(listItem.YAS) &&
                            !String.IsNullOrEmpty(listItem.AT_SIGORTA_TUTARI) && !String.IsNullOrEmpty(listItem.TOPLAM_YILLIK_PRIM) &&
                            !String.IsNullOrEmpty(listItem.DONEM_PRIMI))
                            list.Add(listItem);
                    }
                }
                StringBuilder sb = new StringBuilder();

                List<AegonKGTable> tableList = new List<AegonKGTable>();

                if (list != null && list.Count() > 0)
                {
                    for (int i = 0; i < list.Count(); i++)
                    {
                        sb.AppendLine("<tr><height=20><backgroundColor=clear>");
                        sb.AppendLine("<td><normal><center>" + list[i].YIL + "</td>");
                        sb.AppendLine("<td>" + list[i].YAS + "</td>");
                        sb.AppendLine("<td>" + (list[i].AT_SIGORTA_TUTARI != ",00" ? list[i].AT_SIGORTA_TUTARI : "") + "</td>");
                        sb.AppendLine("<td>" + (list[i].TOPLAM_YILLIK_PRIM != ",00" ? list[i].TOPLAM_YILLIK_PRIM : "") + "</td>");
                        sb.AppendLine("<td>" + (list[i].DONEM_PRIMI != ",00" ? list[i].DONEM_PRIMI : "") + "</td>");

                        sb.AppendLine("</tr>");
                    }
                }

                parser.ReplacePlaceHolder("SigortaBedeli", sb.ToString());

                #endregion

                #region Footer

                string TeklifTarihi = anaTeklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");
                string TeklifNo = anaTeklif.TeklifNo.ToString();
                string SurumNo = String.Empty;
                string TibbiTetkikSonuc = response.TIBBI_TETKIK_SONUCU;

                if (!String.IsNullOrEmpty(response.SURUM_BILGI))
                    SurumNo = response.SURUM_BILGI;

                if (!String.IsNullOrEmpty(TibbiTetkikSonuc))
                {
                    int index = TibbiTetkikSonuc.IndexOf(".");
                    if (index != -1 && TibbiTetkikSonuc.Length > (index + 3))
                    {
                        string ilk = TibbiTetkikSonuc.Substring(0, index + 1);
                        string son = TibbiTetkikSonuc.Substring(index + 3);

                        TibbiTetkikSonuc = ilk + " \n " + son;
                    }
                }

                parser.SetVariable("$TeklifTarihi$", TeklifTarihi);
                parser.SetVariable("$SurumNo$", SurumNo);
                parser.SetVariable("$TeklifNo$", TeklifNo);
                parser.SetVariable("$Tetkikler$", TibbiTetkikSonuc);

                #endregion

                #endregion

                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("korunan_gelecek_hayat_sigortasi{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("KorunanGelecek", fileName, fileData);

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
