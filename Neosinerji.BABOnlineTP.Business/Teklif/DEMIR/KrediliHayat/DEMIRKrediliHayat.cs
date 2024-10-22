using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Neosinerji.BABOnlineTP.Business.Pdf;

namespace Neosinerji.BABOnlineTP.Business.DEMIR
{
    public class DEMIRKrediliHayat : Teklif, IDEMIRKrediliHayat
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;

        [InjectionConstructor]
        public DEMIRKrediliHayat(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService)
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
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.DEMIR;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region Veri hazırlama / Prim tutarı hesaplama
                string carpanTipi = String.Empty;
                int krediAy = 0;
                decimal primTutari = 0;
                decimal krediTutari = 0;
                int sigortaliYasi = 0;

                TeklifSigortali sigortali = teklif.Sigortalilar.FirstOrDefault();
                MusteriGenelBilgiler musteriBilgileri = _MusteriService.GetMusteri(sigortali.MusteriKodu);

                if (musteriBilgileri.DogumTarihi.HasValue)
                {
                    TimeSpan ts = new TimeSpan();
                    ts = TurkeyDateTime.Today.Subtract(musteriBilgileri.DogumTarihi.Value);
                    double yas = ts.Days / 365.2425;

                    sigortaliYasi = Convert.ToInt32(yas);
                }

                string krediSuresiYilText = teklif.ReadSoru(KrediliHayatSorular.Kredi_Suresi, "1");
                int krediSuresiYil = Convert.ToInt32(krediSuresiYilText);

                krediTutari = teklif.ReadSoru(KrediliHayatSorular.Kredi_Tutari, 0);
                if (krediTutari == 0)
                    throw new Exception("Kredi tutarı okunamadı");

                carpanTipi = krediSuresiYil == 1 ? "18A" : "12A";
                krediAy = krediSuresiYil * 12;

                //Tip, yaş ve vadeye göre çarpan bulunuyor
                CR_KrediHayatCarpan carpan = _CRService.GetKrediHayatCarpan(TeklifUretimMerkezleri.DEMIR, carpanTipi, sigortaliYasi, krediSuresiYil);

                if (carpan != null && carpan.Carpan.HasValue)
                {
                    primTutari = carpan.Carpan.Value * krediTutari;

                    string hesapLog = String.Format("Çarpan Tipi : {0}, Sigortali Yaşı : {1}, Kredi Süresi : {2}, Kredi Tutarı : {3}, Çarpan : {4}, Prim Tutarı : {5}", carpanTipi, sigortaliYasi, krediSuresiYil, krediTutari, carpan.Carpan.Value, primTutari);
                    _Log.Info(hesapLog);
                }
                else
                    throw new Exception("Kredi tutarı çarpanı bulunamadı. Yaş aralığı 18 - 54");
                #endregion

                #region Genel bilgiler
                this.Import(teklif);

                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BitisTarihi = TurkeyDateTime.Today.AddYears(krediSuresiYil);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = primTutari;
                this.GenelBilgiler.NetPrim = primTutari;

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
                this.AddTeminat(KrediliHayatTeminatlar.Kredi, krediTutari, 0, primTutari, primTutari, 1);
                #endregion

                #region Ödeme Planı
                this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
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
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(this.TeklifNo, this.GenelBilgiler.TVMKodu);
                long policeNo = _TeklifService.GetOfflinePoliceNo(-1, UrunKodlari.KrediHayat);

