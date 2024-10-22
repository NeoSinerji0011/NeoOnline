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
    public class IsYeriModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public IsYeriTeklifOdemeModel Odeme { get; set; }

        public IsYeriRizikoAdresModel RizikoAdresBilgiler { get; set; }
        public IsYeriRizikoDigerBilgiler RizikoDigerBilgiler { get; set; }
        public IsYeriRizikoGenelBilgiler RizikoGenelBilgiler { get; set; }
        public IsYeriTeminatBedelBilgileri IsYeriTeminatBedelBilgileri { get; set; }
        public IsYeriTeminatBilgileri IsYeriTeminatBilgileri { get; set; }
        public IsYeriNotModel IsYeriNotModel { get; set; }
    }

    public class DetayIsYeriModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public IsYeriPoliceOdemeModel OdemeBilgileri { get; set; }

        public IsYeriRizikoAdresModel RizikoAdresBilgiler { get; set; }
        public IsYeriRizikoDigerBilgiler RizikoDigerBilgiler { get; set; }
        public IsYeriRizikoGenelBilgiler RizikoGenelBilgiler { get; set; }
        public IsYeriTeminatBedelBilgileri IsYeriTeminatBedelBilgileri { get; set; }
        public IsYeriTeminatBilgileri IsYeriTeminatBilgileri { get; set; }
        public IsYeriNotModel IsYeriNotModel { get; set; }
    }

    public class OdemeIsYeriModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class IsYeriTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class IsYeriPoliceOdemeModel
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

    public class IsYeriRizikoGenelBilgiler
    {
        //Yürürlükte Dask Poliçesi Var mı?
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

    public class IsYeriRizikoAdresModel
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

    public class IsYeriRizikoDigerBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? BelediyeKodu { get; set; }


        public string BelediyeAdi { get; set; }
        public string BelediyeIlAdi { get; set; }


        //public string DepremMuafiyetOrani { get; set; }
        //public string BinaMuafiyetOrani { get; set; }
        //public string DemirbasMuafiyetOrani { get; set; }
        public int? CatiTipi { get; set; }
        public string CatiTipiText { get; set; }

        public int? KatTipi { get; set; }
        public string KatTipiText { get; set; }

        public string AsansorSayisi { get; set; }

        public int? BosKalmaSuresi { get; set; }
        public string BosKalmaSuresiText { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? IstigalKonusuKodu { get; set; }

        public string IstigalKonusuText { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? DaireBrutYuzOlcumu { get; set; }
        public int? IsYerindeCalisanSayisi { get; set; }

        public bool TemperliCam { get; set; }
        public bool BekciVarMi { get; set; }
        public bool AlarmVarMi { get; set; }
        public bool KepenkDemirVarMi { get; set; }
        public bool KameraVarMi { get; set; }
        public bool PasajIciUstKatMi { get; set; }


        public bool CelikKapiVarMi { get; set; }
        public bool DemirParmaklikVarMi { get; set; }
        public byte? SigortalanacakYerKacinciKatta { get; set; }
        public int? HasarsizlikIndirimOrani { get; set; }
        public int? YillikEnflasyonKorumaOrani { get; set; }

        [Range(1, 100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Between_1_and_100")]
        public int? EnflasyonOrani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? YapiTarzi { get; set; }

        public string YapiTarziText { get; set; }

        //1. Çelik - Betonarme - Karkas / 2. Diğer / 3. Yığma Kagir 
        public List<SelectListItem> YapiTarzlari { get; set; }
        public List<SelectListItem> IstigalKonusu { get; set; }
        public List<SelectListItem> belediyeiller { get; set; }
        public List<SelectListItem> belediyeler { get; set; }
        public List<SelectListItem> BosKalmaSureleri { get; set; }
        public List<SelectListItem> CatiTipleri { get; set; }
        public List<SelectListItem> KatTipleri { get; set; }
        public List<SelectListItem> BinaMuafiyetOranlari { get; set; }
        public List<SelectListItem> DemirbasMuafiyetOranlari { get; set; }
        public List<SelectListItem> DepremMuafiyetOranlari { get; set; }
    }

    public class IsYeriTeminatBedelBilgileri
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? DemirbasBedeli { get; set; }

        public int? BinaBedeli { get; set; }
        public int? DekorasyonBedeli { get; set; }
        public int? EmteaBedeli { get; set; }
    }

    public class IsYeriTeminatBilgileri
    {
        //ANA TEMİNATLAR
        public bool DemirbasYangin { get; set; }
        public int? DemirbasYanginBedel { get; set; }

        public bool DemirbasDeprem { get; set; }
        public int? DemirbasDepremBedel { get; set; }

        public bool BinaYangin { get; set; }
        public int? BinaYanginBedel { get; set; }

        public bool BinaDeprem { get; set; }
        public int? BinaDepremBedel { get; set; }

        public bool EmteaYangin { get; set; }
        public int? EmteaYanginBedel { get; set; }

        public bool EmteaDeprem { get; set; }
        public int? EmteaDepremBedel { get; set; }

        public bool DekorasyonYangin { get; set; }
        public int? DekorasyonYanginBedel { get; set; }

        public bool DekorasyonDeprem { get; set; }
        public int? DekorasyonDepremBedel { get; set; }

        public bool MakinaTechizat { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? MakinaTechizatBedel { get; set; }

        public bool SahisMallariYangin3 { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? SahisMallariYangin3Bedel { get; set; }

        public bool KasaMuhteviyatYangin { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KasaMuhteviyatYanginBedel { get; set; }

        public bool TemellerYangin { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? TemellerYanginBedel { get; set; }


        //EK TEMİNATLAR

        //SOL
        public bool EkTeminatMuhteviyat { get; set; }
        public int? EkTeminatMuhteviyatBedel { get; set; }

        public bool Hirsizlik { get; set; }
        public int? HirsizlikBedel { get; set; }

        public bool MakinaKirilmasi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? MakinaKirilmasiBedel { get; set; }

        public bool Firtina { get; set; }
        public int? FirtinaBedel { get; set; }

        public bool DepremYanardagPuskurmesiMuhteviyat { get; set; }
        public int? DepremYanardagPuskurmesiMuhteviyatBedel { get; set; }

        public bool SelVeSuBaskini { get; set; }
        public int? SelVeSuBaskiniBedel { get; set; }

        public bool CamKirilmasi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? CamKirilmasiBedel { get; set; }

        public bool DahiliSu { get; set; }
        public int? DahiliSuBedel { get; set; }

        public bool KaraTasitlariCarpmasi { get; set; }
        public int? KaraTasitlariCarpmasiBedel { get; set; }

        public bool KarAgirligi { get; set; }
        public int? KarAgirligiBedel { get; set; }

        public bool FerdiKaza { get; set; }
        public bool FerdiKazaOlum { get; set; }
        public bool MaliSorumlulukYangin { get; set; }

        public bool EnkazKaldirma { get; set; }
        public int? EnkazKaldirmaBedel { get; set; }

        public bool KiraKaybi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KiraKaybiBedel { get; set; }

        public bool IsverenMaliMesuliyetKazaBasinaBedeni { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? IsverenMaliMesuliyetKazaBasinaBedeniBedel { get; set; }

        public bool SahisMaliSorumlulukKisiBasinaBedeni3 { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? SahisMaliSorumlulukKisiBasinaBedeni3Bedel { get; set; }

        public bool SahisMaliSorumlulukKazaBasinaMaddi3 { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? SahisMaliSorumlulukKazaBasinaMaddi3Bedel { get; set; }

        public bool KomsulukMaliSorumlulukYanginDahiliSuDuman { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KomsulukMaliSorumlulukYanginDahiliSuDumanBedel { get; set; }

        public bool KiraciMaliSorumlulukYanginDahiliSuDuman { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KiraciMaliSorumlulukYanginDahiliSuDumanBedel { get; set; }



        //SAĞ
        public bool EkTeminatBina { get; set; }
        public int? EkTeminatBinaBedel { get; set; }

        public bool KasaHirsizlik { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KasaHirsizlikBedel { get; set; }

        public bool ElektronikCihaz { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? ElektronikCihazBedel { get; set; }

        public bool DepremYanardagPuskurmesi { get; set; }
        public int? DepremYanardagPuskurmesiBedel { get; set; }

        public bool DepremYanardagPuskurmesiBina { get; set; }
        public int? DepremYanardagPuskurmesiBinaBedel { get; set; }

        public bool GLKHHKNHTeror { get; set; }
        public int? GLKHHKNHTerorBedel { get; set; }

        public bool AsistanHizmeti { get; set; }
        public bool HukuksalKoruma { get; set; }

        public bool Duman { get; set; }
        public int? DumanBedel { get; set; }

        public bool HavaTasitlariCarpmasi { get; set; }
        public int? HavaTasitlariCarpmasiBedel { get; set; }

        public bool YazDurmasi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? YazDurmasiBedel { get; set; }

        public bool FerdiKazaSurekliSakatlik { get; set; }
        public bool MaliSorumlulukEkTeminat { get; set; }

        public bool EnkazKaldirmaBina { get; set; }
        public int? EnkazKaldirmaBinaBedel { get; set; }

        public bool YerKaymasi { get; set; }
        public int? YerKaymasiBedel { get; set; }

        public bool IsverenMaliMesuliyetKisiBasinaBedeni { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? IsverenMaliMesuliyetKisiBasinaBedeniBedel { get; set; }

        public bool SahisMaliSorumlulukKazaBasinaBedeni3 { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? SahisMaliSorumlulukKazaBasinaBedeni3Bedel { get; set; }

        public bool KomsulukMaliSorumlulukTeror { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KomsulukMaliSorumlulukTerorBedel { get; set; }

        public bool KiraciMaliSorumlulukTeror { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? KiraciMaliSorumlulukTerorBedel { get; set; }


        public bool EkTeminatEkPrimi { get; set; }
    }

    public class IsYeriNotModel
    {
        public string Not { get; set; }
    }
}