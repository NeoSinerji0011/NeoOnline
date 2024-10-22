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
using Neosinerji.BABOnlineTP.Business.AEGON;
using Newtonsoft.Json;
using Neosinerji.BABOnlineTP.Business.CHARTIS;
using System.IO;
using System.Globalization;


namespace Neosinerji.BABOnlineTP.Business
{
    public class TESabitPrimliTeklif : TeklifBase, ITESabitPrimliTeklif
    {
        public TESabitPrimliTeklif()
            : base()
        {

        }

        public TESabitPrimliTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool aegon = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.AEGON) == 1;

            if (aegon)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IAEGONTESabitPrimli TESabitPirimli = DependencyResolver.Current.GetService<IAEGONTESabitPrimli>();
                    TESabitPirimli.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(TESabitPirimli);
                }
            }

            return base.Hesapla(teklif);
        }

        public override void CreatePDF()
        {
            PDFHelper pdf = null;
            try
            {

                #region Template Hazırlama

                string _RootPath = System.Web.HttpContext.Current.Server.MapPath("/");
                string template = PdfTemplates.GetTemplate(_RootPath + "Content/templates/", PdfTemplates.TE_SABITPRIMLI);

                pdf = new PDFHelper("Babonline", "Turuncu Elma Hayat Sigortası", "Turuncu Elma Hayat Sigortası", 8, _RootPath + "Content/fonts/",
                                    PdfTemplates.SenticoSansDT_Regular);



                PDFCustomEventHelperAEGON pdfHelper = new PDFCustomEventHelperAEGON();

                //SAYFA NUMARASI YAZIYOR
                pdf.SetPageEventHelper(pdfHelper);

                PDFParser parser = new PDFParser(template, pdf);
                CultureInfo culture = new CultureInfo("tr-TR");

                ITeklif aegonTeklif = this.TUMTeklifler.Where(w => w.GenelBilgiler.Basarili.Value && w.TUMKodu == TeklifUretimMerkezleri.AEGON).FirstOrDefault();
                if (aegonTeklif != null)
                    aegonTeklif = _TeklifService.GetTeklif(aegonTeklif.GenelBilgiler.TeklifId);

                #endregion

                #region Set Parameter

                #region Header

                // ==== SİGORTA ADAYI BİLGİLERİ
                string AdiSoyadi = String.Empty;
                string DogumTarihi = String.Empty;
                string Cinsiyeti = String.Empty;
                string Yas = String.Empty;

                // ==== VERGİ İLE İLGİLİ BİLGİLER
                string BeyanEdilenGelirVergisiOrani = String.Empty;
                string PrimdenVergiAvantaji = String.Empty;
                string VergiAvantajiSonrasiPrimMaliyeti = String.Empty;

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                if (musteri != null)
                {
                    AdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                    Cinsiyeti = musteri.Cinsiyet == "E" ? "Erkek" : "Kadın";
                    DogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty;

                    switch (musteri.CiroBilgisi)
                    {
                        case "1": BeyanEdilenGelirVergisiOrani = "%15"; break;
                        case "2": BeyanEdilenGelirVergisiOrani = "%20"; break;
                        case "3": BeyanEdilenGelirVergisiOrani = "%27"; break;
                        case "4": BeyanEdilenGelirVergisiOrani = "%35"; break;
                        case "5": BeyanEdilenGelirVergisiOrani = "%27"; break;
                    }
                }

                PrimdenVergiAvantaji = aegonTeklif.ReadSoru(TESabitPrimliSorular.PrimdenVergiAvantaji, decimal.Zero).ToString("N2", culture);
                VergiAvantajiSonrasiPrimMaliyeti = aegonTeklif.ReadSoru(TESabitPrimliSorular.VergiAvantajiSonrasiPrimMaliyeti, decimal.Zero).ToString("N2", culture);

                #region Yas Hesabı

                //Sigorta Başlangıç Tarihi
                DateTime SigortaBaslangicTar = this.Teklif.ReadSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, DateTime.MinValue);
                Yas = AEGONTESabitPrimli.AegonYasHesapla(musteri.DogumTarihi.Value, SigortaBaslangicTar).ToString();

                #endregion

                // ==== SİGORTA ADAYI BİLGİLERİ
                parser.SetVariable("$AdiSoyadi$", AdiSoyadi);
                parser.SetVariable("$DogumTarihi$", DogumTarihi);
                parser.SetVariable("$Cinsiyeti$", Cinsiyeti);
                parser.SetVariable("$Yas$", Yas);


                // ==== VERGİ İLE İLGİLİ BİLGİLER 
                parser.SetVariable("$BEGVO$", BeyanEdilenGelirVergisiOrani);
                parser.SetVariable("$PrimdenVergiAvantaji$", PrimdenVergiAvantaji);
                parser.SetVariable("$VASPM$", VergiAvantajiSonrasiPrimMaliyeti);


                #region Extra

                string imgpath = Path.Combine(_RootPath, "Content/img/Aegon//aegonlogo-pdf.jpg");
                string aegonFooter = Path.Combine(_RootPath, "Content/img/Aegon/aegonFooter.jpg");

                parser.SetVariable("$TVMLogo$", imgpath);
                parser.SetVariable("$aegonFooter$", aegonFooter);

                #endregion

                #endregion

                #region Body

                #region Teminatlar ( Tutar - Prim)

                // ==== Ana Teminat
                string anaTeminatTipi = this.Teklif.ReadSoru(TESabitPrimliSorular.AnaTeminat, String.Empty);
                string AnaTeminat = String.Empty;
                string AnaTeminatBedeli = String.Empty;
                string PRIM_AnaTeminat_Yillik = String.Empty;
                string PRIM_AnaTeminat_Donemsel = String.Empty;


                // ==== Kritik Hastalıklar Ek Teminatı
                string KritikHastaliklarEkTeminatPrice = "0";
                string PRIM_KritikHastalik_Yillik = "0";
                string PRIM_KritikHastalik_Donemsel = "0";

                //Kaza Sonucu Vefat Ek Teminatı
                string KazaSonucuVefatEkTeminatPrice = "0";
                string PRIM_KazaSonucuVefat_Yillik = "0";
                string PRIM_KazaSonucuVefat_Donemsel = "0";

                //Tam ve Daimi Maluliyet Ek Teminatı
                string TamVeDaimiMaluliyetEkTeminatPrice = "0";
                string PRIM_TamVeDaimiMaluliyet_Yillik = "0";
                string PRIM_TamVeDaimiMaluliyet_Donemsel = "0";

                //Maluliyet Yıllık Destek Ek Teminatı
                string MaluliyetTillikDestekEkTeminatPrice = "0";
                string PRIM_MaluliyetYillikDestek_Yillik = "0";
                string PRIM_MaluliyetYillikDestek_Donemsel = "0";

                //Toplu Taşıma Araçlarında Kaza Sonucu Vefat Ek Teminatı
                string TopluTasimaAraclarindaKazaEkTeminatPrice = "0";
                string PRIM_TTAKSV_Yillik = "0";
                string PRIM_TTAKSV_Donemsel = "0";

                //Kaza Sonucu Tedavi Masrafları Ek Teminatı
                string KazaSonucuTedaviMasraflariEkTeminati = "0";
                string PRIM_KSTMEK_Yillik = "0";
                string PRIM_KSTMEK_Donemsel = "0";

                //Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Haftalık Ödeme Ek Teminatı	
                string KSHYTDHOdeme = "0";
                string PRIM_KSHYTDHO_Yillik = "0";
                string PRIM_KSHYTDHO_Donemsel = "0";



                switch (anaTeminatTipi)
                {
                    case "1":
                        AnaTeminat = "Vefat Teminatı";
                        TeklifTeminat vefatTeminati = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat).FirstOrDefault();
                        if (vefatTeminati != null && vefatTeminati.TeminatBedeli.HasValue)
                        {
                            AnaTeminatBedeli = vefatTeminati.TeminatBedeli.Value.ToString("N2", culture);
                            PRIM_AnaTeminat_Yillik = vefatTeminati.TeminatBrutPrim.Value.ToString("N2", culture);
                            PRIM_AnaTeminat_Donemsel = vefatTeminati.TeminatNetPrim.Value.ToString("N2", culture);
                        }
                        break;
                    case "2":
                        AnaTeminat = "Vefat veya Kritik Hastalıklar Teminatı";
                        TeklifTeminat vefatKritikTeminat = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat_KritikHastalik).FirstOrDefault();
                        if (vefatKritikTeminat != null && vefatKritikTeminat.TeminatBedeli.HasValue)
                        {
                            AnaTeminatBedeli = vefatKritikTeminat.TeminatBedeli.Value.ToString("N2", culture);
                            PRIM_AnaTeminat_Yillik = vefatKritikTeminat.TeminatBrutPrim.Value.ToString("N2", culture);
                            PRIM_AnaTeminat_Donemsel = vefatKritikTeminat.TeminatNetPrim.Value.ToString("N2", culture);
                        }
                        break;
                }


                // ==== Kritik Hastalık 
                TeklifTeminat KritikHastalik = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KritikHastaliklar).FirstOrDefault();
                if (KritikHastalik != null && KritikHastalik.TeminatBedeli.HasValue)
                {
                    KritikHastaliklarEkTeminatPrice = KritikHastalik.TeminatBedeli.Value.ToString("N2", culture);
                    PRIM_KritikHastalik_Yillik = KritikHastalik.TeminatBrutPrim.Value.ToString("N2", culture);
                    PRIM_KritikHastalik_Donemsel = KritikHastalik.TeminatNetPrim.Value.ToString("N2", culture);
                }

                // ==== Kaza Sonucu Vefat
                TeklifTeminat KazaSonucuVefat = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucuVefat).FirstOrDefault();
                if (KazaSonucuVefat != null && KazaSonucuVefat.TeminatBedeli.HasValue)
                {
                    KazaSonucuVefatEkTeminatPrice = KazaSonucuVefat.TeminatBedeli.Value.ToString("N2", culture);
                    PRIM_KazaSonucuVefat_Yillik = KazaSonucuVefat.TeminatBrutPrim.Value.ToString("N2", culture);
                    PRIM_KazaSonucuVefat_Donemsel = KazaSonucuVefat.TeminatNetPrim.Value.ToString("N2", culture);
                }

                // ==== Tam ve Daimi Maluliyet
                TeklifTeminat TamVeDaimiMaluliyet = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.TamVeDaimiMaluliyet).FirstOrDefault();
                if (TamVeDaimiMaluliyet != null && TamVeDaimiMaluliyet.TeminatBedeli.HasValue)
                {
                    TamVeDaimiMaluliyetEkTeminatPrice = TamVeDaimiMaluliyet.TeminatBedeli.Value.ToString("N2", culture);
                    PRIM_TamVeDaimiMaluliyet_Yillik = TamVeDaimiMaluliyet.TeminatBrutPrim.Value.ToString("N2", culture);
                    PRIM_TamVeDaimiMaluliyet_Donemsel = TamVeDaimiMaluliyet.TeminatNetPrim.Value.ToString("N2", culture);
                }


                // ==== Tam ve Daimi Maluliyet
                TeklifTeminat MaluliyetYillikDestek = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.MaluliyetYillikDestek).FirstOrDefault();
                if (MaluliyetYillikDestek != null && MaluliyetYillikDestek.TeminatBedeli.HasValue)
                {
                    MaluliyetTillikDestekEkTeminatPrice = MaluliyetYillikDestek.TeminatBedeli.Value.ToString("N2", culture);
                    PRIM_MaluliyetYillikDestek_Yillik = MaluliyetYillikDestek.TeminatBrutPrim.Value.ToString("N2", culture);
                    PRIM_MaluliyetYillikDestek_Donemsel = MaluliyetYillikDestek.TeminatNetPrim.Value.ToString("N2", culture);
                }

                // ==== TopluTasimaAraclariKSV
                TeklifTeminat TopluTasimaAraclariKSV = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.TopluTasimaAraclariKSV).FirstOrDefault();
                if (TopluTasimaAraclariKSV != null && TopluTasimaAraclariKSV.TeminatBedeli.HasValue)
                {
                    TopluTasimaAraclarindaKazaEkTeminatPrice = TopluTasimaAraclariKSV.TeminatBedeli.Value.ToString("N2", culture);
                    PRIM_TTAKSV_Yillik = TopluTasimaAraclariKSV.TeminatBrutPrim.Value.ToString("N2", culture);
                    PRIM_TTAKSV_Donemsel = TopluTasimaAraclariKSV.TeminatNetPrim.Value.ToString("N2", culture);
                }

                // ==== KazaSonucu_TedaviMasraflari_EkTeminati
                TeklifTeminat KSTM = aegonTeklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati).FirstOrDefault();
                if (KSTM != null && KSTM.TeminatBedeli.HasValue)
                {
                    KazaSonucuTedaviMasraflariEkTeminati = KSTM.TeminatBedeli.Value.ToString("N2", culture);
                    PRIM_KSTMEK_Yillik = KSTM.TeminatBrutPrim.Value.ToString("N2", culture);
                    PRIM_KSTMEK_Donemsel = KSTM.TeminatNetPrim.Value.ToString("N2", culture);
                }

                // ==== KazaSonucu_TedaviMasraflari_EkTeminati
                TeklifTeminat KSHYTD_HaftalikOdeme = aegonTeklif.Teminatlar
                                                    .Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme).FirstOrDefault();
                if (KSHYTD_HaftalikOdeme != null && KSHYTD_HaftalikOdeme.TeminatBedeli.HasValue)
                {
                    KSHYTDHOdeme = KSHYTD_HaftalikOdeme.TeminatBedeli.Value.ToString("N2", culture);
                    PRIM_KSHYTDHO_Yillik = KSHYTD_HaftalikOdeme.TeminatBrutPrim.Value.ToString("N2", culture);
                    PRIM_KSHYTDHO_Donemsel = KSHYTD_HaftalikOdeme.TeminatNetPrim.Value.ToString("N2", culture);
                }




                // ==== Ana Teminat
                parser.SetVariable("$AnaTeminati$", AnaTeminat);
                parser.SetVariable("$AnaTeminatiBedeli$", AnaTeminatBedeli);
                parser.SetVariable("$PRIM_AnaTeminatBaslik$", AnaTeminat);
                parser.SetVariable("$PRIM_AnaTeminat_Yillik$", PRIM_AnaTeminat_Yillik);
                parser.SetVariable("$PRIM_AnaTeminat_Donemsel$", PRIM_AnaTeminat_Donemsel);



                //Kritik Hastalıklar Ek Teminatı
                parser.SetVariable("$KritikHastaliklarEkTeminatPrice$", KritikHastaliklarEkTeminatPrice);
                parser.SetVariable("$PRIM_KritikHastalik_Yillik$", PRIM_KritikHastalik_Yillik);
                parser.SetVariable("$PRIM_KritikHastalik_Donemsel$", PRIM_KritikHastalik_Donemsel);

                //Kaza Sonucu Vefat Ek Teminatı
                parser.SetVariable("$KazaSonucuVefatEkTeminatPrice$", KazaSonucuVefatEkTeminatPrice);
                parser.SetVariable("$PRIM_KazaSonucuVefat_Yillik$", PRIM_KazaSonucuVefat_Yillik);
                parser.SetVariable("$PRIM_KazaSonucuVefat_Donemsel$", PRIM_KazaSonucuVefat_Donemsel);

                //Tam ve Daimi Maluliyet Ek Teminatı
                parser.SetVariable("$TamVeDaimiMaluliyetEkTeminatPrice$", TamVeDaimiMaluliyetEkTeminatPrice);
                parser.SetVariable("$PRIM_TamVeDaimiMaluliyet_Yillik$", PRIM_TamVeDaimiMaluliyet_Yillik);
                parser.SetVariable("$PRIM_TamVeDaimiMaluliyet_Donemsel$", PRIM_TamVeDaimiMaluliyet_Donemsel);

                //Maluliyet Yıllık Destek Ek Teminatı
                parser.SetVariable("$MaluliyetTillikDestekEkTeminatPrice$", MaluliyetTillikDestekEkTeminatPrice);
                parser.SetVariable("$PRIM_MaluliyetYillikDestek_Yillik$", PRIM_MaluliyetYillikDestek_Yillik);
                parser.SetVariable("$PRIM_MaluliyetYillikDestek_Donemsel$", PRIM_MaluliyetYillikDestek_Donemsel);

                //Toplu Taşıma Araçlarında Kaza Sonucu Vefat Ek Teminatı
                parser.SetVariable("$TopluTasimaAraclarindaKazaEkTeminatPrice$", TopluTasimaAraclarindaKazaEkTeminatPrice);
                parser.SetVariable("$PRIM_TTAKSV_Yillik$", PRIM_TTAKSV_Yillik);
                parser.SetVariable("$PRIM_TTAKSV_Donemsel$", PRIM_TTAKSV_Donemsel);

                //Kaza Sonucu Tedavi Masrafları Ek Teminatı
                parser.SetVariable("$KazaSonucuTedaviMasraflariEkTeminatPrice$", KazaSonucuTedaviMasraflariEkTeminati);
                parser.SetVariable("$PRIM_KSTMEK_Yillik$", PRIM_KSTMEK_Yillik);
                parser.SetVariable("$PRIM_KSTMEK_Donemsel$", PRIM_KSTMEK_Donemsel);

                //Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Haftalık Ödeme Ek Teminatı	
                parser.SetVariable("$KS_HastanedeYatarakTedaviDurumunda_HO_EkTeminatPrice$", KSHYTDHOdeme);
                parser.SetVariable("$PRIM_KSHYTDHO_Yillik$", PRIM_KSHYTDHO_Yillik);
                parser.SetVariable("$PRIM_KSHYTDHO_Donemsel$", PRIM_KSHYTDHO_Donemsel);

                #endregion

                #region Sigorta İle İlgili Bilgiler

                // ==== SİGORTA BİLGİLERİ
                string SigortaSuresi = String.Empty;
                string PrimOdemeSikligi = String.Empty;
                string BaslangicTarihi = String.Empty;
                string ParaBirimi = String.Empty;


                BaslangicTarihi = this.Teklif.ReadSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");
                SigortaSuresi = this.Teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);


                switch (this.Teklif.ReadSoru(TESabitPrimliSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": PrimOdemeSikligi = "Aylık"; break;
                    case "2": PrimOdemeSikligi = "3 Aylık"; break;
                    case "3": PrimOdemeSikligi = "6 Aylık"; break;
                    case "4": PrimOdemeSikligi = "Yıllık"; break;
                }


                switch (this.Teklif.ReadSoru(TESabitPrimliSorular.ParaBirimi, String.Empty))
                {
                    case "1":
                        ParaBirimi = "EUR";
                        parser.SetVariable("$ParaBirimiTuru$", "Euro");
                        break;
                    case "2":
                        ParaBirimi = "ABD Doları";
                        parser.SetVariable("$ParaBirimiTuru$", "ABD Doları");
                        break;
                }


                // ==== SİGORTA BİLGİLERİ
                parser.SetVariable("$SigortaSuresi$", SigortaSuresi);
                parser.SetVariable("$BaslangicTarihi$", BaslangicTarihi);
                parser.SetVariable("$PrimOdemesi$", PrimOdemeSikligi);
                parser.SetVariable("$ParaBirimi$", ParaBirimi);

                #endregion

                #region Sigorta Primleri

                // ==== PRIM BILGILERI
                string YillikPrimToplami = "0.00";
                string DonemselPrim = "0.00";
                string SureBoyuncaOdenecekToplam = "0.00";


                YillikPrimToplami = aegonTeklif.GenelBilgiler.BrutPrim.HasValue ? aegonTeklif.GenelBilgiler.BrutPrim.Value.ToString("N2", culture) : "";
                DonemselPrim = aegonTeklif.GenelBilgiler.NetPrim.HasValue ? aegonTeklif.GenelBilgiler.NetPrim.Value.ToString("N2", culture) : "";
                SureBoyuncaOdenecekToplam = aegonTeklif.ReadSoru(TESabitPrimliSorular.SureBoyuncaOdenecekToplamP, decimal.Zero).ToString("N2", culture);


                // ==== PRIM BILGILERI
                parser.SetVariable("$YillikPrimToplam$", YillikPrimToplami);
                parser.SetVariable("$DonemselPrimToplam$", DonemselPrim);
                parser.SetVariable("$SureBoyuncaOdenecekToplam$", SureBoyuncaOdenecekToplam);

                #endregion

                #endregion

                #region Footer

                string Tetkikler = String.Empty;
                string SurumNo = String.Empty;
                string TeklifNo = this.Teklif.TeklifNo.ToString();
                string TeklifTarihi = this.Teklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy");


                SurumNo = aegonTeklif.ReadWebServisCevap(WebServisCevaplar.SurumNo, String.Empty);
                pdfHelper.SetSurumNo(SurumNo);

                Tetkikler = aegonTeklif.ReadWebServisCevap(WebServisCevaplar.TibbiTetkikSonucu, String.Empty);

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

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("turuncu_elma_hayat_sigortasi{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("TESabitPrimli", fileName, fileData);

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

        public override void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();
            bool pdfDosyasiVar = false;
            if (this.Teklif.GenelBilgiler.TUMKodu == 0)
                pdfDosyasiVar = !String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFDosyasi);


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
                email.SendAegonEMailTeklif(this.Teklif, DigerAdSoyad, DigerEmail, false);
            }
        }
    }
}
