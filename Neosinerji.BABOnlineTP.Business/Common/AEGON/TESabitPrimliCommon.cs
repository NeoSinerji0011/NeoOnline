using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class TESabitPrimliParaBirimi
    {
        public const string Avro = "EUR";
        public const string ABD_Dolar = "USD";

        public static string ParaBirimiText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "EUR"; break;
                case "2": result = "USD"; break;
            }

            return result;
        }
    }

    public class TESabitPrimliHesaplamaSecenegi
    {
        public const int AnaTeminatTutari = 1;
        public const int YillikPrim = 2;

        public static string GetText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "Ana Teminat"; break;
                case "2": result = "Yıllık Prim"; break;
            }

            return result;
        }
    }

    public class TESPAnaTeminatlar
    {
        public const byte Vefat = 1;
        public const byte VefatVeKritikHastalik = 2;

        public static string AnaTeminatText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "Vefat"; break;
                case "2": result = "Vefat ve Kritik Hastalık"; break;
            }

            return result;
        }
    }

    public class TESabitPrimliSorular
    {
        public const int SigortaBaslangicTarihi = 212;
        public const int ParaBirimi = 213;
        public const int SigortaSuresi = 214;
        public const int PrimOdemeDonemi = 215;
        public const int MusteriGelirVergisiOrani = 223;
        public const int HesaplamaSecenegi = 224;
        public const int YillikPrimTutari = 225;

        //Teminat il ilgili Sorular
        //Ana Teminatlar
        public const int AnaTeminat = 216;
        public const int Vefat_KritikHastalik = 217;


        //EK TEMINATLAR
        public const int KritikHastaliklar = 218;
        public const int TamVeDaimiMaluliyet = 219;
        public const int KazaSonucuVefat = 220;
        public const int TopluTasimaAraclariKSV = 221;
        public const int MaluliyetYillikDestek = 222;
        //Yeni Teminatlar
        public const int KazaSonucu_TedaviMasraflari = 280;
        public const int KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme = 281;



        //Yeni Sorular
        public const int VergiAvantajiSonrasiPrimMaliyeti = 282;
        public const int PrimdenVergiAvantaji = 283;
        public const int SureBoyuncaOdenecekToplamP = 284;
    }

    public class TESabitPrimliTeminatlar
    {
        //ANA TEMINATLAR
        public const int Vefat = 132;
        public const int Vefat_KritikHastalik = 133;

        //EK TEMINATLAR
        public const int KritikHastaliklar = 134;
        public const int TamVeDaimiMaluliyet = 135;
        public const int KazaSonucuVefat = 136;
        public const int TopluTasimaAraclariKSV = 137;
        public const int MaluliyetYillikDestek = 138;
        public const int KazaSonucu_TedaviMasraflari_EkTeminati = 147;
        public const int KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme = 148;

        public static string GetTeminatAdiText(int kodu)
        {
            string result = String.Empty;
            switch (kodu)
            {
                case TESabitPrimliTeminatlar.Vefat: result = "Vefat AnaTeminatı"; break;
                case TESabitPrimliTeminatlar.Vefat_KritikHastalik: result = "Vefat veya Kritik Hastalık Ana Teminatı"; break;
                case TESabitPrimliTeminatlar.KritikHastaliklar: result = "Kritik Hastalıklar Ek Teminatı"; break;
                case TESabitPrimliTeminatlar.KazaSonucuVefat: result = "Kaza Sonucu Vefat Ek Teminatı"; break;
                case TESabitPrimliTeminatlar.TamVeDaimiMaluliyet: result = "Tam ve Daimi Maluliyet Ek Teminatı"; break;
                case TESabitPrimliTeminatlar.MaluliyetYillikDestek: result = "Maluliyet Yıllık Destek Ek Teminatı"; break;
                case TESabitPrimliTeminatlar.TopluTasimaAraclariKSV: result = "Toplu Taşıma Araçlarında Kaza Sonucu Vefat Ek Teminatı"; break;
            }
            return result;
        }
    }
}
