using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class LilyumFerdiKazaModel
    {
        public AracBilgiModel Arac { get; set; }

        public bool TekrarTeklif { get; set; }
        public LilyumGenelBilgilerModel GenelBilgiler { get; set; }
        public SigortaliModel Musteri { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public LilyumTeklifOdemeModel Odeme { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        
        public bool KrediKartiMi { get; set; }

        public string UrunAdi { get; set; }
        public bool AdresAyniMi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IletisimAdresi { get; set; }
        public string TeslimatAdresi { get; set; }
        public string TeslimatIlKodu { get; set; }
        public string TeslimatIlceKodu { get; set; }
        public string IletisimIlKodu { get; set; }
        public string IletisimIlceKodu { get; set; }
        public List<SelectListItem> TeslimatIller { get; set; }
        public List<SelectListItem> TeslimatIlceler { get; set; }
        public List<SelectListItem> IletisimIller { get; set; }
        public List<SelectListItem> IletisimIlceler { get; set; }
       
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool KullaniciSozlesmesi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool ETicaretSozlesmesi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool LilyumKartHizmeti { get; set; }
        public string SigortaliMeslekKodu { get; set; }
        public List<SelectListItem> SigortaliMeslekListesi { get; set; }
    }
    public class LilyumKartBilgilendirme
    {
        //ödeme başarılı ise true, ödeme başarısız ise false.
        public string odemeDurumu { get; set; }

    }
    public class LilyumGenelBilgilerModel
    {
        public DateTime PoliceBaslangicTarihi { get; set; }

        public DateTime PoliceBitisTarihi { get; set; }
        public DetayAracBilgiModel Arac { get; set; }

        public string PersonelGrubu { get; set; }
        public bool SporlaUgrasiyorMu { get; set; }
        public bool MotorsikletKullaniyorMu { get; set; }
        public string KimlikNo { get; set; }
        public string PlakaIlKodu { get; set; }
        public string PlakaNo { get; set; }
        public List<SelectListItem> PlakaKoduListe { get; set; }
    }
    public class DetayLilyumModel
    {
        public string Parartika3DLilyumSonOdemeDurumu { get; set; }
        public int TeklifId { get; set; }
        public int PoliceId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayLilyumMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public string TUMLogoURL { get; set; }
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte? TaksitSayisi { get; set; }
        public decimal? BrutPrim { get; set; }
        public string TUMPoliceNo { get; set; }
        //koru tarafı çalışmadığında tutuarı odemeturarından alınır.
        public string OdemeTutari { get; set; }
        public string KoruPoliceDurumu { get; set; }
        public string TUMUnvani { get; set; }
        public LilyumGenelBilgilerDetayModel GenelBilgiler { get; set; }
        public LilyumSigortalilarList Sigortalilar { get; set; }
    }
    public class LilyumGenelBilgilerDetayModel
    {
        public string PoliceNo { get; set; }
        public string Ekno { get; set; }
        public string YenilemeNo { get; set; }
        public string PoliceBaslangicTarihi { get; set; }
        public string PoliceBitisTarihi { get; set; }
        public string YenilemeMi { get; set; }
        public string OncekiPoliceBaslangicTarihi { get; set; }
        public string OncekiSigortaSirketi { get; set; }
        public string OncekiPoliceNo { get; set; }
        public string Meslek { get; set; }
        public string Tarife { get; set; }
        public int TeklifId { get; set; }
        public decimal? NetPrim { get; set; }
        public decimal? ToplamVergi { get; set; }
        public decimal? ToplamTutar { get; set; }
        public string SporlaUgrasiyorMu { get; set; }
        public string MotorsikletKullaniyorMu { get; set; }
        public string Plaka { get; set; }

    }
    public class LilyumTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte? TaksitSayisi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
        public decimal? DigerOdemeTutari { get; set; }

    }
    public class LilyumPoliceOdemeModel
    {
        public int teklifId { get; set; }

        public decimal NetPrim { get; set; }
        public decimal BrutPrim { get; set; }
        public decimal Vergi { get; set; }
        public int TaksitSayisi { get; set; }

        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMLogoURL { get; set; }

        public string PoliceURL { get; set; }
        public string TUMPoliceNo { get; set; }
        public string BilgilendirmePDF { get; set; }
        public string DekontPDF { get; set; }
        public bool DekontPDFGoster { get; set; }
    }
    public class DetayLilyumMusteriModel : DetayMusteriModel
    {
        public string IletisimAcikAdres { get; set; }
        public string TeslimatAcikAdres { get; set; }
        public string TeslimatIlAdi { get; set; }
        public string TeslimatIlceAdi { get; set; }
        public string CepTelText { get; set; }

    }
    public class LilyumSigortalilarList
    {
        public List<LilyumSigortalilar> SigortaliList { get; set; }

        public List<SelectListItem> BireyTipleri { get; set; }

    }
    public class LilyumSigortalilar
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Soyadi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime DogumTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Uyruk { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte KimlikTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KimlikNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte BireyTipi { get; set; }

        public List<SelectListItem> Uyruklar { get; set; }
        public List<SelectListItem> KimlikTipleri { get; set; }
    }

    public class OdemeLilyumModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class LilyumPoliceBitisTarihiModel
    {
        public string PoliceBitisTarihi { get; set; }
    }


    public class MusteriLilyumSatinAlModel 
    {
       public LilyumFerdiKazaModel lilyumFerdiKazaModel { get; set; }
    
    }
}