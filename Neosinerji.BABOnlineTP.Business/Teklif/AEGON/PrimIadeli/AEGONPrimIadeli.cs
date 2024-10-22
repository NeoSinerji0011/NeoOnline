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
    public class AEGONPrimIadeli : Teklif, IAEGONPrimIadeli
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IAktifKullaniciService _AktifKullaniciService;

        string _RootPath;

        [InjectionConstructor]
        public AEGONPrimIadeli(ICRService crService,
                                    ICRContext crContext,
                                    IMusteriService musteriService,
                                    IAracContext aracContext,
                                    IKonfigurasyonService konfigurasyonService,
                                    ITVMContext tvmContext,
                                    ILogService log,
                                    ITeklifService teklifService,
                                    IAktifKullaniciService aktifKullanici)
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
            _AktifKullaniciService = aktifKullanici;
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

                AegonPIRequest request = new AegonPIRequest();

                request.TeklifNo = teklif.TeklifNo;
                request.teklifTarihi = TurkeyDateTime.Today.ToString("dd.MM.yyyy");

                //Sigorta Başlangıç Tarihi
                DateTime SigortaBaslangicTar = teklif.ReadSoru(PrimIadeliSorular.SigortaBaslangicTarihi, DateTime.MinValue);

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
                switch (teklif.ReadSoru(PrimIadeliSorular.SigortaSuresi, String.Empty))
                {
                    case "1": request.sigortaSure = 12; break;
                    case "2": request.sigortaSure = 15; break;
                    case "3": request.sigortaSure = 18; break;
                    case "4": request.sigortaSure = 24; break;
                    case "5": request.sigortaSure = 30; break;
                }


                //Prim Odeme Donemi
                switch (teklif.ReadSoru(PrimIadeliSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": request.odeDonem = "Aylık"; break;
                    case "2": request.odeDonem = "3 Aylık"; break;
                    case "3": request.odeDonem = "6 Aylık"; break;
                    case "4": request.odeDonem = "Yıllık"; break;
                }

                //Para Birimi
                request.parabirimi = "USD";

                string hesapYontem = teklif.ReadSoru(PrimIadeliSorular.HesaplamaSecenegi, String.Empty);

                switch (hesapYontem)
                {
                    case "1":
                        request.tutar = Convert.ToDouble(teklif.ReadSoru(PrimIadeliSorular.YillikPrimTutari, decimal.Zero));
                        request.hesapYontem = Convert.ToInt32(hesapYontem);
                        break;
                    case "2":
                        request.tutar = Convert.ToDouble(teklif.ReadSoru(PrimIadeliSorular.VefatTeminatTutari, decimal.Zero));
                        request.hesapYontem = Convert.ToInt32(hesapYontem);
                        break;
                }

                request.suprimOran = 0;

                #endregion

                #region RESPONSE | LOG

                this.BeginLog(request, typeof(AegonPIRequest), WebServisIstekTipleri.Teklif);

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.AEGON_ServiceURL);

                aegon.Service1 servis = new aegon.Service1();
                servis.Timeout = 150000;
                servis.Url = serviceURL;


                DataTable dt = servis.PrimIadeli_PI01(request.TeklifNo, request.cinsiyet, request.dogTar, request.yas, request.sigBasTar,
                                                      request.sigortaSure, request.odeDonem, request.parabirimi, request.hesapYontem, request.tutar,
                                                      request.suprimOran, request.gvo, request.teklifTarihi, request.musteriAdSoyad, AegonCommon.FirmaKisaAdi);

                if (dt == null || dt.Rows.Count == 0 || dt.Rows[0]["HATA"] == null)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    this.EndLog("Web Servise yanıt dönmedi", false);
                    this.AddHata("Web Servise yanıt dönmedi");
                    return;
                }

                DataRow row = dt.Rows[0];
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
                    this.EndLog(dt, true, dt.GetType());

                #endregion

                #region SUCCESS

                #region Genel bilgiler

                CultureInfo turkey = new CultureInfo("tr-TR");

                AegonPIResponse response = new AegonPIResponse();

                #region Mapper

                response.HATA = row["HATA"].ToString();

                if (row["SURUM_BILGI"] != null)
                    response.SURUM_BILGI = row["SURUM_BILGI"].ToString();

                if (row["B_ANA_TEM_ADI"] != null)
                    response.B_ANA_TEM_ADI = row["B_ANA_TEM_ADI"].ToString();

                if (row["B_TEMINAT_TUTARI"] != null)
                    response.B_TEMINAT_TUTARI = row["B_TEMINAT_TUTARI"].ToString();

                if (row["B_SS_PRIM_IADE_TUTAR"] != null)
                    response.B_SS_PRIM_IADE_TUTAR = row["B_SS_PRIM_IADE_TUTAR"].ToString();

                if (row["B_YILLIK_PRIM"] != null)
                    response.B_YILLIK_PRIM = row["B_YILLIK_PRIM"].ToString();

                if (row["B_DONEMLIK_PRIM"] != null)
                    response.B_DONEMLIK_PRIM = row["B_DONEMLIK_PRIM"].ToString();

                if (row["D_POLICE_YILI"] != null)
                    response.D_POLICE_YILI = row["D_POLICE_YILI"].ToString();

                if (row["D_KUMULATIF_YILLIK_PRIM"] != null)
                    response.D_KUMULATIF_YILLIK_PRIM = row["D_KUMULATIF_YILLIK_PRIM"].ToString();

                if (row["D_MATEMATIK_KARSILIK"] != null)
                    response.D_MATEMATIK_KARSILIK = row["D_MATEMATIK_KARSILIK"].ToString();

                if (row["D_ISTIRA_KESINTI_ORAN"] != null)
                    response.D_ISTIRA_KESINTI_ORAN = row["D_ISTIRA_KESINTI_ORAN"].ToString();

                if (row["D_ISTIRA_KESINTI_TUTAR"] != null)
                    response.D_ISTIRA_KESINTI_TUTAR = row["D_ISTIRA_KESINTI_TUTAR"].ToString();

                if (row["D_ISTIRA_BEDEL"] != null)
                    response.D_ISTIRA_BEDEL = row["D_ISTIRA_BEDEL"].ToString();

                if (row["TIBBI_TETKIK_SONUCU"] != null)
                    response.TIBBI_TETKIK_SONUCU = row["TIBBI_TETKIK_SONUCU"].ToString();

                if (row["B_SS_TOPLAM_PRIM"] != null)
                    response.B_SS_TOPLAM_PRIM = row["B_SS_TOPLAM_PRIM"].ToString();

                if (row["PRIMDEN_VERGI_AVANTAJI"] != null)
                    response.PRIMDEN_VERGI_AVANTAJI = row["PRIMDEN_VERGI_AVANTAJI"].ToString();

                if (row["VAS_PRIM_MALIYETI"] != null)
                    response.VAS_PRIM_MALIYETI = row["VAS_PRIM_MALIYETI"].ToString();

                #endregion

                this.Import(teklif);

                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = SigortaBaslangicTar;
                this.GenelBilgiler.BitisTarihi = SigortaBaslangicTar.AddYears(request.sigortaSure);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Now.AddDays(30);

                if (!String.IsNullOrEmpty(response.B_YILLIK_PRIM))
                    this.GenelBilgiler.BrutPrim = response.B_YILLIK_PRIM == "" ? 0 : Convert.ToDecimal(response.B_YILLIK_PRIM, turkey);

                if (!String.IsNullOrEmpty(response.B_DONEMLIK_PRIM))
                    this.GenelBilgiler.NetPrim = response.B_DONEMLIK_PRIM == "" ? 0 : Convert.ToDecimal(response.B_DONEMLIK_PRIM, turkey);

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

                if (!String.IsNullOrEmpty(response.B_TEMINAT_TUTARI))
                    this.AddTeminat(PrimIadeliTeminatlar.VefatTeminati, Convert.ToDecimal(response.B_TEMINAT_TUTARI, turkey), 0, 0, 0, 0);
                if (!String.IsNullOrEmpty(response.B_SS_PRIM_IADE_TUTAR))
                    this.AddTeminat(PrimIadeliTeminatlar.SureSonuPrimIadesiTeminati, Convert.ToDecimal(response.B_SS_PRIM_IADE_TUTAR, turkey), 0, 0, 0, 0);

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

                CreateTeklifPDF(teklif, dt, response);

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

        public void CreateTeklifPDF(ITeklif teklif, DataTable dt, AegonPIResponse response)
        {
            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(this.TeklifNo, this.GenelBilgiler.TVMKodu);

                string template = PdfTemplates.GetTemplate(_RootPath + "Content/templates/", PdfTemplates.PRIM_IADELI_YEDEK);

                pdf = new PDFHelper("Babonline", "Prim İadeli Hayat Sigortası", "Prim İadeli Hayat Sigortası", 8, _RootPath + "Content/fonts/",
                                    PdfTemplates.SenticoSansDT_Regular);
                string surumNo = String.Empty;
                surumNo = response.SURUM_BILGI;
                //SAYFA NUMARASI YAZIYOR
                pdf.SetPageEventHelper(new PDFCustomEventHelperAEGON(surumNo));

                PDFParser parser = new PDFParser(template, pdf);

                #endregion

                #region Data Fill

                #region TE Teklif Poliçesi Bilgileri
                string imgpath = Path.Combine(_RootPath, "Content/img/Aegon/aegonlogo-pdf.jpg");
                string OB_FooterBorder = Path.Combine(_RootPath, "Content/img/Aegon/OB_FooterBorder.jpg");

                parser.SetVariable("$TVMLogo$", imgpath);
                parser.SetVariable("$OB_FooterBorder$", OB_FooterBorder);

                CultureInfo culture = new CultureInfo("tr-TR");

                #region Sigorta Adayı ile ilgili Bilgiler

                //Sigorta Adayı Bilgileri
                string AdiSoyadi = String.Empty;
                string DogumTarihi = String.Empty;
                string Cinsiyet = String.Empty;
                string GirisYasi = String.Empty;

                //Vergi ile İlgili Bilgiler
                string BeyanEdilenGelirVergisiOrani = String.Empty;
                string PrimdenVergiAvantaji = String.Empty;
                string VergiAvantajiSonrasiPrimMaliyeti = String.Empty;


                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(anaTeklif.SigortaEttiren.MusteriKodu);
                if (musteri != null)
                {
                    AdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                    Cinsiyet = musteri.Cinsiyet == "E" ? "Erkek" : "Kadın";
                    DogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty;
                    DateTime SBTarihi = anaTeklif.ReadSoru(PrimIadeliSorular.SigortaBaslangicTarihi, DateTime.MinValue);
                    if (musteri.DogumTarihi.HasValue)
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


                //Sigortalı adayı
                parser.SetVariable("$AdiSoyadi$", AdiSoyadi);
                parser.SetVariable("$DogumTarihi$", DogumTarihi);
                parser.SetVariable("$Cinsiyet$", Cinsiyet);
                parser.SetVariable("$GirisYasi$", GirisYasi);

                PrimdenVergiAvantaji = response.PRIMDEN_VERGI_AVANTAJI;
                VergiAvantajiSonrasiPrimMaliyeti = response.VAS_PRIM_MALIYETI;

                //Vergi Bilgileri
                parser.SetVariable("$BEGVO$", BeyanEdilenGelirVergisiOrani);
                parser.SetVariable("$PrimdenVergiAvantaji$", PrimdenVergiAvantaji);
                parser.SetVariable("$VASPM$", VergiAvantajiSonrasiPrimMaliyeti);

                #endregion

                #region Sigorta ile ilgili bilgiler

                //Sigorta bilgileri
                string SigortaSuresi = String.Empty;
                string PrimOdemeSuresi = String.Empty;
                string BaslangicTarihi = String.Empty;
                string ParaBirimi = "ABD Doları";
                string PrimOdemeSikligi = String.Empty;
                string DonemPrimi = String.Empty;
                string AnaTeminatSigortaBedeli = String.Empty;
                string SureSonuPrimIadesi = String.Empty;
                string ToplamYillikPrim = String.Empty;
                string SureBoyuncaOdenecekToplamPrim = String.Empty;

                ToplamYillikPrim = (this.GenelBilgiler.BrutPrim ?? 0).ToString("N2", culture);
                DonemPrimi = (this.GenelBilgiler.NetPrim ?? 0).ToString("N2", culture);

                string Surprim = anaTeklif.ReadSoru(PrimIadeliSorular.SurprimOrani, String.Empty);
                if (!String.IsNullOrEmpty(Surprim))
                    Surprim = "%" + Surprim;
                else
                {
                    Surprim = "%0";
                }

                switch (anaTeklif.ReadSoru(PrimIadeliSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1":
                        PrimOdemeSikligi = "Aylık";
                        break;
                    case "2":
                        PrimOdemeSikligi = "3 Aylık";
                        break;
                    case "3":
                        PrimOdemeSikligi = "6 Aylık";
                        break;
                    case "4":
                        PrimOdemeSikligi = "Yıllık";
                        break;
                }

                //Ana TEmnatlar
                TeklifTeminat vefat = this.Teminatlar.Where(s => s.TeminatKodu == PrimIadeliTeminatlar.VefatTeminati).FirstOrDefault();
                if (vefat != null && vefat.TeminatBedeli.HasValue && vefat.TeminatNetPrim.HasValue)
                    AnaTeminatSigortaBedeli = vefat.TeminatBedeli.Value.ToString("N2", culture);

                TeklifTeminat sureSonuPrim = this.Teminatlar.Where(s => s.TeminatKodu == PrimIadeliTeminatlar.SureSonuPrimIadesiTeminati).FirstOrDefault();
                if (sureSonuPrim != null && sureSonuPrim.TeminatBedeli.HasValue)
                    SureSonuPrimIadesi = sureSonuPrim.TeminatBedeli.Value.ToString("N2", culture);

                SigortaSuresi = anaTeklif.ReadSoru(PrimIadeliSorular.SigortaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(SigortaSuresi))
                    SigortaSuresi = ROP_SigortaSureleri.GetSigortaSuresi(Convert.ToInt32(SigortaSuresi));

                PrimOdemeSuresi = SigortaSuresi;
                SureBoyuncaOdenecekToplamPrim = response.B_SS_TOPLAM_PRIM;
                BaslangicTarihi = anaTeklif.ReadSoru(PrimIadeliSorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");




                //Sigorta Bilgileri
                parser.SetVariable("$SigortaSuresi$", SigortaSuresi);
                parser.SetVariable("$PrimOdemeSuresi$", PrimOdemeSuresi);
                parser.SetVariable("$BaslangicTarihi$", BaslangicTarihi);
                parser.SetVariable("$ParaBirimi$", ParaBirimi);
                parser.SetVariable("$PrimOdemeSikligi$", PrimOdemeSikligi);


                //Teminat Bilgileri ve Prim Bilgileri
                parser.SetVariable("$YillikSigortaPrimi$", ToplamYillikPrim);
                parser.SetVariable("$DonemselSigortaPrimi$", DonemPrimi);
                parser.SetVariable("$TeminatTutari$", AnaTeminatSigortaBedeli);
                parser.SetVariable("$SureSonuPrimIadeTutari$", SureSonuPrimIadesi);
                parser.SetVariable("$SurPrimOrani$", Surprim);
                parser.SetVariable("$SBOTP$", SureBoyuncaOdenecekToplamPrim);

                List<AegonPIResponse> primTabloList = new List<AegonPIResponse>();


                foreach (DataRow row in dt.Rows)
                {
                    AegonPIResponse listItem = new AegonPIResponse();

                    if (row["HATA"] != null && row["HATA"].ToString() == "HATA YOK")
                    {
                        if (row["D_POLICE_YILI"] != null)
                            listItem.D_POLICE_YILI = row["D_POLICE_YILI"].ToString();

                        if (row["D_KUMULATIF_YILLIK_PRIM"] != null && row["D_KUMULATIF_YILLIK_PRIM"].ToString() != "")
                            listItem.D_KUMULATIF_YILLIK_PRIM = row["D_KUMULATIF_YILLIK_PRIM"].ToString();

                        if (row["D_MATEMATIK_KARSILIK"] != null && row["D_MATEMATIK_KARSILIK"].ToString() != "")
                            listItem.D_MATEMATIK_KARSILIK = row["D_MATEMATIK_KARSILIK"].ToString();

                        if (row["D_ISTIRA_KESINTI_ORAN"] != null && row["D_ISTIRA_KESINTI_ORAN"].ToString() != "")
                            listItem.D_ISTIRA_KESINTI_ORAN = row["D_ISTIRA_KESINTI_ORAN"].ToString();

                        if (row["D_ISTIRA_KESINTI_TUTAR"] != null && row["D_ISTIRA_KESINTI_TUTAR"].ToString() != "")
                            listItem.D_ISTIRA_KESINTI_TUTAR = row["D_ISTIRA_KESINTI_TUTAR"].ToString();

                        if (row["D_ISTIRA_BEDEL"] != null && row["D_ISTIRA_BEDEL"].ToString() != "")
                            listItem.D_ISTIRA_BEDEL = row["D_ISTIRA_BEDEL"].ToString();

                        if (!String.IsNullOrEmpty(listItem.D_KUMULATIF_YILLIK_PRIM) || !String.IsNullOrEmpty(listItem.D_MATEMATIK_KARSILIK) ||
                            !String.IsNullOrEmpty(listItem.D_ISTIRA_KESINTI_ORAN) || !String.IsNullOrEmpty(listItem.D_ISTIRA_KESINTI_TUTAR))
                            primTabloList.Add(listItem);
                    }
                }

                StringBuilder sb = new StringBuilder();

                foreach (var item in primTabloList)
                {
                    sb.AppendLine("<tr><height=25><backgroundColor=255,255,255>");
                    sb.AppendLine("<td><normal><center>" + item.D_POLICE_YILI + "</td>");
                    sb.AppendLine("<td>" + item.D_KUMULATIF_YILLIK_PRIM + "</td>");
                    sb.AppendLine("<td>" + item.D_MATEMATIK_KARSILIK + "</td>");
                    sb.AppendLine("<td>" + item.D_ISTIRA_KESINTI_ORAN + "</td>");
                    sb.AppendLine("<td>" + item.D_ISTIRA_KESINTI_TUTAR + "</td>");
                    sb.AppendLine("<td>" + item.D_ISTIRA_BEDEL + "</td>");
                    sb.AppendLine("</tr>");
                }


                parser.ReplacePlaceHolder("MatematikKarsilik", sb.ToString());

                #endregion

                string TeklifTarihi = anaTeklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");
                string TeklifNo = anaTeklif.TeklifNo.ToString();
                string TibbiTetkikSonuc = response.TIBBI_TETKIK_SONUCU;
                
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
                parser.SetVariable("$TeklifNo$", TeklifNo);
                parser.SetVariable("$Tetkikler$", TibbiTetkikSonuc);

                #endregion

                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("Prim_iadeli_hayat_sigortasi{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("PrimIadeliHayat", fileName, fileData);

                teklif.GenelBilgiler.PDFDosyasi = url;

                // _TeklifService.UpdateGenelBilgiler(teklif.GenelBilgiler);

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
