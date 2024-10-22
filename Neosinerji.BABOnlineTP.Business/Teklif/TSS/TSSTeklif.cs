using System;
using System.Collections.Generic;
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
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Business.ANADOLU;
using Neosinerji.BABOnlineTP.Business.RAY;
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN;
using Neosinerji.BABOnlineTP.Business.EUREKO;
using Neosinerji.BABOnlineTP.Business.ERGO.Trafik;
using Neosinerji.BABOnlineTP.Business.AXA.Trafik;
using Neosinerji.BABOnlineTP.Business.ALLIANZ;
using Neosinerji.BABOnlineTP.Business.GROUPAMA;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.YabanciSaglik;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TSSTeklif : TeklifBase, ITSSTeklif
    {
        public TSSTeklif()
            : base()
        {

        }

        public TSSTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool turkNippon = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.TURKNIPPON) == 1;


            if (turkNippon)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    ITURKNIPPONYabanciSaglik yabanciSaglik = DependencyResolver.Current.GetService<ITURKNIPPONYabanciSaglik>();
                    yabanciSaglik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(yabanciSaglik);
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
            IAracService arac = base._AracService;
            IKullaniciFotografStorage logoService = DependencyResolver.Current.GetService<IKullaniciFotografStorage>();

            PDFHelper pdf = null;
            try
            {
                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.TRAFIK_KARSILASTIRMA);

                pdf = new PDFHelper("NeoOnline", "SAĞLIĞINIZ BİZDE HMO SİGORTASI KARŞILAŞTIRMA TABLOSU", "SAĞLIĞINIZ BİZDE HMO SİGORTASI KARŞILAŞTIRMA TABLOSU", 8, _RootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());

                PDFParser parser = new PDFParser(template, pdf);

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                string adUnvan = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                parser.SetVariable("$AdiSoyadiUnvani$", adUnvan);

                //TODO : TVM Logosu yoksa default bir icon koyulması
                TVMDetay tvmDetay = tvm.GetDetay(this.Teklif.GenelBilgiler.TVMKodu);

                #region Logo

                // Default Logo
                string tvmLogo = "https://neobabstoragetest.blob.core.windows.net/musteri-dokuman/176/default_logo.jpg";
                if (!String.IsNullOrEmpty(tvmDetay.Logo))
                {
                    tvmLogo = tvmDetay.Logo;
                }

                #endregion

                string tvmUnvani = tvmDetay.Unvani;

                parser.SetVariable("$TVMLogo$", tvmLogo);
                parser.SetVariable("$Tarih$", TurkeyDateTime.Today.ToString("dd.MM.yyyy"));
                parser.SetVariable("$TVMUnvani$", tvmUnvani);

                List<TUMDetay> tumler = new List<TUMDetay>();

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

                    TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu & w.GenelBilgiler.Basarili.Value)
                                                           .Select(f => f.GenelBilgiler).FirstOrDefault();

                    #region Logo

                    // Default Logo
                    string logo = "https://neobabstoragetest.blob.core.windows.net/musteri-dokuman/176/default_logo.jpg";
                    if (!String.IsNullOrEmpty(tumDetay.Logo))
                    {
                        logo = tumDetay.Logo;
                    }

                    #endregion

                    fiyatSatir = fiyatSatirTemplate.Replace("$TUMLogo$", logo)
                                                   .Replace("$TUMUnvani$", "TRAFİK SİGORTASI");
                    if (teklif1 != null)
                    {
                        if (teklif1.TaksitSayisi.HasValue && teklif1.TaksitSayisi.Value > 1)
                            fiyatSatir = fiyatSatir.Replace("$Fiyat1$", String.Format("{0:N2} TL ({1} Taksit)", teklif1.BrutPrim, teklif1.TaksitSayisi));
                        else
                            fiyatSatir = fiyatSatir.Replace("$Fiyat1$", String.Format("{0:N2} TL", teklif1.BrutPrim));
                    }
                    else
                    {
                        fiyatSatir = fiyatSatir.Replace("$Fiyat1$", "-");
                    }

                    //TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu &&
                    //                                                   w.GenelBilgiler.OdemePlaniAlternatifKodu == OdemePlaniAlternatifKodlari.Pesin)
                    //                                       .Select(f => f.GenelBilgiler).FirstOrDefault();

                    //string logo = logoService.DownloadToFiles(tumDetay.Logo);

                    //fiyatSatir = fiyatSatirTemplate.Replace("$TUMLogo$", logo)
                    //                               .Replace("$TUMUnvani$", "TRAFİK SİGORTASI");
                    //if (teklif1 != null)
                    //{
                    //    fiyatSatir = fiyatSatir.Replace("$Fiyat1$", String.Format("{0:N2} TL", teklif1.BrutPrim));
                    //}
                    //else
                    //{
                    //    fiyatSatir = fiyatSatir.Replace("$Fiyat1$", "-");
                    //}

                    parser.AppendToPlaceHolder("fiyatSatirlari", fiyatSatir);
                }
                #endregion

                #region Araç Bilgileri
                string plaka = String.Format("{0} {1}", this.Teklif.Arac.PlakaKodu, this.Teklif.Arac.PlakaNo);
                AracMarka aracMarka = arac.GetAracMarka(this.Teklif.Arac.Marka);
                AracTip aracTip = arac.GetAracTip(this.Teklif.Arac.Marka, this.Teklif.Arac.AracinTipi);
                string[] kullanimTarziParts = this.Teklif.Arac.KullanimTarzi.Split('-');
                AracKullanimTarzi kullanimTarzi = arac.GetAracKullanimTarzi(kullanimTarziParts[0], kullanimTarziParts[1]);

                parser.SetVariable("$Plaka$", plaka);
                parser.SetVariable("$Tarih$", this.Teklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy"));
                parser.SetVariable("$AracMarka$", aracMarka.MarkaAdi);
                parser.SetVariable("$TeklifNo$", this.Teklif.GenelBilgiler.TeklifNo.ToString());
                parser.SetVariable("$AracTip$", aracTip.TipAdi);
                parser.SetVariable("$AracModel$", this.Teklif.Arac.Model.HasValue ? this.Teklif.Arac.Model.Value.ToString() : "");
                parser.SetVariable("$AracKullanimTarzi$", kullanimTarzi.KullanimTarzi);
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
                    //TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu &&
                    //                                                   w.GenelBilgiler.OdemePlaniAlternatifKodu == OdemePlaniAlternatifKodlari.Pesin)
                    //                                       .Select(f => f.GenelBilgiler).FirstOrDefault();

                    TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu & w.GenelBilgiler.Basarili.Value)
                                                          .Select(f => f.GenelBilgiler).FirstOrDefault();

                    decimal kisiBasinaOS = 0;
                    decimal kazaBasinaOS = 0;
                    decimal kisiBasinaTG = 0;
                    decimal kazaBasinaTG = 0;
                    decimal kisiBasinaMadddi = 0;
                    decimal kazaBasinaMaddi = 0;
                    string asistans = "YOK";

                    ITeklif teklif = this.TUMTeklifler.FirstOrDefault(f => f.GenelBilgiler.TeklifId == teklif1.TeklifId);
                    var olumSakatlikKisiBasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina);
                    if (olumSakatlikKisiBasina != null && olumSakatlikKisiBasina.TeminatBedeli.HasValue)
                        kisiBasinaOS = olumSakatlikKisiBasina.TeminatBedeli.Value;

                    var olumSakatlikKazaBasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina);
                    if (olumSakatlikKazaBasina != null && olumSakatlikKazaBasina.TeminatBedeli.HasValue)
                        kazaBasinaOS = olumSakatlikKazaBasina.TeminatBedeli.Value;

                    var tedaviKisiBasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == TrafikTeminatlar.Tedavi_Kisi_Basina);
                    if (tedaviKisiBasina != null && tedaviKisiBasina.TeminatBedeli.HasValue)
                        kisiBasinaTG = tedaviKisiBasina.TeminatBedeli.Value;

                    var tedaviKazaBasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == TrafikTeminatlar.Tedavi_Kaza_Basina);
                    if (tedaviKazaBasina != null && tedaviKazaBasina.TeminatBedeli.HasValue)
                        kazaBasinaTG = tedaviKazaBasina.TeminatBedeli.Value;

                    var maddiAracBasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == TrafikTeminatlar.Maddi_Arac_Basina);
                    if (maddiAracBasina != null && maddiAracBasina.TeminatBedeli.HasValue)
                        kisiBasinaMadddi = maddiAracBasina.TeminatBedeli.Value;

                    var maddiKazaBasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == TrafikTeminatlar.Maddi_Kaza_Basina);
                    if (maddiKazaBasina != null && maddiKazaBasina.TeminatBedeli.HasValue)
                        kazaBasinaMaddi = maddiKazaBasina.TeminatBedeli.Value;

                    var asistansTeminat = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == TrafikTeminatlar.Asistans);
                    if (asistansTeminat != null && asistansTeminat.TeminatNetPrim.HasValue)
                        asistans = asistansTeminat.TeminatNetPrim.Value > 0 ? "√" : "";

                    string immText = "-";
                    string fkText = "-";


                    string kullanimTarzitext = this.Teklif.Arac.KullanimTarzi;

                    if (teklif.TUMKodu == TeklifUretimMerkezleri.HDI)
                    {
                        string immKademe = this.Teklif.ReadSoru(TrafikSorular.Teminat_IMM_Kademe, "0");
                        if (!String.IsNullOrEmpty(immKademe) && immKademe != "0")
                        {
                            var imm = _CRService.GetTrafikIMM(TeklifUretimMerkezleri.HDI, Convert.ToInt16(immKademe));
                            if (imm != null)
                            {
                                immText = imm.Text;
                            }
                        }

                        string fkKademe = this.Teklif.ReadSoru(TrafikSorular.Teminat_FK_Kademe, "0");
                        if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                        {
                            var fk = _CRService.GetTrafikFK(TeklifUretimMerkezleri.HDI, Convert.ToInt16(fkKademe));
                            if (fk != null)
                            {
                                fkText = fk.Text;
                            }
                        }
                    }

                    string karsilastirmaResult = karsilastirma.Replace("$PesinPrim$", String.Format("{0:N2}", teklif1.BrutPrim))
                                                              .Replace("$Hasarsizlik$", String.Format("% {0:N2}", teklif1.HasarsizlikIndirimYuzdesi))
                                                              .Replace("$PlakaIndirim$", "% 0")
                                                              .Replace("$GecikmeSurprimi$", String.Format("% {0:N2}", teklif1.GecikmeZammiYuzdesi))
                                                              .Replace("$KisiBasinaOS$", String.Format("{0:N2}", kisiBasinaOS))
                                                              .Replace("$KazaBasinaOS$", String.Format("{0:N2}", kazaBasinaOS))
                                                              .Replace("$KisiBasinaTG$", String.Format("{0:N2}", kisiBasinaTG))
                                                              .Replace("$KazaBasinaTG$", String.Format("{0:N2}", kazaBasinaTG))
                                                              .Replace("$KisiBasinaMaddi$", String.Format("{0:N2}", kisiBasinaMadddi))
                                                              .Replace("$KazaBasinaMaddi$", String.Format("{0:N2}", kazaBasinaMaddi))
                                                              .Replace("$IMM$", immText)
                                                              .Replace("$FK$", fkText)
                                                              .Replace("$Asistans$", asistans);

                    parser.SetColumnValues("karsilastirma", karsilastirmaResult);
                }
                #endregion

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("trafik_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("trafik", fileName, fileData);

                this.Teklif.GenelBilgiler.PDFDosyasi = url;

                _TeklifService.UpdateGenelBilgiler(this.Teklif.GenelBilgiler);

                _LogService.Info("PDF dokumanı oluşturuldu : {0}", url);
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

        public override void TSSEPostaGonder(string teklifPDF, string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();
            bool pdfDosyasiVar = !String.IsNullOrEmpty(teklifPDF);

            if (!pdfDosyasiVar)
            {
                _LogService.Error("Teklif PDF ine ulaşılamadı.");
            }

            if (pdfDosyasiVar)
                email.GenelUrunMailGonder(this, DigerAdSoyad, DigerEmail);
        }

        private static string GetClientIP()
        {
            if (System.Web.HttpContext.Current != null)
            {
                string ip = System.Web.HttpContext.Current.Request.UserHostAddress;

                if (String.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (String.IsNullOrEmpty(ip))
                        ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                return ip;
            }

            return String.Empty;
        }
    }
}

