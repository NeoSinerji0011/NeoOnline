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
    public class TrafikModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public AracBilgiModel Arac { get; set; }
        public EskiPoliceModel EskiPolice { get; set; }
        public TasiyiciSorumlulukModel Tasiyici { get; set; }
        public TrafikTeminatModel Teminat { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public TeklifOdemeListeModel Odeme { get; set; }
        public TrafikTeklifOdemeModel TaksitliOdeme { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public string ProjeKodu { get; set; }
    }

    public class DetayTrafikModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public DetayAracBilgiModel Arac { get; set; }
        public DetayEskiPoliceModel EskiPolice { get; set; }
        public DetayTasiyiciSorumlulukModel Tasiyici { get; set; }
        public DetayTrafikTeminatModel Teminat { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public OdemeBilgileriModel OdemeBilgileri { get; set; }
    }

    // Sonradan Eklendi
    public class TrafikTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class TrafikTeminatModel
    {
        public short? IMMKodu { get; set; }
        public short? FKKodu { get; set; }
        public bool Asistans { get; set; }
        public bool SinirsizIMM { get; set; }

        public bool BelediyeHalkOtobusu { get; set; }

        public List<SelectListItem> IMM { get; set; }
        public List<SelectListItem> FK { get; set; }
    }

    public class DetayTrafikTeminatModel
    {
        public string IMM { get; set; }
        public string FK { get; set; }
        public bool Asistans { get; set; }
        public bool SinirsizIMM { get; set; }
        public bool BelediyeHalkOtobusu { get; set; }
    }

    public class PlakaSorgulaModel
    {
        [Required]
        public string PlakaKodu { get; set; }

        [Required]
        public string PlakaNo { get; set; }

        [Required]
        public int MusteriKodu { get; set; }
    }

    public class EgmSorgulaModel
    {
        [Required]
        public string PlakaKodu { get; set; }

        [Required]
        public string PlakaNo { get; set; }

        public string AracRuhsatSeriNo { get; set; }

        public string AracRuhsatNo { get; set; }
        public string AsbisNo { get; set; }
    }

    public class PlakaSorgu
    {
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public string AracKullanimSekli { get; set; }
        public string AracKullanimTarzi { get; set; }
        public string AracMarkaKodu { get; set; }
        public string AracTipKodu { get; set; }
        public string AracModelYili { get; set; }
        public string AracMotorNo { get; set; }
        public string AracSasiNo { get; set; }
        public string AracTescilTarih { get; set; }
        public string TrafigeCikisTarihi { get; set; }
        public string AracKoltukSayisi { get; set; }
        public string AracDegeri { get; set; }
        public string AracSilindir { get; set; }
        public string YeniPoliceBaslangicTarih { get; set; }
        public string EskiPoliceSigortaSirkedKodu { get; set; }
        public string EskiPoliceAcenteKod { get; set; }
        public string EskiPoliceNo { get; set; }
        public string EskiPoliceYenilemeNo { get; set; }
        public string TasiyiciSigSirkerKod { get; set; }
        public string TasiyiciSigAcenteNo { get; set; }
        public string TasiyiciSigPoliceNo { get; set; }
        public string TasiyiciSigYenilemeNo { get; set; }

        public string HasarsizlikInd { get; set; }
        public string HasarsizlikSur { get; set; }
        public string HasarsizlikKademe { get; set; }
        public string UygulanmisHasarsizlikKademe { get; set; }
        public string HasarsizlikHata { get; set; }

        public string TescilSeri { get; set; }
        public string TescilSeriNo { get; set; }

        public string FesihTarih { get; set; }

        public string MotorGucu { get; set; }
        public string SilindirHacmi { get; set; }
        public byte ImalatYeri { get; set; }
        public string TramerBelgeNumarasi { get; set; }
        public string TramerBelgeTarihi { get; set; }
        public string Renk { get; set; }
        public string AnadoluMarkaKodu { get; set; }

        public List<SelectListItem> Tarzlar { get; set; }
        public List<SelectListItem> Markalar { get; set; }
        public List<SelectListItem> Tipler { get; set; }

        public string ProjeKodu { get; set; }
        public List<SelectListItem> Ams { get; set; }
        public List<SelectListItem> IkameTurleri { get; set; }
        public string IkameTuru { get; set; }

        public List<SelectListItem> AnadoluIkameTurleri { get; set; }
        public string AnadoluIkameTuru { get; set; }


        public List<SelectListItem> AnadoluKullanimTipleri { get; set; }
        public string AnadoluKullanimTipi { get; set; }

        public string AnadoluHata { get; set; }

        public List<SelectListItem> YurticiKademeler { get; set; }
    }

    public class OdemeTrafikModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }


    public class SigortaBilgileriModel
    {
        public string TrafikTescilTarihi { get; set; }
        public string PoliceBaslangicTarihi { get; set; }
        public string PoliceBitisTarihi { get; set; }
        public string AracTuru { get; set; }
        public string PlakaIli { get; set; }
        public string YetkiBelgesiVarmi { get; set; }
        public string SorumlulukVarmi { get; set; }
    }
    public class OdemeBilgileriModel
    {
        public int TeklifId { get; set; }

        public string PoliceNo { get; set; }
        public string PoliceURL { get; set; }
        public string BilgilendirmePDF { get; set; }
        public string DekontPDF { get; set; }
        public bool DekontPDFGoster { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMLogoURL { get; set; }

        public decimal? NetPrim { get; set; }
        public decimal? GiderVergisi { get; set; }
        public decimal? GH { get; set; }
        public decimal? THGF { get; set; }
        public decimal? ToplamVergi { get; set; }
        public decimal? ToplamTutar { get; set; }
        public string Aciklama { get; set; }
    }

    public class OdemeSecenekleriModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class KrediKartiOdemeModel
    {
        public int KK_TeklifId { get; set; }

        public decimal? Tutar { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KartSahibi { get; set; }

        [KrediKartiValidation]
        public KrediKartiModel KartNumarasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string GuvenlikNumarasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SonKullanmaAy { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SonKullanmaYil { get; set; }

        public List<SelectListItem> Aylar { get; set; }
        public List<SelectListItem> Yillar { get; set; }

        public byte KK_OdemeSekli { get; set; }
        public byte KK_OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }

        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }
    }

    public class AcikHesapOdemeModel
    {
        public decimal? Tutar { get; set; }

        public string AdiUnvani { get; set; }

        public string SoyadiUnvani { get; set; }
    }

    public class KrediKartiModel
    {
        public string KK1 { get; set; }
        public string KK2 { get; set; }
        public string KK3 { get; set; }
        public string KK4 { get; set; }

        public new string ToString()
        {
            return String.Format("{0}{1}{2}{3}", this.KK1, this.KK2, this.KK3, this.KK4);
        }
    }
}
