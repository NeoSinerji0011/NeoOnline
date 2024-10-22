using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class TamamlayiciSaglikModel
    {
        public bool TekrarTeklif { get; set; }
        public TSSGenelBilgilerModel GenelBilgiler { get; set; }
        public SigortaliModel Musteri { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public TSSTeklifOdemeModel Odeme { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }

        public bool KrediKartiMi { get; set; }
        public string UrunAdi { get; set; }
    }

    public class TSSGenelBilgilerModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime PoliceBaslangicTarihi { get; set; }

        public DateTime PoliceBitisTarihi { get; set; }

        public bool YenilemeMi { get; set; }
        public DateTime? OncekiPoliceBaslangicTarihi { get; set; }
        public string OncekiSigortaSirketi { get; set; }
        public string OncekiPoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MeslekKodu { get; set; }

        public string TarifeKodu { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Boy { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Kilo { get; set; }

        public int? SigortaliSayisi { get; set; }
        public string PersonelGrubu { get; set; }
        public bool KronikHastalikCerrahisi { get; set; }
        public string KronikHastalikAciklama { get; set; }
        public byte TedaviTipi { get; set; }

        public SelectList TedaviTipleri { get; set; }
        public List<SelectListItem> MeslekKodlari = new List<SelectListItem>();
        public List<SelectListItem> TarifeKodlari = new List<SelectListItem>();
    }
    public class DetayTSSModel
    {
        public int TeklifId { get; set; }
        public int PoliceId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public TSSPoliceOdemeModel Odeme { get; set; }

        public TSSGenelBilgilerDetayModel GenelBilgiler { get; set; }
        public TSSSigortalilarList Sigortalilar { get; set; }
    }
    public class TSSGenelBilgilerDetayModel
    {
        public string PoliceBaslangicTarihi { get; set; }
        public string PoliceBitisTarihi { get; set; }
        public string YenilemeMi { get; set; }
        public string OncekiPoliceBaslangicTarihi { get; set; }
        public string OncekiSigortaSirketi { get; set; }
        public string OncekiPoliceNo { get; set; }
        public string Meslek { get; set; }
        public string Tarife { get; set; }
        public string Boy { get; set; }
        public string Kilo { get; set; }
        public string KronikHastalikCerrahisi { get; set; }
        public string KronikHastalikAciklama { get; set; }
        public string TedaviTipi { get; set; }
        public int TeklifId { get; set; }
        public decimal? NetPrim { get; set; }
        public decimal? ToplamVergi { get; set; }
        public decimal? ToplamTutar { get; set; }

    }
    public class TSSTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }
    public class TSSPoliceOdemeModel
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

    public class TSSSigortalilarList
    {
        public List<TSSSaglikSigortalilar> SigortaliList { get; set; }

        public List<SelectListItem> BireyTipleri { get; set; }

    }
    public class TSSSaglikSigortalilar
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

    public class OdemeTSSModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class PoliceBitisTarihiModel
    {
        public string PoliceBitisTarihi { get; set; }
    }
}