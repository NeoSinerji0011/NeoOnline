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
    public class IsYeriTeklif : TeklifBase, IIsYeriTeklif
    {
        public IsYeriTeklif()
            : base()
        {

        }

        public IsYeriTeklif(int teklifId)
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
                    IHDIIsYeri isYeri = DependencyResolver.Current.GetService<IHDIIsYeri>();
                    isYeri.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(isYeri);
                }
            }

            return base.Hesapla(teklif);
        }

        public override void CreatePDF()
        {
            ITVMService tvm = base._TVMService;
            ITUMService tum = base._TUMService;
            IHDIIsYeri _HDIKonut = DependencyResolver.Current.GetService<IHDIIsYeri>();
            IKullaniciFotografStorage logoService = DependencyResolver.Current.GetService<IKullaniciFotografStorage>();
            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();

            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.ISYERI_KARSILASTIRMA);

                pdf = new PDFHelper("Babonline", "İŞ YERİ SİGORTASI KARŞILAŞTIRMA TABLOSU", "İŞ YERİ SİGORTASI KARŞILAŞTIRMA TABLOSU", 8, _RootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());

                PDFParser parser = new PDFParser(template, pdf);

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                string adUnvan = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                parser.SetVariable("$AdiSoyadiUnvani$", adUnvan);

                //TODO : TVM Logosu yoksa default bir icon koyulması
                TVMDetay tvmDetay = tvm.GetDetay(this.Teklif.GenelBilgiler.TVMKodu);
                // string tvmLogo = logoService.DownloadToFiles(tvmDetay.Logo);
                string tvmUnvani = tvmDetay.Unvani;

                #region Logo

                // Default Logo
                string tvmLogo = "https://neobabstoragetest.blob.core.windows.net/musteri-dokuman/176/default_logo.jpg";
                if (!String.IsNullOrEmpty(tvmDetay.Logo))
                {
                    tvmLogo = tvmDetay.Logo;
                }
                #endregion
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
                                                   .Replace("$TUMUnvani$", "İŞ YERİ SİGORTASI");
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
                string catitipi = String.Empty;
                string Bekci = String.Empty;
                string KepenkDemir = String.Empty;
                string Kamera = String.Empty;
                string boskalmasuresi = String.Empty;
                string AsansorSayisi = String.Empty;
                string EnflasyonOrani = String.Empty;
                string IstigalKonusu = String.Empty;


                //SAĞ
                string TeklifNo = String.Empty;
                string TeklifTarihi = String.Empty;
                string PostaKodu = String.Empty;
                string kattipi = String.Empty;
                string Alarm = String.Empty;
                string PasajIciUstKat = String.Empty;
                string TemperliCam = String.Empty;
                string BinaYuzOlcumu = String.Empty;
                string IsYerindeCalisanKisiSayisi = String.Empty;
                string not = String.Empty;


                //TRUE FALSE 
                Bekci = this.Teklif.ReadSoru(IsYeriSorular.Bekci, false) ? "√" : "";
                KepenkDemir = this.Teklif.ReadSoru(IsYeriSorular.KepenkVeVeyaDemir, false) ? "√" : "";
                Kamera = this.Teklif.ReadSoru(IsYeriSorular.Kamera, false) ? "√" : "";
                Alarm = this.Teklif.ReadSoru(IsYeriSorular.OzelGuvenlik_Alarm_VarMi_EH, false) ? "√" : "";
                TemperliCam = this.Teklif.ReadSoru(IsYeriSorular.TemperliCam, false) ? "√" : "";
                PasajIciUstKat = this.Teklif.ReadSoru(IsYeriSorular.PasajIciUstKatlar, false) ? "√" : "";


                //SOL
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

                //Çatı Tipi
                string cati = this.Teklif.ReadSoru(IsYeriSorular.CatiTipi, String.Empty);
                if (!String.IsNullOrEmpty(cati))
                    catitipi = CatiTipi.Tipi(Convert.ToByte(cati));

                //Boş Kalma Süresi
                string bosKalmaSuresi = this.Teklif.ReadSoru(IsYeriSorular.BosKalmaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(bosKalmaSuresi))
                    boskalmasuresi = BosKalmaSuresi.Sure(Convert.ToByte(bosKalmaSuresi));

                //Asansör Sayısı
                AsansorSayisi = this.Teklif.ReadSoru(IsYeriSorular.AsansorSayisi, String.Empty);

                //Enflasyon Oranı
                EnflasyonOrani = this.Teklif.ReadSoru(IsYeriSorular.EnflasyonOrani, String.Empty);
                if (!String.IsNullOrEmpty(EnflasyonOrani))
                    EnflasyonOrani = "% " + EnflasyonOrani;

                //İştigal Konusu
                string istigalKonusu = this.Teklif.ReadSoru(IsYeriSorular.IstigalKonusu, String.Empty);
                if (!String.IsNullOrEmpty(istigalKonusu))
                {
                    int kodu = Convert.ToInt32(istigalKonusu);
                    Istigal istigal = _CRService.GetIstigal(kodu);
                    if (istigal != null) IstigalKonusu = istigal.Aciklama;
                }


                //SAĞ

                TeklifNo = this.Teklif.GenelBilgiler.TeklifNo.ToString();
                TeklifTarihi = this.Teklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");

                //Kat Tipi
                string kat = this.Teklif.ReadSoru(IsYeriSorular.KatTipi, String.Empty);
                if (!String.IsNullOrEmpty(kat))
                    kattipi = KatTipi.Tipi(Convert.ToByte(kat));

                //Brüt alan (m2)
                BinaYuzOlcumu = this.Teklif.ReadSoru(IsYeriSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);

                //İş yerinde çalışan kişi sayısı
                IsYerindeCalisanKisiSayisi = this.Teklif.ReadSoru(IsYeriSorular.IsYerindeCalisanKisiSayisi, String.Empty);

                //Not
                if (this.Teklif.GenelBilgiler.TeklifNot != null)
                    not = this.Teklif.GenelBilgiler.TeklifNot.Aciklama;


                //SOL
                parser.SetVariable("$Il$", Il);
                parser.SetVariable("$Ilce$", Ilce);
                parser.SetVariable("$Belde$", SemtBelde);
                parser.SetVariable("$CatiTipi$", catitipi);
                parser.SetVariable("$Bekci$", Bekci);
                parser.SetVariable("$KepenkDemir$", KepenkDemir);
                parser.SetVariable("$Kamera$", Kamera);
                parser.SetVariable("$BosKalmaSuresi$", boskalmasuresi);
                parser.SetVariable("$AsansorSayisi$", AsansorSayisi);
                parser.SetVariable("$EnflasyonOrani$", EnflasyonOrani);
                parser.SetVariable("$IstigalKonusu$", IstigalKonusu);


                //SAĞ
                parser.SetVariable("$TeklifNo$", TeklifNo);
                parser.SetVariable("$TeklifTarihi$", TeklifTarihi);
                parser.SetVariable("$PostaKodu$", PostaKodu);
                parser.SetVariable("$KatTipi$", kattipi);
                parser.SetVariable("$Alarm$", Alarm);
                parser.SetVariable("$PasajIci$", PasajIciUstKat);
                parser.SetVariable("$TemperliCam$", TemperliCam);
                parser.SetVariable("$BrutAlan$", BinaYuzOlcumu);
                parser.SetVariable("$IsYerindeCalisanSayisi$", IsYerindeCalisanKisiSayisi);

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

                    //Ana Teminatlar

                    //Yangin
                    string Bina = String.Empty;
                    string Demirbas = String.Empty;
                    string Dekorasyon = String.Empty;
                    string Emtea = String.Empty;
                    string MakineVeTechizat = String.Empty;
                    string SahisMallari3 = String.Empty;
                    string KasaMuhteviyat = String.Empty;
                    string Temeller = String.Empty;

                    //Deprem
                    string DBina = String.Empty;
                    string DDemirbas = String.Empty;
                    string DDekorasyon = String.Empty;
                    string DEmtea = String.Empty;

                    //EkTeminatlar
                    string Hirsizlik = String.Empty;
                    string YazDurmasi = String.Empty;
                    string DahiliSu = String.Empty;
                    string SelSuBaskini = String.Empty;
                    string CamKrilmasi = String.Empty;
                    string Firtina = String.Empty;
                    string Duman = String.Empty;
                    string GLKHHKNHTeror = String.Empty;
                    string YerKaymasi = String.Empty;
                    string HukuksalKoruma = String.Empty;
                    string AsistanHizmeti = String.Empty;
                    string KiraKaybi = String.Empty;
                    string DegerliEsya = String.Empty;
                    string IzolasyonOlayBasi = String.Empty;
                    string Medline = String.Empty;
                    string Kapkac = String.Empty;
                    string AcilTib = String.Empty;


                    //Deprem Yanardağ Püskürme 
                    string DYP = String.Empty;
                    string DYPBina = String.Empty;
                    string DYPMuhteviyat = String.Empty;

                    //EnkazKaldirma 
                    string EKMuhteviyat = String.Empty;
                    string EKDekorasyon = String.Empty;
                    string EKBina = String.Empty;

                    //TasitCarpmasi
                    string TCHava = String.Empty;
                    string TCKara = String.Empty;

                    //MaliSorumluluk
                    string MSYangin = String.Empty;
                    string MSEkTeminat = String.Empty;
                    string MSIsVerenKisiBasinaBedeni = String.Empty;
                    string MSSahisKasaBasinaBedeni = String.Empty;
                    string MSKomsulukTeror = String.Empty;
                    string MSKiraciTeror = String.Empty;

                    // FerdiKaza
                    string FKOlum = String.Empty;
                    string FKSSakatlik = String.Empty;

                    //Elektronik Cihaz

                    string ElektronikCihaz = String.Empty;


                    #region PDF Formati Değerlerini Alıyor

                    string karsilastirmaSon = "";
                    karsilastirmaSon = karsilastirma.Replace("$PesinPrim$", String.Format("{0:N2}", teklif1.BrutPrim));


                    // ========================YANGIN ====================== //

                    // ==== Bina ==== //
                    TeklifTeminat binayanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.BinaYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.BinaYangin, false) && binayanginTEM != null &&
                        binayanginTEM.TeminatBedeli.HasValue && binayanginTEM.TeminatBedeli.Value > 0)
                        Bina = binayanginTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Demirbas ==== //
                    TeklifTeminat demirbasyanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DemirbasYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.DemirbasYangin, false) && demirbasyanginTEM != null &&
                        demirbasyanginTEM.TeminatBedeli.HasValue && demirbasyanginTEM.TeminatBedeli.Value > 0)
                        Demirbas = demirbasyanginTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Dekorasyon ==== //
                    TeklifTeminat dekorasyonyanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DekorasyonYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.DekorasyonYangin, false) && dekorasyonyanginTEM != null &&
                        dekorasyonyanginTEM.TeminatBedeli.HasValue && dekorasyonyanginTEM.TeminatBedeli.Value > 0)
                        Dekorasyon = dekorasyonyanginTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Emtea ==== //
                    TeklifTeminat emteayanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.EmteaYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.EmteaYangin, false) && emteayanginTEM != null &&
                        emteayanginTEM.TeminatBedeli.HasValue && emteayanginTEM.TeminatBedeli.Value > 0)
                        Emtea = emteayanginTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Makine ve Techizat ==== //
                    TeklifTeminat makinatechizatTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.MakinaVeTechizat).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.MakinaTechizat, false) && makinatechizatTEM != null &&
                        makinatechizatTEM.TeminatBedeli.HasValue && makinatechizatTEM.TeminatBedeli.Value > 0)
                        MakineVeTechizat = makinatechizatTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== 3.Şahıs Malları ==== //
                    TeklifTeminat sahismallariyanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.SahisMallariYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.SahisMallariYangin3, false) && sahismallariyanginTEM != null &&
                        sahismallariyanginTEM.TeminatBedeli.HasValue && sahismallariyanginTEM.TeminatBedeli.Value > 0)
                        SahisMallari3 = sahismallariyanginTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Kasa Muhteviyat ==== //
                    TeklifTeminat kasamuhteviyatTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.KasaMuhteviyatYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.KasaMuhteviyatYangin, false) && kasamuhteviyatTEM != null &&
                        kasamuhteviyatTEM.TeminatBedeli.HasValue && kasamuhteviyatTEM.TeminatBedeli.Value > 0)
                        KasaMuhteviyat = kasamuhteviyatTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Temeller ==== //
                    TeklifTeminat temellerYanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.TemellerYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.TemellerYangin, false) && temellerYanginTEM != null &&
                        temellerYanginTEM.TeminatBedeli.HasValue && temellerYanginTEM.TeminatBedeli.Value > 0)
                        Temeller = temellerYanginTEM.TeminatBedeli.Value.ToString("0.##");




                    // ======================== DEPREM ====================== //

                    // ==== Bina ==== //
                    TeklifTeminat binadepremTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.BinaDeprem).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.BinaDeprem, false) && binadepremTEM != null &&
                        binadepremTEM.TeminatBedeli.HasValue && binadepremTEM.TeminatBedeli.Value > 0)
                        DBina = binadepremTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Demirbaş ==== //
                    TeklifTeminat demirbasdepremTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DemirbasDeprem).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.DemirbasDeprem, false) && demirbasdepremTEM != null &&
                        demirbasdepremTEM.TeminatBedeli.HasValue && demirbasdepremTEM.TeminatBedeli.Value > 0)
                        DDemirbas = demirbasdepremTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Dekorasyon ==== //
                    TeklifTeminat dekorasyondepremTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DekorasyonDeprem).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.DekorasyonDeprem, false) && dekorasyondepremTEM != null &&
                        dekorasyondepremTEM.TeminatBedeli.HasValue && dekorasyondepremTEM.TeminatBedeli.Value > 0)
                        DDekorasyon = dekorasyondepremTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Emtea ==== //
                    TeklifTeminat emteadepremTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.EmteaDeprem).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.EmteaDeprem, false) && emteadepremTEM != null &&
                        emteadepremTEM.TeminatBedeli.HasValue && emteadepremTEM.TeminatBedeli.Value > 0)
                        Emtea = emteadepremTEM.TeminatBedeli.Value.ToString("0.##");




                    // EK TEMINATLAR

                    // ==== Hırsızlık ==== //
                    TeklifTeminat hirsizlikTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.Hirsizlik).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.Hirsizlik, false) && hirsizlikTEM != null &&
                        hirsizlikTEM.TeminatBedeli.HasValue && hirsizlikTEM.TeminatBedeli.Value > 0)
                        Hirsizlik = hirsizlikTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Yaz Durması ==== //
                    TeklifTeminat yazdurmasiTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.YazDurmasi).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.YazDurmasi, false) && yazdurmasiTEM != null &&
                        yazdurmasiTEM.TeminatBedeli.HasValue && yazdurmasiTEM.TeminatBedeli.Value > 0)
                        YazDurmasi = yazdurmasiTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Daili Su ==== //
                    TeklifTeminat dahilisuTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DahiliSu).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.DahiliSu, false) && dahilisuTEM != null &&
                        dahilisuTEM.TeminatBedeli.HasValue && dahilisuTEM.TeminatBedeli.Value > 0)
                        DahiliSu = dahilisuTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Sel ve Su Baskını ==== //
                    TeklifTeminat selsuTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.SelVeSuBaskini).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.SelVeSuBaskini, false) && selsuTEM != null &&
                        selsuTEM.TeminatBedeli.HasValue && selsuTEM.TeminatBedeli.Value > 0)
                        SelSuBaskini = selsuTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Cam Kırılması ==== //
                    TeklifTeminat camkirilmasiTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.CamKirilmasi).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.CamKirilmasi, false) && camkirilmasiTEM != null &&
                        camkirilmasiTEM.TeminatBedeli.HasValue && camkirilmasiTEM.TeminatBedeli.Value > 0)
                        CamKrilmasi = camkirilmasiTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Fırtına ==== //
                    TeklifTeminat firtinaTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.Firtina).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.Firtina, false) && firtinaTEM != null &&
                        firtinaTEM.TeminatBedeli.HasValue && firtinaTEM.TeminatBedeli.Value > 0)
                        Firtina = firtinaTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Duman ==== //
                    TeklifTeminat dumanTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.Duman).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.Duman, false) && dumanTEM != null &&
                        dumanTEM.TeminatBedeli.HasValue && dumanTEM.TeminatBedeli.Value > 0)
                        Duman = dumanTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== GLKHHKNH - Terör ==== //
                    TeklifTeminat grevTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.GLKHHKNHTeror).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.GLKHHKNHTeror, false) && grevTEM != null &&
                        grevTEM.TeminatBedeli.HasValue && grevTEM.TeminatBedeli.Value > 0)
                        GLKHHKNHTeror = grevTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Yer Kayması ==== //
                    TeklifTeminat yerkaymasiTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.YerKaymasi).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.YerKaymasi, false) && yerkaymasiTEM != null &&
                        yerkaymasiTEM.TeminatBedeli.HasValue && yerkaymasiTEM.TeminatBedeli.Value > 0)
                        YerKaymasi = yerkaymasiTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Hukuksal Koruma ==== //
                    TeklifTeminat hukuksalKorTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.HukuksalKoruma).FirstOrDefault();
                    if (hukuksalKorTEM != null && hukuksalKorTEM.TeminatBedeli.HasValue && hukuksalKorTEM.TeminatBedeli.Value > 0)
                    {
                        HukuksalKoruma = hukuksalKorTEM.TeminatBedeli.Value.ToString("0.##");
                        HukuksalKoruma = TutarDegistir(HukuksalKoruma);
                    }
                    else
                        HukuksalKoruma = this.Teklif.ReadSoru(IsYeriSorular.HukuksalKoruma, false) ? "√" : "";

                    // ==== Asistans Hizmeti ==== //
                    AsistanHizmeti = this.Teklif.ReadSoru(IsYeriSorular.AsistanHizmeti, false) ? "√" : "";

                    // ==== Kira Kaybı==== //
                    TeklifTeminat kirakaybiTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.KiraKaybi).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.KiraKaybi, false) && kirakaybiTEM != null &&
                        kirakaybiTEM.TeminatBedeli.HasValue && kirakaybiTEM.TeminatBedeli.Value > 0)
                        KiraKaybi = kirakaybiTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Değerli Eşya Yangın ==== //
                    TeklifTeminat degerliesyayanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DegerliEsyaYangin).FirstOrDefault();
                    if (degerliesyayanginTEM != null && degerliesyayanginTEM.TeminatBedeli.HasValue)
                        DegerliEsya = degerliesyayanginTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== İzolasyon / Olay Bş. / Yıl ==== //
                    TeklifTeminat izolasyonolaybasyilTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.IzolasOlayBsYil).FirstOrDefault();
                    if (izolasyonolaybasyilTEM != null && degerliesyayanginTEM.TeminatBedeli.HasValue)
                        IzolasyonOlayBasi = izolasyonolaybasyilTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Medline ==== //
                    Medline = this.Teklif.ReadSoru(IsYeriSorular.Medline, false) ? "√" : "";

                    // ==== Kapkaç ==== //
                    Kapkac = this.Teklif.ReadSoru(IsYeriSorular.Kapkac, false) ? "√" : "";

                    // ==== Acil Tıbbi / Hastane / Ferdi Kaza ==== //
                    AcilTib = this.Teklif.ReadSoru(IsYeriSorular.AcilTibbiHastaneFerdiKaza, false) ? "√" : "";



                    // ===================Deprem Yanardağ Püskürme ==============//

                    // ==== Bina ==== //
                    TeklifTeminat DepremyanardagBTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DepremYanardagPuskurmesiBina).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.DepremYanardagPuskurmesiBina, false) && DepremyanardagBTEM != null &&
                        DepremyanardagBTEM.TeminatBedeli.HasValue && DepremyanardagBTEM.TeminatBedeli.Value > 0)
                        DYPBina = DepremyanardagBTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Muhteviyat ==== //
                    TeklifTeminat DYPMTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.DepremYanardagPuskurmesiMuhteviyat).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.DemirbasYangin, false) && DYPMTEM != null &&
                        DYPMTEM.TeminatBedeli.HasValue && DYPMTEM.TeminatBedeli.Value > 0)
                        DYPMuhteviyat = DYPMTEM.TeminatBedeli.Value.ToString("0.##");



                    //===========================Enkaz Kaldırma====================================//

                    // ==== Muhteviyat ==== //
                    TeklifTeminat enkazkaldırmaTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.EnkazKaldirma).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.EnkazKaldirma, false) && enkazkaldırmaTEM != null &&
                        enkazkaldırmaTEM.TeminatBedeli.HasValue && enkazkaldırmaTEM.TeminatBedeli.Value > 0)
                        EKMuhteviyat = enkazkaldırmaTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Bina ==== //
                    TeklifTeminat enkazkaldırmabinaTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.EnkazKaldirmaBina).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.EnkazKaldirmaBina, false) && enkazkaldırmabinaTEM != null &&
                        enkazkaldırmabinaTEM.TeminatBedeli.HasValue && enkazkaldırmabinaTEM.TeminatBedeli.Value > 0)
                        EKBina = enkazkaldırmabinaTEM.TeminatBedeli.Value.ToString("0.##");

                    // ==== Bina ==== //
                    EKDekorasyon = "";

                    // ====================================Taşıt Çarpması ==== //


                    // Kara
                    TeklifTeminat tasitkaraTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.KaraTasitlariCarpmasi).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.KaraTasitlariCarpmasi, false) && tasitkaraTEM != null &&
                        tasitkaraTEM.TeminatBedeli.HasValue && tasitkaraTEM.TeminatBedeli.Value > 0)
                        TCKara = tasitkaraTEM.TeminatBedeli.Value.ToString("0.##");

                    // Hava
                    TeklifTeminat tasithavaTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.HavaTasitlariCarpmasi).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.HavaTasitlariCarpmasi, false) && tasithavaTEM != null &&
                        tasithavaTEM.TeminatBedeli.HasValue && tasithavaTEM.TeminatBedeli.Value > 0)
                        TCHava = tasithavaTEM.TeminatBedeli.Value.ToString("0.##");



                    // ==================================== Mali Sorumluluk ==== //

                    //  Yangın
                    TeklifTeminat msyanginTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.MaliSorumlulukYangin).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.MaliSorumlulukYangin, false) && msyanginTEM != null &&
                        msyanginTEM.TeminatBedeli.HasValue && msyanginTEM.TeminatBedeli.Value > 0)
                        MSYangin = msyanginTEM.TeminatBedeli.Value.ToString("0.##");

                    //  EK Teminat
                    TeklifTeminat msekteminatTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.MaliMesuliyetEkTeminat).FirstOrDefault();
                    if (msekteminatTEM != null && msekteminatTEM.TeminatBedeli.HasValue)
                        MSEkTeminat = msekteminatTEM.TeminatBedeli.Value.ToString("0.##");

                    //  İş Veren Kişi Başına Bedeni
                    TeklifTeminat msisverenkisibasiBedeniTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKisiBasinaBedeni3).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.SahisMaliSorumlulukKisiBasinaBedeni3, false) && msisverenkisibasiBedeniTEM != null &&
                        msisverenkisibasiBedeniTEM.TeminatBedeli.HasValue && msisverenkisibasiBedeniTEM.TeminatBedeli.Value > 0)
                        MSIsVerenKisiBasinaBedeni = msisverenkisibasiBedeniTEM.TeminatBedeli.Value.ToString("0.##");

                    //  3.Şahıs Kaza Başına Bedeni
                    TeklifTeminat sahiskazabasinabedeniTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKisiBasinaBedeni3).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.MaliSorumlulukYangin, false) && sahiskazabasinabedeniTEM != null && sahiskazabasinabedeniTEM.TeminatBedeli.HasValue)
                        MSSahisKasaBasinaBedeni = sahiskazabasinabedeniTEM.TeminatBedeli.Value.ToString("0.##");

                    //  Komşuluk Terör
                    TeklifTeminat komsulukterorTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.KomsulukMaliSorumlulukTeror).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.KomsulukMaliSorumlulukTeror, false) && komsulukterorTEM != null &&
                        komsulukterorTEM.TeminatBedeli.HasValue && komsulukterorTEM.TeminatBedeli.Value > 0)
                        MSKomsulukTeror = komsulukterorTEM.TeminatBedeli.Value.ToString("0.##");

                    //  Kiracı Terör
                    TeklifTeminat kircaiterorTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.KiraciMaliSorumlulukTeror).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.KiraciMaliSorumlulukTeror, false) && kircaiterorTEM != null &&
                        kircaiterorTEM.TeminatBedeli.HasValue && kircaiterorTEM.TeminatBedeli.Value > 0)
                        MSKiraciTeror = kircaiterorTEM.TeminatBedeli.Value.ToString("0.##");



                    //  ======================= Ferdi Kaza

                    //  Ölüm 
                    TeklifTeminat fkolumTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.FerdiKazaOlum).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.FerdiKazaOlum, false) && fkolumTEM != null &&
                        fkolumTEM.TeminatBedeli.HasValue && fkolumTEM.TeminatBedeli.Value > 0)
                        FKOlum = fkolumTEM.TeminatBedeli.Value.ToString("0.##");

                    //  Sürekli Sakatlık
                    TeklifTeminat fksureklisakatlikTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.FerdiKazaSurekliSakatlik).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.FerdiKazaSurekliSakatlik, false) && fksureklisakatlikTEM != null &&
                        fksureklisakatlikTEM.TeminatBedeli.HasValue && fksureklisakatlikTEM.TeminatBedeli.Value > 0)
                        FKSSakatlik = fksureklisakatlikTEM.TeminatBedeli.Value.ToString("0.##");



                    //Elektronik Cihaz

                    TeklifTeminat elektronikCihazTEM = teklif1.TeklifTeminats.Where(w => w.TeminatKodu == IsYeriTeminatlar.ElektronikCihazSigortasi).FirstOrDefault();
                    if (this.Teklif.ReadSoru(IsYeriSorular.ElektronikCihaz, false) && elektronikCihazTEM != null &&
                        elektronikCihazTEM.TeminatBedeli.HasValue && elektronikCihazTEM.TeminatBedeli.Value > 0)
                        ElektronikCihaz = elektronikCihazTEM.TeminatBedeli.Value.ToString("0.##");

                    Bina = TutarDegistir(Bina);
                    Demirbas = TutarDegistir(Demirbas);
                    Dekorasyon = TutarDegistir(Dekorasyon);
                    Emtea = TutarDegistir(Emtea);
                    MakineVeTechizat = TutarDegistir(MakineVeTechizat);
                    SahisMallari3 = TutarDegistir(SahisMallari3);
                    KasaMuhteviyat = TutarDegistir(KasaMuhteviyat);
                    Temeller = TutarDegistir(Temeller);

                    DBina = TutarDegistir(DBina);
                    DDemirbas = TutarDegistir(DDemirbas);
                    DDekorasyon = TutarDegistir(DDekorasyon);
                    DEmtea = TutarDegistir(DEmtea);

                    Hirsizlik = TutarDegistir(Hirsizlik);
                    YazDurmasi = TutarDegistir(YazDurmasi);
                    DahiliSu = TutarDegistir(DahiliSu);
                    SelSuBaskini = TutarDegistir(SelSuBaskini);
                    CamKrilmasi = TutarDegistir(CamKrilmasi);
                    Firtina = TutarDegistir(Firtina);
                    Duman = TutarDegistir(Duman);
                    GLKHHKNHTeror = TutarDegistir(GLKHHKNHTeror);
                    YerKaymasi = TutarDegistir(YerKaymasi);
                    KiraKaybi = TutarDegistir(KiraKaybi);

                    DegerliEsya = TutarDegistir(DegerliEsya);
                    IzolasyonOlayBasi = TutarDegistir(IzolasyonOlayBasi);

                    DYPBina = TutarDegistir(DYPBina);
                    DYPMuhteviyat = TutarDegistir(DYPMuhteviyat);

                    EKMuhteviyat = TutarDegistir(EKMuhteviyat);
                    EKDekorasyon = TutarDegistir(EKDekorasyon);
                    EKBina = TutarDegistir(EKBina);

                    TCKara = TutarDegistir(TCKara);
                    TCHava = TutarDegistir(TCHava);

                    MSYangin = TutarDegistir(MSYangin);
                    MSEkTeminat = TutarDegistir(MSEkTeminat);
                    MSIsVerenKisiBasinaBedeni = TutarDegistir(MSIsVerenKisiBasinaBedeni);
                    MSSahisKasaBasinaBedeni = TutarDegistir(MSSahisKasaBasinaBedeni);
                    MSKomsulukTeror = TutarDegistir(MSKomsulukTeror);
                    MSKiraciTeror = TutarDegistir(MSKiraciTeror);

                    FKOlum = TutarDegistir(FKOlum);
                    FKSSakatlik = TutarDegistir(FKSSakatlik);

                    ElektronikCihaz = TutarDegistir(ElektronikCihaz);

                    karsilastirmaSon = karsilastirmaSon.Replace("$Bina$", Bina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Demirbas$", Demirbas);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Dekorasyon$", Dekorasyon);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Emtea$", Emtea);
                    karsilastirmaSon = karsilastirmaSon.Replace("$MakineVeTechizat$", MakineVeTechizat);
                    karsilastirmaSon = karsilastirmaSon.Replace("$SahisMallari3$", SahisMallari3);
                    karsilastirmaSon = karsilastirmaSon.Replace("$KasaMuhteviyat$", KasaMuhteviyat);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Temeller$", Temeller);

                    karsilastirmaSon = karsilastirmaSon.Replace("$DBina$", DBina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DDemirbas$", DDemirbas);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DDekorasyon$", DDekorasyon);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DEmtea$", DEmtea);

                    karsilastirmaSon = karsilastirmaSon.Replace("$Hirsizlik$", Hirsizlik);
                    karsilastirmaSon = karsilastirmaSon.Replace("$YazDurmasi$", YazDurmasi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Dahili_Su$", DahiliSu);
                    karsilastirmaSon = karsilastirmaSon.Replace("$SelSuBaskini$", SelSuBaskini);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Cam_Kirilmasi$", CamKrilmasi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Firtina$", Firtina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Duman$", Duman);
                    karsilastirmaSon = karsilastirmaSon.Replace("$GLKHHKNH_Teror$", GLKHHKNHTeror);
                    karsilastirmaSon = karsilastirmaSon.Replace("$YerKaymasi$", YerKaymasi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma$", HukuksalKoruma);
                    karsilastirmaSon = karsilastirmaSon.Replace("$AsistanHizmeti$", AsistanHizmeti);
                    karsilastirmaSon = karsilastirmaSon.Replace("$KiraKaybi$", KiraKaybi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DegerliEsyaYangin$", DegerliEsya);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Izolasyon$", IzolasyonOlayBasi);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Medline$", Medline);
                    karsilastirmaSon = karsilastirmaSon.Replace("$Kapkac$", Kapkac);
                    karsilastirmaSon = karsilastirmaSon.Replace("$AcilTibbi$", AcilTib);

                    karsilastirmaSon = karsilastirmaSon.Replace("$DYPBina$", DYPBina);
                    karsilastirmaSon = karsilastirmaSon.Replace("$DYPMuhteviyat$", DYPMuhteviyat);

                    karsilastirmaSon = karsilastirmaSon.Replace("$EKMuhteviyat$", EKMuhteviyat);
                    karsilastirmaSon = karsilastirmaSon.Replace("$EKDekorasyon$", EKDekorasyon);
                    karsilastirmaSon = karsilastirmaSon.Replace("$EKBina$", EKBina);

                    karsilastirmaSon = karsilastirmaSon.Replace("$TCKara$", TCKara);
                    karsilastirmaSon = karsilastirmaSon.Replace("$TCHava$", TCHava);

                    karsilastirmaSon = karsilastirmaSon.Replace("$MSYangin$", MSYangin);
                    karsilastirmaSon = karsilastirmaSon.Replace("$MSEkTeminat$", MSEkTeminat);
                    karsilastirmaSon = karsilastirmaSon.Replace("$MSIsVerenBasinaBedeni$", MSIsVerenKisiBasinaBedeni);
                    karsilastirmaSon = karsilastirmaSon.Replace("$SahisBasinaKazaBedeni$", MSSahisKasaBasinaBedeni);
                    karsilastirmaSon = karsilastirmaSon.Replace("$KomsulukTeror$", MSKomsulukTeror);
                    karsilastirmaSon = karsilastirmaSon.Replace("$KiraciTeror$", MSKiraciTeror);

                    karsilastirmaSon = karsilastirmaSon.Replace("$FKOlum$", FKOlum);
                    karsilastirmaSon = karsilastirmaSon.Replace("$FKSurekliSakatlik$", FKSSakatlik);

                    karsilastirmaSon = karsilastirmaSon.Replace("$ElektronikCihaz", ElektronikCihaz);

                    #endregion

                    parser.SetColumnValues("karsilastirma", karsilastirmaSon);
                }
                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("isYeri_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("isYeri", fileName, fileData);

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
                email.SendIsYeriTeklif(this.Teklif, DigerAdSoyad, DigerEmail);
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
