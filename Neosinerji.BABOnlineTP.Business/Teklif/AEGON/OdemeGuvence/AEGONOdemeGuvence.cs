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
using Neosinerji.BABOnlineTP.Business.AEGON;

namespace Neosinerji.BABOnlineTP.Business.AEGON
{
    public class AEGONOdemeGuvence : Teklif, IAEGONOdemeGuvence
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
        public AEGONOdemeGuvence(ICRService crService,
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

                AegonOGRequest request = new AegonOGRequest();

                request.TeklifNo = teklif.TeklifNo;
                request.pKapTeklifNo = teklif.GenelBilgiler.IlgiliTeklifNo ?? 0;
                request.teklifTarihi = TurkeyDateTime.Today.ToString("dd.MM.yyyy");

                DateTime SigortaBaslangicTar = teklif.ReadSoru(OdemeGuvenceSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                //Para birimi
                request.paraBirimi = OdemeGuvenceParaBirimi.ParaBirimiText(teklif.ReadSoru(OdemeGuvenceSorular.ParaBirimi, String.Empty));


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
                        request.ogsBastar = SigortaBaslangicTar.ToString("dd.MM.yyyy");
                    }
                }

                //Sigorta Süresi
                string SigortaSuresi = teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(SigortaSuresi))
                    request.sigSure = Convert.ToInt32(SigortaSuresi);

                int odemeSuresi = 1;

                //Prim Odeme Donemi
                switch (teklif.ReadSoru(OdemeGuvenceSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": request.odeDonem = "Aylık"; odemeSuresi = 12; break;
                    case "2": request.odeDonem = "3 Aylık"; odemeSuresi = 6; break;
                    case "3": request.odeDonem = "6 Aylık"; odemeSuresi = 2; break;
                    case "4": request.odeDonem = "Yıllık"; odemeSuresi = 1; break;
                }

                request.kapBastar = teklif.ReadSoru(OdemeGuvenceSorular.PoliceBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");
                request.kapAyPrim = Convert.ToDouble(teklif.ReadSoru(OdemeGuvenceSorular.aylikPrimTutari, decimal.Zero));


                //TEMİNATLAR
                request.KHPM_varmi = "YOK";
                request.MDPM_varmi = "YOK";
                request.IDPM_varmi = "YOK";
                request.KHYPM_varmi = "YOK";

                int teminatSayac = 0;

                if (teklif.ReadSoru(OdemeGuvenceSorular.KritikHastalikDPM, false))
                {
                    request.KHPM_varmi = "VAR";
                    teminatSayac++;
                }

                if (teklif.ReadSoru(OdemeGuvenceSorular.TamVeTaimiMaluliyetDPM, false))
                {
                    request.MDPM_varmi = "VAR";
                    teminatSayac++;
                }

                if (teklif.ReadSoru(OdemeGuvenceSorular.IssizlikDPM, false))
                {
                    request.IDPM_varmi = "VAR";
                    teminatSayac++;
                }

                if (teklif.ReadSoru(OdemeGuvenceSorular.KazaSonucuHastanedeyatarakTDPM, false))
                {
                    request.KHYPM_varmi = "VAR";
                    teminatSayac++;
                }

                //Surprim
                request.surprim_ana = 0;
                request.surprim_khpm = 0;
                request.surprim_mdpm = 0;
                request.surprim_idpm = 0;
                request.surprim_khypm = 0;


                


                // KAP POLICE NO
                //string kapPoliceNo = teklif.ReadSoru(OdemeGuvenceSorular.PoliceNumarasi, String.Empty);
                //if (!String.IsNullOrEmpty(kapPoliceNo))
                //{
                //    //request.pKapTeklifNo = "OB01-" + kapPoliceNo;
                //}
                //else if (teklif.GenelBilgiler.IlgiliTeklifNo.HasValue && teklif.GenelBilgiler.IlgiliTeklifNo.Value > 0)
                //{
                //    //request.pKapTeklifNo = "OB01-" + teklif.GenelBilgiler.IlgiliTeklifNo.Value.ToString();
                //}

                #endregion

                #region RESPONSE | LOG

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.AEGON_ServiceURL);

                aegon.Service1 servis = new aegon.Service1();
                servis.Timeout = 150000;
                servis.Url = serviceURL;

                DataTable prim = servis.OdemeGuvence_OGS01(request.TeklifNo, request.cinsiyet, request.dogTar, request.ogsBastar, request.odeDonem, request.paraBirimi,
                                 request.kapBastar, request.sigSure, request.kapAyPrim, request.KHPM_varmi, request.MDPM_varmi, request.IDPM_varmi, request.KHYPM_varmi,
                                 request.surprim_ana, request.surprim_khpm, request.surprim_mdpm, request.surprim_idpm, request.surprim_khypm, request.gvo,
                                 request.teklifTarihi, request.musteriAdSoyad, AegonCommon.FirmaKisaAdi, request.pKapTeklifNo);

                DataRow row = prim.Rows[0];

                string hataText = row["HATA"].ToString();

                #region Hata Kontrol

                if (hataText != "HATA YOK")
                {
                    string hata = String.Empty;

                    if (hataText.Contains("MYPK_HATA"))
                    {
                        #region MYPK_HATA

                        if (teminatSayac < 4)
                        {
                            #region Teminatlarla ilgili bir hata var

                            if (teminatSayac == 3 && request.IDPM_varmi != "VAR")
                            {
                                if (teklif.ReadSoru(OdemeGuvenceSorular.IssizlikDPM_Faydalanabilirmi, true))
                                {
                                    #region işsizlikten yararlanabiliyormu sorusu

                                    hata = "Teklif, asgari primin altındadır.Kişi İşsizlik Durumunda Prim Muafiyeti Teminatı’ndan faydalanabilecek durumda mı?";
                                    hata += @"<br/><button class='btn btn-mini' id='prim-muafiyeti-hayir'>Hayır</button>
                                               <button class='btn btn-success btn-mini' id='prim-muafiyeti-evet'>Evet</button>";

                                    this.Import(teklif);
                                    this.GenelBilgiler.Basarili = false;

                                    this.EndLog(hata, false);
                                    this.AddHata(hata);
                                    return;

                                    #endregion
                                }
                                else
                                {
                                    //kullanıcının üç teminatı aldı ve işsizlik durumunda prim muafiteyinden yararlanamadığını belitmiş ise 
                                    //web servisle gelen mesajı dikkate almadan teklifi çıkartmanız gerekecek.
                                    this.EndLog(prim, true, prim.GetType());
                                }
                            }
                            else
                            {
                                #region Police limit altında ve ek teminat eklenmeli hatası

                                hata = @"Poliçe ilk yıl primi asgari primin altındadır. Lütfen başka ek teminat ekleyiniz 
                                         <br/><button class='btn yellow' id='3AdimaGit'>Tamam</button>";

                                this.Import(teklif);
                                this.GenelBilgiler.Basarili = false;

                                this.EndLog(hata, false);
                                this.AddHata(hata);
                                return;

                                #endregion
                            }

                            #endregion
                        }
                        else
                        {
                            //kullanıcının tüm teminatları aldığını görüyorsanız web servisle gelen mesajı dikkate almadan teklifi çıkartmanız gerekecek.
                            this.EndLog(prim, true, prim.GetType());
                        }

                        #endregion
                    }
                    else
                    {
                        #region Teklif Hatalı olarak kaydediliyor.

                        hata = AegonHelper.AegonReplace(hataText);

                        this.Import(teklif);
                        this.GenelBilgiler.Basarili = false;

                        this.EndLog(hata, false);
                        this.AddHata(hata);
                        return;

                        #endregion
                    }
                }
                else
                {
                    //Dönen mesaj HATA YOK teklif başarılı.
                    this.EndLog(prim, true, prim.GetType());
                }

                #endregion

                #endregion

                #region Genel bilgiler

                #region DataTable To Response

                AegonOGResponse response = new AegonOGResponse();

                if (row["HATA"] != null)
                    response.HATA = row["HATA"].ToString();
                if (row["SURUM_BILGI"] != null)
                    response.SURUM_BILGI = row["SURUM_BILGI"].ToString();
                if (row["TIBBI_TETKIK_SONUCU"] != null)
                    response.TIBBI_TETKIK_SONUCU = row["TIBBI_TETKIK_SONUCU"].ToString();
                if (row["UYARI"] != null)
                    response.UYARI = row["UYARI"].ToString();


                if (row["KUME_ID"] != null)
                    response.KUME_ID = row["KUME_ID"].ToString();
                if (row["OGS_SURE"] != null)
                    response.OGS_SURE = row["OGS_SURE"].ToString();
                if (row["OGS_SURE_AY"] != null)
                    response.OGS_SURE_AY = row["OGS_SURE_AY"].ToString();
                if (row["PRIM"] != null)
                    response.PRIM = row["PRIM"].ToString();

                //YENİ EKLENEN ALANLAR
                if (row["SS_TOPLAM_PRIM"] != null)
                    response.SS_TOPLAM_PRIM = row["SS_TOPLAM_PRIM"].ToString();
                if (row["VAS_PRIM_MALIYETI"] != null)
                    response.VAS_PRIM_MALIYETI = row["VAS_PRIM_MALIYETI"].ToString();
                if (row["PRIMDEN_VERGI_AVANTAJI"] != null)
                    response.PRIMDEN_VERGI_AVANTAJI = row["PRIMDEN_VERGI_AVANTAJI"].ToString();

                if (row["YILDNM_KADAR_TOP_PRIM"] != null)
                    response.YILDNM_KADAR_TOP_PRIM = row["YILDNM_KADAR_TOP_PRIM"].ToString();
                if (row["YILDNM_SONRA_TOP_PRIM"] != null)
                    response.YILDNM_SONRA_TOP_PRIM = row["YILDNM_SONRA_TOP_PRIM"].ToString();
                if (row["YILDNM_KADAR_VER_AVANTAJI"] != null)
                    response.YILDNM_KADAR_VER_AVANTAJI = row["YILDNM_KADAR_VER_AVANTAJI"].ToString();
                if (row["YILDNM_SONRA_VER_AVANTAJI"] != null)
                    response.YILDNM_SONRA_VER_AVANTAJI = row["YILDNM_SONRA_VER_AVANTAJI"].ToString();



                //TEMINATLAR
                if (row["VFPM_TEM_TUTAR"] != null)
                    response.VFPM_TEM_TUTAR = row["VFPM_TEM_TUTAR"].ToString();
                if (row["KHPM_TEM_TUTAR"] != null)
                    response.KHPM_TEM_TUTAR = row["KHPM_TEM_TUTAR"].ToString();
                if (row["MDPM_TEM_TUTAR"] != null)
                    response.MDPM_TEM_TUTAR = row["MDPM_TEM_TUTAR"].ToString();
                if (row["IDPM_TEM_TUTAR"] != null)
                    response.IDPM_TEM_TUTAR = row["IDPM_TEM_TUTAR"].ToString();
                if (row["KHYPM_TEM_TUTAR"] != null)
                    response.KHYPM_TEM_TUTAR = row["KHYPM_TEM_TUTAR"].ToString();


                int bolen = 1;
                if (!String.IsNullOrEmpty(response.OGS_SURE_AY))
                    bolen = Convert.ToInt32(response.OGS_SURE_AY);

                if (response.PRIM == "0" && response.OGS_SURE_AY == "0" && prim.Rows.Count > 0)
                {
                    DataRow row2 = prim.Rows[1];

                    if (row2["PRIM"] != null)
                        response.PRIM = row2["PRIM"].ToString();

                    bolen = odemeSuresi;
                }
                #endregion

                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = SigortaBaslangicTar;
                this.GenelBilgiler.BitisTarihi = SigortaBaslangicTar.AddYears(request.sigSure);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Now.AddDays(30);

                if (!String.IsNullOrEmpty(response.PRIM))
                {
                    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(response.PRIM, turkey);
                    this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim / bolen;
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

                #endregion

                #region Sorular

                this.AddSoru(OdemeGuvenceSorular.KapYildonumuKalanSure, response.OGS_SURE_AY);
                this.AddSoru(OdemeGuvenceSorular.SS_TOPLAM_PRIM, response.SS_TOPLAM_PRIM);
                this.AddSoru(OdemeGuvenceSorular.VAS_PRIM_MALIYETI, response.VAS_PRIM_MALIYETI);
                this.AddSoru(OdemeGuvenceSorular.PRIMDEN_VERGI_AVANTAJI, response.PRIMDEN_VERGI_AVANTAJI);
                this.AddSoru(OdemeGuvenceSorular.YILDNM_KADAR_TOP_PRIM, response.YILDNM_KADAR_TOP_PRIM);
                this.AddSoru(OdemeGuvenceSorular.YILDNM_SONRA_TOP_PRIM, response.YILDNM_SONRA_TOP_PRIM);
                this.AddSoru(OdemeGuvenceSorular.YILDNM_KADAR_VER_AVANTAJI, response.YILDNM_KADAR_VER_AVANTAJI);
                this.AddSoru(OdemeGuvenceSorular.YILDNM_SONRA_VER_AVANTAJI, response.YILDNM_SONRA_VER_AVANTAJI);

                #endregion

                #region Teminatlar

                if (!String.IsNullOrEmpty(response.VFPM_TEM_TUTAR))
                    this.AddTeminat(OdemeGuvenceTeminatlar.VefatDurumundaPMT, Convert.ToDecimal(response.VFPM_TEM_TUTAR, turkey), 0, 0, 0, 0);

                if (!String.IsNullOrEmpty(response.KHPM_TEM_TUTAR))
                    this.AddTeminat(OdemeGuvenceTeminatlar.KritikHastalikDPM, Convert.ToDecimal(response.KHPM_TEM_TUTAR, turkey), 0, 0, 0, 0);

                if (!String.IsNullOrEmpty(response.MDPM_TEM_TUTAR))
                    this.AddTeminat(OdemeGuvenceTeminatlar.TamVeTaimiMaluliyetDPM, Convert.ToDecimal(response.MDPM_TEM_TUTAR, turkey), 0, 0, 0, 0);

                if (!String.IsNullOrEmpty(response.IDPM_TEM_TUTAR))
                    this.AddTeminat(OdemeGuvenceTeminatlar.IssizlikDPM, Convert.ToDecimal(response.IDPM_TEM_TUTAR, turkey), 0, 0, 0, 0);

                if (!String.IsNullOrEmpty(response.KHYPM_TEM_TUTAR))
                    this.AddTeminat(OdemeGuvenceTeminatlar.KazaSonucuHastanedeyatarakTDPM, Convert.ToDecimal(response.KHYPM_TEM_TUTAR, turkey), 0, 0, 0, 0);

                #endregion

                #region Ödeme Planı

                this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));

                #endregion

                #region Web Servis Response

                this.AddWebServisCevap(Common.WebServisCevaplar.SurumNo, response.SURUM_BILGI);

                response.UYARI = AegonHelper.AegonReplace(response.UYARI);

                //Web servisten gelen uyarı mesajı
                if (!String.IsNullOrEmpty(response.UYARI))
                    this.AddWebServisCevap(Common.WebServisCevaplar.Uyari, response.UYARI.Replace("\n", "|"));

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

        public void CreateTeklifPDF(ITeklif anaTeklif, AegonOGResponse response, DataTable table)
        {
            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(_RootPath + "Content/templates/", PdfTemplates.ODEME_GUVENCE);

                CultureInfo culture = new CultureInfo("tr-TR");

                pdf = new PDFHelper("Babonline", "Ödeme Güvence Sigortası", "Ödeme Güvence Sigortası", 8, _RootPath + "Content/fonts/", PdfTemplates.SenticoSansDT_Regular);

                string surumNo = String.Empty;
                surumNo = response.SURUM_BILGI;


                //SAYFA NUMARASI YAZIYOR
                pdf.SetPageEventHelper(new PDFCustomEventHelperAEGON(surumNo));

                PDFParser parser = new PDFParser(template, pdf);

                #endregion

                #region Data Fill

                #region Odeme Guvence Teklif Poliçesi Bilgileri

                #region IMG

                string imgpath = Path.Combine(_RootPath, "Content/img/Aegon/aegonlogo-pdf.jpg");
                parser.SetVariable("$TVMLogo$", imgpath);

                #endregion

                #region SigortaAdayi Bilgileri

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(anaTeklif.SigortaEttiren.MusteriKodu);

                //Sigorta Adayı Bilgileri
                string AdiSoyadi = String.Empty;
                string DogumTarihi = String.Empty;
                string Cinsiyeti = String.Empty;
                string BaslangicYasi = String.Empty;

                //Vergi ile İlgili Bilgiler
                string BeyanEdilenGelirVergisiOrani = String.Empty;
                string PrimdenVergiAvantaji = String.Empty;
                string VergiAvantajiSonrasiPrimMaliyeti = String.Empty;
                string SureBoyuncaOdenecekToplamPrim = String.Empty;


                DateTime SBTarihi = anaTeklif.ReadSoru(OdemeGuvenceSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                if (musteri != null)
                {
                    AdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                    Cinsiyeti = musteri.Cinsiyet == "E" ? "Erkek" : "Kadın";
                    DogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty;
                    BaslangicYasi = AEGONTESabitPrimli.AegonYasHesapla(musteri.DogumTarihi.Value, SBTarihi).ToString();

                    switch (musteri.CiroBilgisi)
                    {
                        case "1": BeyanEdilenGelirVergisiOrani = "%15"; break;
                        case "2": BeyanEdilenGelirVergisiOrani = "%20"; break;
                        case "3": BeyanEdilenGelirVergisiOrani = "%27"; break;
                        case "4": BeyanEdilenGelirVergisiOrani = "%35"; break;
                        case "5": BeyanEdilenGelirVergisiOrani = "%27"; break;
                    }
                }

                PrimdenVergiAvantaji = response.PRIMDEN_VERGI_AVANTAJI;
                VergiAvantajiSonrasiPrimMaliyeti = response.VAS_PRIM_MALIYETI;
                SureBoyuncaOdenecekToplamPrim = response.SS_TOPLAM_PRIM;

                parser.SetVariable("$AdiSoyadi$", AdiSoyadi);
                parser.SetVariable("$DogumTarihi$", DogumTarihi);
                parser.SetVariable("$Yas$", BaslangicYasi);
                parser.SetVariable("$Cinsiyet$", Cinsiyeti);

                //Vergi Bilgileri
                parser.SetVariable("$BEGVO$", BeyanEdilenGelirVergisiOrani);
                parser.SetVariable("$PrimdenVergiAvantaji$", PrimdenVergiAvantaji);
                parser.SetVariable("$VASPM$", VergiAvantajiSonrasiPrimMaliyeti);
                parser.SetVariable("$SBOTP$", SureBoyuncaOdenecekToplamPrim);

                #endregion

                #region KAP Bilgileri

                decimal aylikPrimTutari = anaTeklif.ReadSoru(OdemeGuvenceSorular.aylikPrimTutari, decimal.Zero);
                parser.SetVariable("$AylikPrimTutari$", aylikPrimTutari.ToString("N2", culture));
                parser.SetVariable("$KAPPoliceNo$", anaTeklif.ReadSoru(OdemeGuvenceSorular.PoliceNumarasi, String.Empty));

                #endregion

                #region Sigorta ile İlgili Bilgiler

                string BaslangicTarihi = String.Empty;
                string SigortaSuresi = String.Empty;
                string PrimOdeme = String.Empty;
                string KalanSigortaSuresi = String.Empty;
                string ParaBirimi = String.Empty;

                BaslangicTarihi = SBTarihi.ToString("dd.MM.yyyy");
                SigortaSuresi = response.OGS_SURE;
                KalanSigortaSuresi = response.OGS_SURE_AY;

                switch (anaTeklif.ReadSoru(OdemeGuvenceSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": PrimOdeme = "Aylık"; break;
                    case "2": PrimOdeme = "3 Aylık"; break;
                    case "3": PrimOdeme = "6 Aylık"; break;
                    case "4": PrimOdeme = "Yıllık"; break;
                }

                ParaBirimi = OdemeGuvenceParaBirimi.ParaBirimiText(anaTeklif.ReadSoru(OdemeGuvenceSorular.ParaBirimi, String.Empty));

                switch (ParaBirimi)
                {
                    case "USD": ParaBirimi = "ABD Doları"; break;
                    case "TL": ParaBirimi = "Türk Lirası"; break;
                }

                parser.SetVariable("$BaslangicTarihi$", BaslangicTarihi);
                parser.SetVariable("$SigortaSuresi$", SigortaSuresi);
                parser.SetVariable("$PrimOdeme$", PrimOdeme);
                parser.SetVariable("$KalanSigortaSuresi$", KalanSigortaSuresi);
                parser.SetVariable("$ParaBirimi$", ParaBirimi);

                #endregion

                #region Teminat Tutarları İle

                string AnaTeminatiBedeli = String.Empty;
                string KritikHastaliklarEkTeminatPrice = String.Empty;
                string TamVeDaimiMaluliyetPrice = String.Empty;
                string IssizlikDurumundaPrimPrice = String.Empty;
                string KazaSonucuHastanedeyatarakTedaviPrice = String.Empty;


                AnaTeminatiBedeli = response.VFPM_TEM_TUTAR;
                KritikHastaliklarEkTeminatPrice = response.KHPM_TEM_TUTAR;
                TamVeDaimiMaluliyetPrice = response.MDPM_TEM_TUTAR;
                IssizlikDurumundaPrimPrice = response.IDPM_TEM_TUTAR;
                KazaSonucuHastanedeyatarakTedaviPrice = response.KHYPM_TEM_TUTAR;


                parser.SetVariable("$AnaTeminatiBedeli$", AnaTeminatiBedeli);
                parser.SetVariable("$KritikHastaliklarEkTeminatPrice$", KritikHastaliklarEkTeminatPrice);
                parser.SetVariable("$TamVeDaimiMaluliyetPrice$", TamVeDaimiMaluliyetPrice);
                parser.SetVariable("$IssizlikDurumundaPrimPrice$", IssizlikDurumundaPrimPrice);
                parser.SetVariable("$KazaSonucuHastanedeyatarakTedaviPrice$", KazaSonucuHastanedeyatarakTedaviPrice);

                #endregion

                #region Table

                List<AegonOGResponse> list = new List<AegonOGResponse>();

                foreach (DataRow row in table.Rows)
                {
                    AegonOGResponse listItem = new AegonOGResponse();

                    if (row["POLICE_DONEMI"] != null)
                        listItem.POLICE_DONEMI = row["POLICE_DONEMI"].ToString();
                    if (row["VEF_TEM"] != null)
                        listItem.VEF_TEM = row["VEF_TEM"].ToString();
                    if (row["KH_TEM_DPMT"] != null)
                        listItem.KH_TEM_DPMT = row["KH_TEM_DPMT"].ToString();
                    if (row["TDM_TEM_DPMT"] != null)
                        listItem.TDM_TEM_DPMT = row["TDM_TEM_DPMT"].ToString();
                    if (row["I_TEM_DPMT"] != null)
                        listItem.I_TEM_DPMT = row["I_TEM_DPMT"].ToString();
                    if (row["KHYT_TEM_DPMT"] != null)
                        listItem.KHYT_TEM_DPMT = row["KHYT_TEM_DPMT"].ToString();
                    if (row["PRIM"] != null)
                        listItem.PRIM = row["PRIM"].ToString();
                    if (row["AYLIK_ESDEGER_PRIM"] != null)
                        listItem.AYLIK_ESDEGER_PRIM = row["AYLIK_ESDEGER_PRIM"].ToString();

                    if (!String.IsNullOrEmpty(listItem.POLICE_DONEMI))
                        list.Add(listItem);
                }


                StringBuilder sb = new StringBuilder();

                int sayac = 0;

                foreach (AegonOGResponse item in list)
                {
                    #region 0 Yazılmıyor.

                    //sb.AppendLine("<td>" + (item.POLICE_DONEMI != "0 " ? item.POLICE_DONEMI : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.VEF_TEM != "0" ? item.VEF_TEM : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.KH_TEM_DPMT != "0" ? item.KH_TEM_DPMT : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.TDM_TEM_DPMT != "0" ? item.TDM_TEM_DPMT : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.I_TEM_DPMT != "0" ? item.I_TEM_DPMT : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.KHYT_TEM_DPMT != "0" ? item.KHYT_TEM_DPMT : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.PRIM != "0" ? item.PRIM : "") + "</td>");
                    //sb.AppendLine("<td>" + (item.AYLIK_ESDEGER_PRIM != "0" ? item.AYLIK_ESDEGER_PRIM : "") + "</td>");
                    //sb.AppendLine("</tr>");

                    #endregion

                    if (sayac == 0)
                        sb.AppendLine("<tr><height=75><backgroundColor=clear><normal><center>");
                    else
                        sb.AppendLine("<tr><height=20><backgroundColor=clear><normal><center>");

                    sb.AppendLine("<td>" + item.POLICE_DONEMI + "</td>");
                    sb.AppendLine("<td>" + item.VEF_TEM + "</td>");
                    sb.AppendLine("<td>" + item.KH_TEM_DPMT + "</td>");
                    sb.AppendLine("<td>" + item.TDM_TEM_DPMT + "</td>");
                    sb.AppendLine("<td>" + item.I_TEM_DPMT + "</td>");
                    sb.AppendLine("<td>" + item.KHYT_TEM_DPMT + "</td>");
                    sb.AppendLine("<td>" + item.PRIM + "</td>");
                    sb.AppendLine("<td>" + item.AYLIK_ESDEGER_PRIM + "</td>");
                    sb.AppendLine("</tr>");

                    sayac++;
                }

                parser.ReplacePlaceHolder("SigortaPrimTutarlari", sb.ToString());

                #endregion

                #region Footer

                string TeklifTarihi = String.Empty;
                string TeklifNo = String.Empty;
                string Tetkikler = response.TIBBI_TETKIK_SONUCU;

                TeklifTarihi = anaTeklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");
                TeklifNo = anaTeklif.TeklifNo.ToString();

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

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("odeme_guvence_sistemi_sigortasi{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("OdemeGuvence", fileName, fileData);

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
