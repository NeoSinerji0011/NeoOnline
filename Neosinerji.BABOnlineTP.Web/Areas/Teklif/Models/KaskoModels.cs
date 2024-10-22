using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class KaskoModel
    {
        public bool TekrarTeklif { get; set; }
        public string ProjeKodu { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public KaskoAracBilgiModel Arac { get; set; }
        public EskiPoliceModel EskiPolice { get; set; }
        public TasiyiciSorumlulukModel Tasiyici { get; set; }
        public KaskoTeminatModel Teminat { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public KaskoTeklifOdemeModel Odeme { get; set; }
        public KaskoDainiMurtein DainiMurtein { get; set; }
        public KaskoDigerTeklifModel KaskoDigerTeklif { get; set; }
    }

    public class DetayKaskoModel
    {
        public int TeklifId { get; set; }
        public int PoliceId { get; set; }
        public string TeklifNo { get; set; }
        public string ProjeKodu { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public DetayAracBilgiModel Arac { get; set; }
        public DetayEskiPoliceModel EskiPolice { get; set; }
        public DetayTasiyiciSorumlulukModel Tasiyici { get; set; }
        public DetayKaskoTeminatModel Teminat { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public KaskoPoliceOdemeModel OdemeBilgileri { get; set; }

        public KaskoDainiMurtein DainiMurtein { get; set; }

        public KaskoDigerTeklifModel KaskoDigerTeklif { get; set; }
    }

    public class KaskoTeminatModel
    {
        public short? IMMKodu { get; set; }
        public short? FKKodu { get; set; }
        public short Kasko_Turu { get; set; }
        public short Kasko_Servis { get; set; }
        public short Kasko_Yedek_Parca { get; set; }
        public string MeslekKodu { get; set; }
        public int? NipponYetkiliIndirimi { get; set; }
        public string NipponServisTuru { get; set; }
        public List<SelectListItem> NipponServisTurleri { get; set; }
        public string NipponMuafiyetTutari { get; set; }
        public List<SelectListItem> NipponMuafiyetTutarlari { get; set; }

        public string FaaliyetKodu { get; set; }
        public bool TicariBireysel { get; set; }

        public bool GLKHHT { get; set; }
        public bool Deprem { get; set; }
        public bool Sel_Su { get; set; }
        public bool Hasarsizlik_Koruma { get; set; }
        public bool Saglik { get; set; }
        public bool Yutr_Disi_Teminat { get; set; }
        public bool LPGLi_Arac { get; set; }
        public bool Elektrikli_Arac { get; set; }
        public bool Hayvanlarin_Verecegi_Zarar_Teminati { get; set; }
        public bool Hukuksal_Koruma_Teminati { get; set; }
        public bool Eskime_Payi_Teminati { get; set; }
        public bool Alarm_Teminati { get; set; }
        public bool Anahtar_Kaybi_Teminati { get; set; }
        public bool Yangin { get; set; }
        public bool Calinma { get; set; }
        public bool KiymetKazanma { get; set; }
        public bool SigaraYanigi { get; set; }
        public bool YetkiliOlmayanCekilme { get; set; }
        public bool Seylap { get; set; }
        public bool AnahtarliCalinma { get; set; }
        public bool CamMuafiyetiKaldirilsinMi { get; set; }

        public int Yurt_Disi_Teminati_Sure { get; set; }
        public int Saglik_Kisi_Sayisi { get; set; }
        public bool LPG_Arac_Orjinalmi { get; set; }
        public LPGAracModel LPGAracModel { get; set; }
        public ElektirkliAracModel ElektirkliAracModel = new ElektirkliAracModel();

        public List<SelectListItem> Saglik_Kisi_Sayilari { get; set; }
        public List<SelectListItem> Yurtdisi_Teminat_Sureleri { get; set; }
        public List<SelectListItem> IMM { get; set; }
        public List<SelectListItem> FK { get; set; }
        public List<SelectListItem> KaskoTurleri { get; set; }
        public List<SelectListItem> KaskoServisleri { get; set; }
        public List<SelectListItem> KaskoYedekParcalari { get; set; }
        public List<SelectListItem> IkameTurleri { get; set; }
        public List<SelectListItem> MeslekKodlari { get; set; }
        public List<SelectListItem> FaaliyetKodlari { get; set; }
        public string IkameTuruGulf { get; set; }
        public List<SelectListItem> IkameTurleriGulf { get; set; }

        public string YakitTuruGulf { get; set; }
        public List<SelectListItem> YakitTurleriGulf { get; set; }

        public string MeslekKoduGulf { get; set; }
        public List<SelectListItem> MeslekListesiGulf { get; set; }

        public string ProjeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AMSKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int OlumSakatlikTeminat { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IkameTuru { get; set; }

        public bool Aksesuar_Teminati { get; set; }
        public List<MapfreAksesuarModel> Aksesuarlar { get; set; }
        public bool ElektronikCihaz_Teminati { get; set; }
        public List<MapfreAksesuarModel> Cihazlar { get; set; }

        public List<SelectListItem> AksesuarTipleri { get; set; }
        public List<SelectListItem> ElektronikCihazTipleri { get; set; }

        public bool TasinanYuk_Teminati { get; set; }
        public List<MapfreTasinanYukModel> TasinanYukler { get; set; }
        public string TasinanYukKademe { get; set; }
        public List<SelectListItem> TasinanYukTipleri { get; set; }

        [Required]
        public string TasinanYukAciklama { get; set; }
        [Required]
        public int TasinanYukBedel { get; set; }

        //Groupama Özel Sorular
        public bool KazaDestekVarMi { get; set; }
        public string AcenteOzelIndirimi { get; set; }
        public bool AsistansPlusPaketi { get; set; }
        public bool PrimKoruma { get; set; }
        public bool Kolonlar { get; set; }
        public string KolonMarka { get; set; }
        public string KolonBedel { get; set; }
        public bool PesinIndirimi { get; set; }
        public bool YurtDisiKasko { get; set; }
        public bool ElitKaskomu { get; set; }

        public bool GroupamaManeviDahilMi { get; set; }
        public int? GroupamaManeviDahilBedeli { get; set; }

        public int? GroupamaAracHukuksalKorumaBedel { get; set; }
        public int? GroupamaSurucuHukuksalKorumaBedel { get; set; }

        public string GroupamaTeminatLimiti { get; set; }
        public List<SelectListItem> GroupamaTeminatLimitleri { get; set; }

        public string GroupamaYHIMSKodu { get; set; }
        public List<SelectListItem> GroupamaYHIMSKodlari { get; set; }

        public string GroupamaYHIMSBasamakKodu { get; set; }
        public List<SelectListItem> GroupamaYHIMSBasamakKodlari { get; set; }
        public int? GroupamaYHIMSSerbestLimit { get; set; }

        public string GroupamaMeslekKodu { get; set; }
        public List<SelectListItem> GroupamaMeslekKodlari { get; set; }

        public string YakinlikDerecesi { get; set; }
        public List<SelectListItem> GroupamaYakinlikDereceleri { get; set; }

        public string RizikoFiyati { get; set; }
        public List<SelectListItem> GroupamaRizikoFiyatlari { get; set; }
        //===================Groupama Özel Sorular

        public bool ErgoHasarsizlikKoruma { get; set; }

        public string ErgoMeslekKodu { get; set; }
        public List<SelectListItem> ErgoMeslekKodlari { get; set; }

        public string ErgoServisTuru { get; set; }
        public List<SelectListItem> ErgoServisTurleri { get; set; }

        public string HKBedeli { get; set; } //Hukuksal koruma bedeli
        public List<SelectListItem> HKBedelleri { get; set; }

        public string UnicoKilitBedeli { get; set; }
        public List<SelectListItem> UnicoKilitBedelleri { get; set; }

        public bool UnicoGenisletilmisCam { get; set; }
        public bool UnicoManeviTazminat { get; set; }
        public bool EngelliAraciMi { get; set; }
        public bool UnicoHasarsizlikKorumaKlozu { get; set; }
        public bool UnicoKiralikAracmi { get; set; }

        public bool UnicoSurucuKursuAracimi { get; set; }
        public string UnicoSurucuSayisi { get; set; }
        public bool UnicoTekSurucuMu { get; set; }
        public bool UnicoTEBUyesi { get; set; }
        public bool UnicoYeniDegerklozu { get; set; }
        public string UnicoIkameSecenegi { get; set; }
        public string UnicoDepremSelMuafiyeti { get; set; }
        public List<SelectListItem> UnicoDepremSelMuafiyetleri { get; set; }
        public List<SelectListItem> UnicoIkameSecenekleri { get; set; }

        public string UnicoKaskoMuafiyeti { get; set; }
        public List<SelectListItem> UnicoKaskoMuafiyetleri { get; set; }

        public string UnicoAksesuarTuru { get; set; }
        public List<SelectListItem> UnicoAksesuarTurleri { get; set; }

        public string UnicoMeslekKodu { get; set; }
        public List<SelectListItem> UnicoMeslekler { get; set; }

        public string AxaHayatTeminatLimiti { get; set; }
        public List<SelectListItem> AxaHayatTeminatLimitleri { get; set; }
        public bool AxaAracaBagliKaravanMi { get; set; }
        public string AxaAracaBagliKaravanBedel { get; set; }
        public bool KullanimGelirKaybiVarMi { get; set; }
        public string KullanimGelirKaybiBedel { get; set; }
        public string AxaAsistansHizmeti { get; set; }
        public List<SelectListItem> AxaAsistansHizmetleri { get; set; }
        //public string AxaIkameSecimi { get; set; }
        //public List<SelectListItem> AxaIkameSecimleri { get; set; }

        public string AxaOnarimSecimi { get; set; }
        public List<SelectListItem> AxaOnarimSecimleri { get; set; }
        public bool AxaPlakaYeniKayitMi { get; set; }

        public string AxaYeniDegerKlozu { get; set; }
        public List<SelectListItem> AxaYeniDegerKlozlari { get; set; }

        public string AxaDepremSelKoasuransi { get; set; }
        public List<SelectListItem> AxaDepremSelKoasuranslari { get; set; }

        public string AxaMuafiyetTutari { get; set; }
        public List<SelectListItem> AxaMuafiyetTutarlari { get; set; }

        public bool AxaCamFilmiLogo { get; set; }

        public string AxaSorumlulukLimiti { get; set; }
        public List<SelectListItem> AxaSorumlulukLimitleri { get; set; }

        public bool SompoArtiTeminatPlani { get; set; }
        public string SompoArtiTeminatDegeri { get; set; }
        public List<SelectListItem> SompoArtiTeminatDegerleri { get; set; }
        public bool HDIRayicDegerKoruma { get; set; }
        public bool YanlisAkaryakitDolumu { get; set; }
        public bool HDIPatlayiciMadde { get; set; }
        public bool KisiselEsya { get; set; }
    }

    public class LPGAracModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Markasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? Bedeli { get; set; }
    }
    public class ElektirkliAracModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PilId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Bedeli { get; set; }
    }
    public class OdemeKaskoModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class DetayKaskoTeminatModel
    {
        public string IMM { get; set; }
        public string FK { get; set; }
        public string KaskoTurleri { get; set; }
        public string KaskoServisleri { get; set; }
        public string KaskoYedekParcalari { get; set; }
        public string SaglikKisiSayisi { get; set; }
        public string YurtDisiTeminatiSuresi { get; set; }
        public string LPGMarkasi { get; set; }
        public string LPGBedeli { get; set; }

        public bool G_L_K_H_H_T { get; set; }
        public bool Deprem { get; set; }
        public bool Sel_Su { get; set; }
        public bool HasarsizlikKoruma { get; set; }
        public bool Calinma { get; set; }
        public bool HukuksalKoruma { get; set; }
        public bool HayvanlarinVerecegiZarar { get; set; }


        public bool Alarm { get; set; }
        public bool AnahtarKaybi { get; set; }
        public bool Yangin { get; set; }
        public bool Eskime { get; set; }
        public bool Saglik { get; set; }
        public bool YurtDisiTeminati { get; set; }
        public bool AracLPGlimi { get; set; }
        //Arac lpg li ise orjinal mi sorusu sorulur
        public bool LPGOrjinalmi { get; set; }

        public string ProjeKodu { get; set; }
        public string AMS { get; set; }
        public string OlumSakatlik { get; set; }

        public string Ikame { get; set; }
    }

    public class AracDegerKisiSayisiModel
    {
        public decimal AracDeger { get; set; }
        public short KisiSayisi { get; set; }
        public List<SelectListItem> IMMList { get; set; }
        public List<SelectListItem> FKList { get; set; }
        public string ProjeKodu { get; set; }
        public bool EgmSorgu { get; set; }
    }

    public class AracDegerKontrolModel
    {
        public decimal YeniDeger { get; set; }
        public decimal OrjinalDeger { get; set; }
        public bool Result { get; set; }
        public string message { get; set; }
    }

    public class KaskoAracBilgiModel
    {
        public string PlakaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PlakaNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullanimSekliKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullanimTarziKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MarkaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Model { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TipKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TrafikTescilTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TrafigeCikisTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime PoliceBaslangicTarihi { get; set; }

        // [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilIl { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilIlce { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KisiSayisi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AracDeger { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MotorNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SaseNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilBelgeSeriKod { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilBelgeSeriNo { get; set; }

        public string AsbisNo { get; set; }
        public string AnadoluMarkaKodu { get; set; }
        public string TramerBelgeTarihi { get; set; }
        public string TramerBelgeNumarasi { get; set; }


        public List<SelectListItem> SigortaSirketleri { get; set; }
        public SelectList PlakaKoduListe { get; set; }

        public List<SelectListItem> KisiSayisiListe { get; set; }
        public List<SelectListItem> KullanimTarzlari { get; set; }
        public List<SelectListItem> KullanimSekilleri { get; set; }
        public List<SelectListItem> Markalar { get; set; }
        public List<SelectListItem> AracTipleri { get; set; }
        public List<SelectListItem> Modeller { get; set; }
        public List<SelectListItem> TescilIller { get; set; }
        public List<SelectListItem> TescilIlceler { get; set; }

        public List<SelectListItem> AnadoluKullanimTipListe { get; set; }
        public string AnadoluKullanimTip { get; set; }

        public List<SelectListItem> AnadoluKullanimSekilleri { get; set; }
        public string AnadoluKullanimSekli { get; set; }

        public string IkameTuruAnadolu { get; set; }

        public List<SelectListItem> IkameTurleriAnadolu { get; set; }

        public string HasarsizlikIndirim { get; set; }
        public string HasarSurprim { get; set; }
        public string UygulananKademe { get; set; }
        public string UygulananOncekiKademe { get; set; }
    }

    public class KaskoTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class KaskoPoliceOdemeModel
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

    public class KaskoDainiMurtein
    {
        //public bool DainiMurtein { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //[MinLength(10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength")]
        //[StringLength(10, MinimumLength = 10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength")]
        //public string KimlikNo { get; set; }

        //[Required(ErrorMessage = "Lütfen önce Daini Murtehin vergi kimlik numarasını sorgulayınız.")]
        //public string Unvan { get; set; }

        public bool DainiMurtein { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [MinLength(10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength")]
        [StringLength(10, MinimumLength = 10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength")]
        public string KimlikNo { get; set; }

        [Required(ErrorMessage = "Lütfen önce Daini Murtehin vergi kimlik numarasını sorgulayınız.")]
        public string Unvan { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [Range(1, 3, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KurumTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KurumKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KurumKodu1 { get; set; }

        public string SubeKodu { get; set; }

        public string KurumTipiAdi { get; set; }
        public string KurumAdi { get; set; }

        public SelectList KurumTipleri { get; set; }
        public SelectList Kurumlar { get; set; }
    }

    public class AracTescilTarihiModel
    {
        public string AracTrafikTescilTarihi { get; set; }
        public string MotorNo { get; set; }
        public string SasiNo { get; set; }
    }

    public class KaskoDigerTeklifModel
    {
        public bool DigerTeklifVarMi { get; set; }
        public int? TeklifId { get; set; }
        public List<DigerTeklifModel> DigerTeklifler { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
    }

    public class DigerTeklifModel
    {
        [Required]
        public string SirketKod { get; set; }
        [Required]
        public decimal TeklifTutar { get; set; }
        public decimal KomisyonTutari { get; set; }
        public int? TaksitSayisi { get; set; }

        public string SirketTeklifNo { get; set; }
        public string HasarsizlikIndirimSurprim { get; set; }
       public HttpPostedFileBase file { get; set; }
    }

    public class OnerilenUrunModel
    {
      public List<DummyPolicyApp.DumyPolicy> list = new List<DummyPolicyApp.DumyPolicy>();
    }
}