using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Pdf;
using Neosinerji.BABOnlineTP.Business.CHARTIS;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Seyahat;
using Newtonsoft.Json;

namespace Neosinerji.BABOnlineTP.Business
{
    public class SeyahatSaglikTeklif : TeklifBase, ISeyahatSaglikTeklif
    {
        public SeyahatSaglikTeklif()
            : base()
        {

        }

        public SeyahatSaglikTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool chartis = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.GULF) == 1;

            if (chartis)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    ICHARTISSeyehatSaglik seyehatSaglik = DependencyResolver.Current.GetService<ICHARTISSeyehatSaglik>();
                    seyehatSaglik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(seyehatSaglik);
                }
            }

            bool nippon = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.TURKNIPPON) == 1;

            if (nippon)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
                    nipponSeyahat.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(nipponSeyahat);
                }
            }

            return base.Hesapla(teklif);
        }

        public override void Policelestir(ITeklif teklif, Odeme odeme)
        {
            teklif.Policelestir(odeme);
        }

        public override void CreatePDF()
        {
            ITVMService tvm = base._TVMService;
            ITUMService tum = base._TUMService;
            IKullaniciFotografStorage logoService = DependencyResolver.Current.GetService<IKullaniciFotografStorage>();

            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.SEYAHAT_KARSILASTIRMA);

                pdf = new PDFHelper("Babonline", "SEYAHAT SAĞLIK SİGORTASI KARŞILAŞTIRMA TABLOSU", "SEYAHAT SAĞLIK SİGORTASI KARŞILAŞTIRMA TABLOSU", 8, _RootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());

                PDFParser parser = new PDFParser(template, pdf);

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                string adUnvan = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                parser.SetVariable("$AdiSoyadiUnvani$", adUnvan);

                //TODO : TVM Logosu yoksa default bir icon koyulması
                TVMDetay tvmDetay = tvm.GetDetay(this.Teklif.GenelBilgiler.TVMKodu);
                string tvmLogo = logoService.DownloadToFiles(tvmDetay.Logo);
                string tvmUnvani = tvmDetay.Unvani;

                parser.SetVariable("$TVMLogo$", tvmLogo);
                parser.SetVariable("$Tarih$", TurkeyDateTime.Today.ToString("dd.MM.yyyy"));
                parser.SetVariable("$TVMUnvani$", tvmUnvani);

                List<TUMDetay> tumler = new List<TUMDetay>();

                #endregion

                #region Fiyat Satırları
                string fiyatSatirTemplate = parser.GetTemplate("FiyatSatiri");

                var tumKodlari = this.TUMTeklifler.Where(w => w.GenelBilgiler.Basarili.Value)
                                                  .GroupBy(g => g.GenelBilgiler.TUMKodu)
                                                  .Select(s => new { TUMKodu = s.Key });
                foreach (var tumKodu in tumKodlari)
                {
                    TUMDetay tumDetay = tum.GetDetay(tumKodu.TUMKodu);
                    tumler.Add(tumDetay);

                    string fiyatSatir = String.Empty;

                    TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu)
                                                           .Select(f => f.GenelBilgiler).FirstOrDefault();

                    string logo = logoService.DownloadToFiles(tumDetay.Logo);

                    fiyatSatir = fiyatSatirTemplate.Replace("$TUMLogo$", logo)
                                                   .Replace("$TUMUnvani$", "SEYAHAT SAĞLIK SİGORTASI");
                    if (teklif1 != null)
                    {
                        if (teklif1.TaksitSayisi.HasValue && teklif1.TaksitSayisi.Value > 1)
                            fiyatSatir = fiyatSatir.Replace("$Fiyat1$", String.Format("{0:N2} TL ({1} Taksit)", (teklif1.BrutPrim.HasValue ? (teklif1.BrutPrim.Value * KurBilgileri.Euro) : 0), teklif1.TaksitSayisi));
                        else
                            fiyatSatir = fiyatSatir.Replace("$Fiyat1$", String.Format("{0:N2} TL", (teklif1.BrutPrim.HasValue ? (teklif1.BrutPrim.Value * KurBilgileri.Euro) : 0)));
                    }
                    else
                    {
                        fiyatSatir = fiyatSatir.Replace("$Fiyat1$", "-");
                    }

                    parser.AppendToPlaceHolder("fiyatSatirlari", fiyatSatir);
                }
                #endregion

                #region Genel Bilgiler

                string ulke = String.Empty;
                string ulketipi = String.Empty;
                string basbittarihi = String.Empty;
                string kisisayisi = String.Empty;


                string seyehatBaslangicTarihi = this.Teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, DateTime.MinValue).ToString("dd.MM.yyyy");
                string seyehatBitisTarihi = this.Teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, DateTime.MinValue).ToString("dd.MM.yyyy");
                string gidilecekUlke = this.Teklif.ReadSoru(SeyehatSaglikSorular.Gidilecek_Ulke, String.Empty);
                string ulkeTipi = this.Teklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, String.Empty);
                kisisayisi = this.Teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1");

                basbittarihi = seyehatBaslangicTarihi + " " + seyehatBitisTarihi;

                switch (Convert.ToByte(this.Teklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, "0")))
                {
                    case 1: ulketipi = "Diğer"; break;
                    case 2: ulketipi = "Schengen"; break;
                }


                string ulkeKodu = this.Teklif.ReadSoru(SeyehatSaglikSorular.Gidilecek_Ulke, String.Empty);
                if (!String.IsNullOrEmpty(ulkeKodu))
                {
                    UlkeKodlari Ulke = _CRService.GetSeyehatUlkesi(ulkeKodu);
                    if (Ulke != null)
                        ulke = Ulke.UlkeAdi;
                }

                parser.SetVariable("$TeklifNo$", this.Teklif.GenelBilgiler.TeklifNo.ToString());
                parser.SetVariable("$KisiSayisi$", kisisayisi);
                parser.SetVariable("$BaslangicBitisTarihi$", basbittarihi);
                parser.SetVariable("$UlkeTipi$", ulketipi);
                parser.SetVariable("$Ulke$", ulke);

                #endregion

                #region Sigortali Satırları
                string sigortaliSatirTemplate = parser.GetTemplate("SigortaliSatiri");

                int adet = Convert.ToInt32(kisisayisi) + 1;
                int sayac = 103;
                for (int i = 1; i < adet; i++)
                {
                    SeyehatSaglikSigortalilar sigortali = new SeyehatSaglikSigortalilar();
                    string sigortaliSatir = String.Empty;


                    var sigortaliJson = this.Teklif.ReadSoru(sayac, string.Empty);
                    if (!String.IsNullOrEmpty(sigortaliJson))
                        sigortali = JsonConvert.DeserializeObject<SeyehatSaglikSigortalilar>(sigortaliJson);
                    sayac++;

                    sigortaliSatir = sigortaliSatirTemplate.Replace("$Sira$", i.ToString())
                                                           .Replace("$AdiSoyadi$", sigortali.Adi + ' ' + sigortali.Soyadi)
                                                           .Replace("$TCKN$", sigortali.KimlikNo);

                    parser.AppendToPlaceHolder("sigortaliSatirlari", sigortaliSatir);
                }
                #endregion

                #region Karşılaştırma tablosu

                int tumCount = tumKodlari.Count();

                if (tumCount == 0)
                    return;

                int columnWidth = 350 / tumCount;
                int columnCount = tumCount + 1;
                int[] columns = new int[columnCount];
                columns[0] = 150;
                for (int i = 1; i < columnCount; i++)
                {
                    columns[i] = columnWidth;
                }
                parser.SetColumns("karsilastirmaBaslikColumns", columns);

                string karsilastirmaBaslikTemplate = parser.GetTemplate("karsilastirmaBaslik");

                foreach (var tumKodu in tumKodlari)
                {
                    TUMDetay detay = tumler.FirstOrDefault(f => f.Kodu == tumKodu.TUMKodu);
                    parser.AppendToPlaceHolder("karsilastirmaBaslik", karsilastirmaBaslikTemplate.Replace("$TUMUnvani$", detay.Unvani));
                }

                parser.SetColumns("karsilastirmaColumns", columns);

                string karsilastirma = parser.GetTemplate("karsilastirma");
                foreach (var tumKodu in tumKodlari)
                {
                    TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu)
                                                           .Select(f => f.GenelBilgiler).FirstOrDefault();

                    if (teklif1 == null)
                        return;

                    #region PDF Formati Değerlerini Alıyor

                    string karsilastirmaSon = "";
                    karsilastirmaSon = karsilastirma.Replace("$PesinPrim$", String.Format("{0:N2}", teklif1.BrutPrim.HasValue ? teklif1.BrutPrim.Value * KurBilgileri.Euro : 0, 00));

                    TeklifTeminat seyehatTeminati = teklif1.TeklifTeminats.Where(s => s.TeminatKodu == SeyehatSaglikTeminatlar.SeyehatSaglik).FirstOrDefault();
                    if (seyehatTeminati != null)
                    {
                        if (seyehatTeminati.TeminatBedeli.HasValue)
                            karsilastirmaSon = karsilastirmaSon.Replace("$SeyahatTeminati$", String.Format("{0:N2}", seyehatTeminati.TeminatBedeli.HasValue ? seyehatTeminati.TeminatBedeli.Value : 0, 00));
                    }

                    if (this.Teklif.ReadSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, false))
                        karsilastirmaSon = karsilastirmaSon.Replace("$KayakTeminati$", "√");
                    else
                        karsilastirmaSon = karsilastirmaSon.Replace("$KayakTeminati$", "");
                    #endregion

                    parser.SetColumnValues("karsilastirma", karsilastirmaSon);
                }
                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("dask_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("dask", fileName, fileData);

                this.Teklif.GenelBilgiler.PDFDosyasi = url;

                _TeklifService.UpdateGenelBilgiler(this.Teklif.GenelBilgiler);

                _LogService.Info("PDF dokumanı oluşturuldu : {0}", url);

                #endregion

            }
            catch (Exception ex)
            {
                _LogService.Error("PDF Oluşturlamadı.");
                _LogService.Error(ex);
                throw;
            }
            finally
            {
                if (pdf != null)
                    pdf.Dispose();
            }
        }

        public void CreatePolicePDF(ITeklif teklif)
        {
            ITVMService tvm = DependencyResolver.Current.GetService<ITVMService>();
            ITUMService tum = DependencyResolver.Current.GetService<ITUMService>();
            IUlkeService ulkeService = DependencyResolver.Current.GetService<IUlkeService>();
            ITeklif seyahatTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.SEYAHAT_POLICE);

                pdf = new PDFHelper("Babonline", "TESABİTPİRİMLİ SİGORTASI POLİÇESİ", "TESABİTPİRİMLİ SİGORTASI POLİÇESİ", 8, _RootPath + "Content/fonts/");
                PDFParser parser = new PDFParser(template, pdf);

                #endregion

                #region Data Fill

                #region Seyahat Poliçesi Bilgileri

                string policeNo = String.Empty;
                string tanzimTarihi = String.Empty;
                string paraBirimi = String.Empty;
                string dovizKuru = String.Empty;
                string baslangicTarihi = String.Empty;
                string sure = String.Empty;
                string bitisTarihi = String.Empty;
                string SIletisimAdresi = String.Empty;
                string acente = String.Empty;
                string SAdiSoyadi = String.Empty;
                string STCKN = String.Empty;
                string cinsiyet = String.Empty;
                string dogumTarihi = String.Empty;
                string pasaportNo = String.Empty;
                string acenteLevhaNo = String.Empty;
                string toplamTl = String.Empty;
                string toplamDoviz = String.Empty;
                string teminatBolgesi = String.Empty;
                string ustLimit = String.Empty;
                string teminatCinsi = String.Empty;

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(this.Teklif.GenelBilgiler.TeklifNo, this.Teklif.GenelBilgiler.TVMKodu);

                switch (Convert.ToByte(anaTeklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, "0")))
                {
                    case UlkeTipleri.Diger:
                        teminatBolgesi = "Diğer Ülkeler";
                        teminatCinsi = "Travel Guard Seyahat Sigortası";
                        break;
                    case UlkeTipleri.Schengen:
                        teminatBolgesi = "Vize / Avrupa / Europe (Schengen, Avrupa Ülkeleri)";
                        teminatCinsi = "SEYAHAT SAĞLIK / TRAVEL HEALTH INSURANCE BENEFIT";
                        ustLimit = "30.000,00 EUR";
                        break;
                }

                toplamTl = teklif.GenelBilgiler.BrutPrim.ToString();

                policeNo = teklif.GenelBilgiler.TUMPoliceNo;
                tanzimTarihi = teklif.GenelBilgiler.TanzimTarihi.ToString("dd/MM/yyyy");
                DateTime BasTarihi = anaTeklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, DateTime.MinValue);
                DateTime BitTarihi = anaTeklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, DateTime.MinValue);
                SAdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                STCKN = musteri.KimlikNo;
                cinsiyet = musteri.Cinsiyet;
                dogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty;
                pasaportNo = musteri.PasaportNo;
                baslangicTarihi = BasTarihi.ToString("dd.MM.yyyy");
                bitisTarihi = BitTarihi.ToString("dd.MM.yyyy");

                TimeSpan fark = BitTarihi - BasTarihi;
                sure = fark.Days.ToString();

                paraBirimi = "EURO";
                dovizKuru = KurBilgileri.Euro.ToString();
                toplamDoviz = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value.ToString() : String.Empty;
                toplamTl = teklif.GenelBilgiler.BrutPrim.HasValue ? (teklif.GenelBilgiler.BrutPrim.Value * KurBilgileri.Euro).ToString() : String.Empty;


                TVMDetay tvmDetay = tvm.GetDetay(teklif.GenelBilgiler.TVMKodu);
                if (tvmDetay != null)
                {
                    acente = tvmDetay.Unvani;
                }

                MusteriAdre musteriAdres = musteri.MusteriAdres.FirstOrDefault();
                if (musteriAdres != null)
                {
                    if (!String.IsNullOrEmpty(musteriAdres.Mahalle))
                        SIletisimAdresi += musteriAdres.Mahalle + " MAH. ";
                    if (!String.IsNullOrEmpty(musteriAdres.Cadde))
                        SIletisimAdresi += musteriAdres.Cadde + " CAD. ";
                    if (!String.IsNullOrEmpty(musteriAdres.Apartman))
                        SIletisimAdresi += musteriAdres.Apartman + " APT. ";
                    if (!String.IsNullOrEmpty(musteriAdres.BinaNo))
                        SIletisimAdresi += " NO : " + musteriAdres.BinaNo;
                    if (!String.IsNullOrEmpty(musteriAdres.DaireNo))
                        SIletisimAdresi += " DAİRE : " + musteriAdres.DaireNo;
                }

                parser.SetVariable("$PolicyNo$", policeNo);
                parser.SetVariable("$TanzimTarihiPropsatDate$", tanzimTarihi);
                parser.SetVariable("$BaslamaTarihiEffectiveDate$", baslangicTarihi);
                parser.SetVariable("$BitisTarihiEffectiveDate$", bitisTarihi);
                parser.SetVariable("$AdresiInsuredsAddress$", SIletisimAdresi);
                parser.SetVariable("$Acente$", acente);
                parser.SetVariable("$SigortalininAdiSoyadi$", SAdiSoyadi);
                parser.SetVariable("$TCKimlikNoTRIdentityID$", STCKN);
                parser.SetVariable("$Cinsiyet$", cinsiyet);
                parser.SetVariable("$DogumTarihiBirthdate$", dogumTarihi);
                parser.SetVariable("$PasaportNoPassportNo$", pasaportNo);
                parser.SetVariable("$DovizKuruExchangeRate$", dovizKuru);
                parser.SetVariable("$ParaBirimiCurrency$", paraBirimi);
                parser.SetVariable("$SuresiDuration$", sure);
                parser.SetVariable("$AcenteLevhaNoAgencyRegisterNo$", acenteLevhaNo);
                parser.SetVariable("$DövizPrimiPremiumInForeignCurrency$", toplamDoviz);
                parser.SetVariable("$TLPrimiPremiumInTL$", toplamTl);
                parser.SetVariable("$TeminatBolgesiCoverageRegion$", teminatBolgesi);
                parser.SetVariable("$UstLimit$", ustLimit);
                parser.SetVariable("$TeminatCinsi$", teminatCinsi);

                #endregion

                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("seyehat_Police{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("seyehat", fileName, fileData);

                teklif.GenelBilgiler.PDFPolice = url;

                _TeklifService.UpdateGenelBilgiler(teklif.GenelBilgiler);

                _LogService.Info("PDF dokumanı oluşturuldu : {0}", url);

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error("PDF Oluşturlamadı.");
                _LogService.Error(ex);
                throw;
            }
            finally
            {
                if (pdf != null)
                    pdf.Dispose();
            }
        }

        public override void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();
            bool pdfDosyasiVar = false;
            if (this.Teklif.GenelBilgiler.TUMKodu == 0)
                pdfDosyasiVar = !String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFDosyasi);
            else
                pdfDosyasiVar = !String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFPolice);

            if (!pdfDosyasiVar)
            {
                try
                {
                    if (this.Teklif.GenelBilgiler.TUMKodu == 0)
                    {
                        this.CreatePDF();
                        pdfDosyasiVar = true;
                    }
                }
                catch (Exception ex)
                {
                    _LogService.Error(ex);
                }
            }
            if (pdfDosyasiVar)
            {
                email.SendSeyahatSaglikTeklif(this.Teklif, DigerAdSoyad, DigerEmail);
            }
        }
    }
}
