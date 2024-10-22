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
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Kasko;
using Neosinerji.BABOnlineTP.Business.RAY;
using Neosinerji.BABOnlineTP.Business.EUREKO.Kasko;
using Neosinerji.BABOnlineTP.Business.ERGO.Kasko;
using Neosinerji.BABOnlineTP.Business.AXA;
using Neosinerji.BABOnlineTP.Business.UNICO.Kasko;
using Neosinerji.BABOnlineTP.Business.GROUPAMA;
using Neosinerji.BABOnlineTP.Business.GULF;
using Neosinerji.BABOnlineTP.Business.AK;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KaskoTeklif : TeklifBase, IKaskoTeklif
    {
        public KaskoTeklif()
            : base()
        {

        }

        public KaskoTeklif(int teklifId)
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
            bool turknippon = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.TURKNIPPON) == 1;
            bool eureko = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.EUREKO) == 1;
            bool ergo = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.ERGO) == 1;
            bool axa = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.AXA) == 1;
            bool unico = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.UNICO) == 1;
            bool gruopoma = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.GROUPAMA) == 1;
            bool gulf = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.GULF) == 1;
            bool ak = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.AK) == 1;



            if (hdi)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IHDIKasko kasko = DependencyResolver.Current.GetService<IHDIKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }

            if (axa)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IAXAKasko kasko = DependencyResolver.Current.GetService<IAXAKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }
            if (gulf)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IGULFKasko kasko = DependencyResolver.Current.GetService<IGULFKasko>();
                    kasko.SetClientIPAdres(GetClientIP());
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }
            if (mapfre)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IMAPFREKasko kasko = DependencyResolver.Current.GetService<IMAPFREKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }


            if (ray)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IRAYKasko kasko = DependencyResolver.Current.GetService<IRAYKasko>();
                    kasko.SetClientIPAdres(GetClientIP());
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }
            if (ergo)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IERGOKasko kasko = DependencyResolver.Current.GetService<IERGOKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }
            if (sompojapan)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    ISOMPOJAPANKasko kasko = DependencyResolver.Current.GetService<ISOMPOJAPANKasko>();
                    kasko.SetClientIPAdres(GetClientIP());
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }
            if (ak)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IAKKasko kasko = DependencyResolver.Current.GetService<IAKKasko>();
                    kasko.SetClientIPAdres(GetClientIP());
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }

            if (turknippon)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    ITURKNIPPONKasko kasko = DependencyResolver.Current.GetService<ITURKNIPPONKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }

            if (eureko)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IEUREKOKasko kasko = DependencyResolver.Current.GetService<IEUREKOKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }


            if (unico)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IUNICOKasko kasko = DependencyResolver.Current.GetService<IUNICOKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }
            if (gruopoma)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IGROUPAMAKasko kasko = DependencyResolver.Current.GetService<IGROUPAMAKasko>();
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
                }
            }
            if (anadolu)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IANADOLUKasko kasko = DependencyResolver.Current.GetService<IANADOLUKasko>();
                    kasko.SetClientIPAdres(GetClientIP());
                    kasko.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kasko);
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
            if (base._Aktif.ProjeKodu == TVMProjeKodlari.Mapfre.ToString())
            {
                ITeklif teklif = this.TUMTeklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE);

                if (teklif != null)
                {
                    teklif = _TeklifService.GetTeklif(teklif.GenelBilgiler.TeklifId);
                    MAPFREKasko mapfreKasko = TeklifUrunFactory.AsUrunClass(teklif) as MAPFREKasko;
                    string teklifUrl = mapfreKasko.TeklifPDF();
                    this.Teklif.GenelBilgiler.PDFDosyasi = teklifUrl;
                    _TeklifService.UpdateGenelBilgiler(this.Teklif.GenelBilgiler);
                    return;
                }
            }

            ITVMService tvm = base._TVMService;
            ITUMService tum = base._TUMService;
            IAracService arac = base._AracService;
            IKullaniciFotografStorage logoService = DependencyResolver.Current.GetService<IKullaniciFotografStorage>();

            PDFHelper pdf = null;

            try
            {
                TVMDetay tvmDetay = tvm.GetDetay(this.Teklif.GenelBilgiler.TVMKodu);
                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.KASKO_KARSILASTIRMA);

                pdf = new PDFHelper("NeoOnline", "KASKO SİGORTASI TEKLİFİ KARŞILAŞTIRMA TABLOSU", "KASKO SİGORTASI TEKLİFİ KARŞILAŞTIRMA TABLOSU", 8, _RootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());
                if (tvmDetay.Tipi == 11)
                {
                    template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.KASKO_KARSILASTIRMA_IHSAN);
                }


                PDFParser parser = new PDFParser(template, pdf);

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                string adUnvan = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                parser.SetVariable("$AdiSoyadiUnvani$", adUnvan);

                //TODO : TVM Logosu yoksa default bir icon koyulması

                string tvmUnvani = tvmDetay.Unvani;

                #region Logo

                // Default Logo
                string tvmLogo = "https://neoonlineteststrg.blob.core.windows.net/kullanici-fotograf/neoonline-logo.jpg";
                if (!String.IsNullOrEmpty(tvmDetay.Logo))
                {
                    tvmLogo = tvmDetay.Logo;
                }

                #endregion

                #region MB Grup 2.logo

                if (this.Teklif.GenelBilgiler.TVMKodu == NeosinerjiTVM.MBGrupTVMKodu)
                {
                    string tvmLogo2 = "https://neoonlinestrg.blob.core.windows.net/tvm-dokuman/acente-logo/sigortam%20market%20logo.jpg";
                    parser.SetVariable("$TVMLogo2$", tvmLogo2);
                }
                else
                {
                    parser.SetVariable("$TVMLogo2$", "");
                }

                #endregion

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
                                                   .Replace("$TUMUnvani$", teklif1.TUMTeklifNo);
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

                #region Diğer Şirket Teklif Fiyat Satırları

                int anaTeklifId = this.Teklif.GenelBilgiler.TeklifId;

                var digerSirketTeklifList = _TeklifService.getDigerTeklifler(anaTeklifId);

                if (digerSirketTeklifList != null)
                {
                    foreach (var item in digerSirketTeklifList)
                    {
                        TUMDetay tumDetay = tum.GetDetay(item.SigortaSirketKodu);
                        tumler.Add(tumDetay);

                        string fiyatSatir = String.Empty;

                        // Default Logo
                        string logo = "https://neobabstoragetest.blob.core.windows.net/musteri-dokuman/176/default_logo.jpg";
                        if (!String.IsNullOrEmpty(tumDetay.Logo))
                        {
                            logo = tumDetay.Logo;
                        }

                        fiyatSatir = fiyatSatirTemplate.Replace("$TUMLogo$", logo)
                                                       .Replace("$TUMUnvani$", item.SigortaSirketTeklifNo);
                        if (item.BrutPrim != null)
                        {
                            if (item.TaksitSayisi.HasValue && item.TaksitSayisi.Value > 1)
                                fiyatSatir = fiyatSatir.Replace("$Fiyat1$", string.Format("{0:n2} TL ({1} taksit)", item.BrutPrim, item.TaksitSayisi));
                            else
                                fiyatSatir = fiyatSatir.Replace("$Fiyat1$", string.Format("{0:n2} TL", item.BrutPrim));
                        }
                        else
                        {
                            fiyatSatir = fiyatSatir.Replace("$Fiyat1$", "-");
                        }

                        parser.AppendToPlaceHolder("fiyatSatirlari", fiyatSatir);
                    }
                }
                #endregion

                #region Araç Bilgileri
                string plaka = String.Format("{0} {1}", this.Teklif.Arac.PlakaKodu, this.Teklif.Arac.PlakaNo);
                AracMarka aracMarka = arac.GetAracMarka(this.Teklif.Arac.Marka);
                AracTip aracTip = arac.GetAracTip(this.Teklif.Arac.Marka, this.Teklif.Arac.AracinTipi);
                if (aracTip == null)
                {
                    aracTip = new AracTip
                    {
                        TipAdi = "BILINMIYOR"
                    };
                }

                if (Teklif.Arac.KullanimTarzi == null)
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
                #endregion

                if(tvmDetay.Tipi != 11)
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
                        TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu & w.GenelBilgiler.Basarili.Value)
                                                               .Select(f => f.GenelBilgiler).FirstOrDefault();

                        if (teklif1 == null)
                            return;

                        // ==== EVET HAYIR teminatlar ==== //
                        string GLKHHT = "";
                        string Deprem = "";
                        string SelSu = "";
                        string HasarsizlikKoruma = "";
                        string Calinma = "";
                        string HayvanlarinVerecegiZarar = "";
                        string Alarm = "";
                        string AnahtarKaybı = "";
                        string Yangin = "";
                        string Eskime = "";
                        string YurtDisiTeminati = "";
                        string LPGLiArac = "";
                        string HukuksalKoruma = "";
                        string Saglik = "";
                        string Ikame = "İkame Yok";
                        string ManeviDahilMi = "";
                        string CamMuafiyetli = "";


                        decimal? IMMSahisBasina = null;
                        decimal? IMMKazaBasina = null;
                        decimal? IMMMaddiKaza = null;
                        decimal? IMMKombine = null;
                        decimal? FerdiKaza = null;
                        decimal? KFK_Olum = null;
                        decimal? KFK_Surekli_Sakatlik = null;
                        decimal? KFK_Tedavi = null;
                        string Medline = "";
                        string AsistanHizmetleri = "";
                        string MiniOnarimHizmetleri = "";
                        string HukuksalKoruma_Genel = "";
                        string HukuksalKoruma_MotorluArac = "";
                        string HukuksalKoruma_Surucu = "";
                        string HukuksalKoruma_OBAzamiAvans = "";
                        string HukuksalKoruma_OBAzami_Kefalet = "";
                        string HukuksalKoruma_OBAzami_Limit = "";
                        string HukuksalKoruma_SSIAzami_Limit = "";

                        ITeklif teklif = this.TUMTeklifler.FirstOrDefault(f => f.GenelBilgiler.TeklifId == teklif1.TeklifId);

                        #region Teminatlar Değerlerle Dolduruluyor.

                        if (this.Teklif.ReadSoru(KaskoSorular.GLKHHT, false))
                            GLKHHT = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false))
                            Deprem = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Sel_Su_VarYok, false))
                            SelSu = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Hasarsizlik_Koruma_VarYok, false))
                            HasarsizlikKoruma = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Calinma_VarYok, false))
                            Calinma = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_VarYok, false))
                            HukuksalKoruma = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Hayvanlarin_Verecegi_Zarar_ZarYok, false))
                            HayvanlarinVerecegiZarar = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false))
                            Alarm = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false))
                            AnahtarKaybı = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Yangin_VarYok, false))
                            Yangin = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Eskime_VarYok, false))
                            Eskime = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Saglik_VarYok, false))
                            Saglik = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, false))
                            YurtDisiTeminati = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.LPG_VarYok, false))
                            LPGLiArac = "√";

                        if (this.Teklif.ReadSoru(KaskoSorular.Ikame_Arac_Teminati_VarYok, false))
                        {
                            string ikameKodu = this.Teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                            switch (ikameKodu)
                            {
                                case "ABC07": Ikame = "7 Gün/24"; break;
                                case "ABC14": Ikame = "14 Gün/24"; break;
                            }
                        }

                        if (this.Teklif.ReadSoru(KaskoSorular.ManeviDahilMi, false))
                            ManeviDahilMi = " - Manevi Dahil";

                        // ==== IMMSahisBasina ==== //
                        var immsahisbasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.IMM_Sahis_Basina);
                        if (immsahisbasina != null && immsahisbasina.TeminatBedeli.HasValue)
                            IMMSahisBasina = immsahisbasina.TeminatBedeli.Value;

                        // ==== IMMKazaBasina ==== //
                        var immkazabasina = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.IMM_Kaza_Basina);
                        if (immkazabasina != null && immkazabasina.TeminatBedeli.HasValue)
                            IMMKazaBasina = immkazabasina.TeminatBedeli.Value;

                        // ==== IMMMaddiKaza ==== //
                        var immmaddikaza = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.IMM_Maddi_Hasar);
                        if (immmaddikaza != null && immmaddikaza.TeminatBedeli.HasValue)
                            IMMMaddiKaza = immmaddikaza.TeminatBedeli.Value;

                        // ==== IMMKombineTekLimit ==== //
                        var immKombine = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi);
                        if (immKombine != null && immKombine.TeminatBedeli.HasValue)
                            IMMKombine = immKombine.TeminatBedeli.Value;

                        // ==== FK Primi ==== //
                        var KoltukFerdiKaza = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.Koltuk_Ferdi_Kaza_Surucu_Yolcu);
                        if (KoltukFerdiKaza != null && KoltukFerdiKaza.TeminatBedeli.HasValue)
                            FerdiKaza = KoltukFerdiKaza.TeminatBedeli.Value;

                        // ==== FK Primi Olum ==== //
                        var KoltukFerdiKaza_Olum = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.KFK_Olum);
                        if (KoltukFerdiKaza_Olum != null && KoltukFerdiKaza_Olum.TeminatBedeli.HasValue)
                            KFK_Olum = KoltukFerdiKaza_Olum.TeminatBedeli.Value;

                        // ==== FK Primi Sakatlik ==== //
                        var KoltukFerdiKaza_Sakatlik = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.KFK_Surekli_Sakatlik);
                        if (KoltukFerdiKaza_Sakatlik != null && KoltukFerdiKaza_Sakatlik.TeminatBedeli.HasValue)
                            KFK_Surekli_Sakatlik = KoltukFerdiKaza_Sakatlik.TeminatBedeli.Value;

                        // ==== FK Primi Tedavi==== //
                        var KoltukFerdiKaza_Tedavi = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.KFK_Tedavi);
                        if (KoltukFerdiKaza_Tedavi != null && KoltukFerdiKaza_Tedavi.TeminatBedeli.HasValue)
                            KFK_Tedavi = KoltukFerdiKaza_Tedavi.TeminatBedeli.Value;

                        // ==== Medline ==== //
                        var MedlineTeminat = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.Medline);
                        if (MedlineTeminat != null)
                            Medline = "√";

                        // ==== Asistan ==== //
                        var asistansTeminat = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim);
                        if (asistansTeminat != null)
                            AsistanHizmetleri = "√";

                        // ==== Mini Onarim Hizmetleri ==== //
                        var miniOnarimHiz = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.Mini_Onrarim_Hizmeti);
                        if (miniOnarimHiz != null)
                            MiniOnarimHizmetleri = "√";

                        // ==== HukuksalKoruma Genel==== //
                        var hukuksalkoruma = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.Hukuksal_Koruma);
                        if (hukuksalkoruma != null)//        HukuksalKoruma_Genel = "√";                
                            HukuksalKoruma_Genel = hukuksalkoruma.TeminatBedeli.HasValue ? hukuksalkoruma.TeminatBedeli.Value.ToString("N2") : "√";

                        // ==== HukuksalKoruma_MotorluArac ==== //
                        var hukuksalkorumamotorluarac = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.HK_Motorlu_Araca_Bagli);
                        if (hukuksalkorumamotorluarac != null) //HukuksalKoruma_MotorluArac = "√";                       
                            HukuksalKoruma_MotorluArac = hukuksalkorumamotorluarac.TeminatBedeli.HasValue ? hukuksalkorumamotorluarac.TeminatBedeli.Value.ToString("N2") : "√";

                        // ==== HukuksalKoruma_Surucu ==== //
                        var hukuksalkorumasurucu = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.HK_Surucuye_Bagli);
                        if (hukuksalkorumasurucu != null)   // HukuksalKoruma_Surucu = "√";
                            HukuksalKoruma_Surucu = hukuksalkorumasurucu.TeminatBedeli.HasValue ? hukuksalkorumasurucu.TeminatBedeli.Value.ToString("N2") : "√";


                        // ==== HukuksalKoruma_OBAzamiAvans ==== //
                        var HK_Olay_Basina_Azami_Avans = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.HK_Olay_Basina_Azami_Avans);
                        if (HK_Olay_Basina_Azami_Avans != null)
                            HukuksalKoruma_OBAzamiAvans = "√";

                        // ==== HK_Olay_Basina_Azami_Kefalet ==== //
                        var HK_Olay_Basina_Azami_Kefalet = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.HK_Olay_Basina_Azami_Kefalet);
                        if (HK_Olay_Basina_Azami_Kefalet != null)
                            HukuksalKoruma_OBAzami_Kefalet = "√";

                        // ==== HK_Olay_Basina_Azami_Kefalet ==== //
                        var HK_Olay_Basina_Azami_Limit = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.HK_Olay_Basina_Azami_Limit);
                        if (HK_Olay_Basina_Azami_Limit != null)
                            HukuksalKoruma_OBAzami_Limit = "√";

                        // ==== HK_Sigorta_Suresi_Icinde_Azami_Limit ==== //
                        var HK_Sigorta_Suresi_Icinde_Azami_Limit = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.HK_Sigorta_Suresi_Icinde_Azami_Limit);
                        if (HK_Sigorta_Suresi_Icinde_Azami_Limit != null)
                            HukuksalKoruma_SSIAzami_Limit = "√";

                        var CamMuafiyetliMi = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == KaskoTeminatlar.CamMuafiyeti);
                        if (CamMuafiyetliMi != null)
                            CamMuafiyetli = "√";
                        else CamMuafiyetli = "";

                        #endregion

                        #region PDF Formati Değerlerini Alıyor
                        string karsilastirmaSon = "";

                        karsilastirmaSon = karsilastirma.Replace("$PesinPrim$", String.Format("{0:N2}", teklif1.BrutPrim));

                        string fkdefult = "";
                        string[] kullanimtarziParts = this.Teklif.Arac.KullanimTarzi.Split('-');
                        string fkKademesi = this.Teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, "");
                        if (!String.IsNullOrEmpty(fkKademesi))
                        {
                            fkdefult = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademesi), kullanimtarziParts[0].ToString(), kullanimtarziParts[1].ToString()).Text;
                            //fkdefult = _CRService.GetKaskoFK(TeklifUretimMerkezleri.HDI, Convert.ToInt16(fkKademesi), kullanimtarziText).Text;
                        }

                        // ==== IMM TEXT ==== //                  

                        if (IMMSahisBasina.HasValue && IMMSahisBasina.Value > 0)
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMSahisBasina$", String.Format("{0:N2}", IMMSahisBasina.Value) + ManeviDahilMi);
                        else
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMSahisBasina$", "");
                        if (IMMKazaBasina.HasValue && IMMKazaBasina.Value > 0)
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMKazaBasina$", String.Format("{0:N2}", IMMKazaBasina.Value) + ManeviDahilMi);
                        else
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMKazaBasina$", "");
                        if (IMMMaddiKaza.HasValue && IMMMaddiKaza.Value > 0)
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMMaddiKaza$", String.Format("{0:N2}", IMMMaddiKaza.Value) + ManeviDahilMi);
                        else
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMMaddiKaza$", "");
                        if (IMMKombine.HasValue && IMMKombine.Value > 0)
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMKombineTekLimit$", String.Format("{0:N2}", IMMKombine.Value) + ManeviDahilMi);
                        else
                            karsilastirmaSon = karsilastirmaSon.Replace("$IMMKombineTekLimit$", "");

                        // ==== FerdiKaza TEXT ==== //

                        if (KFK_Olum.HasValue && KFK_Olum.Value > 0)
                            karsilastirmaSon = karsilastirmaSon.Replace("$KFK_Olum$", String.Format("{0:N2}", KFK_Olum.Value));
                        else
                            karsilastirmaSon = karsilastirmaSon.Replace("$KFK_Olum$", "");

                        if (KFK_Surekli_Sakatlik.HasValue && KFK_Surekli_Sakatlik.Value > 0)
                            karsilastirmaSon = karsilastirmaSon.Replace("$KFK_Surekli_Sakatlik$", String.Format("{0:N2}", KFK_Surekli_Sakatlik.Value));
                        else
                            karsilastirmaSon = karsilastirmaSon.Replace("$KFK_Surekli_Sakatlik$", "");
                        if (KFK_Tedavi.HasValue && KFK_Tedavi.Value > 0)
                            karsilastirmaSon = karsilastirmaSon.Replace("$KFK_Tedavi$", String.Format("{0:N2}", KFK_Tedavi.Value));
                        else
                            karsilastirmaSon = karsilastirmaSon.Replace("$KFK_Tedavi$", "");

                        karsilastirmaSon = karsilastirmaSon.Replace("$Medline$", Medline);
                        karsilastirmaSon = karsilastirmaSon.Replace("$AsistanHizmetleri$", AsistanHizmetleri);
                        karsilastirmaSon = karsilastirmaSon.Replace("$MiniOnarimHizmeti$", MiniOnarimHizmetleri);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKorumaGenel$", HukuksalKoruma_Genel);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma$", HukuksalKoruma_MotorluArac);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma_Surucu$", HukuksalKoruma_Surucu);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma_OBAzami_Avans$", HukuksalKoruma_OBAzamiAvans);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma_OBAzami_Kefalet$", HukuksalKoruma_OBAzami_Kefalet);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma_OBAzami_Limit$", HukuksalKoruma_OBAzami_Limit);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma_SSIAzami_Limit$", HukuksalKoruma_SSIAzami_Limit);

                        karsilastirmaSon = karsilastirmaSon.Replace("$GLKHHT$", GLKHHT);
                        karsilastirmaSon = karsilastirmaSon.Replace("$Deprem$", Deprem);
                        karsilastirmaSon = karsilastirmaSon.Replace("$SelSu$", SelSu);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HasarsizlikKoruma$", HasarsizlikKoruma);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HukuksalKoruma$", HukuksalKoruma);
                        karsilastirmaSon = karsilastirmaSon.Replace("$Calinma$", Calinma);
                        karsilastirmaSon = karsilastirmaSon.Replace("$HayvanlarinVerecegiZarar$", HayvanlarinVerecegiZarar);
                        karsilastirmaSon = karsilastirmaSon.Replace("$Alarm$", Alarm);
                        karsilastirmaSon = karsilastirmaSon.Replace("$AnahtarKaybi$", AnahtarKaybı);
                        karsilastirmaSon = karsilastirmaSon.Replace("$Yangin$", Yangin);
                        karsilastirmaSon = karsilastirmaSon.Replace("$Eskime$", Eskime);
                        karsilastirmaSon = karsilastirmaSon.Replace("$Saglik$", Saglik);
                        karsilastirmaSon = karsilastirmaSon.Replace("$YurtDisiTeminat$", YurtDisiTeminati);
                        karsilastirmaSon = karsilastirmaSon.Replace("$LPGliArac$", LPGLiArac);
                        karsilastirmaSon = karsilastirmaSon.Replace("$Ikame$", Ikame);
                        karsilastirmaSon = karsilastirmaSon.Replace("$CamMuafiyetli$", CamMuafiyetli);

                        #endregion

                        parser.SetColumnValues("karsilastirma", karsilastirmaSon);
                    }
                    #endregion
                }
                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                TeklifPDFStorage storage = DependencyResolver.Current.GetService<TeklifPDFStorage>();
                string fileName = String.Format("kasko_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("kasko", fileName, fileData);

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
                email.SendKaskoTeklif(this.Teklif, DigerAdSoyad, DigerEmail);
        }

        public string GetClientIP()
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
