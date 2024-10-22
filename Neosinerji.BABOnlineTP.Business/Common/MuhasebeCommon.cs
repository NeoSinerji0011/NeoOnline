using Neosinerji.BABOnlineTP.Business.Tools.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public static class MuhasebeCommon
    {
        public static string GetCariHesapTipAdi(string tipKodu)
        {
            string tipAdi = "";
            if (tipKodu.Contains("120."))
            {
                tipKodu = "120.01.";
            }
            else if (tipKodu.Contains("320."))
            {
                tipKodu = "320.01.";
            }
            else if (tipKodu.Contains("600."))
            {
                tipKodu = "600.01.";

            }
            else if (tipKodu.Contains("610."))
            {
                tipKodu = "610.01.";
            }
            else if (tipKodu.Contains("340"))
            {
                tipKodu = "340.";
            }

            switch (tipKodu)
            {
                case "120.01": tipAdi = babonline.Customer; break; // "Müşteri"
                case "320.01": tipAdi = babonline.InsuranceCompany; break; // Sigorta Şirketi
                case "340.": tipAdi = babonline.TVM_Broker; break; // Yurt Dışı Broker
                case "330.": tipAdi = babonline.ThirdPartyCompanies; break; // Üçüncü Şahıs Firmalar
                case "600.01": tipAdi = babonline.Commission_Income_Account; break; // Komisyon Gelirleri Hesabı
                case "610.01": tipAdi = babonline.SalesReturns; break; // Satış İadeleri
                case "611.01": tipAdi = babonline.Sales_Commissions; break; // Satış Komisyonları
                case "100.01": tipAdi = babonline.Cash_Account; break; // Kasa Hesabı
                case "102.01": tipAdi = babonline.Bank_Account; break; // Banka Hesabı
                case "101.01": tipAdi = babonline.Checks_Received; break; // Alınan Çekler
                case "103.01": tipAdi = babonline.Issued_Checks + " / " + babonline.Payment_Orders; break; // Verilen Çekler / Ödeme Emirleri
                case "121.01": tipAdi = babonline.Notes_Receivable; break; // Alacak Senetleri
                case "321.01": tipAdi = babonline.Notes_Payable; break; // Borç Senetleri
                case "108.01": tipAdi = babonline.AgencyPosAccount; break; // Acente Pos Hesabı
                case "309.01": tipAdi = babonline.Agency + " " + babonline.Credit_Card; break; // Acente Kredi Kartı
                case "399.01": tipAdi = babonline.AgencyIndividualCCard; break; //Acente Bireysel K. Kratı
                case "612.01": tipAdi = babonline.Other_Discounts; break; // Diğer İndirimler
                case "740.": tipAdi = babonline.General_Expenses; break; // Genel Giderler
                case "612.02": tipAdi = babonline.Discounts; break; // İskontolar
                case "602.": tipAdi = babonline.Other_Income; break; // Diğer Gelirler

                default:
                    tipAdi = "";
                    break;
            }

            return tipAdi;
        }
        public static string GetCariHesapTipAdiNoktali(string tipKodu)
        {
            string tipAdi = "";
            if (tipKodu.Contains("120."))
            {
                tipKodu = "120.01.";
            }
            else if (tipKodu.Contains("320."))
            {
                tipKodu = "320.01.";
            }
            else if (tipKodu.Contains("600."))
            {
                tipKodu = "600.01.";
        
            }
            else if (tipKodu.Contains("610."))
            {
                tipKodu = "610.01.";
            }
            else if (tipKodu.Contains("340"))
            {
                tipKodu = "340.";
            }
            switch (tipKodu)
            {
                case "120.01.": tipAdi = babonline.Customer; break; // Müşteri
                case "320.01.": tipAdi = babonline.InsuranceCompany; break; // "Sigorta Şirketi"
                case "340.": tipAdi = babonline.TVM_Broker; break; // Yurt Dışı Broker
                case "330.": tipAdi = babonline.ThirdPartyCompanies; break; //Üçüncü Şahıs Firmalar
                case "600.01.": tipAdi = babonline.Commission_Income_Account; break; // Komisyon Gelirleri Hesabı
                case "610.01.": tipAdi = babonline.SalesReturns; break; // Satış İadeleri
                case "611.01.": tipAdi = babonline.Sales_Commissions; break; // Satış Komisyonları
                case "100.01.": tipAdi = babonline.Cash_Account; break; // Kasa Hesabı
                case "102.01.": tipAdi = babonline.Bank_Account; break; // Banka Hesabı
                case "101.01.": tipAdi = babonline.Checks_Received; break; // Alınan Çekler
                case "103.01.": tipAdi = babonline.Issued_Checks + " / " + babonline.Payment_Orders; break; // Verilen Çekler / Ödeme Emirleri
                case "121.01.": tipAdi = babonline.Notes_Receivable; break; // Alacak Senetleri
                case "321.01.": tipAdi = babonline.Notes_Payable; break; // Borç Senetleri
                case "108.01.": tipAdi = babonline.AgencyPosAccount; break; // Acente Pos Hesabı
                case "309.01.": tipAdi = babonline.Agency + " " + babonline.Credit_Card; break; // Acente Kredi Kartı
                case "399.01.": tipAdi = babonline.AgencyIndividualCCard; break; //Acente Bireysel K. Kratı
                case "612.01.": tipAdi = babonline.Other_Discounts; break; // Diğer İndirimler
                case "612.02": tipAdi = babonline.Discounts; break; // İskontolar
                case "740.": tipAdi = babonline.General_Expenses; break; // Genel Giderler
                case "602.": tipAdi = babonline.Other_Income; break; // Diğer Gelirler

                default:
                    tipAdi = "";
                    break;
            }

            return tipAdi;
        }
    }
    public static class CariEvrakTipKodu
    {
        public static byte PrimTahakkuk = 1;
        public static byte PrimIadesi = 2;
        public static byte KomisyonTahakkuk = 3;
        public static byte KomisyonIadesi = 4;
        public static byte KomisyonTahsilati = 5;
        public static byte KomisyonOdemesi = 6;
        public static byte AlisFaturasi = 7;
        public static byte SatisFaturasi = 8;
        public static byte DevirKaydi = 9;
        public static byte TahsilatIptal = 14;
        public static byte PrimTahsilat = 15;
        public static byte PesinOdenenVergi = 24;
        public static byte DigerUwKomisyon = 25;
        public static byte DigerUWPrim = 26;
        public static byte PesinOdenenVergiIadesi = 27;
        public static byte DigerUWPrimIadesi = 28;
        public static byte DigerUwKomisyonIadesi = 29;
        public static byte UWKomisyonTahsilati = 30;
        public static byte YDBRKRKomisyonOdemesi = 31;
        public static byte UWPrimOdemesi = 32;
    }
    public static class CariOdemeTipKodu
    {
        public static int Nakit = 1;
        public static int MusteriKrediKarti = 2;
        public static int Havale = 3;
        public static int Cek = 4;
        public static int AcenteKrediKarti = 5;
        public static int AcentePOSHesabi = 6;
        public static int Senet = 7;
        public static int AcenteBireyselKart = 9;
    }
}
