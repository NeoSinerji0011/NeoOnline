using Neosinerji.BABOnlineTP.Database.Models;
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
    public class FerdiKazaPlusModel
    {

        public FerdiKazaPlusModel()
        {
            this.SorgulamaSonuc = true;
            this.DisableControls = false;
            this.DisableManualGiris = false;
        }

        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }

        public SigortaliBilgileriModel Sigortali { get; set; }
        public FerdiKazaPlusTeminatlarModel Teminatlar { get; set; }
        public FerdiKazaPlusLehtarBilgileriModel Lehtar { get; set; }
        public FerdiKazaPlusTeklifOdemeModel Odeme { get; set; }
        public IletisimBilgieriModel Iletisim { get; set; }
        public PrimOdeyenBilgileriModel PrimOdeme { get; set; }

        public bool SorgulamaSonuc { get; set; }
        public string HataMesaj { get; set; }
        public bool DisableControls { get; set; }

        /// <summary>
        /// Sorgulama sonucu hatalı ise elle girişe müsade edilmeyecek.
        /// </summary>
        public bool DisableManualGiris { get; set; }
        public void SorgulamaHata(string mesaj)
        {
            this.SorgulamaSonuc = false;
            this.HataMesaj = mesaj;
        }
    }

    public class DetayFerdiKazaPlusModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }

        public SigortaliBilgileriModel Sigortalilar { get; set; }
        public FerdiKazaPlusTeminatlarModel Teminatlar { get; set; }
        public FerdiKazaPlusLehtarBilgileriModel Lehtar { get; set; }
        public FerdiKazaPlusTeklifOdemeModel Odeme { get; set; }
        public IletisimBilgieriModel Iletisim { get; set; }
        public PrimOdeyenBilgileriModel PrimOdeme { get; set; }


    }

    public class SigortaliBilgileriModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(11, ErrorMessage = "TC Kimlik Numarası 11 karakter olabilir.")]
        public string TCKimlikNo { get; set; }
        public string Ad { get; set; }
        public Nullable<DateTime> DogumTarihi { get; set; }
        public string DogumTarihiText { get; set; }
        public string Cinsiyet { get; set; }
        public byte? MeslekGrubu { get; set; }
        public byte? UrunAd { get; set; }
        public byte? SigortaSuresi { get; set; }

        public int? MusteriNo { get; set; }
        public string SoyAd { get; set; }
        public string DogumYeri { get; set; }
        public string BabaAdi { get; set; }

        public string MeslekGrubuText { get; set; }
        public string UrunAdi { get; set; }
        public string SigortaSure { get; set; }

        public SelectList MeslekGruplari { get; set; }
        public SelectList Urunler { get; set; }
        public SelectList SigortaSureler { get; set; }
        public SelectList CinsiyetTipler { get; set; }
    }

    public class FerdiKazaPlusTeminatlarModel
    {

        public int? teminatTutari { get; set; }
        public byte? OdemeSecenegi { get; set; }
        public int? KazaSonucuVefatBedeli { get; set; }
        public int? kazaSonucuMaluliyetBedeli { get; set; }
        public bool asistansHizmeti { get; set; }
        public int? SigortaPrimTutari { get; set; }
        public int? taksitSayisi { get; set; }
        public string teminatTutariText { get; set; }
        public string odemeSecenegiText { get; set; }
        public SelectList teminatTutarlari { get; set; }
        public SelectList odemeSecenekleri { get; set; }
    }

    public class FerdiKazaPlusLehtarBilgileriModel
    {
        public byte kisiSayisi { get; set; }
        public byte Lehtar { get; set; }

        public SelectList LehtarList { get; set; }

        public List<FerdiKazaPlusLehtarBilgileri> LehterList { get; set; }
        public List<SelectListItem> kisiSayilari { get; set; }
    }

    public class FerdiKazaPlusTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public bool Yenilensin { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class IletisimBilgieriModel
    {
        public int? AdresTipi { get; set; }
        public bool AdresMevcut { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public bool EmailMevcut { get; set; }
        public string Tel1 { get; set; }
        public bool Tel1Mevcut { get; set; }
        public string Tel2 { get; set; }

        public SelectList AdresTipleri { get; set; }
        public string AdresTipiText { get; set; }
    }

    public class PrimOdeyenBilgileriModel
    {
        public DateTime SigortaBaslangicTarihi { get; set; }
        public string SonKullanmaTarihi { get; set; }
        public bool KartNoMevcut { get; set; }

        public string ay { get; set; }
        public string yil { get; set; }

        [KrediKartiValidation]
        public KrediKartiModel KartNo { get; set; }

        public List<SelectListItem> Aylar { get; set; }
        public List<SelectListItem> Yillar { get; set; }

        public string SigortaBaslangicTarihiText { get; set; }
        public string KartNumarasi { get; set; }
    }

    public class FerdiKazaPlusLehtarBilgileri
    {
        public string AdiSoyadi { get; set; }
        public DateTime? DogumTarihi { get; set; }
        [Range(1, 100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Between_1_and_100")]
        public int? Oran { get; set; }
    }

    public class FerdiKazaPlusIsDetayModel
    {
        public byte Adim { get; set; }
        public DateTime Tarih { get; set; }
        public int IsNo { get; set; }
        public string AdiSoyadi { get; set; }
        public string IliskiliBelge { get; set; }
        public string TVMKullaniciAdiSoyadi { get; set; }
        public string KullaniciGrubu { get; set; }
        public string IsTipi { get; set; }
        public string IsTipiDetay { get; set; }
        public int TeklifId { get; set; }

        public IsTakipDokumanlarListModel IsDetayDokuman { get; set; }
        public List<IsTakipTarihce> IsTakipTarihce { get; set; }
    }

    public class IsTakipDokumanlarListModel
    {
        public List<IsTakipDokumanlar> Items { get; set; }
    }

    public class IsTakipDokumanlar
    {
        public string DokumanTuru { get; set; }
        public DateTime Tarihi { get; set; }
        public string KaydiEkleyen { get; set; }
        public string DosyaAdi { get; set; }
        public string DokumanURL { get; set; }
        public int IsTakipDokumanId { get; set; }
        public int IsTakipId { get; set; }
    }

    public class IsTakipTarihce
    {
        public int IsNo { get; set; }
        public int IsTipi { get; set; }
        public string TVMKullaniciAdiSoyadi { get; set; }
        public string HareketTipi { get; set; }
        public DateTime KayitTarihi { get; set; }
    }

    public class IsTakipListeEkranModel
    {
        public string IsNo { get; set; }
        public string IsTipi { get; set; }
        public string KullaniciAdiSoyadi { get; set; }
        public string HareketTipi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        public List<SelectListItem> HareketTipleri { get; set; }
        public List<SelectListItem> IsTipleri { get; set; }
    }


}