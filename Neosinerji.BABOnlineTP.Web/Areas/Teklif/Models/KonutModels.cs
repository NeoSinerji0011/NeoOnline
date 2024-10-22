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
    public class KonutModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public KonutTeklifOdemeModel Odeme { get; set; }

        public KonutRizikoAdresModel RizikoAdresBilgiler { get; set; }
        public KonutRizikoDigerBilgiler RizikoDigerBilgiler { get; set; }
        public KonutRizikoGenelBilgiler RizikoGenelBilgiler { get; set; }
        public KonutTeminatBedelBilgileri KonutTeminatBedelBilgileri { get; set; }
        public KonutTeminatBilgileri KonutTeminatBilgileri { get; set; }
        public KonutNotModel KonutNotModel { get; set; }
    }

    public class DetayKonutModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public KonutPoliceOdemeModel OdemeBilgileri { get; set; }

        public KonutRizikoAdresModel RizikoAdresBilgiler { get; set; }
        public KonutRizikoDigerBilgiler RizikoDigerBilgiler { get; set; }
        public KonutRizikoGenelBilgiler RizikoGenelBilgiler { get; set; }
        public KonutTeminatBedelBilgileri KonutTeminatBedelBilgileri { get; set; }
        public KonutTeminatBilgileri KonutTeminatBilgileri { get; set; }
        public KonutNotModel KonutNotModel { get; set; }
    }

    public class OdemeKonutModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class KonutTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class KonutPoliceOdemeModel
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

    public class KonutRizikoGenelBilgiler
    {
        //Yürürlükte Dask Poliçesi Var mı?
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool YururlukteDaskPolicesiVarmi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime? PoliceninVadeTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNumarasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? DaskSigortaBedeli { get; set; }


        //Rehinli Alacaklı (Dain-i Mürtehin) Var mı?
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool RehinliAlacakliDainMurtehinVarmi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Tipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KurumBanka { get; set; }
        public string KurumBankaAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Sube { get; set; }
        public string SubeAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KrediReferansNo_HesapSozlesmeNo { get; set; }

        public DateTime? KrediBitisTarihi { get; set; }
        public int? KrediTutari { get; set; }
        public byte? DovizKodu { get; set; }
        public string DovizKoduText { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Tipler { get; set; }
        public List<SelectListItem> DovizKodlari { get; set; }
        public List<SelectListItem> Kurum_Bankalar { get; set; }
        public List<SelectListItem> Subeler { get; set; }
    }

    public class KonutRizikoAdresModel
    {
        //Adres Bilgileri
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Il { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Ilce { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SemtBelde { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Mahalle { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Cadde { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Sokak { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Apartman { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Bina { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Daire { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? PostaKodu { get; set; }

        public string Han_Aprt_Fab { get; set; }

        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }

        public List<SelectListItem> HanAptFabList { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> Ilceler { get; set; }
    }

    public class KonutRizikoDigerBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int BelediyeKodu { get; set; }
        public string BelediyeAdi { get; set; }

        public bool CelikKapiVarMi { get; set; }
        public bool DemirParmaklikVarMi { get; set; }
        public bool OzelGuvenlikAlarmVarMi { get; set; }
        public bool KislikMi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? DaireBrutYuzOlcumu { get; set; }
        public string BosKalmaSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte? SigortalanacakYerKacinciKatta { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YapiTarzi { get; set; }

        [Range(1, 100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Between_1_and_100")]
        public int? EnflasyonOrani { get; set; }

        [Range(1, 100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Between_1_and_100")]
        public int? YillikEnflasyonKorumaOrani { get; set; }

        [Range(1, 100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Between_1_and_100")]
        public int? HasarsizlikIndirimOrani { get; set; }

        //1. Çelik - Betonarme - Karkas / 2. Diğer / 3. Yığma Kagir 
        public List<SelectListItem> YapiTarzlari { get; set; }
        public List<SelectListItem> BosKalmaSureleri { get; set; }
        public List<SelectListItem> Belediyeler { get; set; }
    }

    public class KonutTeminatBedelBilgileri
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? EsyaBedeli { get; set; }

        public int? BinaBedeli { get; set; }

    }

    public class KonutTeminatBilgileri
    {
        public bool AsistanHizmeti { get; set; }
        public bool HukuksalKoruma { get; set; }
        public bool FerdiKaza { get; set; }
        public bool FerdiKazaOlum { get; set; }
        public bool FerdiKazaSurekliSakatlik { get; set; }
    

        //Sonradan Eknendi
        public bool Medline { get; set; }
        public bool AcilTibbiHastaneFerdiKaza { get; set; }

        public bool EsyaYangin { get; set; }
        public int? EsyaYanginBedel { get; set; }

        public bool EsyaDeprem { get; set; }
        public int? EsyaDepremBedel { get; set; }

        public bool BinaYangin { get; set; }
        public int? BinaYanginBedel { get; set; }

        public bool BinaDeprem { get; set; }
        public int? BinaDepremBedel { get; set; }

        public bool Hirsizlik { get; set; }
        public int? HirsizlikBedel { get; set; }

        public bool EkTeminatEsya { get; set; }
        public int? EkTeminatEsyaBedel { get; set; }

        public bool EkTeminatBina { get; set; }
        public int? EkTeminatBinaBedel { get; set; }

        public bool YerKaymasi { get; set; }
        public int? YerKaymasiBedel { get; set; }

        public bool Firtina { get; set; }
        public int? FirtinaBedel { get; set; }

        public bool DepremYanardagPuskurmesi { get; set; }
        public int? DepremYanardagPuskurmesiBedel { get; set; }

        public bool DepremYanardagPuskurmesiEsya { get; set; }
        public int? DepremYanardagPuskurmesiEsyaBedel { get; set; }

        public bool DepremYanardagPuskurmesiBina { get; set; }
        public int? DepremYanardagPuskurmesiBinaBedel { get; set; }

        public bool SelVeSuBaskini { get; set; }
        public int? SelVeSuBaskiniBedel { get; set; }

        public bool GLKHHKNHTeror { get; set; }
        public int? GLKHHKNHTerorBedel { get; set; }

        public bool DahiliSu { get; set; }
        public int? DahiliSuBedel { get; set; }

        public bool Duman { get; set; }
        public int? DumanBedel { get; set; }

        public bool KaraTasitlariCarpmasi { get; set; }
        public int? KaraTasitlariCarpmasiBedel { get; set; }

        public bool HavaTasitlariCarpmasi { get; set; }
        public int? HavaTasitlariCarpmasiBedel { get; set; }

        public bool KarAgirligi { get; set; }
        public int? KarAgirligiBedel { get; set; }

        public bool TemellerYangin { get; set; }
        public int? TemellerYanginBedel { get; set; }

        public bool EnkazKaldirmaBina { get; set; }
        public int? EnkazKaldirmaBinaBedel { get; set; }

        public bool EnkazKaldirmaEsya { get; set; }
        public int? EnkazKaldirmaEsyaBedel { get; set; }


        //Değer giriliyor
        public bool KiraKaybi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KiraKaybiBedel { get; set; }

        public bool CamKirilmasi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? CamKirilmasiBedel { get; set; }


        public bool Kapkac { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KapkacBedel { get; set; }

        public bool MaliMesuliyetYangin { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? MaliMesuliyetYanginBedel { get; set; }


        public bool MaliSorumlulukEkTeminat { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? MaliSorumlulukEkTeminatBedel { get; set; }

        //public bool DegerliEsyaYangin { get; set; }
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public int? DegerliEsyaYanginBedel { get; set; }

        public bool IzolasOlayBsYil { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? IzolasOlayBsYilBedel { get; set; }

        public bool EkTeminatEkPrimi { get; set; }
        public int? EkTeminatEkPrimiBedel { get; set; }
    }

    public class KonutNotModel
    {
        public string Not { get; set; }
    }
}