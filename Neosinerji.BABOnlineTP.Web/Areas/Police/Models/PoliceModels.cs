using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Police.Models
{
    //Police Arama Ekranında Kullanilan Model
    public class PoliceAramaModel
    {
        public string TeklifNo { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int? TUMKodu { get; set; }
        public int UrunKodu { get; set; }
        public string PoliceNo { get; set; }
        public int HazirlayanKodu { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }


        public List<SelectListItem> Kullanicilar { get; set; }
        public List<SelectListItem> TUMler { get; set; }
        public List<SelectListItem> Urunler { get; set; }
        public SelectList Durumlar { get; set; }
        public Nullable<byte> MuhasebeyeAktarildiMi { get; set; }
    }
    public class PoliceOffLineModel
    {
        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
        public List<SelectListItem> AcenteBankaHesapList { get; set; }
        public int TVMKodu { get; set; }
        public string TvmUnvani { get; set; }
        public string TcknVkn { get; set; }
        public string PoliceNo { get; set; }
        public string PlakaNo { get; set; }
        public string PlakaKodu { get; set; }
        public string Unvan { get; set; }
        public string UnvanFirma { get; set; }
        public string UnvanSoyad { get; set; }
        public byte Durum { get; set; }
        public SelectList Durumlar { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public string Donem { get; set; }
        public List<PoliceAraRaporOffLineModel> listPolOffline { get; set; }
        public Nullable<byte> MuhasebeyeAktarildiMi { get; set; }
        public Nullable<byte> ManuelPoliceMi { get; set; }
        
       
    }
    public class PoliceAraRaporOffLineModel
    {
        public string PoliceNo { get; set; }
        public int PoliceId { get; set; }
        public string TcknVkn { get; set; }
        public string SigortaEttirenUnvani { get; set; }
        public string SigortaliUnvani { get; set; }

        public string YenilemeNo { get; set; }
        public string EkNo { get; set; }
        public string UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public string BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string TaliAcenteKodu { get; set; }
        public string TaliAcenteAdi { get; set; }
        public string DisKaynakKodu { get; set; }
        public string DisKaynakAdi { get; set; }
        public string SigortaSirketi { get; set; }
        public string AcenteKodu { get; set; }
        public string AcenteUnvani { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal? NetPrim { get; set; }
        public decimal? BrütPrim { get; set; }
        public decimal? Komisyon { get; set; } // Tali acente bu alanı göremiyecek. Merkez acentenin aldığı komisyon.
       
        public decimal? VerilenKomisyon { get; set; } //Tali acenteye verilen komisyon.
        public decimal? DovizliNetPrim { get; set; }
        public decimal? DovizliBrütPrim { get; set; }
        public decimal? DovizliKomisyon { get; set; }
        public string PlakaNo { get; set; }  /// Kasko ve trafik branşında gösterilecek. Diğer branşarlar için boş.
        public string PlakaKodu { get; set; }
        public string OdemeTipi { get; set; }
        public string OdemeSekli { get; set; }
        public int TaksitSayisi { get; set; }
        public string OdemeTipim { get; set; }
        public string IlIlce { get; set; } 
        public int Yeni_is { get; set; }

        public Nullable<byte> MuhasebeyeAktarildiMi { get; set; }
        public Nullable<byte> ManuelPoliceMi { get; set; }
        public string OnaylayanUnvan { get; set; }
        public object AdiSoyadi { get; set; }
        public object SigortaliKimlikNo { get; set; }
    }

    public class PoliceUretimAramaModel
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int TUMKodu { get; set; }
        public int BransKodu { get; set; }
        public SelectList RaporTipleri { get; set; }
        public byte RaporTipi { get; set; }
        public DateTime Donem { get; set; }
        public bool BolgeYetkilisiMi { get; set; }
        public int DisUretimTVMKodu { get; set; }
        public string DisUretimTVMUnvani { get; set; }

        //public List<SelectListItem> Aylar { get; set; }
        //public int Ay { get; set; }
        //public List<SelectListItem> Yillar { get; set; }
        //public int Yil { get; set; }

        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        public string BransList { get; set; }
        public string TVMListe { get; set; }
        public string UretimTVMListe { get; set; }
        public string SigortaSirket { get; set; }

        public MultiSelectList SigortaSirketleri { get; set; }
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] SigortaSirketleriSelectList { get; set; }

        public MultiSelectList Branslar { get; set; }
        // [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] BranslarSelectList { get; set; }

        public MultiSelectList tvmler { get; set; }
        // [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] tvmList { get; set; }

        public MultiSelectList uretimTvmler { get; set; }
        // [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] uretimTvmList { get; set; }

        public List<PoliceUretimRaporProcedureModel> raporList { get; set; }
        public List<PoliceUretimGrupModel> raporGrupList { get; set; }

        public PoliceUretimGrupToplamModel raporGrupToplamlar { get; set; }
        public List<PoliceUretimGrupToplamModel> raporGrupToplamlarList { get; set; }

    }

    public class PoliceUretimGrupModel
    {
        public int PoliceId { get; set; }
        public string SirketAdi { get; set; }
        public string BransAdi { get; set; }
        public decimal? NetToplam { get; set; }
        public decimal? BrutToplam { get; set; }
        public decimal? Komisyon { get; set; }
        public decimal? VerilenToplam { get; set; }
        public decimal? NetToplamIptal { get; set; }
        public decimal? BrutToplamIptal { get; set; }
        public decimal? KomisyonIptal { get; set; }
        public decimal? VerilenToplamIptal { get; set; }

        public decimal? TotalNetToplam { get; set; }
        public decimal? TotalBrutToplam { get; set; }
        public decimal? TotalKomisyon { get; set; }
        public decimal? TotalVerilenToplam { get; set; }
        public List<PoliceUretimRaporProcedureModel> list { get; set; }
    }

    public class PoliceUretimGrupToplamModel
    {
        public string SirketAdi { get; set; }
        public decimal? SirketNetToplam { get; set; }
        public decimal? SirketBrutToplam { get; set; }
        public decimal? SirketKomisyon { get; set; }
        public decimal? SirketVerilenToplam { get; set; }
        public decimal? IptalSirketNetToplam { get; set; }
        public decimal? IptalSirketBrutToplam { get; set; }
        public decimal? IptalSirketKomisyon { get; set; }
        public decimal? IptalSirketVerilenToplam { get; set; }

        public string BransAdi { get; set; }

        public decimal? GenelNetToplam { get; set; }
        public decimal? GenelBrutToplam { get; set; }
        public decimal? GenelKomisyon { get; set; }
        public decimal? GenelVerilenToplam { get; set; }
        public decimal? IptalGenelNetToplam { get; set; }
        public decimal? IptalGenelBrutToplam { get; set; }
        public decimal? IptalGenelKomisyon { get; set; }
        public decimal? IptalGenelVerilenToplam { get; set; }

        public decimal? TotalGenelNetToplam { get; set; }
        public decimal? TotalGenelBrutToplam { get; set; }
        public decimal? TotalGenelKomisyon { get; set; }
        public decimal? TotalGenelVerilenToplam { get; set; }
    }

    public class PoliceUretimIcmalModel
    {
        //public DateTime BaslangicTarih { get; set; }
        //public DateTime BitisTarih { get; set; }

        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }
        public List<SelectListItem> Yillar { get; set; }
        public int Yil { get; set; }

        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public int TUMKodu { get; set; }
        public int BransKodu { get; set; }
        public SelectList RaporTipleri { get; set; }
        public byte RaporTipi { get; set; }

        public string BransList { get; set; }
        public string TVMListe { get; set; }
        public string SigortaSirket { get; set; }
        public string UretimTVMListe { get; set; }


        public decimal? IptalKomisyon { get; set; }
        public decimal? TahakkukKomisyon { get; set; }
        public decimal? IptalBrutPrim { get; set; }
        public decimal? TahakkukBrutPrim { get; set; }
        public decimal? IptalToplamVergi { get; set; }
        public decimal? TahakkukToplamVergi { get; set; }
        public decimal? IptalVerilenKomisyon { get; set; }
        public decimal? TahakkukVerilenKomisyon { get; set; }

        public bool BolgeYetkilisiMi { get; set; }

        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }

        public MultiSelectList Branslar { get; set; }
        public string[] BranslarSelectList { get; set; }

        public MultiSelectList tvmler { get; set; }
        public string[] tvmList { get; set; }

        public MultiSelectList uretimTvmler { get; set; }
        public string[] uretimTvmList { get; set; }

        public List<PoliceUretimIcmalRaporProcedureModel> procedureList { get; set; }
        public PoliceUretimIcmalGrupToplamModel raporGrupToplamlar { get; set; }
        public List<PoliceUretimIcmalGrupToplamModel> raporGrupToplamlarList { get; set; }
    }
    public class PoliceUretimIcmalGroupModel
    {
        public string SirketAdi { get; set; }
        public string BransAdi { get; set; }
        public decimal? BrutToplam { get; set; }
        public decimal? VergiToplam { get; set; }
        public decimal? VergiToplamIptal { get; set; }
        public decimal? Komisyon { get; set; }
        public decimal? KomisyonToplam { get; set; }
        public decimal? VerilenToplam { get; set; }
        public decimal? BrutToplamIptal { get; set; }
        public decimal? VerilenKomisyonToplam { get; set; }
        public decimal? KomisyonIptal { get; set; }
        public decimal? VerilenToplamIptal { get; set; }
        public decimal? VerilenKomisyonIptal { get; set; }

        public List<PoliceUretimIcmalRaporProcedureModel> liste { get; set; }

    }
    public class PoliceUretimIcmalGrupToplamModel
    {
        public string SirketAdi { get; set; }
        public decimal? SirketBrutToplam { get; set; }
        public decimal? SirketKomisyon { get; set; }
        public decimal? SirketVerilenKomisyonToplam { get; set; }
        public decimal? SirketVergiToplam { get; set; }
        public decimal? SirketKomisyonToplam { get; set; }
        public decimal? KomisyonToplam { get; set; }
        public decimal? IptalSirketBrutToplam { get; set; }
        public decimal? IptalSirketKomisyonToplam { get; set; }
        public decimal? IptalSirketVerilenKomisyonToplam { get; set; }
        public decimal? IptalSirketVergiToplam { get; set; }
        public string BransAdi { get; set; }
        public decimal? VergiToplam { get; set; }
        public decimal? VergiToplamIptal { get; set; }
        public decimal? VerilenKomisyonToplam { get; set; }
        public decimal? GenelBrutToplam { get; set; }
        public decimal? GenelKomisyonToplam { get; set; }
        public decimal? GenelVerilenKomisyonToplam { get; set; }
        public decimal? GenelVergiToplam { get; set; }

        public decimal? IptalGenelBrutToplam { get; set; }
        public decimal? IptalGenelKomisyonToplam { get; set; }
        public decimal? IptalGenelVerilenKomisyonToplam { get; set; }
        public decimal? IptalGenelVergiToplam { get; set; }
    }


    public class PoliceUretimIcmalToplamModel
    {
        public string SirketAdi { get; set; }
        public decimal? SirketNetToplam { get; set; }
        public decimal? SirketBrutToplam { get; set; }
        public decimal? SirketKomisyon { get; set; }
        public decimal? SirketVerilenToplam { get; set; }

        public decimal? IptalSirketBrutToplam { get; set; }
        public decimal? IptalSirketKomisyon { get; set; }
        public decimal? IptalSirketVerilenToplam { get; set; }
        public decimal? KomisyonToplam { get; set; }
        public string BransAdi { get; set; }
        public decimal? VergiToplam { get; set; }
        public decimal? VergiToplamIptal { get; set; }
        public decimal? VerilenKomisyonIptal { get; set; }
        public decimal? VerilenKomisyonToplam { get; set; }
        public decimal? GenelBrutToplam { get; set; }
        public decimal? GenelVergiToplam { get; set; }
        public decimal? GenelKomisyon { get; set; }
        public decimal? GenelVerilenKomisyonToplam { get; set; }

        public decimal? IptalGenelBrutToplam { get; set; }
        public decimal? IptalGenelKomisyon { get; set; }
        public decimal? IptalGenelVergiToplam { get; set; }
        public decimal? IptalGenelVerilenKomisyonToplam { get; set; }
    }


    public class PoliceHedefModel
    {
        public int AcenteTVMKodu { get; set; }
        public string AcenteTVMUnvani { get; set; }
        public List<SelectListItem> Yillar { get; set; }
        public int Yil { get; set; }

        public List<PoliceHedefRaporModel> Rapor { get; set; }
        public List<PoliceHedefRaporEkranModel> RaporEkranList { get; set; }
        public PoliceHedefRaporEkranModel RaporEkran { get; set; }
    }


    public class PoliceUretimRaporViewModel
    {
        public string SirketAdi { get; set; }
        public string BransAdi { get; set; }
        public int? TaliKodu { get; set; }
        public string TaliUnvani { get; set; }
        public string PoliceNumarasi { get; set; }
        public int? YenilemeNo { get; set; }
        public int? EkNo { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal? NetPrim { get; set; }
        public decimal? BrutPrim { get; set; }
        public Nullable<decimal> Komisyon { get; set; }
        public Nullable<decimal> TaliKomisyon { get; set; }

        public string Unvani { get; set; }
        public int? TaliTVMKodu { get; set; }
        public string TaliTVMUnvani { get; set; }
        public int? BransKodu { get; set; }
        public string TUMBransKodu { get; set; }
        public string TUMBransAdi { get; set; }

    }

    public class PoliceUretimRaporGroupModel
    {
        public string SirketAdi { get; set; }
        public string BransAdi { get; set; }
        public List<PoliceUretimRaporViewModel> GroupList { get; set; }

    }
    public class PoliceTahsilatEkranModel
    {
        public int PoliceId { get; set; }
        public string KimlikNo { get; set; }
        public string PoliceNo { get; set; }
        public string ZeyilNo { get; set; }
        public decimal BrutPrim { get; set; }
        public int OdemTipi { get; set; }
        public int TaksitNo { get; set; }
        public System.DateTime TaksitVadeTarihi { get; set; }
        public decimal TaksitTutari { get; set; }
        public decimal OdenenTutar { get; set; }
        public string OdemeBelgeNo { get; set; }
        public System.DateTime OdemeBelgeTarihi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public System.DateTime GuncellemeTarihi { get; set; }
        public int KaydiEkleyenKullaniciKodu { get; set; }
        public virtual PoliceGenel PoliceGenel { get; set; }
    }
    public class PoliceTahsilatRaporModel
    {
        public byte AramaTip { get; set; }
        public SelectList AramaTipTipleri { get; set; }
        public string MusteriGurpKodu { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public MultiSelectList tvmler { get; set; }
        public string[] tvmList { get; set; }
        public string TVMListe { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public string SigortaSirket { get; set; }
        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }

        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }
        public string tcVkn { get; set; }
        public SelectList RaporTipleri { get; set; }
        public byte RaporTipim { get; set; }
        public List<PoliceTahsilatRaporProcedureModel> procedureTahsilatList { get; set; }
        public MultiSelectList DisKaynakTvmler { get; set; }
        public string[] DisKaynakTvmList { get; set; }
        public string DisKaynakListesi { get; set; }
    }

    public class PoliceGirisiModel
    {
        public PoliceGirisiModel()
        {
            TaksitSatirlar = new List<TaksitModel>();
        }
        public byte? Durum { get; set; }
        public int? PoliceId { get; set; }
        public int? MuhasebeyeAktarildiMi { get; set; }
        public string TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string DisKaynakKodu { get; set; }
        public string DisKaynakUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }
        public string SigortaSirketiUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }
        public string BransAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YenilemeNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkNo { get; set; }
        public string ZeylAciklama { get; set; }

        public DateTime TanzimTarihi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        //Partial View dan Poliçe Arama sonucunda kullanılıyor
        public string tTarihi { get; set; }
        public string bTarihi { get; set; }
        public string bitTarihi { get; set; }

        public string ParaBirimi { get; set; }
        public string Yeni_is { get; set; }
        public string DovizKuru { get; set; }
        public string NetPrim { get; set; }
        public string NetPrimTL { get; set; }
        public string BrutPrim { get; set; }
        public string BrutPrimTL { get; set; }
        public string KomisyonTutari { get; set; }
        public string KomisyonTutariTL { get; set; }
        public string VergiTutari { get; set; }
        public string VergiTutariTL { get; set; }
        public string THG { get; set; }
        public string THGFonuTL { get; set; }
        public string GiderVergisi { get; set; }
        public string GiderVergisiTL { get; set; }
        public string KTGS { get; set; }
        public string KTGSHesabiTL { get; set; }
        public string YSV { get; set; }
        public string YSVTL { get; set; }

        public bool SigortaliSigortaEttirenAynimi { get; set; }
        public Sigortali Sigortali { get; set; }
        public SigortaEttiren SigortaEttiren { get; set; }

        public string DainiMurteinSubeAdi { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNumarasi { get; set; }
        public string TescilBelgeSeriKodu { get; set; }
        public string TescilBelgeSeriNo { get; set; }
        public string AsbisNo { get; set; }

        public string IslemMesaji { get; set; }

        public SelectList ParaBirimleri { get; set; }
        public SelectList YeniIsList { get; set; }

        public List<SelectListItem> Tvmler { get; set; }
        public List<SelectListItem> DisKaynaklar { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }

        public byte PoliceTipi { get; set; }
        public List<SelectListItem> PoliceTipleri { get; set; }

        public bool OdemeSekli { get; set; }
        public byte TaksitSayisi { get; set; }

        public byte OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<TaksitModel> TaksitSatirlar { get; set; }
        

    }
    public class ManuelTeklifPoliceGirisi
    {
        public ManuelTeklifPoliceGirisi()
        {
            TaksitSatirlar = new List<TaksitModel>();
        }
        public string TeklifUrl { get; set; }
        public string PoliceUrl { get; set; }
        public string DebitNoteUrl { get; set; }
        public string CreditNoteUrl { get; set; }
        public HttpPostedFileBase fileTeklif { get; set; }
        public HttpPostedFileBase filePolice { get; set; }
        public HttpPostedFileBase fileDebitNote { get; set; }
        public HttpPostedFileBase fileCreditNote { get; set; }
        public int? PoliceId { get; set; }
        public int? TeklifId { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DisKaynakKodu { get; set; }
        public string DisKaynakUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }
        public string SigortaSirketiUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }
        public string BransAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YenilemeNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkNo { get; set; }
        public string YurtdisiPoliceNo { get; set; }
        public string Aciklama { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TanzimTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BaslangicTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BitisTarihi { get; set; }
        public string PoliceBaslangicSaat { get; set; }
        public string PoliceBitisSaat { get; set; }

        //Partial View dan Poliçe Arama sonucunda kullanılıyor
        public string tTarihi { get; set; }
        public string bTarihi { get; set; }
        public string bitTarihi { get; set; }

        public string ParaBirimi { get; set; }
        public string DovizKuru { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TeminatTutari { get; set; }
        public string TeminatTutariTL { get; set; }
        public string YurtdisiPrim { get; set; }
        public string YurtdisiPrimTL { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YurtdisiDisKaynakKomisyon { get; set; }
        public string YurtdisiDisKaynakKomisyonTL { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YurtdisiAlinanKomisyon { get; set; }
        public string YurtdisiAlinanKomisyonTL { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string FrontingSigortaSirketiKomisyon { get; set; }
        public string FrontingSigortaSirketiKomisyonTL { get; set; }
        public string SatisKanaliKomisyon { get; set; }
        public string SatisKanaliKomisyonTL { get; set; }
        public string YurticiAlinanKomisyon  { get; set; }
        public string YurticiAlinanKomisyonTL  { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YurtdisiNetPrim { get; set; }
        public string YurtdisiNetPrimTL { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YurtdisiBrokerNetPrim { get; set; }
        public string YurtdisiBrokerNetPrimTL { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YurticiNetPrim { get; set; }
        public string YurticiNetPrimTL { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")] 
        public string YurticiBrutPrim { get; set; }
        public string YurticiBrutPrimTL { get; set; }

        public string Bsmv { get; set; }
        public string BsmvTL { get; set; }

        public bool SigortaliSigortaEttirenAynimi { get; set; }
        public Sigortali Sigortali { get; set; }
        public SigortaEttiren SigortaEttiren { get; set; }

        public string DainiMurteinSubeAdi { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNumarasi { get; set; }
        public string TescilBelgeSeriKodu { get; set; }
        public string TescilBelgeSeriNo { get; set; }
        public string AsbisNo { get; set; }

        public string IslemMesaji { get; set; }

        public SelectList ParaBirimleri { get; set; }
        public List<SelectListItem> Tvmler { get; set; }
        public List<SelectListItem> DisKaynaklar { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> UnderwriterListesi { get; set; }
        public List<SelectListItem> Branslar { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte PoliceTipi { get; set; }
        public List<SelectListItem> PoliceTipleri { get; set; }

        public bool OdemeSekli { get; set; }
        public byte TaksitSayisi { get; set; }
        public byte UnderwriterSayisi { get; set; }

        public byte OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<TaksitModel> TaksitSatirlar { get; set; }

        public List<SelectListItem> UnderwriterSayilari { get; set; }
        public List<UnderwriterModel> UnderwriterSatirlar { get; set; }
        public SelectList durumlar { get; set; }
        public byte durum { get; set; }
        public string sistemTeklifNo { get; set; }
        public Nullable<byte> MuhasebeyeAktarildiMi { get; set; }
        public string EMail { get; set; } = "";

    }
    public class PoliceAraReasurorCariHareketGirisiModel
    {
        public PoliceAraReasurorCariHareketGirisiModel()
        {
            TaksitSatirlar = new List<TaksitModel>();
        }
        
      
        public int? PoliceId { get; set; }
        public int? TeklifId { get; set; } 
        public string TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
         
        public string DisKaynakKodu { get; set; }
        public string DisKaynakUnvani { get; set; }
         
        public string SigortaSirketiKodu { get; set; }
        public string SigortaSirketiUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }
        public string BransAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YenilemeNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkNo { get; set; }
        public string YurtdisiPoliceNo { get; set; }
        public string Aciklama { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TanzimTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BaslangicTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BitisTarihi { get; set; }
        public string PoliceBaslangicSaat { get; set; }
        public string PoliceBitisSaat { get; set; }

        //Partial View dan Poliçe Arama sonucunda kullanılıyor
        public string tTarihi { get; set; }
        public string bTarihi { get; set; }
        public string bitTarihi { get; set; }

        public string ParaBirimi { get; set; }
        public string DovizKuru { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
         
        public string DainiMurteinSubeAdi { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNumarasi { get; set; }
        public string TescilBelgeSeriKodu { get; set; }
        public string TescilBelgeSeriNo { get; set; }
        public string AsbisNo { get; set; }

        public string IslemMesaji { get; set; }

        public SelectList ParaBirimleri { get; set; }
        public List<SelectListItem> Tvmler { get; set; }
        public List<SelectListItem> DisKaynaklar { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> UnderwriterListesi { get; set; }
        public List<SelectListItem> Branslar { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte PoliceTipi { get; set; }
        public List<SelectListItem> PoliceTipleri { get; set; }

        public bool OdemeSekli { get; set; }
        public byte TaksitSayisi { get; set; }
        public byte UnderwriterSayisi { get; set; }

        public byte OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<TaksitModel> TaksitSatirlar { get; set; }

        public List<SelectListItem> UnderwriterSayilari { get; set; }
        public List<UnderwriterModel> UnderwriterSatirlar { get; set; }
        public List<PoliceYaslandirmaTablosuModelItem> UWBrokerList = new List<PoliceYaslandirmaTablosuModelItem>();
        public bool UwVarmi { get; set; } = false;
        public Nullable<byte> MuhasebeyeAktarildiMi { get; set; }

    }

    public class Sigortali
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KimlikNo { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
    }
    public class SigortaEttiren
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KimlikNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Soyadi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Telefon { get; set; } = "";
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adres { get; set; } = "-";
    }
    public class TaksitModel
    {
        public int TaksitNo { get; set; }
        public string VadeTarihi { get; set; }
        public string TaksitTutari { get; set; }
        public string TaksitTutariTL { get; set; }
    }
    public class UnderwriterModel
    {
        public string UnderwriterKodu { get; set; }
        public string UnderwriterAdi { get; set; }
        public decimal? UnderwriterPayOrani { get; set; }
        public decimal? UnderwriterPrim { get; set; }
        public decimal? UnderwriterKomisyonOrani { get; set; }
        public decimal? UnderwriterKomisyon { get; set; }
        //UnderwriterAdi UnderwriterPayOrani  UnderwriterPrim UnderwriterKomisyonOrani UnderwriterKomisyon
    }
    public class PoliceDokumanModel
    {
        public int PoliceId { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<int> YenilemeNo{ get; set; }
        public Nullable<int> EkNo{ get; set; }
        public Nullable<System.DateTime> TanzimTarihi { get; set; }
        public Nullable<System.DateTime> BaslangicTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
        public string SigortaSirketi { get; set; }
        public string SigortaliTcknVkn { get; set; }
        public string SigortaliAdi { get; set; }
        public string SigortaEttirenTcknVkn { get; set; }
        public string SigortaEttirenAdi { get; set; }
        public List<PoliceDokuman> PoliceDokumanlar { get; set; }
        public string DokumanAdi { get; set; }
        public string IslemMesaji { get; set; }
        public HttpPostedFileBase Dokuman { get; set; }
    }
    public class ManuelPoliceGirisiMusteriBilgileri
    {
        public string ad { get; set; }
        public string soyad { get; set; }
        public string tcknVkn { get; set; }
        public string adres { get; set; }
        public string tel { get; set; }
    }
    public class PoliceAraPartialModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YenilemeNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkNo { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }
    }

    public class PoliceAraReasurorPartialModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YenilemeNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkNo { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }
    }
    public class PoliceAraReasurorCariHareketGirisModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YenilemeNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int AramaTuru { get; set; } //0:Underwriter arama,1-broker arama

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }
    }
    public class PoliceEkiReasurorAraPartialModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YenilemeNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkNo { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }
    }
    public class TeklifAraReasurorPartialModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }
        public string TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BransKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TeklifNo { get; set; }

        public List<SelectListItem> Tvmler { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }
    }
    public class PoliceListesiOfflineModelim
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string TVMListe { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public string BransList { get; set; }
        public string SigortaSirket { get; set; }

        public bool BolgeYetkilisiMi { get; set; }

        public byte AramaTip { get; set; }
        public SelectList AramaTipTipleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }
        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] BransSelectList { get; set; }
        public MultiSelectList BranslarItems { get; set; }

        public MultiSelectList Branslar { get; set; }
        public byte PoliceTarihTipi { get; set; }

        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }


        public string[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }
        public MultiSelectList uretimTvmler { get; set; }

        public string[] uretimTvmList { get; set; }
        public string UretimTVMListe { get; set; }

        public List<SelectListItem> PoliceTarihiTipleri { get; set; }
        public List<PoliceListesiOfflineRaporProcedureModel> procedurePoliceOfflineList { get; set; }
        public string PDFURL { get; set; }
        public bool pdfVar { get; set; }
    }
    public class TeklifPoliceListesi
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string TVMListe { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public string BransList { get; set; }
        public string SigortaSirket { get; set; }

        public bool BolgeYetkilisiMi { get; set; }

        public string[] SigortaSirketleriSelectList { get; set; }
        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] BransSelectList { get; set; }
        public MultiSelectList BranslarItems { get; set; }


        public MultiSelectList Branslar { get; set; }
        public byte PoliceTarihTipi { get; set; }

        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }


        public string[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }
        public MultiSelectList uretimTvmler { get; set; }

        public string[] uretimTvmList { get; set; }
        public string UretimTVMListe { get; set; }

        public List<SelectListItem> PoliceTarihiTipleri { get; set; }
        public List<ReasurorPoliceListesiProcedureModel> procedurePoliceOfflineList { get; set; }

        public List<ReasurorTeklifListesiProcedureModel> procedureTeklifOfflineList { get; set; }

        public SelectList durumlar { get; set; }
        public byte durum { get; set; }

        public int? GenelPolToplamSayac { get; set; }
        public int? GenelZeylToplamSayac { get; set; }

        // tl tahakkuk
        public decimal? ToplamTeminatTutariTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiDisKaynakKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiAlinanKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamFrontingSigortaSirketiKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamSatisKanaliKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamYurticiAlinanKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiNetPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiBrokerNetPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurticiNetPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurticiBrutPrimTLTahakkuk { get; set; }
        public decimal? ToplamBsmvTLTahakkuk { get; set; }

        // tl iptal

        public decimal? ToplamTeminatTutariTLIptal { get; set; }
        public decimal? ToplamYurtdisiPrimTLIptal { get; set; }
        public decimal? ToplamYurtdisiDisKaynakKomisyonTLIptal { get; set; }
        public decimal? ToplamYurtdisiAlinanKomisyonTLIptal { get; set; }
        public decimal? ToplamFrontingSigortaSirketiKomisyonTLIptal { get; set; }
        public decimal? ToplamSatisKanaliKomisyonTLIptal { get; set; }
        public decimal? ToplamYurticiAlinanKomisyonTLIptal { get; set; }
        public decimal? ToplamYurtdisiNetPrimTLIptal { get; set; }
        public decimal? ToplamYurtdisiBrokerNetPrimTLIptal { get; set; }
        public decimal? ToplamYurticiNetPrimTLIptal { get; set; }
        public decimal? ToplamYurticiBrutPrimTLIptal { get; set; }
        public decimal? ToplamBsmvTLIptal { get; set; }

        // tl toplam

        public decimal? ToplamTeminatTutariTL { get; set; }
        public decimal? ToplamYurtdisiPrimTL { get; set; }
        public decimal? ToplamYurtdisiDisKaynakKomisyonTL { get; set; }
        public decimal? ToplamYurtdisiAlinanKomisyonTL { get; set; }
        public decimal? ToplamFrontingSigortaSirketiKomisyonTL { get; set; }
        public decimal? ToplamSatisKanaliKomisyonTL { get; set; }
        public decimal? ToplamYurticiAlinanKomisyonTL { get; set; }
        public decimal? ToplamYurtdisiNetPrimTL { get; set; }
        public decimal? ToplamYurtdisiBrokerNetPrimTL { get; set; }
        public decimal? ToplamYurticiNetPrimTL { get; set; }
        public decimal? ToplamYurticiBrutPrimTL { get; set; }
        public decimal? ToplamBsmvTL { get; set; }

        //Dolar
        public decimal? ToplamDolarTeminatTutari { get; set; }
        public decimal? ToplamDolarYurtdisiPrim { get; set; }
        public decimal? ToplamDolarYurtdisiDisKaynakKomisyon { get; set; }
        public decimal? ToplamDolarYurtdisiAlinanKomisyon { get; set; }
        public decimal? ToplamDolarFrontingSigortaSirketiKomisyon { get; set; }
        public decimal? ToplamDolarSatisKanaliKomisyon { get; set; }
        public decimal? ToplamDolarYurticiAlinanKomisyon { get; set; }
        public decimal? ToplamDolarYurtdisiNetPrim { get; set; }
        public decimal? ToplamDolarYurtdisiBrokerNetPrim { get; set; }
        public decimal? ToplamDolarYurticiNetPrim { get; set; }
        public decimal? ToplamDolarYurticiBrutPrim { get; set; }
        public decimal? ToplamDolarBsmv { get; set; }

        //euro
        public decimal? ToplamEuroTeminatTutari { get; set; }
        public decimal? ToplamEuroYurtdisiPrim { get; set; }
        public decimal? ToplamEuroYurtdisiDisKaynakKomisyon { get; set; }
        public decimal? ToplamEuroYurtdisiAlinanKomisyon { get; set; }
        public decimal? ToplamEuroFrontingSigortaSirketiKomisyon { get; set; }
        public decimal? ToplamEuroSatisKanaliKomisyon { get; set; }
        public decimal? ToplamEuroYurticiAlinanKomisyon { get; set; }
        public decimal? ToplamEuroYurtdisiNetPrim { get; set; }
        public decimal? ToplamEuroYurtdisiBrokerNetPrim { get; set; }
        public decimal? ToplamEuroYurticiNetPrim { get; set; }
        public decimal? ToplamEuroYurticiBrutPrim { get; set; }
        public decimal? ToplamEuroBsmv { get; set; }

        //Aed
        public decimal? ToplamAedTeminatTutari { get; set; }
        public decimal? ToplamAedYurtdisiPrim { get; set; }
        public decimal? ToplamAedYurtdisiDisKaynakKomisyon { get; set; }
        public decimal? ToplamAedYurtdisiAlinanKomisyon { get; set; }
        public decimal? ToplamAedFrontingSigortaSirketiKomisyon { get; set; }
        public decimal? ToplamAedSatisKanaliKomisyon { get; set; }
        public decimal? ToplamAedYurticiAlinanKomisyon { get; set; }
        public decimal? ToplamAedYurtdisiNetPrim { get; set; }
        public decimal? ToplamAedYurtdisiBrokerNetPrim { get; set; }
        public decimal? ToplamAedYurticiNetPrim { get; set; }
        public decimal? ToplamAedYurticiBrutPrim { get; set; }
        public decimal? ToplamAedBsmv { get; set; }

    }
    public class Teklif1PoliceListesiUWDetay
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string TVMListe { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public string BransList { get; set; }
        public string SigortaSirket { get; set; }

        public bool BolgeYetkilisiMi { get; set; }

        public string[] SigortaSirketleriSelectList { get; set; }
        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] BransSelectList { get; set; }
        public MultiSelectList BranslarItems { get; set; }


        public MultiSelectList Branslar { get; set; }
        public byte PoliceTarihTipi { get; set; }

        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }


        public string[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }
        public MultiSelectList uretimTvmler { get; set; }

        public string[] uretimTvmList { get; set; }
        public string UretimTVMListe { get; set; }

        public List<SelectListItem> PoliceTarihiTipleri { get; set; }
        public List<ReasurorPoliceListesiProcedureModel> procedurePoliceOfflineList { get; set; }

        public List<ReasurorTeklifListesiProcedureModel> procedureTeklifOfflineList { get; set; }


        public SelectList durumlar { get; set; }
        public byte durum { get; set; }

        public int? GenelPolToplamSayac { get; set; }
        public int? GenelZeylToplamSayac { get; set; }
        public List<TeklifPoliceParaBirimiToplamItem> TeklifPoliceUWToplamItem = new List<TeklifPoliceParaBirimiToplamItem>();

        // tl tahakkuk
        public decimal? ToplamTeminatTutariTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiDisKaynakKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiAlinanKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamFrontingSigortaSirketiKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamSatisKanaliKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamYurticiAlinanKomisyonTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiNetPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurtdisiBrokerNetPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurticiNetPrimTLTahakkuk { get; set; }
        public decimal? ToplamYurticiBrutPrimTLTahakkuk { get; set; }
        public decimal? ToplamBsmvTLTahakkuk { get; set; }

        // tl iptal

        public decimal? ToplamTeminatTutariTLIptal { get; set; }
        public decimal? ToplamYurtdisiPrimTLIptal { get; set; }
        public decimal? ToplamYurtdisiDisKaynakKomisyonTLIptal { get; set; }
        public decimal? ToplamYurtdisiAlinanKomisyonTLIptal { get; set; }
        public decimal? ToplamFrontingSigortaSirketiKomisyonTLIptal { get; set; }
        public decimal? ToplamSatisKanaliKomisyonTLIptal { get; set; }
        public decimal? ToplamYurticiAlinanKomisyonTLIptal { get; set; }
        public decimal? ToplamYurtdisiNetPrimTLIptal { get; set; }
        public decimal? ToplamYurtdisiBrokerNetPrimTLIptal { get; set; }
        public decimal? ToplamYurticiNetPrimTLIptal { get; set; }
        public decimal? ToplamYurticiBrutPrimTLIptal { get; set; }
        public decimal? ToplamBsmvTLIptal { get; set; }

        // tl toplam

        public decimal? ToplamTeminatTutariTL { get; set; }
        public decimal? ToplamYurtdisiPrimTL { get; set; }
        public decimal? ToplamYurtdisiDisKaynakKomisyonTL { get; set; }
        public decimal? ToplamYurtdisiAlinanKomisyonTL { get; set; }
        public decimal? ToplamFrontingSigortaSirketiKomisyonTL { get; set; }
        public decimal? ToplamSatisKanaliKomisyonTL { get; set; }
        public decimal? ToplamYurticiAlinanKomisyonTL { get; set; }
        public decimal? ToplamYurtdisiNetPrimTL { get; set; }
        public decimal? ToplamYurtdisiBrokerNetPrimTL { get; set; }
        public decimal? ToplamYurticiNetPrimTL { get; set; }
        public decimal? ToplamYurticiBrutPrimTL { get; set; }
        public decimal? ToplamBsmvTL { get; set; }

        //Dolar
        public decimal? ToplamDolarTeminatTutari { get; set; }
        public decimal? ToplamDolarYurtdisiPrim { get; set; }
        public decimal? ToplamDolarYurtdisiDisKaynakKomisyon { get; set; }
        public decimal? ToplamDolarYurtdisiAlinanKomisyon { get; set; }
        public decimal? ToplamDolarFrontingSigortaSirketiKomisyon { get; set; }
        public decimal? ToplamDolarSatisKanaliKomisyon { get; set; }
        public decimal? ToplamDolarYurticiAlinanKomisyon { get; set; }
        public decimal? ToplamDolarYurtdisiNetPrim { get; set; }
        public decimal? ToplamDolarYurtdisiBrokerNetPrim { get; set; }
        public decimal? ToplamDolarYurticiNetPrim { get; set; }
        public decimal? ToplamDolarYurticiBrutPrim { get; set; }
        public decimal? ToplamDolarBsmv { get; set; }

        //euro
        public decimal? ToplamEuroTeminatTutari { get; set; }
        public decimal? ToplamEuroYurtdisiPrim { get; set; }
        public decimal? ToplamEuroYurtdisiDisKaynakKomisyon { get; set; }
        public decimal? ToplamEuroYurtdisiAlinanKomisyon { get; set; }
        public decimal? ToplamEuroFrontingSigortaSirketiKomisyon { get; set; }
        public decimal? ToplamEuroSatisKanaliKomisyon { get; set; }
        public decimal? ToplamEuroYurticiAlinanKomisyon { get; set; }
        public decimal? ToplamEuroYurtdisiNetPrim { get; set; }
        public decimal? ToplamEuroYurtdisiBrokerNetPrim { get; set; }
        public decimal? ToplamEuroYurticiNetPrim { get; set; }
        public decimal? ToplamEuroYurticiBrutPrim { get; set; }
        public decimal? ToplamEuroBsmv { get; set; }

        //Aed
        public decimal? ToplamAedTeminatTutari { get; set; }
        public decimal? ToplamAedYurtdisiPrim { get; set; }
        public decimal? ToplamAedYurtdisiDisKaynakKomisyon { get; set; }
        public decimal? ToplamAedYurtdisiAlinanKomisyon { get; set; }
        public decimal? ToplamAedFrontingSigortaSirketiKomisyon { get; set; }
        public decimal? ToplamAedSatisKanaliKomisyon { get; set; }
        public decimal? ToplamAedYurticiAlinanKomisyon { get; set; }
        public decimal? ToplamAedYurtdisiNetPrim { get; set; }
        public decimal? ToplamAedYurtdisiBrokerNetPrim { get; set; }
        public decimal? ToplamAedYurticiNetPrim { get; set; }
        public decimal? ToplamAedYurticiBrutPrim { get; set; }
        public decimal? ToplamAedBsmv { get; set; }

    }
    public class Teklif1PoliceUWToplamItem
    {
        public byte Tur { get; set; }
        public string TurAdi { get; set; }
        public decimal? ToplamTeminatTutari { get; set; }
        public decimal? ToplamYurtdisiPrim { get; set; }
        public decimal? ToplamYurtdisiDisKaynakKomisyon { get; set; }
        public decimal? ToplamYurtdisiAlinanKomisyon { get; set; }
        public decimal? ToplamFrontingSigortaSirketiKomisyon { get; set; }
        public decimal? ToplamSatisKanaliKomisyon { get; set; }
        public decimal? ToplamYurticiAlinanKomisyon { get; set; }
        public decimal? ToplamYurtdisiNetPrim { get; set; }
        public decimal? ToplamYurtdisiBrokerNetPrim { get; set; }
        public decimal? ToplamYurticiNetPrim { get; set; }
        public decimal? ToplamYurticiBrutPrim { get; set; }
        public decimal? ToplamBsmv { get; set; }

        public class ToplamTuru
        {
            public static byte Iptal = 1;
            public static byte Tahakkuk = 1; 
        }
    }
    public class UnderwritersPartialView
    {
        public List<UnderwriterModel> UnderwriterSatirlar { get; set; }
        public string Aciklama { get; set; }
        public TeklifAraReasurorPartialModel teklifara { get; set; }
        public PoliceAraReasurorPartialModel policeara { get; set; }
        public PoliceEkiReasurorAraPartialModel policeekiara { get; set; }

    }
    public class KartListesi
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string TVMListe { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public bool BolgeYetkilisiMi { get; set; }
        public byte PoliceTarihTipi { get; set; }

        public string ad { get; set; }
        public string soyad { get; set; }
        public string tckn { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public byte AramaTip { get; set; }
        public SelectList AramaTipTipleri { get; set; }

        public string[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<KartListesiProcedureModel> kartList { get; set; }
    }
    public class referansNoEdit
    {
        public string adSoyad { get; set; }
        public string referansNo { get; set; }
        public string TeklifId { get; set; }
        public string brut { get; set; }
        public string odemeSekli { get; set; }
        public string taksitSayisi { get; set; }
    }

}
