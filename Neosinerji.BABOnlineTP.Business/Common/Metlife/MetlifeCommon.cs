using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class MetlifeHareketTipleri
    {
        public const byte Baslangic = 0;
        public const byte Ileri = 1;
        public const byte Beklet = 2;
        public const byte Iade = 3;
        public const byte Sonlandir = 4;

        public static string MetlifeHareketTipleriText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "0": result = "Başlangıç"; break;
                case "1": result = "İleri"; break;
                case "2": result = "Beklet"; break;
                case "3": result = "İade"; break;
                case "4": result = "Sonlandır"; break;
            }
            return result;
        }
    }

    public class MetlifeSoruTipleri
    {
        public const byte EklenenBasvuruFormu = 1;
        public const byte EksikImza = 2;
        public const byte EksikImzaAciklama = 3;
        public const byte EksikBelge = 4;
        public const byte KuryeyeTeslimEdildi = 5;
        public const byte FormAsliGeldi = 6;
        public const byte FormBelgeNumarasi = 7;
        public const byte EksikBelgeAciklama = 8;

        public static string MetlifeSoruTipleriText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "Eklenen Başvuru Formu"; break;
                case "2": result = "Eksik İmza"; break;
                case "3": result = "Eksik İmza Açıklama"; break;
                case "4": result = "EksikBelge"; break;
                case "5": result = "Kuryeye Teslim Edildi"; break;
                case "6": result = "Form Aslı Geldi"; break;
                case "7": result = "Form Belge Numarasi"; break;
                case "8": result = "Eksik Belge Aciklama"; break;
            }
            return result;
        }
    }

    public class MetlifeCevapTipleri
    {
        public const byte EvetHayir = 1;
        public const byte Metin = 2;

        public static string MetlifeCevapTipleriText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "Evet Hayır"; break;
                case "2": result = "Metin"; break;
            }   
            return result;
        }
    }

    public class MetlifeIsTipleri
    {
        public const byte IsOlusturldu = 1;
        public const byte BasvuruSureci2Adim = 2;
        public const byte BasvuruSureci3Adim = 3;
        public const byte BasvuruSureci4Adim = 4;
        public const byte SurecSonu = 5;

        public static string MetlifeIsTipleriText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "İş Oluşturuldu"; break;
                case "2": result = "Başvuru Süreci 2. Adım"; break;
                case "3": result = "Başvuru Süreci 3. Adım"; break;
                case "4": result = "Başvuru Süreci 4. Adım"; break;
                case "5": result = "Süreç Sonu"; break;
            }
            return result;
        }
    }

    public class IslerimListeModel
    {
        public List<IslerimModel> Items { get; set; }
    }

    public class IslerimModel
    {
        public int IsTakipId { get; set; }
        public int TeklifId { get; set; }
        public int Asama { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int TVMKullaniciId { get; set; }
        public int MusteriKodu { get; set; }
        public string MusteriAdiSoyadi { get; set; }
        public string IsTipi { get; set; }
        public string IsTipiDetay { get; set; }
        public string EkleyenKullanici { get; set; }
    }

    public class MetlifeKullaniciGruplari
    {
        public const byte SatisEkibi = 2;
        public const byte MetlifeOperasyon = 1;

        public static string MetlifeKullaniciGruplariText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "2": result = "Satış Ekibi"; break;
                case "1": result = "Metlife Operasyon"; break;
            }
            return result;
        }
    }

    public class OnayladiklarimListeModel
    {
        public List<OnayladiklarimModel> Items { get; set; }
    }

    public class OnayladiklarimModel
    {
        public int IsTakipId { get; set; }
        public int TeklifId { get; set; }
        public string Asama { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int TVMKullaniciId { get; set; }       
        public string IsTipi { get; set; }
        public string IsTipiDetay { get; set; }
        public string EkleyenKullanici { get; set; }
        public string HareketTipi { get; set; }
    }
}
