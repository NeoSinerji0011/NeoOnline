using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TaliAcente.Models
{
    public class TaliAcenteModel
    {
        public int KayitId { get; set; }
        public string SigortaSirketiKodu { get; set; }
        public string tcVkn { get; set; }
        public bool UretimDurumu { get; set; }
        public bool AnaTVMmi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string PoliceNo { get; set; }
        public string EkNo { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<TaliPoliceListe> taliPoliceListe { get; set; }
        public List<TaliAcenteRaporEkran> taliAcenteRaporEkranListe { get; set; }
        public string KayitTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int PoliceTVMKodu { get; set; }
        public string PoliceTVMUnvani { get; set; }
        public string tarih { get; set; }
        public string PoliceBordroKayitTarihi { get; set; }

        public SelectList RaporTipleri { get; set; }
        public byte RaporTipi { get; set; }

        public MultiSelectList tvmler { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] tvmList { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime KayitBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime KayitBitisTarihi { get; set; }
    }

    public class TaliKimlikModel
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public bool SorgulamaSonuc { get; set; }
        public string HataMesaj { get; set; }
        public void SorgulamaHata(string mesaj)
        {
            this.SorgulamaSonuc = false;
            this.HataMesaj = mesaj;
        }

    }

    public class TaliPoliceListe
    {
        public int Id { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public string KimlikNo { get; set; }
        public string AdUnvan_ { get; set; }
        public string SoyadUnvan { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<int> EkNo { get; set; }
        public string SigortaSirketAdi { get; set; }
        public string KayitTarihi_ { get; set; }
        public string GuncellemeTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string tarih { get; set; }


    }

    public class TaliAcenteRaporEkran
    {
        public Nullable<int> TVMKodu { get; set; }
        public Nullable<byte> UretimVAR_YOK { get; set; }
        public Nullable<byte> GunKapandimi { get; set; }
        public Nullable<int> Police_EkAdedi { get; set; }
        public string TVMAdi { get; set; }
        public Nullable<System.DateTime> GuncellemeTarihi { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string tarih { get; set; }
        public string PoliceBordroKayitTarihi { get; set; }
    }

    public class PoliceTrasferTaliAcenteKoduEslemeModel
    {
        public int TVMKodu { get; set; }
        public string KimlikNo { get; set; }
        public string AdUnvan { get; set; }
        public string SoyadUnvan { get; set; }
        public string PoliceNo { get; set; }
        public int EkNo { get; set; }
        public string SigortaSirketNo { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public byte PoliceTransferEslestimi { get; set; }

        public MultiSelectList tvmler { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] tvmList { get; set; }

        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime KayitBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime KayitBitisTarihi { get; set; }

        public List<PoliceTaliAcentelerModel> PoliceListe { get; set; }
        public List<PoliceGenelModel> PoliceGenelListe { get; set; }
        public List<PoliceTaliAcentelerTVMModel> taliPoliceGrupListe { get; set; }

        public SelectList RaporTipleri { get; set; }
        public byte RaporTipi { get; set; }
    }

    public class PoliceTaliAcentelerModel
    {
        public Nullable<int> TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public string KimlikNo { get; set; }
        public string AdUnvan_ { get; set; }
        public string SoyadUnvan { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<int> EkNo { get; set; }
        public string SigortaSirketNo { get; set; }
        public string SigortaSirketAdi { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public Nullable<System.DateTime> GuncellemeTarihi { get; set; }
        public Nullable<byte> PoliceTransferEslestimi { get; set; }
    }

    public class PoliceTaliAcentelerTVMModel
    {
        public Nullable<int> TVMKodu { get; set; }
    }

    public class PoliceGenelModel
    {
        public Nullable<int> TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public string SigortaliKimlikNo { get; set; }
        public string SigortaliAdiSoyAdi { get; set; }
        public string SigortaEttirenKimlikNo { get; set; }
        public string SigortaEttirenAdiSoyAdi { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<int> YenilemeNo { get; set; }
        public Nullable<int> EkNo { get; set; }
        public string TumBirlikKodu { get; set; }
        public string TumBirlikAciklama { get; set; }
        public Nullable<System.DateTime> BaslangicTarihi { get; set; }
        public Nullable<System.DateTime> TanzimTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
    }

    public class PaylasimliPoliceUretimModel
    {
        public int TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public int TaliTVMKodu { get; set; }
        public string TaliTVMUnvan { get; set; }
        public int DisKaynakKodu { get; set; }


        public string SigortaSirketiKodu { get; set; }
        public string BransKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YenilemeNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string ZeylNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal BrutPrim { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal NetPrim { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TanzimTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }

        public decimal PoliceKomisyonTutari { get; set; }
        public decimal TVMKomisyonTutari { get; set; }
        public int TVMKomisyonOrani { get; set; }
        public decimal TaliTVMKomisyonTutari { get; set; }
        public int TaliTVMKomisyonOrani { get; set; }
        public int KaydiEkleyenKullanici { get; set; }

        public List<SelectListItem> TaliTVMList { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }

    }

    public class PoliceBordroModel
    {
        public string SigortaSirketiKodu { get; set; }
        public string tcVkn { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string PoliceNo { get; set; }
        public string EkNo { get; set; }
        public string KayitTarihi { get; set; }
        public int PoliceTVMKodu { get; set; }
        public string PoliceTVMUnvani { get; set; }
        public string tarih { get; set; }
        public string PoliceBordroKayitTarihi { get; set; }

        public MultiSelectList tvmler { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] tvmList { get; set; }

        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime KayitBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime KayitBitisTarihi { get; set; }

        public List<TaliPoliceListe> taliPoliceListe { get; set; }
        public List<TaliAcenteRaporEkran> taliAcenteRaporEkranListe { get; set; }
        public List<PoliceTaliAcentelerTVMModel> taliPoliceGrupListe { get; set; }
    }

    public class PoliceOnaylamaModel
    {
        public bool MerkezAcentemi { get; set; }
        public int MerkezAcenKodu { get; set; }
        public byte IslemTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public int? DisKaynakKodu { get; set; }
        public string DisKaynakUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }
        public string SigortaSirketiUnvani { get; set; }

        public string tcVkn { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }
        public int EkNo { get; set; }
        public string TanzimTarihi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int? SonPoliceOnayTarihi { get; set; }
        public string PoliceBordroKayitTarihi { get; set; }
        public string BransKodu { get; set; }

        public List<PoliceOnaylamaListModel> policeListesi { get; set; }
        public List<SelectListItem> Tvmler { get; set; }
        public List<SelectListItem> DisKaynakList { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }
        public SelectList Islemler { get; set; }

        public MultiSelectList tvmMultiList { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] tvmMultiLists { get; set; }

        public MultiSelectList sirketMultiList { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] sirketMultiLists { get; set; }

        public List<PoliceTaliAcentelerTVMModel> taliPoliceGrupListe { get; set; }
        public string plaka { get; set; }
    }
    public class PoliceOnaylamaListModel
    {
        public int PoliceId { get; set; }
        public string PoliceDurumu { get; set; }
        public int? TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int? DisKaynakKodu { get; set; }
        public string DisKaynakUnvani { get; set; }
        public string SigortaSirketiKodu { get; set; }
        public string SigortaSirketiUnvani { get; set; }
        public string tcVkn { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string PoliceNo { get; set; }
        public string EkNo { get; set; }
        public string TanzimTarihi { get; set; }
        public string BaslangicTarihi { get; set; }
        public string BitisTarihi { get; set; }
        public decimal? KomisyonOrani { get; set; }
        public decimal? KomisyonTutari { get; set; }
        public decimal? NetPrim { get; set; }
        public decimal? DovizliNetPrim { get; set; }
        public decimal? DovizliBrütPrim { get; set; }

    }

}