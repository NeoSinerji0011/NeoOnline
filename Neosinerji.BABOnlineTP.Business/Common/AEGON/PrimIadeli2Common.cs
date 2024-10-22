using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common.AEGON
{
    public class PrimIadeli2YillikPrim
    {
        public const string bir = "480";
        public const string iki = "960";
        public const string uc = "1440";

        public static string YillikPrimText(string kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case "1": result = "480"; break;
                case "2": result = "960"; break;
                case "3": result = "1440"; break;
            }

            return result;
        }
    }

    public class PrimIadeli2Sorular
    {
        //Genel sorular
        public const int SigortaBaslangicTarihi = 212;
        public const int ParaBirimi = 213;
        public const int SigortaSuresi = 214;
        public const int PrimOdemeDonemi = 215;
        public const int SurprimOrani = 227;

        public const int HesaplamaSecenegi = 224;
        public const int YillikPrimTutari = 225;
        public const int VefatTeminatTutari = 252;

        //Teminat Soruları
        public const int Vefat = 216;
        public const int SureSonuPrimIadesiTeminati = 226;
    }

    public class PrimIadeli2Teminatlar
    {
        //Ana teminatlar
        public const int VefatTeminati = 132;
        public const int SureSonuPrimIadesiTeminati = 139;
    }

    public class Prim_HesaplamaSecenekleri
    {
        public const int YillikPrim = 1;
        public const int VefatTeminati = 2;

        public static string GetHesaplamaSecenegi(int kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case Prim_HesaplamaSecenekleri.YillikPrim: result = "Yıllık Prim"; break;
                case Prim_HesaplamaSecenekleri.VefatTeminati: result = "Vefat Teminatı"; break;
            }

            return result;
        }

    }

    public class Prim_YillikTutarSecenekleri
    {
        public const int Tutar1 = 1;
        public const int Tutar2 = 2;
        public const int Tutar3 = 3;

        public static string GetYillikPrimSecenegi(int kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case Prim_YillikTutarSecenekleri.Tutar1: result = "480"; break;
                case Prim_YillikTutarSecenekleri.Tutar2: result = "960"; break;
                case Prim_YillikTutarSecenekleri.Tutar3: result = "1440"; break;
            }

            return result;
        }
    }
}