                string krediSuresiYilText = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Suresi, "1");
                int krediSuresiYil = Convert.ToInt32(krediSuresiYilText);

                this.GenelBilgiler.TUMPoliceNo = policeNo.ToString();
                this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                this.GenelBilgiler.TanzimTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.TanzimTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.TanzimTarihi.AddYears(krediSuresiYil);

                //Muhasebe aktarımı
                //this.SendMuhasebe();

                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
            }
            catch (Exception ex)
            {
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        public override void PolicePDF()
        {
            PDFHelper pdf = null;
            try
            {
                string rootPath = System.Web.HttpContext.Current.Server.MapPath("/");
                string template = PdfTemplates.GetTemplate(rootPath + "Content/templates/", PdfTemplates.DEMIRKREDILIHAYAT_POLICE);

                pdf = new PDFHelper("Babonline", "KREDİ HAYAT SİGORTA SERTİFİKASI", "KREDİ HAYAT SİGORTA SERTİFİKASI", 8, rootPath + "Content/fonts/");
                PDFParser parser = new PDFParser(template, pdf);

                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(this.TeklifNo, this.GenelBilgiler.TVMKodu);

                string babaAdi = anaTeklif.ReadSoru(KrediliHayatSorular.Baba_Adi, String.Empty);
                decimal krediTutari = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Tutari, decimal.Zero);
                string krediTuru = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Turu, String.Empty);
                string krediTuruText = String.Empty;
                int krediTuruInt = 0;

                if (!String.IsNullOrEmpty(krediTuru) && int.TryParse(krediTuru, out krediTuruInt))
                {
                    switch (krediTuruInt)
                    {
                        case KrediTurleri.Araba: krediTuruText = "Araba"; break;
                        case KrediTurleri.Konut: krediTuruText = "Konut"; break;
                        case KrediTurleri.KrediKarti: krediTuruText = "Kredi Kartı"; break;
                        case KrediTurleri.KrediMevduat: krediTuruText = "Kredi Mevduat"; break;
                        case KrediTurleri.Tuketici: krediTuruText = "Tüketici"; break;
                        case KrediTurleri.CekKarnesi: krediTuruText = "Çek Karnesi"; break;
                    }
                }

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(anaTeklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                parser.SetVariable("$img$", System.Web.HttpContext.Current.Server.MapPath("/Content/img/"));
                parser.SetVariable("$PoliceNo$", this.GenelBilgiler.TUMPoliceNo);
                parser.SetVariable("$KrediTuru$", krediTuruText);
                parser.SetVariable("$MusteriKodu$", sigortali.MusteriKodu.ToString());
                parser.SetVariable("$AdiSoyadi$", String.Format("{0} {1}", sigortali.AdiUnvan, sigortali.SoyadiUnvan));
                parser.SetVariable("$DogumTarihi$", sigortali.DogumTarihi.Value.ToString("dd.MM.yyyy"));
                parser.SetVariable("$BabaAdi$", babaAdi);
                parser.SetVariable("$BaslangicTarih$", this.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy"));
                parser.SetVariable("$BitisTarih$", this.GenelBilgiler.BitisTarihi.ToString("dd.MM.yyyy"));
                parser.SetVariable("$Menfaattar$", "Rehin Alacaklısı EUROBANK TEKFEN A.Ş.");
                parser.SetVariable("$Teminat$", krediTutari.ToString("N2") + " TL");
                parser.SetVariable("$Prim$", this.GenelBilgiler.BrutPrim.Value.ToString("N2") + " TL");

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                string fileName = String.Format("demirkredihayat_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("kredilihayat", fileName, fileData);

                this.GenelBilgiler.PDFPolice = url;

                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                _Log.Info("PDF dokumanı oluşturuldu : {0}", url);
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

        public override System.Collections.Hashtable BilgilendirmeFormu(string formName)
        {
            TeklifSigortali sigortali = this.Sigortalilar.FirstOrDefault();
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(sigortali.MusteriKodu);
            TVMDetay tvm = _TVMContext.TVMDetayRepository.FindById(this.GenelBilgiler.TVMKodu);
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.FindById(this.GenelBilgiler.TVMKullaniciKodu);

            Hashtable ht = new Hashtable();

            ht.Add("$Today$", TurkeyDateTime.Today.ToString("dd.MM.yyyy"));
            ht.Add("$CustomerName$", String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan));
            ht.Add("$CustomerAgentName$", tvm.Unvani);
            ht.Add("$MTName$", String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi));

            return ht;
        }
    }
}
