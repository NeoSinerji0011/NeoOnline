using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap.Muhasebe_CariHesapService;

namespace Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Models
{
    public class HesapEkstresiModel
    {
        public string IslemTipi { get; set; }
        public DateTime IslemTarih { get; set; }
        public string Aciklama { get; set; }
        public string ParaBirimi { get; set; }
        public decimal? Borc { get; set; }
        public decimal? Alacak { get; set; }
        public decimal? Bakiye { get; set; }
        public decimal? ToplamBakiye { get; set; }

        public string EvrakNo { get; set; }
        public string EvrakTipi { get; set; }
        public DateTime VadeTarihi { get; set; }
        public string BorcTipi { get; set; }
        public string Adi { get; set; }
        public string Soyad { get; set; }
        public decimal Devir { get; set; }
        public decimal DevirBorc { get; set; }
        public decimal DevirAlacak { get; set; }
        //****//
        public string OdemeTipim { get; set; }
        public string OdemeSekli { get; set; }
        public string PoliceNo { get; set; }
        public int PoliceId { get; set; }
        public string SigortaEttirenUnvani { get; set; }
        public string SigortaliUnvani { get; set; }

        public string YenilemeNo { get; set; }
        public string EkNo { get; set; }
        public string UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public int BransKodu { get; set; }
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
        public decimal? VerilenKomisyon { get; set; }
        public int TaksitSayisi { get; set; }
        public string KimlikNo { get; set; }
        public static List<SelectListItem> CariHesapTipleriModel()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.PleaseSelect }, // Lütfen Seçiniz
                new SelectListItem() { Value = "120.01.", Text = babonline.Customer }, // Müşteri
                new SelectListItem() { Value = "320.01.", Text = babonline.InsuranceCompany }, // Sigorta Şirketi
                new SelectListItem() { Value = "330.", Text = babonline.ThirdPartyCompanies }, // Üçüncü Şahıs Firmalar
                new SelectListItem() { Value = "340.", Text = babonline.TVM_Broker}, // Yurt Dışı Broker
                new SelectListItem() { Value = "600.01.", Text = babonline.CommissionIncomeCalculation }, // Komisyon Gelirleri Hesabı
                new SelectListItem() { Value = "610.01.", Text = babonline.SalesReturns }, // Satış İadeleri
                new SelectListItem() { Value = "611.01.", Text = babonline.SalesCommissionPayments + "-" + babonline.SharedAgent }, // Satış Komisyon Ödemeleri-Paylaşımlı Acente
                new SelectListItem() { Value = "100.01.", Text = babonline.Cash_Account }, //  "Kasa Hesabı"
                new SelectListItem() { Value = "102.01.", Text = babonline.Bank_Account }, // Banka Hesabı
                new SelectListItem() { Value = "101.01.", Text = babonline.Checks_Received }, // Alınan Çekler
                new SelectListItem() { Value = "103.01.", Text = babonline.Issued_Checks + " / " + babonline.Payment_Orders}, //Verilen Çekler / Ödeme Emirleri
                new SelectListItem() { Value = "121.01.", Text = babonline.Notes_Receivable }, // Alacak Senetleri
                new SelectListItem() { Value = "321.01.", Text = babonline.Notes_Payable }, // Borç Senetleri
                new SelectListItem() { Value = "108.01.", Text = babonline.AgencyPosAccount }, // Acente Pos Hesabı
                new SelectListItem() { Value = "309.01.", Text = babonline.Agency + " " + babonline.Credit_Card }, // Acente Kredi Kartı
                new SelectListItem() { Value = "399.01.", Text = babonline.AgencyIndividualCCard }, // Acente Bireysel K. Kartı
                new SelectListItem() { Value = "612.01.", Text = babonline.Other_Discounts + " - " + babonline.Sales_Channel_Payments }, // Diğer İndirimler-Satış Kanalı Ödemeleri
                new SelectListItem() { Value = "612.02.", Text = babonline.Discounts }, // İskontolar
                new SelectListItem() { Value = "740.", Text = babonline.General_Expenses }, // Genel Giderler
                new SelectListItem() { Value = "602.", Text = babonline.Other_Income }, // Diğer Gelirler
            });

            return list;
        }



    }

    public class HesapEkstresiAraModel
    {
        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }
        public List<SelectListItem> Yillar { get; set; }
        public int Yil { get; set; }
        public byte tt { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string TcknVkn { get; set; }
        public string OdemeBelgeNo { get; set; }
        public string Unvan { get; set; }
        public string UnvanFirma { get; set; }
        public string UnvanSoyad { get; set; }
        public string GrupKodu { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public MultiSelectList tvmler { get; set; }
        public string[] tvmList { get; set; }
        public List<int> TVMListe { get; set; }
        public byte Durum { get; set; }
        public SelectList Durumlar { get; set; }
        public List<HesapEkstresiModel> list { get; set; }
    }

    public class CariHareketEkleModel
    {
        public int TVMKodu { get; set; }
        public string CariHesapAdi { get; set; }
        public string CariHesapKodu { get; set; }
        public DateTime CariHareketTarihi { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public byte EvrakTipi { get; set; }
        public byte OdemeTipi { get; set; }
        public string EvrakNo { get; set; }
        public string Tutar { get; set; }
        public int? MasrafMerkezi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string BorcAlacakTipi { get; set; }
        public string DovizTipi { get; set; }
        public string DovizKuru { get; set; }
        public string DovizTutari { get; set; }
        public string Aciklama { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public List<SelectListItem> CariHesapList { get; set; }
        public string CariEvrakNo { get; set; }
        public List<SelectListItem> CariEvrakList { get; set; }
        public SelectList BorcAlacakTipler { get; set; }
        public SelectList EvrakTipleri { get; set; }
        public SelectList OdemeTipleri { get; set; }
        public SelectList DovizTipleri { get; set; }
        public SelectList MasrafMerkezleri { get; set; }

        public KarsiCariHareketEkleModel KarsiCari { get; set; }

    }
    public class BrokerCariHareketEkleModel
    {
        public int TVMKodu { get; set; }
        public string CariHesapAdi { get; set; }
        public string CariHesapKodu { get; set; }
        public DateTime CariHareketTarihi { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public byte EvrakTipi { get; set; }
        public byte OdemeTipi { get; set; }
        public string EvrakNo { get; set; }
        public string PoliceNumarasi { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeyilNo { get; set; }

        public string Tutar { get; set; }
        public int? MasrafMerkezi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string BorcAlacakTipi { get; set; }
        public string DovizTipi { get; set; }
        public string DovizKuru { get; set; }
        public string DovizTutari { get; set; }
        public string Aciklama { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public List<SelectListItem> CariHesapList { get; set; }
        public string CariEvrakNo { get; set; }
        public List<SelectListItem> CariEvrakList { get; set; }
        public SelectList BorcAlacakTipler { get; set; }
        public SelectList EvrakTipleri { get; set; }
        public SelectList OdemeTipleri { get; set; }
        public SelectList DovizTipleri { get; set; }
        public SelectList MasrafMerkezleri { get; set; }

        public KarsiCariHareketEkleModel KarsiCari { get; set; }

    }

    public class CariHareketDetayModel
    {
        public string CariHesapKodu { get; set; }
        public string CariHareketTarihi { get; set; }
        public string OdemeTarihi { get; set; }
        public string EvrakTipiText { get; set; }
        public string OdemeTipiText { get; set; }
        public string EvrakNo { get; set; }
        public string Tutar { get; set; }
        public string MasrafMerkeziText { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string BorcAlacakTipiText { get; set; }
        public string DovizTipiText { get; set; }
        public string DovizKuru { get; set; }
        public string DovizTutari { get; set; }
        public string Aciklama { get; set; }
        public string KayitTarihi { get; set; }
        public string GuncellemeTarihi { get; set; }
    }
    public class KarsiCariHareketEkleModel : CariHareketEkleModel
    {
        //Karşı Cari Model
        public string KarsiCariHesapAdi { get; set; }
        public string KarsiCariHesapKodu { get; set; }
        public string KarsiDovizTipi { get; set; }

        public byte KarsiEvrakTipi { get; set; }
        public string KarsiEvrakNo { get; set; }

        public List<SelectListItem> KarsiCariHesapList { get; set; }
        public SelectList KarsiEvrakTipleri { get; set; }
        public SelectList KarsiDovizTipleri { get; set; }
        public SelectList KarsiBorcAlacakTipler { get; set; }
    }
    public class CariHesapEkleModel
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string CariHesapKodu { get; set; }
        public string SatisIadeleriMuhasebeKodu { get; set; }
        public string CariHesapTipi { get; set; }
        public string sigortaSirketi { get; set; }
        public string yurtDisiBroker{ get; set; }
        public string paraBirimi{ get; set; }


        public SelectList CariHesapTipleri { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string CepTel { get; set; }
        public string Email { get; set; }
        public string DisaktarimMuhasebeKodu { get; set; }
        public string DisaktarimCariKodu { get; set; }
        public string KomisyonGelirleriMuhasebeKodu { get; set; }
        public string WebAdresi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public int IlceKodu { get; set; }
        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }
        public List<SelectListItem> sigortaSirketleri { get; set; }
        public List<SelectListItem> yurtDisiBrokerlar { get; set; }
        public List<SelectListItem> paraBirimleri { get; set; }

        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public int PostaKodu { get; set; }
        public string Adres { get; set; }
        public string UyariNotu { get; set; }
        public string BilgiNotu { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }

    }
    public class CariHesapEklePartialModel
    {
        public string CariHesapKodu { get; set; }
    }
    public class CariHesapEkstreListModel
    {
        //Filtreler
        //public string TcknVkn { get; set; }

        public string BaslangicGunAy { get; set; }
        public string BitisGunAy { get; set; }
        public byte AramaTip { get; set; }
        public byte MizanTip { get; set; }
        public byte PdfTip { get; set; }
        public string Donem { get; set; }
        public string CariHesapAdi { get; set; }
        public string CariHesapKodu { get; set; }
        public string MusteriGurpKodu { get; set; }
        public List<SelectListItem> CariHesapList { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public SelectList AramaTipTipleri { get; set; }
        public SelectList MizanTipleri { get; set; }
        public SelectList PdfTipleri { get; set; }


        //Liste Parametreleri
        public decimal? BorcToplam { get; set; }
        public decimal? AlacakToplam { get; set; }
        public decimal? BakiyeToplam { get; set; }

        public string CariHesapText { get; set; }

        public List<CariHesapEkstreEkranModel> list = new List<CariHesapEkstreEkranModel>();
        public CariHesapBAModel cariHesapBA = new CariHesapBAModel();
        public string PDFURL { get; set; }
        public bool pdfVar { get; set; }
    }
    public class CariHesapEkstreEkranModel
    {
        public string OdemeTipi { get; set; }
        public DateTime? OdemeTarihi { get; set; }
        public string Aciklama { get; set; }
        public string ParaBirimi { get; set; }
        public decimal? Borc { get; set; }
        public decimal? Alacak { get; set; }
        public decimal? Bakiye { get; set; }
        public string EvrakNo { get; set; }
        public string EvrakTipi { get; set; }
        public DateTime VadeTarihi { get; set; }
        public string BorcTipi { get; set; }
        public string Adi { get; set; }
        public string Soyad { get; set; }
        public string MusteriGrupkodu { get; set; }

    }
    public class CariHesapListesiModel
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string CariHesapKodu { get; set; }
        public string CariHesapTipiAdi { get; set; }
        public SelectList CariHesapTipleri { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string CepTel { get; set; }
        public string Email { get; set; }
        public string DisaktarimMuhasebeKodu { get; set; }
        public string DisaktarimCariKodu { get; set; }
        public string KomisyonGelirleriMuhasebeKodu { get; set; }
        public string WebAdresi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public int IlceKodu { get; set; }
        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public int PostaKodu { get; set; }
        public string Adres { get; set; }
        public string UyariNotu { get; set; }
        public string BilgiNotu { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }


    }
    public class CariHesapListesiAraModel
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string CariHesapKodu { get; set; }
        public string CariHesapTipiAdi { get; set; }
        public SelectList CariHesapTipleri { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string CepTel { get; set; }
        public string Email { get; set; }
        public string DisaktarimMuhasebeKodu { get; set; }
        public string DisaktarimCariKodu { get; set; }
        public string KomisyonGelirleriMuhasebeKodu { get; set; }
        public string WebAdresi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public int IlceKodu { get; set; }
        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public int PostaKodu { get; set; }
        public string Adres { get; set; }
        public string UyariNotu { get; set; }
        public string BilgiNotu { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public List<CariHesapListesiModel> list { get; set; }
    }
    public class CariHesapGuncelleModel
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string CariHesapKodu { get; set; }
        public string CariHesapTipi { get; set; }
        public string sigortaSirketi { get; set; }
        public string SatisIadeleriMuhasebeKodu { get; set; }
        public SelectList CariHesapTipleri { get; set; }
        public List<SelectListItem> sigortaSirketleri { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string CepTel { get; set; }
        public string Email { get; set; }
        public string DisaktarimMuhasebeKodu { get; set; }
        public string DisaktarimCariKodu { get; set; }
        public string KomisyonGelirleriMuhasebeKodu { get; set; }
        public string WebAdresi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public int IlceKodu { get; set; }
        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public int PostaKodu { get; set; }
        public string Adres { get; set; }
        public string UyariNotu { get; set; }
        public string BilgiNotu { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }

    }

    public class CariHesapHareketKontrolListesiModel
    {
        public int id { get; set; }
        public byte AramaTip { get; set; }
        public string CariHesapAdi { get; set; }
        public string MusteriGurpKodu { get; set; }
        public List<SelectListItem> CariHesapList { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public int Donem { get; set; }
        public DateTime baslangicTarihi { get; set; }
        public DateTime bitisTarihi { get; set; }
        public SelectList AramaTipTipleri { get; set; }
        //Liste Parametreleri
        public decimal? BorcToplam { get; set; }
        public decimal? AlacakToplam { get; set; }
        public decimal? BakiyeToplam { get; set; }

        public string CariHesapText { get; set; }

        public List<CariHareketListeEkranModel> list = new List<CariHareketListeEkranModel>();

        public string CariHesapKodu { get; set; }
        // 0 ise ada göre  1 ise başlangıç tarihi bitiş tarihine göre arama
        public Boolean AramaDurumu { get; set; }

    }
    public class CariHareketListeEkranModel
    {
        public int id { get; set; }
        public string OdemeTipi { get; set; }
        public DateTime? OdemeTarihi { get; set; }
        public string Aciklama { get; set; }
        public string ParaBirimi { get; set; }
        public decimal? Borc { get; set; }
        public decimal? Alacak { get; set; }
        public string EvrakNo { get; set; }
        public string EvrakTipi { get; set; }
        public DateTime VadeTarihi { get; set; }
        public string BorcTipi { get; set; }
        public string CariHesapAdiKodu { get; set; }
        public string CariHesapKodu { get; set; }
        public string MusteriGrupkodu { get; set; }

    }

    public class CariHesapBAModel
    {
        public decimal Borc1 { get; set; }
        public decimal Alacak1 { get; set; }
        public decimal Borc2 { get; set; }
        public decimal Alacak2 { get; set; }
        public decimal Borc3 { get; set; }
        public decimal Alacak3 { get; set; }
        public decimal Borc4 { get; set; }
        public decimal Alacak4 { get; set; }
        public decimal Borc5 { get; set; }
        public decimal Alacak5 { get; set; }
        public decimal Borc6 { get; set; }
        public decimal Alacak6 { get; set; }
        public decimal Borc7 { get; set; }
        public decimal Alacak7 { get; set; }
        public decimal Borc8 { get; set; }
        public decimal Alacak8 { get; set; }
        public decimal Borc9 { get; set; }
        public decimal Alacak9 { get; set; }
        public decimal Borc10 { get; set; }
        public decimal Alacak10 { get; set; }
        public decimal Borc11 { get; set; }
        public decimal Alacak11 { get; set; }
        public decimal Borc12 { get; set; }
        public decimal Alacak12 { get; set; }

        public decimal toplamBorc { get; set; }
        public decimal toplamAlacak { get; set; }
        public decimal yuruyenBakiye { get; set; }
    }

    public class CariHesaplaraAktarModel
    {
        public DateTime TanzimBasTarihi { get; set; }
        public DateTime TanzimBitTarihi { get; set; }
        public int TvmKodu { get; set; }

        public List<CariHesapBAReturnModel> returnList = new List<CariHesapBAReturnModel>();

        public int? basariliKayitSayisi { get; set; }
        public int? hataliKayitSayisi { get; set; }
    }
    public class CariHesapMizani
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public int Donem { get; set; }
        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }
        public string CariHesapKodu { get; set; }
        public string CariHesapTipiAdi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public decimal? CariAyBorc { get; set; }
        public decimal? CariAyBorcToplam { get; set; }
        public decimal? CariAyAlacak { get; set; }
        public decimal? CariAyAlacakToplam { get; set; }
        public decimal? BakiyeAy { get; set; }
        public decimal? BakiyeAyToplam { get; set; }
        public decimal? KumulatifBorc { get; set; }
        public decimal? KumulatifBorcToplam { get; set; }
        public decimal? KumulatifAlacak { get; set; }
        public decimal? KumulatifAlacakToplam { get; set; }
        public decimal? KumulatifBakiye { get; set; }
        public decimal? KumulatifBakiyeToplam { get; set; }

    }
    public class CariHesapARAMizani
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public int Donem { get; set; }
        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }
        public string CariHesapKodu { get; set; }
        public string CariHesapTipiAdi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public decimal? CariAyBorc { get; set; }
        public decimal? CariAyBorcToplam { get; set; }
        public decimal? CariAyAlacak { get; set; }
        public decimal? CariAyAlacakToplam { get; set; }
        public decimal? BakiyeAy { get; set; }
        public decimal? BakiyeAyToplam { get; set; }
        public decimal? KumulatifBorc { get; set; }
        public decimal? KumulatifBorcToplam { get; set; }
        public decimal? KumulatifAlacak { get; set; }
        public decimal? KumulatifAlacakToplam { get; set; }
        public decimal? KumulatifBakiye { get; set; }
        public decimal? KumulatifBakiyeToplam { get; set; }
        public List<CariHesapMizani> list { get; set; }
    }
    public class GelirGiderDönemi
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public int Donem { get; set; }
        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }

        public byte AramaTip { get; set; }
        public SelectList AramaTipTipleri { get; set; }
        public string CariHesapKodu { get; set; }
        public string CariHesapTipiAdi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public decimal? CariAyBorc { get; set; }
        public decimal? CariAyBorcToplam { get; set; }
        public decimal? CariAyAlacak { get; set; }
        public decimal? CariAyAlacakToplam { get; set; }
        public decimal? BakiyeAy { get; set; }
        public decimal? BakiyeAyToplam { get; set; }
        public decimal? KumulatifBorc { get; set; }
        public decimal? KumulatifBorcToplam { get; set; }
        public decimal? KumulatifAlacak { get; set; }
        public decimal? KumulatifAlacakToplam { get; set; }
        public decimal? KumulatifBakiye { get; set; }
        public decimal? KumulatifBakiyeToplam { get; set; }



        public decimal? CariAyBorcHer { get; set; }
        public decimal? CariAyBorcToplamHer { get; set; }
        public decimal? CariAyAlacakHer { get; set; }
        public decimal? CariAyAlacakToplamHer { get; set; }
        public decimal? BakiyeAyHer { get; set; }
        public decimal? BakiyeAyToplamHer { get; set; }
        public decimal? KumulatifBorcHer { get; set; }
        public decimal? KumulatifBorcToplamHer { get; set; }
        public decimal? KumulatifAlacakHer { get; set; }
        public decimal? KumulatifAlacakToplamHer { get; set; }
        public decimal? KumulatifBakiyeHer { get; set; }
        public decimal? KumulatifBakiyeToplamHer { get; set; }
    }
    public class GelirGiderARADönemi
    {
        public int id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public int Donem { get; set; }
        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }
        public byte AramaTip { get; set; }
        public SelectList AramaTipTipleri { get; set; }
        public string CariHesapKodu { get; set; }
        public string CariHesapTipiAdi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public decimal? CariAyBorc { get; set; }
        public decimal? CariAyBorcToplam { get; set; }
        public decimal? CariAyAlacak { get; set; }
        public decimal? CariAyAlacakToplam { get; set; }
        public decimal? BakiyeAy { get; set; }
        public decimal? BakiyeAyToplam { get; set; }
        public decimal? KumulatifBorc { get; set; }
        public decimal? KumulatifBorcToplam { get; set; }
        public decimal? KumulatifAlacak { get; set; }
        public decimal? KumulatifAlacakToplam { get; set; }
        public decimal? KumulatifBakiye { get; set; }
        public decimal? KumulatifBakiyeToplam { get; set; }



        public decimal? CariAyBorcHer { get; set; }
        public decimal? CariAyBorcToplamHer { get; set; }
        public decimal? CariAyAlacakHer { get; set; }
        public decimal? CariAyAlacakToplamHer { get; set; }
        public decimal? BakiyeAyHer { get; set; }
        public decimal? BakiyeAyToplamHer { get; set; }
        public decimal? KumulatifBorcHer { get; set; }
        public decimal? KumulatifBorcToplamHer { get; set; }
        public decimal? KumulatifAlacakHer { get; set; }
        public decimal? KumulatifAlacakToplamHer { get; set; }
        public decimal? KumulatifBakiyeHer { get; set; }
        public decimal? KumulatifBakiyeToplamHer { get; set; }
        public List<GelirGiderDönemi> list { get; set; }
    }


    public class TopluPoliceTahsilatOdemeModel
    {
        //public Nullable<byte> OdemeTipi { get; set; }
        //public List<SelectListItem> OdemeTipleri { get; set; }
        //public List<SelectListItem> AcenteBankaHesapList { get; set; }

        public string BaslangicGunAy { get; set; }
        public string BaslangicGunAy1 { get; set; }
        public string BitisGunAy { get; set; }
        public string BitisGunAy1 { get; set; }
        public int TVMKodu { get; set; }
        public int TaliAcenteKodu { get; set; }
        public string TvmUnvani { get; set; }
        public string TcknVkn { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string Unvan { get; set; }
        public string UnvanFirma { get; set; }
        public string UnvanSoyad { get; set; }
        public byte Durum { get; set; }
        public SelectList Durumlar { get; set; }
        public string SigortaEttiren { get; set; }
        public List<SelectListItem> TVMList { get; set; }

        public List<TopluPoliceTahsilatOdemeAraModel> listPolOffline { get; set; }
        public Neosinerji.BABOnlineTP.Business.PoliceTransfer.PoliceService.PoliceOffLineServiceModel policeList { get; set; }
        public Neosinerji.BABOnlineTP.Business.PoliceTransfer.PoliceService.PoliceOffLineServiceModel police { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public int Donem { get; set; }
    }
    public class TopluPoliceTahsilatOdemeAraModel
    {
        public string PoliceNo { get; set; }
        public int PoliceId { get; set; }
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
        public string PlakaNo { get; set; }  /// Kasko ve trafik branşında gösterilecek. Diğer branşarlar için boş.
        public string PlakaKodu { get; set; }
        public string OdemeTipi { get; set; }
        public string OdemeSekli { get; set; }
        public int TaksitSayisi { get; set; }
        public string OdemeTipim { get; set; }
    }
    public class PoliceleriCariyeAktar
    {
        public int SiraNo { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string SirketKodu { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public byte AktarimTamamlandi { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Branslar { get; set; }
    }
    public class MuhasebeAktarimModel
    {
        public List<MuhasebeAktarim> list = new List<MuhasebeAktarim>();
        public string hata { get; set; }
    }
    public class MuhasebeAktarim
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string SirketKodu { get; set; }
        public string SirketUnvan { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public string BransAdi { get; set; }
        public byte AktarimTamamlandi { get; set; }
        public string AktarimTamamlandiText { get; set; }
    }

}