﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.ComponentModel.DataAnnotations;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class SeyehatSaglikModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public SeyehatSaglikTeklifOdemeModel Odeme { get; set; }

        public SeyehatSaglikGenelBilgiler GenelBilgiler { get; set; }
        public SeyehatSaglikSigortalilarList Sigortalilar { get; set; }
    }

    public class DetaySeyehatSaglikModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public SeyehatSaglikPoliceOdemeModel OdemeBilgileri { get; set; }

        public NipponSeyehatSaglikGenelBilgiler GenelBilgiler { get; set; }
        public NipponSeyehatSaglikSigortalilarList Sigortalilar { get; set; }
    }

    public class DetayNipponSeyahatModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public SeyehatSaglikPoliceOdemeModel OdemeBilgileri { get; set; }

        public NipponSeyehatSaglikGenelBilgiler GenelBilgiler { get; set; }
        public NipponSeyehatSaglikSigortalilarList Sigortalilar { get; set; }
    }

    public class OdemeSeyehatSaglikModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class SeyehatSaglikTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class SeyehatSaglikPoliceOdemeModel
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
    }

    public class SeyehatSaglikGenelBilgiler
    {
        //SigortaEttirenSigortalilardanBirimi?
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool SigortaEttirenSigortalilardanBirimi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte UlkeTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool KayakTeminati { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime SeyehatBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime SeyehatBitisTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string GidilecekUlke { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte KisiSayisi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool SigortalilarAilemi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Plan { get; set; }

        //Detay ve poliçe sayfası icin
        public string PlanTipiText { get; set; }

        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> UlkeTipleri { get; set; }
        public List<SelectListItem> KisiSayilari { get; set; }
        public List<SelectListItem> Planlar { get; set; }
    }

    public class SeyehatSaglikSigortalilarList
    {
        public List<SeyehatSaglikSigortalilar> SigortaliList { get; set; }

        public List<SelectListItem> BireyTipleri { get; set; }

    }

    public class SeyehatSaglikSigortalilar
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

  
}