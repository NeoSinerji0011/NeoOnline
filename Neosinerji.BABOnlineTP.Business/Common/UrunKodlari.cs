using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class UrunKodlari
    {
        public const int TrafikSigortasi = 1;
        public const int KaskoSigortasi = 2;
        public const int DogalAfetSigortasi_Deprem = 3;
        public const int KonutSigortasi_Paket = 4;
        public const int FerdiKazaSigortasi = 5;
        public const int AcilSaglik = 6;
        public const int YurtDisiSeyehatSaglik = 7;
        public const int TamamlayiciSaglik = 8;
        public const int KrediHayat = 9;
        public const int IkinciElGaranti = 10;
        public const int IsYeri = 11;
        public const int TESabitPrimli = 12;
        public const int OdulluBirikim = 13;
        public const int KorunanGelecek = 14;
        public const int PrimIadeli = 15;
        public const int Egitim = 16;
        public const int MapfreKasko = 17;
        public const int MapfreTrafik = 18;
        public const int OdemeGuvence = 19;
        public const int KritikHastalik = 20;
        public const int PrimIadeli2 = 21;
        public const int FerdiKazaPlus = 22;
        public const int Lilyum = 24;
        public const int SeyahatSigortasi = 25;
        public const int NipponSaglik = 26;
        public const int Reasuror = 27;
        public static string GetUrunAdi(int UrunKodu)
        {
            string result = String.Empty;

            switch (UrunKodu)
            {
                case UrunKodlari.TrafikSigortasi: result = ResourceHelper.GetString("Traffic"); break;
                case UrunKodlari.KaskoSigortasi: result = ResourceHelper.GetString("Insurance"); break;
                case UrunKodlari.DogalAfetSigortasi_Deprem: result = "Dask"; break;
                case UrunKodlari.KonutSigortasi_Paket: result = ResourceHelper.GetString("Home2"); break;
                case UrunKodlari.FerdiKazaSigortasi: result = ResourceHelper.GetString("PersonalAccident"); break;
                case UrunKodlari.AcilSaglik: result = ResourceHelper.GetString("Emergency_Healt"); break;
                case UrunKodlari.YurtDisiSeyehatSaglik: result = ResourceHelper.GetString("ForeignTravelHealth_Short"); break;
                case UrunKodlari.TamamlayiciSaglik: result = ResourceHelper.GetString("Your_health_is_HMO"); break;
                case UrunKodlari.KrediHayat: result = ResourceHelper.GetString("CreditLife"); break;
                case UrunKodlari.IkinciElGaranti: result = ResourceHelper.GetString("Used_Guarantee"); break;
                case UrunKodlari.IsYeri: result = ResourceHelper.GetString("Workplace"); break;
                case UrunKodlari.TESabitPrimli: result = ResourceHelper.GetString("TE_Fixed_Payable"); break;
                case UrunKodlari.OdulluBirikim: result = ResourceHelper.GetString("TheAwardWinningAccumulation"); break;
                case UrunKodlari.KorunanGelecek: result = ResourceHelper.GetString("ProtectedFuture"); break;
                case UrunKodlari.PrimIadeli: result = ResourceHelper.GetString("PrimeReturnable"); break;
                case UrunKodlari.Egitim: result = ResourceHelper.GetString("Education"); break;
                case UrunKodlari.MapfreKasko: result = ResourceHelper.GetString("Insurance"); break;
                case UrunKodlari.MapfreTrafik: result = ResourceHelper.GetString("Traffic"); break;
                case UrunKodlari.OdemeGuvence: result = ResourceHelper.GetString("PaymentProtectionSystem"); break;
                case UrunKodlari.PrimIadeli2: result = ResourceHelper.GetString("PrimeReturnable2"); break;
                case UrunKodlari.FerdiKazaPlus: result = ResourceHelper.GetString("PersonalAccidentPlus"); break;
                case UrunKodlari.Lilyum: result = ResourceHelper.GetString("Lilyum"); break;
                case UrunKodlari.SeyahatSigortasi: result = ResourceHelper.GetString("Travel"); break;
                case UrunKodlari.NipponSaglik: result = ResourceHelper.GetString("Healty"); break;
                case UrunKodlari.Reasuror: result = ResourceHelper.GetString("Reasuror"); break;
            }

            return result;
        }

        public static string GetUrunAdiBASE(int UrunKodu)
        {
            string result = String.Empty;
            switch (UrunKodu)
            {
                case UrunKodlari.TrafikSigortasi: result = "Trafik"; break;
                case UrunKodlari.KaskoSigortasi: result = "Kasko"; break;
                case UrunKodlari.DogalAfetSigortasi_Deprem: result = "Dask"; break;
                case UrunKodlari.KonutSigortasi_Paket: result = "Konut"; break;
                case UrunKodlari.FerdiKazaSigortasi: result = "FerdiKaza"; break;
                case UrunKodlari.AcilSaglik: result = "AcilSaglik"; break;
                case UrunKodlari.YurtDisiSeyehatSaglik: result = "YurtDisiSeyehat"; break;
                case UrunKodlari.TamamlayiciSaglik: result = "Sağlığınız Bizde-HMO"; break;
                case UrunKodlari.KrediHayat: result = "KrediliHayat"; break;
                case UrunKodlari.IkinciElGaranti: result = "ikinciElGaranti"; break;
                case UrunKodlari.IsYeri: result = "isYeri"; break;
                case UrunKodlari.TESabitPrimli: result = "TESabitPirimli"; break;
                case UrunKodlari.OdulluBirikim: result = "OdulluBirikim"; break;
                case UrunKodlari.KorunanGelecek: result = "KorunanGelecek"; break;
                case UrunKodlari.PrimIadeli: result = "PrimIadeli"; break;
                case UrunKodlari.Egitim: result = "Egitim"; break;
                case UrunKodlari.MapfreKasko: result = "MapfreKasko"; break;
                case UrunKodlari.MapfreTrafik: result = "MapfreTrafik"; break;
                case UrunKodlari.OdemeGuvence: result = "OdemeGuvence"; break;
                case UrunKodlari.PrimIadeli2: result = "PrimIadeli2"; break;
                case UrunKodlari.FerdiKazaPlus: result = "FerdiKazaPlus"; break;
                case UrunKodlari.Lilyum: result = "LilyumFerdiKaza"; break;
                case UrunKodlari.SeyahatSigortasi: result = "Travel"; break;
                case UrunKodlari.NipponSaglik: result = "NipoonSaglikSigortasi"; break;
                case UrunKodlari.Reasuror: result = "Reasuror"; break;

            }

            return result;
        }
    }
}
