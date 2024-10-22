using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.ComponentModel.DataAnnotations;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class KorunanGelecekModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public KorunanGelecekTeklifOdemeModel Odeme { get; set; }

        public KorunanGelecekGenelBilgiler GenelBilgiler { get; set; }
        public KorunanGelecekAnaTeminatlar AnaTeminatlar { get; set; }
        public KorunanGelecekEkTeminatlar EkTeminatlar { get; set; }
    }

    public class DetayKorunanGelecekModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }

        public KorunanGelecekGenelBilgiler GenelBilgiler { get; set; }
        public KorunanGelecekAnaTeminatlar AnaTeminatlar { get; set; }
        public KorunanGelecekEkTeminatlar EkTeminatlar { get; set; }
    }

    public class OdemeKorunanGelecekModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class KorunanGelecekTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class KorunanGelecekGenelBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime SigortaBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte? SigortaSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte? PrimOdemeDonemi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte? ParaBirimi { get; set; }

        public SelectList ParaBirimleri { get; set; }
        public SelectList PrimDonemleri { get; set; }

        public string PrimOdemeDonemiText { get; set; }
        public string ParaBirimiText { get; set; }
    }

    public class KorunanGelecekSigortalilar
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

    public class KorunanGelecekAnaTeminatlar
    {
        public bool Vefat { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? VefatBedeli { get; set; }
    }

    public class KorunanGelecekEkTeminatlar
    {
        public bool MaluliyetYillikDestekTeminati { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? MaluliyetYillikDestekTeminatiBedeli { get; set; }
    }

    public class KGDetayPartialModel
    {
        public string AdSoyad { get; set; }
        public string SigortaBaslangicTarihi { get; set; }
        public string SigortaSuresi { get; set; }
        public string PrimOdemeDonemi { get; set; }
        public string ParaBirimi { get; set; }

        //ANA TEMINAT
        public bool AnaTeminat { get; set; }
        public string AnaTeminatTutar { get; set; }
        //EK TEMINATLAR
        public bool MaluliyetYillikDestek { get; set; }
        public string MaluliyetYillikDestekTutar { get; set; }
    }
}







