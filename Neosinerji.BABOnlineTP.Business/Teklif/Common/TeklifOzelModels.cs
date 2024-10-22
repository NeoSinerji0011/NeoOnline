using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;
using System.ComponentModel.DataAnnotations;
//using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Business
{

    public class TeklifBaseClass
    {
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }

   public class LilyumMusteriKartlari: TeklifBaseClass
    {
        public string ReferansNo { get; set; }
        public string KimlikNo { get; set; }
        public string MusteriAdiSoyadi { get; set; }
        public string MusteriKonutAdresi { get; set; }
        public string MusteriKonutIlKodu { get; set; }
        public string MusteriKonutIlceKodu { get; set; }
        public string MusteriKonutIlIlce { get; set; }

        public string Plaka { get; set; }
        public string KartNo { get; set; }
        public decimal? BrutPrim { get; set; }
        public byte? TaksitSayisi { get; set; }
        public bool? iptal { get; set; }
        public string UrunAdi { get; set; }
        public string KartValue { get; set; }
    }

    //Yönetim Index sayfasında son 5 teklifi göstermek için kullanılan model
    public class TeklifOzelDetay
    {
        public int TVMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
      
        public int TeklifNo { get; set; }
        public string TVMUnvani { get; set; }
        public string KaydedenTVMUnvani { get; set; }
        public int TeklifId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public DateTime TanzimTarihi { get; set; }
        public string MusteriAdiSoyadi { get; set; }
        public string TVMKullaniciAdSoyad { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public DateTime GecerlilikBitisTarihi { get; set; }
        public bool Aktif { get; set; }
        public string PDFDosyasi { get; set; }
        public string DetayAdres { get; set; }
        public string EkleAdres { get; set; }

        //Police icin eklendi
        public string TUMUnvani { get; set; }

        public string OzelAlan { get; set; }

        public string TUMTeklifNo { get; set; }
        public string TUMPoliceNo { get; set; }


    }

    //public class AnaSayfaTeklifModel
    //{
    //    public ToplamTrafikTeklifModel TTrafikTeklifi { get; set; }
    //    public ToplamKaskoTeklifModel TKaskoTeklifi { get; set; }
    //    public ToplamDaskTeklifModel TDaskTeklifi { get; set; }
    //    public ToplamFerdiKazaTeklifModel TFerdiKazaTeklifi { get; set; }
    //    public ToplamKonutTeklifModel TKonutTeklifi { get; set; }
    //}

    //Ana Sayfada Teklifleri chart icerisinde gostermek icin
    public class ToplamTrafikTeklifModel : ToplamTeklifDetayBase
    { }
    public class ToplamKaskoTeklifModel : ToplamTeklifDetayBase
    { }
    public class ToplamDaskTeklifModel : ToplamTeklifDetayBase
    { }
    public class ToplamFerdiKazaTeklifModel : ToplamTeklifDetayBase
    { }
    public class ToplamKonutTeklifModel : ToplamTeklifDetayBase
    { }
    public class ToplamTeklifDetayBase
    {
        public int GenelToplam { get; set; }
        public int HDI { get; set; }
        public int MApfre { get; set; }
        public int Anadolu { get; set; }
        public int Aja { get; set; }
    }


    //Teklif Arama Sayfasında Gosterilen degerler.
    public class TeklifAramaTableModel
    {
        public int TeklifId { get; set; }
        public int AnaTeklifId { get; set; }
        public string AnaTeklifPDF { get; set; }
        public int TeklifNo { get; set; }
        public string TUMTeklifNo { get; set; }

        public string TUMPoliceNo { get; set; }
        public string TUMUnvani { get; set; }

        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }

        public string UrunAdi { get; set; }
        public int UrunKodu { get; set; }

        public DateTime TanzimTarihi { get; set; }
        public DateTime PoliceBitisTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }

        public string OzelAlan { get; set; }
        public string PdfURL { get; set; }
        public string İngilizcePdfURL { get; set; }

        public bool Otorizasyon { get; set; }

        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string KaydiEKleyenTVMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public string TVMKullaniciAdSoyad { get; set; }
        public string KaydedenTVMUnvani { get; set; }

        public string DetailIcon { get; set; }
        public object AdiSoyadi { get; set; }
        public object SigortaliKimlikNo { get; set; }
    }

    public class TeklifOtorizasyonTableModel
    {
        public int TeklifId { get; set; }
        public int TeklifNo { get; set; }
        public string TUMTeklifNo { get; set; }

        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }

        public string UrunAdi { get; set; }
        public int UrunKodu { get; set; }

        public DateTime TanzimTarihi { get; set; }

        public string OzelAlan { get; set; }

        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public string TVMKullaniciAdSoyad { get; set; }
    }

    //Müşteri Teklifleri Model
    public class MusteriTeklifleriModel : TeklifBaseClass
    {
        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }

        public byte TeklifTarihi { get; set; }

        public List<TeklifOzelDetay> Teklifleri { get; set; }
        public List<SelectListItem> TeklifTarihiTipleri { get; set; }
    }

    //PotansiyelMusteri Teklifleri Model
    public class PotansiyelMusteriTeklifleriModel : TeklifBaseClass
    {
        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }

        public byte TeklifTarihi { get; set; }

        public List<TeklifOzelDetay> Teklifleri { get; set; }
        public List<SelectListItem> TeklifTarihiTipleri { get; set; }
    }


    public class TeklifTUMDetayPartialModel
    {
        public int TeklifNo { get; set; }
        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMTeklifNo { get; set; }
        public string TUMPoliceNo { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public List<string> Hatalar { get; set; }
        public byte TeklifDurumKodu { get; set; }
        public string PoliceURL { get; set; }
        public string TeklifURL { get; set; }
        public decimal? BrutPrim { get; set; }
        public bool Otorizasyon { get; set; }
    }


    public class TeklifListe : DataTableParameters<TeklifAramaTableModel>
    {
        public TeklifListe(HttpRequestBase httpRequest, Expression<Func<TeklifAramaTableModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public TeklifListe(HttpRequestBase httpRequest,
                                      Expression<Func<TeklifAramaTableModel, object>>[] selectColumns,
                                      Expression<Func<TeklifAramaTableModel, object>> rowIdColumn,
                                      Expression<Func<TeklifAramaTableModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }



        public Nullable<int> TVMKodu { get; set; }
        public Nullable<int> TUMKodu { get; set; }
        public Nullable<int> UrunKodu { get; set; }
        public Nullable<int> HazirlayanKodu { get; set; }

        public string TeklifNo { get; set; }
        public Nullable<DateTime> BaslangisTarihi { get; set; }
        public Nullable<DateTime> BitisTarihi { get; set; }
        public Nullable<int> TeklifDurumu { get; set; }
        public Nullable<int> MusteriKodu { get; set; }
        public string PoliceNo { get; set; }
        public string DetailIcon { get; set; }

    }

    public class TeklifOtorizasyonListe : DataTableParameters<TeklifOtorizasyonTableModel>
    {
        public TeklifOtorizasyonListe(HttpRequestBase httpRequest, Expression<Func<TeklifOtorizasyonTableModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public TeklifOtorizasyonListe(HttpRequestBase httpRequest,
                                      Expression<Func<TeklifOtorizasyonTableModel, object>>[] selectColumns,
                                      Expression<Func<TeklifOtorizasyonTableModel, object>> rowIdColumn,
                                      Expression<Func<TeklifOtorizasyonTableModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public int TVMKodu { get; set; }
        public Nullable<int> TeklifNo { get; set; }
        public string MapfreTeklifNo { get; set; }
        public Nullable<DateTime> BaslangisTarihi { get; set; }
        public Nullable<DateTime> BitisTarihi { get; set; }
    }



    //Metlife Ferdi Kaza Plus İş Arama Table Model

    [Serializable]
    public class IsTakipDetaylar
    {
        public IsTakipDetaylar()
        {

        }
        public string IsNo { get; set; }
        public string IsTipi { get; set; }
        public string KullaniciAdiSoyadi { get; set; }
        public string HareketTipi { get; set; }
        public string KayitTarihi { get; set; }

    }

    public class IsTakipDetayArama : DataTableParameters<IsTakipDetayListeModel>
    {
        public IsTakipDetayArama(HttpRequestBase httpRequest, Expression<Func<IsTakipDetayListeModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public IsTakipDetayArama(HttpRequestBase httpRequest,
                                   Expression<Func<IsTakipDetayListeModel, object>>[] selectColumns,
                                   Expression<Func<IsTakipDetayListeModel, object>> rowIdColumn,
                                   Expression<Func<IsTakipDetayListeModel, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public string IsNo { get; set; }
        public string IsTipi { get; set; }
        public string HareketTipi { get; set; }
        public string BaslangicTarihi { get; set; }
        public string BitisTarihi { get; set; }
    }

    public class IsTakipDetayListeModel
    {
        public int IsNo { get; set; }
        public string IsTipi { get; set; }
        public string KullaniciAdiSoyadi { get; set; }
        public string HareketTipi { get; set; }
        public string KayitTarihi { get; set; }

    }



    public class EmailMetlifeKullanicilar
    {
        public List<EmailKullanicilari> Items { get; set; }
    }
    public class EmailKullanicilari
    {
        public string adi { get; set; }
        public string soyadi { get; set; }
        public string email { get; set; }
    }

    #region Mapfre Police Sorgu

    public class PoliceSorguListe : DataTableParameters<PoliceSorguProcedurModel>
    {
        public PoliceSorguListe(HttpRequestBase httpRequest, Expression<Func<PoliceSorguProcedurModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        { }

        public bool TeklifMi { get; set; }
        public bool PoliceMi { get; set; }
        public string KimlikNo { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public int? SorguTipi { get; set; }
    }

    #endregion
}
