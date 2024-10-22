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
using Newtonsoft.Json;
using Neosinerji.BABOnlineTP.Business.HDI;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KonutTeklif : TeklifBase, IKonutTeklif
    {
        public KonutTeklif()
            : base()
        {

        }

        public KonutTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool hdi = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.HDI) == 1;

            if (hdi)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IHDIKonut konut = DependencyResolver.Current.GetService<IHDIKonut>();
                    konut.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(konut);
                }
            }

            return base.Hesapla(teklif);
        }

        public override void CreatePDF()
        {
            ITVMService tvm = base._TVMService;
            ITUMService tum = base._TUMService;
            IHDIKonut _HDIKonut = DependencyResolver.Current.GetService<IHDIKonut>();
            IKullaniciFotografStorage logoService = DependencyResolver.Current.GetService<IKullaniciFotografStorage>();
            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();

            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.KONUT_KARSILASTIRMA);

                pdf = new PDFHelper("Babonline", "KONUT SİGORTASI KARŞILAŞTIRMA TABLOSU", "KONUT SİGORTASI KARŞILAŞTIRMA TABLOSU", 8, _RootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());

                PDFParser parser = new PDFParser(template, pdf);

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                string adUnvan = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                parser.SetVariable("$AdiSoyadiUnvani$", adUnvan);

                //TODO : TVM Logosu yoksa default bir icon koyulması
                TVMDetay tvmDetay = tvm.GetDetay(this.Teklif.GenelBilgiler.TVMKodu);
                //string tvmLogo = logoService.DownloadToFiles(tvmDetay.Logo);
                string tvmUnvani = tvmDetay.Unvani;

                #region Logo

                // Default Logo
                string tvmLogo = "https://neobabstoragetest.blob.core.windows.net/musteri-dokuman/176/default_logo.jpg";
                if (!String.IsNullOrEmpty(tvmDetay.Logo))
                {
                    tvmLogo = tvmDetay.Logo;
                }

                parser.SetVariable("$TVMLogo$", tvmLogo);
                parser.SetVariable("$Tarih$", TurkeyDateTime.Today.ToString("dd.MM.yyyy"));
                parser.SetVariable("$TVMUnvani$", tvmUnvani);

                #endregion


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
                                                   .Replace("$TUMUnvani$", "KONUT SİGORTASI");
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

                    parser.AppendToPlaceHolder("fiyatSatirlari", fiyatSatir);
                }
                #endregion

                #region Genel Bilgiler

                //SOL
                string Il = String.Empty;
                string Ilce = String.Empty;
                string SemtBelde = String.Empty;
                string CelikKapi = String.Empty;
                string DemirParmaklik = String.Empty;
                string OzelGuvenlikAlarm = String.Empty;
                string KislikMi = String.Empty;
                string DaskPoliceBinaBedeli = String.Empty;
                string YillikEnflasyonKorumaOrani = String.Empty;
                string EnflasyonOrani = String.Empty;
                string HasarsizlikIndirimOrani = String.Empty;


                //SAĞ
                string TeklifNo = String.Empty;
                string TeklifTarihi = String.Empty;
                string PostaKodu = String.Empty;
                string KatNo = String.Empty;
                string BinaYuzOlcumu = String.Empty;
                string BosKalmaSuresi = String.Empty;
                string RehinAlacakliVarmi = String.Empty;


                string not = String.Empty;

                TeklifRizikoAdresi adres = this.Teklif.RizikoAdresi;
                if (adres != null)
                {
                    Il il = _UlkeService.GetIl("TUR", adres.IlKodu.HasValue ? adres.IlKodu.Value.ToString() : "");
                    if (il != null)
                        Il = il.IlAdi;
                    Ilce ilce = _UlkeService.GetIlce(adres.IlceKodu.HasValue ? adres.IlceKodu.Value : 999);
                    if (ilce != null)
                        Ilce = ilce.IlceAdi;

                    SemtBelde = adres.SemtBelde;
                    PostaKodu = adres.PostaKodu.HasValue ? adres.PostaKodu.Value.ToString() : "";
                }

                if (this.Teklif.ReadSoru(KonutSorular.Celik_Kapı_VarMi_EH, false))
                    CelikKapi = "√";

                if (this.Teklif.ReadSoru(KonutSorular.OzelGuvenlik_Alarm_VarMi_EH, false))
                    OzelGuvenlikAlarm = "√";

                if (this.Teklif.ReadSoru(KonutSorular.DemirPArmaklik_VarMi_EH, false))
                    DemirParmaklik = "√";

                if (this.Teklif.ReadSoru(KonutSorular.KislikMi, false))
                    KislikMi = "√";

                string binaBedeli = this.Teklif.ReadSoru(KonutSorular.Dask_Sigorta_Bedeli, "0");
                if (binaBedeli != "0")
                    DaskPoliceBinaBedeli = binaBedeli;

                YillikEnflasyonKorumaOrani = this.Teklif.ReadSoru(KonutSorular.YillikEnflasyonundan_Koruma_Orani, String.Empty);
                EnflasyonOrani = this.Teklif.ReadSoru(KonutSorular.EnflasyonOrani, String.Empty);
                HasarsizlikIndirimOrani = this.Teklif.ReadSoru(KonutSorular.HasarsizlikIndirimOrani, String.Empty);

                if (!String.IsNullOrEmpty(YillikEnflasyonKorumaOrani))
                    YillikEnflasyonKorumaOrani = "% " + YillikEnflasyonKorumaOrani;

                if (!String.IsNullOrEmpty(EnflasyonOrani))
                    EnflasyonOrani = "% " + EnflasyonOrani;

                if (!String.IsNullOrEmpty(HasarsizlikIndirimOrani))
                    HasarsizlikIndirimOrani = "% " + HasarsizlikIndirimOrani;

                TeklifNo = this.Teklif.GenelBilgiler.TeklifNo.ToString();
                TeklifTarihi = this.Teklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");
                KatNo = this.Teklif.ReadSoru(KonutSorular.SigortalanacakYer_Kacinci_Katta, String.Empty);
                BinaYuzOlcumu = this.Teklif.ReadSoru(KonutSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                BosKalmaSuresi = this.Teklif.ReadSoru(KonutSorular.BosKalmaSuresi, String.Empty);


                if (this.Teklif.ReadSoru(KonutSorular.RA_Dain_i_Muhtehin_VarYok, false))
                    RehinAlacakliVarmi = "√";

                if (this.Teklif.GenelBilgiler.TeklifNot != null)
                    not = this.Teklif.GenelBilgiler.TeklifNot.Aciklama;

                if (!String.IsNullOrEmpty(DaskPoliceBinaBedeli))
                    DaskPoliceBinaBedeli = TutarDegistir(DaskPoliceBinaBedeli);


                //SOL
                parser.SetVariable("$Il$", Il);
                parser.SetVariable("$Ilce$", Ilce);
                parser.SetVariable("$Belde$", SemtBelde);
                parser.SetVariable("$CelikKapi$", CelikKapi);
                parser.SetVariable("$DemirParmaklik$", DemirParmaklik);
                parser.SetVariable("$Alarm$", OzelGuvenlikAlarm);
                parser.SetVariable("$DaskPoliceBinaBedeli$", DaskPoliceBinaBedeli);
                parser.SetVariable("$YillikEnflasyon$", YillikEnflasyonKorumaOrani);
                parser.SetVariable("$EnflasyonOrani$", EnflasyonOrani);
                parser.SetVariable("$HasarsizlikIndirimOrani$", HasarsizlikIndirimOrani);


                //SAG
                parser.SetVariable("$TeklifNo$", TeklifNo);
                parser.SetVariable("$TeklifTarihi$", TeklifTarihi);
                parser.SetVariable("$PostaKodu$", PostaKodu);
                parser.SetVariable("$KatNo$", KatNo);
                parser.SetVariable("$YuzOlcumu$", BinaYuzOlcumu);
                parser.SetVariable("$BosKalma$", BosKalmaSuresi);
                parser.SetVariable("$RehinAlacakli$", RehinAlacakliVarmi);
                parser.SetVariable("$Kislik$", KislikMi);

                //EXTRA
                parser.SetVariable("$Not$", not);

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

                    string YanginBina = String.Empty;
                    string YanginEsya = String.Empty;
                    string DepremBina = String.Empty;
                    string DepremEsya = String.Empty;

                    string Hirsizlik = String.Empty;
                    string DahiliSu = String.Empty;
                    string SelSuBaskini = String.Empty;
                    string CamKrilmasi = String.Empty;
                    string Firtina = String.Empty;
                    string Duman = String.Empty;
                    string Grev = String.Empty;
                    string YerKaymasi = String.Empty;
                    string TemellerYangin = String.Empty;
                    string HukuksalKoruma = String.Empty;
                    string AsistanHizmeti = String.Empty;
                    string KiraKaybi = String.Empty;
                    string DegerliEsya = String.Empty;
                    string IzolasyonOlayBasi = String.Empty;
                    string Medline = String.Empty;
                    string Kapkac = String.Empty;
                    string AcilTib = String.Empty;

                    string DepremYanardagPuskurtme = String.Empty;
                    string DepremYanardagPuskurtmeBina = String.Empty;
                    string DepremYanardagPuskurtmeEsya = String.Empty;

                    string EnkazKaldirma = String.Empty;
                    string EnkazKaldirmaBina = String.Empty;
                    string EnkazKaldirmaEsya = String.Empty;

                    string TasitCarpmasiHava = String.Empty;
                    string TasitCarpmasiKara = String.Empty;

                    string MaliSorumlulukYangin = String.Empty;
                    string MaliSorumlulukEkTeminat = String.Empty;

                    string FerdiKaza = String.Empty;
                    string FKOlum = String.Empty;
                    string FKSSakatlik = String.Empty;

                    #region PDF Formati Değerlerini Alıyor

                    string karsilastirmaSon = "";
                    karsilastirmaSon = karsilastirma.Replace("$PesinPrim$", String.Format("{0:N2} TL", teklif1.BrutPrim));

                    int esyabedeli = Convert.ToInt32(this.Teklif.ReadSoru(KonutSorular.EsyaBedeli, "0"));
                    int binabedeli = Convert.ToInt32(this.Teklif.ReadSoru(KonutSorular.BinaBedeli, "0"));

                    YanginBina = binabedeli > 0 ? binabedeli.ToString() : "";
                    DepremBina = binabedeli > 0 ? binabedeli.ToString() : "";
                    YanginEsya = esyabedeli.ToString();
                    DepremEsya = esyabedeli.ToString();


                    if (this.Teklif.ReadSoru(KonutSorular.Yururlukte_dask_policesi_VarYok, false))
                    {
                        int daskBedel = Convert.ToInt32(this.Teklif.ReadSoru(KonutSorular.Dask_Sigorta_Bedeli, "0"));
                        DepremBina = (binabedeli - daskBedel).ToString();
                    }


                    Hirsizlik = esyabedeli.ToString();
                    DahiliSu = (esyabedeli + binabedeli).ToString();
                    SelSuBaskini = (esyabedeli + binabedeli).ToString();
                    Firtina = (esyabedeli + binabedeli).ToString();
                    Duman = (esyabedeli + binabedeli).ToString();
                    Grev = (esyabedeli + binabedeli).ToString();
                    YerKaymasi = (esyabedeli + binabedeli).ToString();
                    TemellerYangin = binabedeli == 0 ? "" : binabedeli.ToString();
                    DepremYanardagPuskurtme = (esyabedeli + binabedeli).ToString();
                    DepremYanardagPuskurtmeBina = binabedeli == 0 ? "" : binabedeli.ToString();
                    DepremYanardagPuskurtmeEsya = esyabedeli.ToString();
                    EnkazKaldirma = (esyabedeli + binabedeli).ToString();
                    EnkazKaldirmaBina = binabedeli == 0 ? "" : binabedeli.ToString();
                    EnkazKaldirmaEsya = esyabedeli.ToString();
                    TasitCarpmasiHava = (esyabedeli + binabedeli).ToString();
                    TasitCarpmasiKara = (esyabedeli + binabedeli).ToString();


                    AsistanHizmeti = this.Teklif.ReadSoru(KonutSorular.AsistanHizmeti, false) ? "√" : "";
                    HukuksalKoruma = this.Teklif.ReadSoru(KonutSorular.HukuksalKoruma, false) ? "√" : "";
                    Medline = this.Teklif.ReadSoru(KonutSorular.Medline, false) ? "√" : "";
                    AcilTib = this.Teklif.ReadSoru(KonutSorular.AcilTibbiHastaneFerdiKaza, false) ? "√" : "";
                    FerdiKaza = this.Teklif.ReadSoru(KonutSorular.FerdiKaza, false) ? "√" : "";
                    FKOlum = this.Teklif.ReadSoru(KonutSorular.FerdiKazaOlum, false) ? "√" : "";
                    FKSSakatlik = this.Teklif.ReadSoru(KonutSorular.FerdiKazaSurekliSakatlik, false) ? "√" : "";

                    //Cam Krilmasi
                    if (this.Teklif.ReadSoru(KonutSorular.CamKirilmasi, false))
                    {
                        TeklifTeminat camkrilmasiTeminati = this.Teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.CamKirilmasi).FirstOrDefault();
                        if (camkrilmasiTeminati != null)
                            CamKrilmasi = camkrilmasiTeminati.TeminatBedeli.HasValue ? camkrilmasiTeminati.TeminatBedeli.Value.ToString("0.##") : "";
                    }


                    //Kapkac
                    if (this.Teklif.ReadSoru(KonutSorular.Kapkac, false))
                    {
                        TeklifTeminat kapkacTeminati = this.Teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.Kapkac).FirstOrDefault();
                        if (kapkacTeminati != null)
                            Kapkac = kapkacTeminati.TeminatBedeli.HasValue ? kapkacTeminati.TeminatBedeli.Value.ToString("0.##") : "";
                    }
                    //Mali Mesuliyet Yangin
                    if (this.Teklif.ReadSoru(KonutSorular.MaliMesuliyetYangin, false))
                    {
                        TeklifTeminat maliMesuliyetTeminati = this.Teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetYangin).FirstOrDefault();
                        if (maliMesuliyetTeminati != null)
                            MaliSorumlulukYangin = maliMesuliyetTeminati.TeminatBedeli.HasValue ? maliMesuliyetTeminati.TeminatBedeli.Value.ToString("0.##") : "";
                    }


                    //Kira Kaybı
                    if (this.Teklif.ReadSoru(KonutSorular.KiraKaybi, false))
                    {
                        TeklifTeminat kirakaybiTeminati = this.Teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.KiraKaybi).FirstOrDefault();
                        if (kirakaybiTeminati != null)
                            KiraKaybi = kirakaybiTeminati.TeminatBedeli.HasValue ? kirakaybiTeminati.TeminatBedeli.Value.ToString("0.##") : "";
                    }

                    //Değerli Eşya
                    if (this.Teklif.ReadSoru(KonutSorular.DegerliEsyaYangin, false))
                    {
                        TeklifTeminat degerliesyaTeminati = this.Teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.DegerliEsyaYangin).FirstOrDefault();
                        if (degerliesyaTeminati != null)
                            DegerliEsya = degerliesyaTeminati.TeminatBedeli.HasValue ? degerliesyaTeminati.TeminatBedeli.Value.ToString("0.##") : "";
                    }

                    //İzolasyon olay bası
                    if (this.Teklif.ReadSoru(KonutSorular.IzolasOlayBsYil, false))
                    {
                        TeklifTeminat izolasyonTeminati = this.Teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.IzolasOlayBsYil).FirstOrDefault();
                        if (izolasyonTeminati != null)
                            IzolasyonOlayBasi = izolasyonTeminati.TeminatBedeli.HasValue ? izolasyonTeminati.TeminatBedeli.Value.ToString("0.##") : "";
                    }

                    //Mali Sorumluluk ek teminat
                    if (this.Teklif.ReadSoru(KonutSorular.MaliMesuliyetEkTeminat, false))
                    {
                        TeklifTeminat malisorumlulukTeminati = this.Teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetEkTeminat).FirstOrDefault();
                        if (malisorumlulukTeminati != null)
                            MaliSorumlulukEkTeminat = malisorumlulukTeminati.TeminatBedeli.HasValue ? malisorumlulukTeminati.TeminatBedeli.Value.ToString("0.##") : "";
                    }

                    //Tutarlara virgul ekleniyor.
                    YanginBina = TutarDegistir(YanginBina);
                    YanginEsya = TutarDegistir(YanginEsya);
                    DepremBina = TutarDegistir(DepremBina);
                    DepremEsya = TutarDegistir(DepremEsya);

                    Hirsizlik = TutarDegistir(Hirsizlik);
                    DahiliSu = TutarDegistir(DahiliSu);
                    SelSuBaskini = TutarDegistir(SelSuBaskini);
                    CamKrilmasi = TutarDegistir(CamKrilmasi);
                    Kapkac = TutarDegistir(Kapkac);
                    MaliSorumlulukYangin = TutarDegistir(MaliSorumlulukYangin);
                    Firtina = TutarDegistir(Firtina);
                    Duman = TutarDegistir(Duman);
                    Grev = TutarDegistir(Grev);
                    YerKaymasi = TutarDegistir(YerKaymasi);
                    TemellerYangin = TutarDegistir(TemellerYangin);
                    DepremYanardagPuskurtme = TutarDegistir(DepremYanardagPuskurtme);
                    DepremYanardagPuskurtmeBina = TutarDegistir(DepremYanardagPuskurtmeBina);
                    DepremYanardagPuskurtmeEsya = TutarDegistir(DepremYanardagPuskurtmeEsya);
                    EnkazKaldirma = TutarDegistir(EnkazKaldirma);
                    EnkazKaldirmaBina = TutarDegistir(EnkazKaldirmaBina);
                    EnkazKaldirmaEsya = TutarDegistir(EnkazKaldirmaEsya);
                    TasitCarpmasiKara = TutarDegistir(TasitCarpmasiKara);
                    TasitCarpmasiHava = TutarDegistir(TasitCarpmasiHava);
                    MaliSorumlulukEkTeminat = TutarDegistir(MaliSorumlulukEkTeminat);
                    KiraKaybi = TutarDegistir(KiraKaybi);
                    IzolasyonOlayBasi = TutarDegistir(IzolasyonOlayBasi);



                    karsilastirmaSon = karsilastirmaSon.Replace("$YBina$", YanginBina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$YEsya$", YanginEsya);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DBina$", DepremBina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DEsya$", DepremEsya);


                    karsilastirmaSon = karsilastirmaSon.Replace("$Hirsizlik$", Hirsizlik);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Dahili_Su$", DahiliSu);
                    karsilastirmaSon = karsilastirmaSon.Replace("$SelSuBaskini$", SelSuBaskini);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Cam_Kirilmasi$", CamKrilmasi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Firtina$", Firtina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Duman$", Duman);
                    karsilastirmaSon = karsilastirmaSon.Replace("$GLKHHKNH_Teror$", Grev);
                    karsilastirmaSon = karsilastirmaSon.Replace("$YerKaymasi$", YerKaymasi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$TemellerYangin$", TemellerYangin);
                    karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma$", HukuksalKoruma);
                    karsilastirmaSon = karsilastirmaSon.Replace("$AsistanHizmeti$", AsistanHizmeti);
                    karsilastirmaSon = karsilastirmaSon.Replace("$KiraKaybi$", KiraKaybi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DegerliEsyaYangin$", DegerliEsya);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Izolasyon$", IzolasyonOlayBasi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Medline$", Medline);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Kapkac$", Kapkac);
                    karsilastirmaSon = karsilastirmaSon.Replace("$AcilTibbi$", AcilTib);

                    karsilastirmaSon = karsilastirmaSon.Replace("$DYP$", DepremYanardagPuskurtme);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DYPBina$", DepremYanardagPuskurtmeBina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DYPEsya$", DepremYanardagPuskurtmeEsya);

                    karsilastirmaSon = karsilastirmaSon.Replace("$EK$", EnkazKaldirma);
                    karsilastirmaSon = karsilastirmaSon.Replace("$EKBina$", EnkazKaldirmaBina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$EKEsya$", EnkazKaldirmaEsya);

                    karsilastirmaSon = karsilastirmaSon.Replace("$TCKara$", TasitCarpmasiKara);
                    karsilastirmaSon = karsilastirmaSon.Replace("$TCHava$", TasitCarpmasiHava);

                    karsilastirmaSon = karsilastirmaSon.Replace("$MSYangin$", MaliSorumlulukYangin);
                    karsilastirmaSon = karsilastirmaSon.Replace("$MSEkTeminat$", MaliSorumlulukEkTeminat);

                    karsilastirmaSon = karsilastirmaSon.Replace("$FK$", FerdiKaza);
                    karsilastirmaSon = karsilastirmaSon.Replace("$FKOlum$", FKOlum);
                    karsilastirmaSon = karsilastirmaSon.Replace("$FKSurekliSakatlik$", FKSSakatlik);

                    #endregion

                    parser.SetColumnValues("karsilastirma", karsilastirmaSon);
                }
                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("konut_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("konut", fileName, fileData);

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

        public override void Policelestir(ITeklif teklif, Odeme odeme)
        {
            //base.Policelestir(teklif, odeme);
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
                email.SendKonutTeklif(this.Teklif, DigerAdSoyad, DigerEmail);
        }

        private static string TutarDegistir(string eskiTutar)
        {
            string result = "";

            int length = eskiTutar.Length;
            int sayac = 0;
            if (length > 0)
            {
                if (length > 3 && length < 8)
                {
                    for (int i = 0; i < length; i++)
                    {
                        sayac++;
                        result += eskiTutar[i];
                        if (sayac == (length - 3) || sayac == (length - 6))
                            result += ",";
                    }
                    result = result + " TL";
                }
                else if (length > 0 && length < 4) result = eskiTutar + " TL";
            }
            return result;
        }
    }
}
