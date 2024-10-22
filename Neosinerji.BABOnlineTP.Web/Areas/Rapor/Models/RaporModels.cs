using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database.Models;
using System.ComponentModel.DataAnnotations;
using static Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers.RaporController;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models
{
    public class RaporBaseClass
    {
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }

    public class SubeSatisRaporModel : RaporBaseClass
    {
        public byte TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte PoliceTarihi { get; set; }

        public byte DovizTL { get; set; }

        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<SubeSatisRaporProcedureModel> RaporSonuc { get; set; }
    }

    public class MTSatisRaporModel : RaporBaseClass
    {
        public byte TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte PoliceTarihi { get; set; }
        public byte DovizTL { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public byte OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        public byte OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }

        public List<SelectListItem> Kullanicilar { get; set; }
        public List<MTSatisRaporProcedureModel> RaporSonuc { get; set; }
    }

    public class PoliceRaporuModel : RaporBaseClass
    {
        public byte TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte PoliceTarihi { get; set; }
        public byte DovizTL { get; set; }

        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }


        public Nullable<byte> Durumu { get; set; }
        public List<SelectListItem> Durumlari { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<PoliceRaporProcedureModel> RaporSonuc { get; set; }
    }

    public class PoliceListesiOfflineModel : RaporBaseClass
    {
        public int[] SigortaSirketleriSelectList { get; set; }
        public MultiSelectList SigortaSirketleri { get; set; }

        public byte TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte PoliceTarihTipi { get; set; }
        public byte DovizTL { get; set; }

        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }


        public Nullable<byte> Durumu { get; set; }
        public List<SelectListItem> Durumlari { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<PoliceListesiOfflineRaporProcedureModel> RaporSonuc { get; set; }
    }

    public class VadeTakipRaporuModel : RaporBaseClass
    {
        public byte TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte DovizTL { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        public Nullable<byte> TarihTipi { get; set; }
        public List<SelectListItem> TarihTipleri { get; set; }


        public Nullable<byte> Durumu { get; set; }
        public List<SelectListItem> Durumlari { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<VadeTakipRaporuProcedureModel> RaporSonuc { get; set; }
    }


    public class RaporListProvider
    {
        public static List<SelectListItem> GetTahsilatIptalList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem() {Value="0",Text=babonline.All},
                new SelectListItem() {Value="1",Text=babonline.Accrual},
                new SelectListItem() {Value="2",Text=babonline.Cancel}
            });

            return list;
        }

        public static List<SelectListItem> GetPoliceTarihiTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value="1",Text=babonline.Disposal},
                new SelectListItem() { Value="2",Text=babonline.Starting},
                new SelectListItem() { Value="3",Text=babonline.Finish}
            });

            return list;
        }

        public static List<SelectListItem> GetDovizTLList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Value="0",Text=babonline.All},
                new SelectListItem(){Value="1",Text=babonline.TL_TurkishLiras},
                new SelectListItem(){Value="2",Text=babonline.ForeignCurrency}
            });

            return list;
        }

        public static List<SelectListItem> GetUrunList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value = "1",Text = babonline.Traffic},
                new SelectListItem(){ Value = "2",Text = babonline.Insurance},
            });

            return list;
        }

        public static List<SelectListItem> GetMapfreUrunList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value = "18",Text = babonline.Traffic},
                new SelectListItem(){ Value = "17",Text = babonline.Insurance},
            });

            return list;
        }

        public static List<SelectListItem> GetIsDurumList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem() {Value="1",Text="Beklemede"},
                new SelectListItem() {Value="2",Text="Devam Ediyor"},
                new SelectListItem() {Value="3",Text="Tamamlandı-Olumsuz"},
                new SelectListItem() {Value="4",Text="Tamamlandı-Olumlu"},
                new SelectListItem() {Value="5",Text="İptal"}
            });

            return list;
        }
      

        public static string GetDurumAciklama(byte DurumKodu)
        {
            string durumAciklama = "";
            switch (DurumKodu)
            {
                case 1: durumAciklama = "&nbsp Beklemede &nbsp"; break;
                case 2: durumAciklama = "&nbsp Devam Ediyor &nbsp"; break;
                case 3: durumAciklama = "&nbsp Tamamlandı-Olumsuz &nbsp"; break;
                case 4: durumAciklama = "&nbsp Tamamlandı-Olumlu &nbsp"; break;
                case 5: durumAciklama = "&nbsp &nbsp İptal &nbsp &nbsp"; break;
                default: break;
            }
            return durumAciklama;
        }
        public static string GetDurumAciklamaDetay(byte DurumKodu)
        {
            string durumAciklama = "";
            switch (DurumKodu)
            {
                case 1: durumAciklama = "Beklemede"; break;
                case 2: durumAciklama = "Devam Ediyor"; break;
                case 3: durumAciklama = "Tamamlandı-Olumsuz"; break;
                case 4: durumAciklama = "Tamamlandı-Olumlu"; break;
                case 5: durumAciklama = "İptal"; break;
                default: break;
            }
            return durumAciklama;
        }

        public static List<SelectListItem> GetOncelikSeviyeleri()
        {
            List<SelectListItem> oncelikTipList = new List<SelectListItem>();
            oncelikTipList.AddRange(new SelectListItem[]{
                    new SelectListItem(){ Text="Yüksek", Value="1"},
                    new SelectListItem(){ Text="Orta", Value="2"},
                    new SelectListItem(){ Text="Düşük", Value="3"}
            });

            return oncelikTipList;
        }

        public static string GetOncelikSeviye(byte OncelikSeviyesi)
        {
            string OncelikSeviyeAciklama = "";
            switch (OncelikSeviyesi)
            {
                case 1: OncelikSeviyeAciklama = "&nbsp Yüksek &nbsp"; break;
                case 2: OncelikSeviyeAciklama = "&nbsp &nbsp  Orta &nbsp  &nbsp"; break;
                case 3: OncelikSeviyeAciklama = "&nbsp Düşük &nbsp"; break;
                default: break;
            }
            return OncelikSeviyeAciklama;
        }
        public static string GetOncelikSeviyeDetay(byte OncelikSeviyesi)
        {
            string OncelikSeviyeAciklama = "";
            switch (OncelikSeviyesi)
            {
                case 1: OncelikSeviyeAciklama = "Yüksek"; break;
                case 2: OncelikSeviyeAciklama = "Orta"; break;
                case 3: OncelikSeviyeAciklama = "Düşük"; break;
                default: break;
            }
            return OncelikSeviyeAciklama;
        }
        public static List<SelectListItem> GetIsTipleri()
        {
            List<SelectListItem> oncelikTipList = new List<SelectListItem>();
            oncelikTipList.AddRange(new SelectListItem[]{
                    new SelectListItem(){ Text="Poliçe Yenileme", Value="1"}
            });

            return oncelikTipList;
        }
        public static string GetIsTipi(byte IsTipi)
        {
            string IsTipiAciklama = "";
            switch (IsTipi)
            {
                case 1: IsTipiAciklama = "Yenileme"; break;
                default: break;
            }
            return IsTipiAciklama;
        }
    }
    public class OdemeTipleriRaporModel
    {
        public const byte Yok = 0;
        public const byte Nakit = 1;
        public const byte KrediKarti = 2;
        public const byte Havale = 3;
        public const byte CekSenet = 4;

        public static List<SelectListItem> OdemeTipleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                    new SelectListItem(){ Text=babonline.Cash, Value="1"},
                    new SelectListItem(){ Text=babonline.Credit_Card, Value="2"},
                    new SelectListItem(){ Text=babonline.Transfer, Value="3"},
                    new SelectListItem(){ Text=babonline.CekSenet,Value="4"}
            });

            return list;
        }

        public static string OdemeTipi(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case OdemeTipleriRaporModel.Nakit: result = babonline.Cash; break;
                    case OdemeTipleriRaporModel.KrediKarti: result = babonline.Credit_Card; break;
                    case OdemeTipleriRaporModel.Havale: result = babonline.Transfer; break;
                    case OdemeTipleriRaporModel.CekSenet: result = babonline.CekSenet; break;
                }

            return result;
        }


    }
    public class OdemeSekilleriRaporModel
    {
        public const byte Yok = 0;
        public const byte Pesin = 1;
        public const byte Vadeli = 2;

        public static List<SelectListItem> OdemeSekilleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                    new SelectListItem(){ Value = "1", Text = babonline.SinglePayment},
                    new SelectListItem(){ Value = "2", Text = babonline.Forward}
            });

            return list;
        }

        public static string OdemeSekli(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case OdemeSekilleriRaporModel.Pesin: result = babonline.SinglePayment; break;
                    case OdemeSekilleriRaporModel.Vadeli: result = babonline.Forward; break;
                }

            return result;
        }
    }

    public class PerformansSayilariModel
    {
        public string TarihAraligi { get; set; }

    }

    public class KrediliHayatPoliceRaporuModel : RaporBaseClass
    {
        public byte TahsIptal { get; set; }
        public byte PoliceTarihi { get; set; }
        public byte DovizTL { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<PoliceRaporProcedureModel> RaporSonuc { get; set; }

        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

    }

    public class TeklifRaporuModel : RaporBaseClass
    {
        public byte TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte PoliceTarihi { get; set; }
        public byte DovizTL { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<TeklifRaporuProcedureModel> RaporSonuc { get; set; }

        public Nullable<int> TeklifNo { get; set; }

    }

    public class OfflinePoliceRaporModel : RaporBaseClass
    {
        // ==== Filters ==== //
        public string[] UrunSelectList { get; set; }
        public MultiSelectList UrunItems { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMItems { get; set; }

        public byte OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public byte OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        public byte SatisTuru { get; set; }
        public List<SelectListItem> SatisTurleri { get; set; }

        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeyilNo { get; set; }
        public string SatisTemsilcisi { get; set; }

        public byte TarihTipi { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        // ==== Procedure Return ==== //
        public List<PoliceRaporProcedureModel> RaporSonuc { get; set; }
    }

    public class OzetRaporModel : RaporBaseClass
    {
        public byte TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte PoliceTarihi { get; set; }
        public byte DovizTL { get; set; }
        public byte OdemeTipi { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> OdemeTipleri { get; set; }
        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<OzetRaporProcedureModel> RaporSonuc { get; set; }
    }

    public class OzetRaporTestModel : RaporBaseClass
    {
        public byte? TahsIptal { get; set; }

        public MultiSelectList BranslarItems { get; set; }
        public int[] BransSelectList { get; set; }

        public MultiSelectList UrunlerItems { get; set; }
        public int[] UrunSelectList { get; set; }

        public byte PoliceTarihi { get; set; }
        public byte? DovizTL { get; set; }
        public byte? OdemeTipi { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }

        public List<SelectListItem> OdemeTipleri { get; set; }
        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }

        public List<OzetRaporProcedureModel> RaporSonuc { get; set; }

        public byte? AramaKriteri { get; set; }
        public int? BirincilData { get; set; }
        public List<OzetRaporXMLModel> XMLModel { get; set; }
    }

    public class OzetRaporXMLModel
    {
        public string Tip { get; set; }
        public string Value { get; set; }
    }

    public class AracSigortalariIstatistikRaporModel : RaporBaseClass
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public byte TahsIptal { get; set; }
        public byte Urun { get; set; }
        public byte PoliceTarihi { get; set; }
        public byte DovizTL { get; set; }

        public int[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }
        public List<SelectListItem> UrunlerItems { get; set; }
        public List<SelectListItem> TahsilatIptalList { get; set; }
        public List<SelectListItem> DovizTlList { get; set; }
        public List<SelectListItem> PoliceTarihiTipleri { get; set; }
        public List<AracSigortalariIstatistikRaporuProcedureModel> RaporSonuc { get; set; }
    }
    public class IslemAtamaModel
    {
        public int TVMKodu { get; set; }
        public List<SelectListItem> TVMSelectList { get; set; }
        public int AcenteTVMKodu { get; set; }
        public string AcenteTVMUnvani { get; set; }
        public string Aciklama { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public string PoliceBitisTarihi { get; set; }
        public byte OncelikSeviyesi { get; set; }
        public string Kullanici { get; set; }
        public int KullaniciKodu { get; set; }
        public List<SelectListItem> KullanicilarList { get; set; }
        public List<SelectListItem> OncelikSeviyeleri { get; set; }
        public string Kullanicilar { get; set; }

        public string IslemMesaji { get; set; }

    }
    public class IslemPoliceListesi
    {
        public int PoliceId { get; set; }
        public string PoliceNumarasi { get; set; }
        public string SigortaSirketiKodu { get; set; }
        public int YenilemeNo { get; set; }
        public int ZeyilNo { get; set; }
        public int BransKodu { get; set; }
        public string PoliceBitisTarihi { get; set; }
        public int AcenteTVMKodu { get; set; }
        public string Aciklama { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public byte OncelikSeviyesi { get; set; }

    }
    public class IsAtamaModel
    {
        public int basariliKayitlar { get; set; }
        public int basarisizKayitlar { get; set; }

    }  
}
