using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    [Serializable]
    public class SubeSatisRaporProcedureModel
    {
        public int TVMKodu { get; set; }
        public string Unvani { get; set; }
        public int? ToplamPolice { get; set; }
        public int? BrutTutar { get; set; }
        public int? ToplamKomisyon { get; set; }
    }

    [Serializable]
    public class MTSatisRaporProcedureModel
    {
        public int TVMKodu { get; set; }
        public string Unvani { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public string AdiSoyadi { get; set; }
        public int? ToplamPolice { get; set; }
        public int? BrutTutar { get; set; }
        public int? ToplamKomisyon { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }

        public string TVMUnvani { get; set; }
        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }
    }

    [Serializable]
    public class PoliceUretimRaporProcedureModel
    {

        public string SirketAdi { get; set; }
        public string BransAdi { get; set; }
        public int? TaliKodu { get; set; }
        public string TaliUnvani { get; set; }
        public int? DisUretimTvmKodu { get; set; }
        public string DisUretimTvmUnvani { get; set; }
        public int PoliceId { get; set; }
        public string PoliceNumarasi { get; set; }
        public int? YenilemeNo { get; set; }
        public int? EkNo { get; set; }
        public string ZeyilKodu { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal? NetPrim { get; set; }
        public decimal? BrutPrim { get; set; }
        public Nullable<decimal> Komisyon { get; set; }
        public decimal? NetPrimDoviz { get; set; }
        public decimal? BrutPrimDoviz { get; set; }
        public Nullable<decimal> KomisyonDoviz { get; set; }
        public Nullable<decimal> TaliKomisyon { get; set; }

        public string Unvani { get; set; }
        public int? TaliTVMKodu { get; set; }
        public string TaliTVMUnvani { get; set; }
        public int? BransKodu { get; set; }
        public string TUMBransKodu { get; set; }
        public string TUMBransAdi { get; set; }
        public string SEUnvan { get; set; }
        public string ParaBirimi { get; set; }
        public Nullable<decimal> DovizKur { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }

    }

    [Serializable]
    public class PoliceUretimIcmalRaporProcedureModel
    {
        public int SatisKanaliKodu { get; set; }
        public string SatisKanaliUnvani { get; set; }
        public int? DisKaynakKodu { get; set; }
        public string DisKaynakUnvani { get; set; }
        //public int? SigortaUzmaniKodu { get; set; }
        //public string SigortaUzmaniUnvani { get; set; }
        public string SigortaSirketiKodu { get; set; }
        public string SirketAdi { get; set; }
        public int? BransKodu { get; set; }
        public string BransAdi { get; set; }
        //public decimal? BrutPrim { get; set; }
        //public decimal? NetPrim { get; set; }
        //public Nullable<decimal> Komisyon { get; set; }
        //public Nullable<decimal> TaliKomisyon { get; set; }
        public decimal? IptalKomisyon { get; set; }
        public decimal? TahakkukKomisyon { get; set; }
        public decimal? IptalBrutPrim { get; set; }
        public decimal? TahakkukBrutPrim { get; set; }
        public decimal? IptalToplamNetPrim { get; set; }
        public decimal? TahakkukToplamNetPrim { get; set; }
        public decimal? IptalVerilenKomisyon { get; set; }
        public decimal? TahakkukVerilenKomisyon { get; set; }
        //public decimal? NetPrim { get; set; }
        //public decimal? IptalNetPrim { get; set; }
        //public decimal? TahakkukNetPrim { get; set; }
    }


    [Serializable]
    public class PoliceUretimRaporProcedureModelTali
    {
        public string SirketAdi { get; set; }
        public string BransAdi { get; set; }
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
        public Nullable<decimal> TaliKomisyon { get; set; }

        public string Unvani { get; set; }
        public int? TaliTVMKodu { get; set; }
        public string TaliTVMUnvani { get; set; }
        public int? BransKodu { get; set; }
        public string TUMBransKodu { get; set; }
        public string TUMBransAdi { get; set; }



    }

    [Serializable]
    public class PoliceRaporProcedureModel
    {
        #region Eski
        //public int TeklifId { get; set; }
        //public int AnaTeklifId { get; set; }
        //public string AnaTeklifPDF { get; set; }
        //public int TeklifNo { get; set; }
        //public int Urunkodu { get; set; }
        //public string UrunAdi { get; set; }
        //public string MT { get; set; }
        //public string TVM { get; set; }
        //public string TUM { get; set; }
        //public string TUMPoliceNo { get; set; }
        //public string TUMTeklifNo { get; set; }
        //public string ZeyilNo { get; set; }
        //public string YenilemeNo { get; set; }
        //public string Sigortali { get; set; }
        //public string SigortaEttiren { get; set; }
        //public string OzelAlan { get; set; }
        //public decimal? BrutPrim { get; set; }
        //public DateTime? BaslamaTarihi { get; set; }
        //public DateTime? BitisTarihi { get; set; }
        //public DateTime? TanzimTarihi { get; set; }
        //public DateTime? KayitTarihi { get; set; }
        //public string PoliceNo { get; set; }
        //public Nullable<byte> OdemeTipi { get; set; }
        //public Nullable<byte> OdemeSekli { get; set; }
        //public Nullable<byte> TaksitSayisi { get; set; }
        //public Nullable<int> PoliceSuresi { get; set; }

        #endregion

        public int TeklifId { get; set; }
        public int AnaTeklifId { get; set; }
        public string AnaTeklifPDF { get; set; }
        public string TUMPoliceNo { get; set; }
        public string ZeyilNo { get; set; }
        public string YenilemeNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public int TeklifNo { get; set; }
        public string UrunAdi { get; set; }
        public string Sigortali { get; set; }
        public string SigortaEttiren { get; set; }
        public string OzelAlan { get; set; }
        public decimal? BrutPrim { get; set; }
        public decimal? ToplamKomisyon { get; set; }
        //public decimal? TarifeBasamakKodu { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<byte> TaksitSayisi { get; set; }
        public Nullable<int> PoliceSuresi { get; set; }

        public string TUM { get; set; }
        public string TVM { get; set; }
        public string Kullanici { get; set; }


        // public string BankaAdi { get; set; }
        public Nullable<byte> SatisTuru { get; set; }
        //public string Durumu { get; set; }
        public string PDFDosyasi { get; set; }
    }

    [Serializable]
    public class PoliceListesiOfflineRaporProcedureModel
    {
        public string PoliceNumarasi { get; set; }
        public int PoliceId { get; set; }
        public Nullable<int> EkNo { get; set; }

        public Nullable<int> YenilemeNo { get; set; }
        public string TUMBirlikKodu { get; set; }
        public string SirketAdi { get; set; }
        public int BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public string SliUnvan { get; set; }
        public string SEUnvan { get; set; }
        public Nullable<DateTime> TanzimTarihi { get; set; }
        public Nullable<DateTime> BaslangicTarihi { get; set; }
        public Nullable<DateTime> BitisTarihi { get; set; }
        public Nullable<decimal> BrutPrim { get; set; }
        public Nullable<decimal> NetPrim { get; set; }
        public Nullable<decimal> Komisyon { get; set; }
        public Nullable<decimal> DovizliBrutPrim { get; set; }
        public Nullable<decimal> DovizliNetPrim { get; set; }
        public Nullable<decimal> DovizliKomisyon { get; set; }
        public Nullable<decimal> TaliKomisyon { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<decimal> DovizKur { get; set; }
        public string ParaBirimi { get; set; }
        public Nullable<int> TaksitSayisi { get; set; }
        public int TvmDetayKodu { get; set; }
        public string TvmDetayUnvani { get; set; }
        public Nullable<int> TaliKodu { get; set; }
        public string TaliUnvani { get; set; }
        public Nullable<int> DisUretimTvmKodu { get; set; }
        public string DisUretimTvmUnvani { get; set; }
        public string PsliIlAdi { get; set; }
        public string PsliIlceAdi { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public string TcknVkn { get; set; }
        public string OnaylayanUnvan { get; set; }
        public Nullable<byte> MuhasebeyeAktarildiMi { get; set; }
        public Nullable<byte> ManuelPoliceMi { get; set; }
        public string email { get; set; }
        public Nullable<byte> Yeni_is { get; set; }


        // public int mGrupKodu { get; set; }
        //public string cepTel { get; set; }
    }


    [Serializable]
    public class ReasurorPoliceListesiProcedureModel
    {
        public string PoliceNumarasi { get; set; }
        public int PoliceId { get; set; }
        public Nullable<int> EkNo { get; set; }
        public Nullable<int> YenilemeNo { get; set; }
        public string TUMBirlikKodu { get; set; }
        public string SirketAdi { get; set; }
        public int BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public string SliUnvan { get; set; }
        public string SEUnvan { get; set; }
        public Nullable<DateTime> TanzimTarihi { get; set; }
        public Nullable<DateTime> BaslangicTarihi { get; set; }
        public Nullable<DateTime> BitisTarihi { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<decimal> DovizKur { get; set; }
        public string ParaBirimi { get; set; }
        public Nullable<int> TaksitSayisi { get; set; }
        public int TvmDetayKodu { get; set; }
        public string TvmDetayUnvani { get; set; }
        public Nullable<int> TaliKodu { get; set; }
        public string TaliUnvani { get; set; }
        public Nullable<int> DisUretimTvmKodu { get; set; }
        public string DisUretimTvmUnvani { get; set; }
        public string PsliIlAdi { get; set; }
        public string PsliIlceAdi { get; set; }
        public string TcknVkn { get; set; }
        public Nullable<decimal> TeminatTutari { get; set; }
        public Nullable<decimal> TeminatTutariTL { get; set; }
        public Nullable<decimal> YurtdisiPrim { get; set; }
        public Nullable<decimal> YurtdisiPrimTL { get; set; }
        public Nullable<decimal> YurtdisiDisKaynakKomisyon { get; set; }
        public Nullable<decimal> YurtdisiDisKaynakKomisyonTL { get; set; }
        public Nullable<decimal> YurtdisiAlinanKomisyon { get; set; }
        public Nullable<decimal> YurtdisiAlinanKomisyonTL { get; set; }
        public Nullable<decimal> FrontingSigortaSirketiKomisyon { get; set; }
        public Nullable<decimal> FrontingSigortaSirketiKomisyonTL { get; set; }
        public Nullable<decimal> SatisKanaliKomisyon { get; set; }
        public Nullable<decimal> SatisKanaliKomisyonTL { get; set; }
        public Nullable<decimal> YurticiAlinanKomisyon { get; set; }
        public Nullable<decimal> YurticiAlinanKomisyonTL { get; set; }
        public Nullable<decimal> YurtdisiNetPrim { get; set; }
        public Nullable<decimal> YurtdisiNetPrimTL { get; set; }
        public Nullable<decimal> YurtdisiBrokerNetPrim { get; set; }
        public Nullable<decimal> YurtdisiBrokerNetPrimTL { get; set; }
        public Nullable<decimal> YurticiNetPrim { get; set; }
        public Nullable<decimal> YurticiNetPrimTL { get; set; }
        public Nullable<decimal> YurticiBrutPrim { get; set; }
        public Nullable<decimal> YurticiBrutPrimTL { get; set; }
        public Nullable<decimal> Bsmv { get; set; }
        public Nullable<decimal> BsmvTL { get; set; }
        public string PdfPoliceCreditNote { get; set; }
        public string PdfPoliceDebitNote { get; set; }
        public string PdfPoliceDosyasi { get; set; }
        public string Aciklama { get; set; }
        public Nullable<decimal> UWKomisyon { get; set; }

        public Nullable<decimal> UWPrim { get; set; }

    }

    [Serializable]
    public class ReasurorTeklifListesiProcedureModel
    {
        public int TeklifNo { get; set; }
        public int TeklifId { get; set; }
        public int TUMKodu { get; set; }
        public string SirketAdi { get; set; }
        public int? BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string SliUnvan { get; set; }
        public string SEUnvan { get; set; }
        public DateTime TanzimTarihi { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<decimal> DovizKur { get; set; }
        public string DovizTuru { get; set; }
        public int TaksitSayisi { get; set; }
        public int TvmDetayKodu { get; set; }
        public string TvmDetayUnvani { get; set; }
        public Nullable<int> TaliKodu { get; set; }
        public string TaliUnvani { get; set; }
        public Nullable<int> DisUretimTvmKodu { get; set; }
        public string DisUretimTvmUnvani { get; set; }
        public string TcknVkn { get; set; }
        public Nullable<decimal> TeminatTutari { get; set; }
        public Nullable<decimal> TeminatTutariTL { get; set; }
        public Nullable<decimal> YurtdisiPrim { get; set; }
        public Nullable<decimal> YurtdisiPrimTL { get; set; }
        public Nullable<decimal> YurtdisiDisKaynakKomisyon { get; set; }
        public Nullable<decimal> YurtdisiDisKaynakKomisyonTL { get; set; }
        public Nullable<decimal> YurtdisiAlinanKomisyon { get; set; }
        public Nullable<decimal> YurtdisiAlinanKomisyonTL { get; set; }
        public Nullable<decimal> FrontingSigortaSirketiKomisyon { get; set; }
        public Nullable<decimal> FrontingSigortaSirketiKomisyonTL { get; set; }
        public Nullable<decimal> SatisKanaliKomisyon { get; set; }
        public Nullable<decimal> SatisKanaliKomisyonTL { get; set; }
        public Nullable<decimal> YurticiAlinanKomisyon { get; set; }
        public Nullable<decimal> YurticiAlinanKomisyonTL { get; set; }
        public Nullable<decimal> YurtdisiNetPrim { get; set; }
        public Nullable<decimal> YurtdisiNetPrimTL { get; set; }
        public Nullable<decimal> YurtdisiBrokerNetPrim { get; set; }
        public Nullable<decimal> YurtdisiBrokerNetPrimTL { get; set; }
        public Nullable<decimal> YurticiNetPrim { get; set; }
        public Nullable<decimal> YurticiNetPrimTL { get; set; }
        public Nullable<decimal> YurticiBrutPrim { get; set; }
        public Nullable<decimal> YurticiBrutPrimTL { get; set; }
        public Nullable<decimal> Bsmv { get; set; }
        public Nullable<decimal> BsmvTL { get; set; }
        public string PdfTeklifDosyasi { get; set; }
        public string Aciklama { get; set; }
    }
    [Serializable]
    public class PoliceTahsilatRaporProcedureModel
    {
        public int PoliceId { get; set; }
        public string PoliceNo { get; set; }
        public string SatisKanaliUnvani { get; set; }
        public string BransAdi { get; set; }
        public string TUMUrunAdi { get; set; }
        public int YenilemeNo { get; set; }
        public int EkNo { get; set; }
        public int TaksitNo { get; set; }
        public Nullable<decimal> TaksitTutari { get; set; }
        public DateTime? TaksitVadeTarihi { get; set; }
        public Nullable<decimal> OdenenTutar { get; set; }
        public Nullable<decimal> KalanTaksitTutari { get; set; }
        public int OdemTipi { get; set; }
        public string OdemeBelgeNo { get; set; }
        public DateTime? OdemeBelgeTarihi { get; set; }
        public string TahsilatiYapanKullaniciAdi { get; set; }
        public string TahsilatiYapanKullaniciSoyadi { get; set; }
        public string SigortaEttirenAdi { get; set; }
        public string SigortaEttirenSoyadi { get; set; }
        public string SigortaliAdi { get; set; }
        public string SigortaliSoyadi { get; set; }
        public string SigortaSirketi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public int DisKaynakKodu { get; set; }
        public string kimlikNo { get; set; }
        public string vergiKimlikNo { get; set; }
        // public virtual PoliceGenel PoliceGenel { get; set; }
    }
    public class VadeTakipRaporuProcedureModel
    {
        public int PoliceId { get; set; }
        public string PoliceNo { get; set; }
        public string SigortaSirketKodu { get; set; }
        public int BransKodu { get; set; }
        public int Urunkodu { get; set; }
        public string BransAdi { get; set; }
        public string TUM { get; set; }
        public int YenilemeNo { get; set; }
        public int EkNo { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string OzelAlan { get; set; }
        public string SigortaliAdi { get; set; }
        public string SigortaliSoyadi { get; set; }
        public string SigortaEttirenAdi { get; set; }
        public string SigortaEttirenSoyadi { get; set; }
        public int TaksitSayisi { get; set; }
        public Nullable<int> PoliceSuresi { get; set; }
        public string TVM { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public string TcknVkn { get; set; }
        public byte? OdemeTipi { get; set; }
        public byte? OdemeSekli { get; set; }
        public string MuhasebeKodu { get; set; }
        public Nullable<byte> Yeni_is { get; set; }

    }

    [Serializable]
    public class KrediliHayatPoliceRaporProcedureModel
    {
        public int TeklifId { get; set; }
        public string TUMPoliceNo { get; set; }
        public string PDFDosyasi { get; set; }
        public string TVM { get; set; }
        public string SigortaliKimlikNo { get; set; }
        public string SogrataliAdSoyad { get; set; }
        public Nullable<DateTime> DogumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string BabaAdi { get; set; }
        public string Adres { get; set; }
        public string ililce { get; set; }
        public int PostaKodu { get; set; }
        public string Numara { get; set; }
        public Nullable<DateTime> KrediBaslangicTarihi { get; set; }
        public Nullable<DateTime> KrediBitisTarihi { get; set; }
        public string KrediVade { get; set; }
        public Nullable<decimal> KrediTutari { get; set; }
        public byte ParaBirimi { get; set; }
        public Nullable<decimal> KrediMiktarKarsiligi { get; set; }
        public Nullable<decimal> TeminatMiktari { get; set; }
        public Nullable<decimal> Primi { get; set; }
        //public string VergiDairesi { get; set; }
        //public string VerdiNumarasi { get; set; }
        public short Uyruk { get; set; }
        public string Meslek { get; set; }
        public Nullable<DateTime> TanzimTarihi { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
    }

    [Serializable]
    public class TeklifRaporuProcedureModel
    {
        public int TeklifId { get; set; }
        public Nullable<int> TeklifNo { get; set; }
        public string AdiSoyadi { get; set; }
        public string PDFDosyasi { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public Nullable<DateTime> TanzimTarihi { get; set; }
        public Nullable<DateTime> BitisTarihi { get; set; }
        public string OzelAlan { get; set; }
        public string Unvani { get; set; }
        public string EkleyenKullanici { get; set; }

        public string DetailIcon { get; set; }

    }

    [Serializable]
    public class KartListesiProcedureModel
    {
        public string referansNo { get; set; }
        public int TeklifId { get; set; }
        public string kartNo { get; set; }
        public string AdiSoyadi { get; set; }
        public DateTime TanzimTarihi { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal? Brut { get; set; }
        public decimal? anaTeklifBrut { get; set; }
        public byte? odemeSekli { get; set; }
        public byte? anaTeklifOdemeSekli { get; set; }
        public byte? taksitSayisi { get; set; }
        public byte? anaTeklifTaksitSayisi { get; set; }
        public int? EkleyenTVMKodu { get; set; }
        public string KaydiEKleyenKullanici { get; set; }
        public string EkleyenTVMAdi { get; set; }
        public string ilKodu { get; set; }
        public string plakaKodu { get; set; }
        public string ilceKodu { get; set; }
        public string ilVeIlce { get; set; }
        public int TeklifNo { get; set; }
        public int TVMKodu { get; set; }
        public int TUMKodu { get; set; }
        public byte TeklifDurumKodu { get; set; }
    }

    [Serializable]
    public class OzetRaporProcedureModel
    {
        public int Kodu { get; set; }
        public string Adi { get; set; }
        public Nullable<decimal> BrutPrim { get; set; }
        public Nullable<decimal> NetPrim { get; set; }
        public Nullable<decimal> ToplamKomisyon { get; set; }
        public Nullable<decimal> TaliKomisyon { get; set; }
        public Nullable<decimal> NetKomisyon { get; set; }
        public Nullable<int> PoliceAdedi { get; set; }
    }

    public class OzetRaporProcedureRequestModel
    {
        public string BransList { get; set; }
        public string UrunList { get; set; }
        public int TarihTipi { get; set; }
        public int OdemeTipi { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int TahIpt { get; set; }
        public int DovizTl { get; set; }
        public string XML { get; set; }
    }

    [Serializable]
    public class AracSigortalariIstatistikRaporuProcedureModel
    {
        public int Kodu { get; set; }
        public string Adi { get; set; }
        public Nullable<decimal> BrutPrim { get; set; }
        public Nullable<decimal> NetPrim { get; set; }
        public Nullable<decimal> ToplamKomisyon { get; set; }
        public Nullable<decimal> TaliKomisyon { get; set; }
        public Nullable<decimal> NetKomisyon { get; set; }
        public Nullable<int> PoliceAdedi { get; set; }
    }


    #region AegonRaporPS_Model

    [Serializable]
    public class AegonTeklifRaporuProcedureModel
    {
        public int TeklifId { get; set; }
        public Nullable<int> TeklifNo { get; set; }

        public Nullable<int> IlgiliTeklifId { get; set; }
        public Nullable<int> IlgiliTeklifNo { get; set; }
        public Nullable<int> IlgiliTeklifUrunKodu { get; set; }

        public string MusteriAdiSoyadi { get; set; }
        public string PDFDosyasi { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }

        public Nullable<DateTime> TeklifTarihi { get; set; }
        public Nullable<DateTime> BaslangicTarihi { get; set; }
        public Nullable<DateTime> BitisTarihi { get; set; }

        public string TVMUnvani { get; set; }
        public string EkleyenKullanici { get; set; }
        public string ParaBirimi { get; set; }
        public string PrimOdemeDonemi { get; set; }



        public Nullable<int> SigortaSuresi { get; set; }
        public Nullable<decimal> DonemselPrim { get; set; }
        public Nullable<decimal> YillikPrim { get; set; }
        public Nullable<decimal> ToplamPrim { get; set; }


        public string RiskDegerlendirmeSonucu { get; set; }
        public bool OnProvizyon { get; set; }
        public int Total_Rows { get; set; }
    }

    #endregion

    [Serializable]
    public class GenelRaporProcedureModel
    {
        public int AktifAcenteSayisi { get; set; }
        public int PasifAcenteSayisi { get; set; }
        public int AktifKullaniciSayisi { get; set; }
        public int PasifKullaniciSayisi { get; set; }
        public int LoginKullaniciBugun { get; set; }

    }

    [Serializable]
    public class TeklifAraProcedureModel
    {
        public int TeklifId { get; set; }
        public int TeklifNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public Nullable<int> MusteriKodu { get; set; }
        public string MusteriAdiSoyadi { get; set; }
        public string PDFDosyasi { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public Nullable<DateTime> TanzimTarihi { get; set; }
        public string OzelAlan { get; set; }
        public string TVMUnvan { get; set; }
        public string EkleyenKullaniciAdi { get; set; }
        public int Total_Rows { get; set; }

        public string DetailIcon { get; set; }

    }

    [Serializable]
    public class AegonTeklifAraProcedureModel
    {
        public int TeklifId { get; set; }
        public int TeklifNo { get; set; }
        public Nullable<int> MusteriKodu { get; set; }
        public string MusteriAdiSoyadi { get; set; }
        public string PDFDosyasi { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public Nullable<DateTime> TanzimTarihi { get; set; }
        public string TVMUnvan { get; set; }
        public string EkleyenKullaniciAdi { get; set; }
        public string DetailIcon { get; set; }
        public int Total_Rows { get; set; }
    }
    [Serializable]
    public class MapfreTeklifAraProcedureModel
    {
        public int TeklifId { get; set; }
        public int TeklifNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public Nullable<int> MusteriKodu { get; set; }
        public string MusteriAdiSoyadi { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public Nullable<DateTime> TanzimTarihi { get; set; }
        public string PDFDosyasi { get; set; }
        public string OzelAlan { get; set; }
        public string TVMUnvan { get; set; }
        public string EkleyenKullaniciAdi { get; set; }
        public string DetailIcon { get; set; }
        public int Total_Rows { get; set; }

    }
    [Serializable]
    public class AtananIslerProcedureModel
    {
        public int IsNumarasi { get; set; }
        public string Baslik { get; set; }
        public string TalepYapanAcente { get; set; }
        public int IsAlanTvmKodu { get; set; }
        public string IsAlanTvmUnvani { get; set; }
        public Nullable<int> IsAlanKullaniciKodu { get; set; }
        public string IsAlanKullaniciUnvani { get; set; }
        public string PoliceNumarasi { get; set; }
        public int YenilemeNo { get; set; }
        public int EkNo { get; set; }
        public string SigortaSirketKodu { get; set; }
        public string SigortaSirketAdi { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public string BransAdi { get; set; }
        public byte IsTipi { get; set; }
        public byte Durum { get; set; }
        public byte OncelikSeviyesi { get; set; }
        public string Aciklama { get; set; }
        public DateTime AtamaTarihi { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime TahminiBitisTarihi { get; set; }
        public Nullable<DateTime> TamamlanmaTarihi { get; set; }
        public int IsAtayanTVMKodu { get; set; }
        public string IsAtayanTvmUnvani { get; set; }
        public int IsAtayanKullaniciKodu { get; set; }
        public string IsAtayanKullaniciUnvani { get; set; }
        public int IseGecBaslamaGunSayisi { get; set; }
        public Nullable<int> IsiGecBitirmeGunSayisi { get; set; }
        public int IsBitimineKalanGunSayisi { get; set; }


    }
    [Serializable]
    public class PoliceYaslandirmaTablosuPolice
    {
        public string PoliceNumarasi { get; set; }
        public string SigortaliAdSoyad { get; set; }
        public string SigortaEttirenAdSoyad { get; set; }
        public string ParaBirimi { get; set; }
        public string SatisKanali { get; set; }
        public string SigortaSirketi { get; set; }
        public bool MuhasebeIslimi { get; set; }
        //public bool Odendimi { get; set; }
        public List<PoliceYaslandirmaTablosuModelItem> PoliceYaslandirmaTablosuList = new List<PoliceYaslandirmaTablosuModelItem>();
        public ReasurorGenel ReasurorGenel { get; set; }

    }
    public class PoliceYaslandirmaTablosuModel
    {
        public List<PoliceYaslandirmaTablosuPolice> policeYaslandirmaTablosuPolice = new List<PoliceYaslandirmaTablosuPolice>();
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public string RaporTipi { get; set; }
        public string Mesaj { get; set; }
        public List<SelectListItem> RaporTipleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }
        public MultiSelectList SigortaSirketleri { get; set; }
    }
    public class PoliceYaslandirmaTablosuModelItem
    {

        public string ReasurorAd { get; set; }
        public string UwKodu { get; set; }
        public DateTime TaksitTarihi { get; set; }
        public int TaksitNo { get; set; }
        public int TaksitAdet { get; set; }
        public decimal Prim { get; set; }
        public decimal Odenen { get; set; }
        public decimal Kalan { get; set; }
        public decimal YurtDisiBrokerKomisyon { get; set; }
        public decimal AlinanKomisyon { get; set; }
        public decimal KalanKomisyon { get; set; }
        public decimal ToplamPrim { get; set; }
        public decimal ToplamOdenen { get; set; }
        public decimal ToplamKalan { get; set; }
        public decimal ToplamYurtDisiBrokerKomisyon { get; set; }
        public decimal ToplamAlinanKomisyon { get; set; }
        public decimal ToplamKalanKomisyon { get; set; }
        public bool Odendimi { get; set; } = false;
        



    }
    public class PoliceYaslandirmaGenelToplam
    {
        public string ParaBirimi { get; set; }
        public decimal GenelToplamPrim { get; set; }
        public decimal GenelToplamOdenen { get; set; }
        public decimal GenelToplamKalan { get; set; }
        public decimal GenelToplamYurtDisiBrokerKomisyon { get; set; }
        public decimal GenelToplamAlinanKomisyon { get; set; }
        public decimal GenelToplamKalanKomisyon { get; set; }
    }
    public class TeklifPoliceListesiUWDetay
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
        public List<UWDetayUnderwritersListItem> Underwriters = new List<UWDetayUnderwritersListItem>();

        #region eski toplamtutar yapisi
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
        #endregion
    }
    public class UWDetayUnderwritersListItem
    {
        public int PoliceId { get; set; }
        public List<Underwriters> Underwriters = new List<Underwriters>();
    }

    public class TeklifPoliceParaBirimiToplamItem
    {
        public byte Tur { get; set; }
        public string TurAdi { get; set; }
        public string ParaBirimi { get; set; }
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
        public int PoliceId { get; set; }

        public class ToplamTuru
        {
            public static byte Iptal = 1;
            public static byte Tahakkuk = 1;
        }
    }

}