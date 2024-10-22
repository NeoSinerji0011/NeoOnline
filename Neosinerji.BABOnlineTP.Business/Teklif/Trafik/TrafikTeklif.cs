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

namespace Neosinerji.BABOnlineTP.Business
{
    public class TrafikTeklif : TeklifBase, ITrafikTeklif
    {
        public TrafikTeklif()
            : base()
        {

        }

        public TrafikTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool hdi = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.HDI) == 1;
            bool mapfre = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.MAPFRE) == 1;
            bool anadolu = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.ANADOLU) == 1;
            bool ray = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.RAY) == 1;
            bool sompojapan = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.SOMPOJAPAN) == 1;
            bool eureko = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.EUREKO) == 1;
            bool allianz = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.ALLIANZ) == 1;
            bool ergo = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.ERGO) == 1;
            bool axa = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.AXA) == 1;
            bool groupama = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.GROUPAMA) == 1;


            if (hdi)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IHDITrafik trafik = DependencyResolver.Current.GetService<IHDITrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }

            if (mapfre)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IMAPFRETrafik trafik = DependencyResolver.Current.GetService<IMAPFRETrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }

            if (anadolu)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IANADOLUTrafik trafik = DependencyResolver.Current.GetService<IANADOLUTrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu; 
                    trafik.SetClientIPAdres(GetClientIP());
                    this.AddTeklif(trafik);
                }
            }
            if (ray)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IRAYTrafik trafik = DependencyResolver.Current.GetService<IRAYTrafik>();
                    trafik.SetClientIPAdres(GetClientIP());
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }
            if (sompojapan)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    ISOMPOJAPANTrafik trafik = DependencyResolver.Current.GetService<ISOMPOJAPANTrafik>();
                    trafik.SetClientIPAdres(GetClientIP());
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }
            if (eureko)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IEUREKOTrafik trafik = DependencyResolver.Current.GetService<IEUREKOTrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }
            if (ergo)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IERGOTrafik trafik = DependencyResolver.Current.GetService<IERGOTrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }
            if (axa)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IAXATrafik trafik = DependencyResolver.Current.GetService<IAXATrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }
            if (allianz)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IALLIANZTrafik trafik = DependencyResolver.Current.GetService<IALLIANZTrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
                }
            }
            if (groupama)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IGROUPAMATrafik trafik = DependencyResolver.Current.GetService<IGROUPAMATrafik>();
                    trafik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(trafik);
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
                TVMDetay tvmDetay = tvm.GetDetay(this.Teklif.GenelBilgiler.TVMKodu);
                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.TRAFIK_KARSILASTIRMA);

                pdf = new PDFHelper("Babonline", "TRAFİK SİGORTASI KARŞILAŞTIRMA TABLOSU", "TRAFİK SİGORTASI KARŞILAŞTIRMA TABLOSU", 8, _RootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());
                if (tvmDetay.Tipi == 11)
                {
                    template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.TRAFIK_KARSILASTIRMA_IHSAN);
                }

                PDFParser parser = new PDFParser(template, pdf);


                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                MusteriAdre mus = _MusteriService.GetAdres(this.Teklif.SigortaEttiren.MusteriKodu, this.Teklif.SigortaEttiren.SiraNo);

                string adUnvan = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                string kimlikno = musteri.KimlikNo;
                string musteriAdres = mus.Adres;
                //MusteriGenelBilgiler musterik = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                parser.SetVariable("$AdiSoyadiUnvani$", adUnvan);
                parser.SetVariable("$KimlikNo$", kimlikno);

                //TODO : TVM Logosu yoksa default bir icon koyulması
               
               
               
                #region Logo

                // Default Logo
                string tvmLogo = "https://neoonlinestrg.blob.core.windows.net/kullanici-fotograf/319f9a2b-fb2a-4c78-9e87-b56e7d40e85f..png";
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
                    string logo = "https://neoonlinestrg.blob.core.windows.net/kullanici-fotograf/319f9a2b-fb2a-4c78-9e87-b56e7d40e85f..png";
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
                if(aracTip == null)
                {
                    aracTip = new AracTip
                    {
                        TipAdi = "BILINMIYOR"
                    };
                }

                if(Teklif.Arac.KullanimTarzi == null)
                {
                    Teklif.Arac.KullanimTarzi = "111-10";
                }
                string[] kullanimTarziParts = this.Teklif.Arac.KullanimTarzi.Split('-');
                AracKullanimTarzi kullanimTarzi = arac.GetAracKullanimTarzi(kullanimTarziParts[0], kullanimTarziParts[1]);

                parser.SetVariable("$Plaka$", plaka);
                parser.SetVariable("$Tarih$", this.Teklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy"));
                parser.SetVariable("$AracMarka$", aracMarka.MarkaAdi);
                parser.SetVariable("$TeklifNo$", this.Teklif.GenelBilgiler.TeklifNo.ToString());
                parser.SetVariable("$AracTip$", aracTip.TipAdi);
                parser.SetVariable("$AracModel$", this.Teklif.Arac.Model.HasValue ? this.Teklif.Arac.Model.Value.ToString() : "");
                parser.SetVariable("$AracKullanimTarzi$", kullanimTarzi.KullanimTarzi);
                parser.SetVariable("$AracKullanimTarzi$", kullanimTarzi.KullanimTarzi);
                #endregion
                if (tvmDetay.Tipi != 11)
                {
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
                    #endregion}
                }   
                    parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                TeklifPDFStorage storage = DependencyResolver.Current.GetService<TeklifPDFStorage>();
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

        public override void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();
            bool pdfDosyasiVar = !String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFDosyasi);

            if (!pdfDosyasiVar)
            {
                try
                {
                    if (this.Teklif.GenelBilgiler.TUMKodu == 0)
                        this.CreatePDF();
                    else
                    {
                        pdfDosyasiVar = !String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFPolice);
                    }
                }
                catch (Exception ex)
                {
                    _LogService.Error(ex);
                }
            }

            if (pdfDosyasiVar)
                email.SendTrafikTeklif(this.Teklif, DigerAdSoyad, DigerEmail);
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
