using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class KritikHastalikModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }

        public KritikHastalikGenelBilgilerModel GenelBilgiler { get; set; }
        public KritikHastalikTeminatlarModel Teminatlar { get; set; }
        public LehtarBilgileriModel Lehtar { get; set; }
        public KritikHastalikTeklifOdemeModel Odeme { get; set; }
    }

    public class DetayKritikHastalikModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class KritikHastalikGenelBilgilerModel
    {
        public byte? meslek { get; set; }
        public byte? paraBirimi { get; set; }
        public byte? sigortaSuresi { get; set; }

        public SelectList meslekler { get; set; }
        public SelectList paraBirimleri { get; set; }
        public SelectList sigortaSureleri { get; set; }

        public string paraBirimiText { get; set; }
        public string meslekText { get; set; }
        public string sigortaSuresiText { get; set; }

    }

    public class KritikHastalikTeminatlarModel
    {
        public int teminatTutari { get; set; }

        public int teminatTutariDiger { get; set; }

        public bool vefatTeminati { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? VefatBedeli { get; set; }

        public bool kazaSonucuMaluliyet { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? kazaSonucuMaluliyetBedeli { get; set; }

        public bool hastalikSonucuMaluliyet { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? hastalikSonucuMaluliyetBedeli { get; set; }

        public bool tehlikeliHastalik { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? tehlikeliHastalikBedeli { get; set; }

        public string teminatTutariText { get; set; }

        public SelectList teminatTutarlari { get; set; }
    }

    public class LehtarBilgileriModel
    {
        public byte kisiSayisi { get; set; }

        public List<LehtarBilgileri> LehterList { get; set; }
        public List<SelectListItem> kisiSayilari { get; set; }


    }

    public class LehtarBilgileri
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public DateTime DogumTarihi { get; set; }
        public int? Oran { get; set; }
    }

    public class KritikHastalikTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public bool Yenilensin { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }


    public class KritikHastalikAdimModel
    {
        public byte Adim { get; set; }
    }

}