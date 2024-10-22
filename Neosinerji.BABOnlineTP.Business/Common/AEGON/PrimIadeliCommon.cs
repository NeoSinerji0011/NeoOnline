using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common.AEGON
{
    public class PrimIadeliSorular
    {
        //Genel sorular
        public const int SigortaBaslangicTarihi = 212;
        public const int ParaBirimi = 213;
        public const int SigortaSuresi = 214;
        public const int PrimOdemeDonemi = 215;
        public const int VefatTeminatTutari = 216;
        public const int HesaplamaSecenegi = 224;
        public const int YillikPrimTutari = 225;
        public const int SurprimOrani = 227;
        public const int SureSonuPrimIadesiTeminati = 226;
    }

    public class PrimIadeliTeminatlar
    {
        //Ana teminatlar
        public const int VefatTeminati = 132;
        public const int SureSonuPrimIadesiTeminati = 139;
    }

    public class ROP_HesaplamaSecenekleri
    {
        public const int YillikPrim = 1;
        public const int VefatTeminati = 2;

        public static string GetHesaplamaSecenegi(int kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case ROP_HesaplamaSecenekleri.YillikPrim: result = "Yıllık Prim"; break;
                case ROP_HesaplamaSecenekleri.VefatTeminati: result = "Vefat Teminatı"; break;
            }

            return result;
        }
    }

    public class ROP_SigortaSureleri
    {
        public const int oniki = 1;
        public const int onbes = 2;
        public const int onsekiz = 3;
        public const int yirmidort = 4;
        public const int otuz = 5;


        public static string GetSigortaSuresi(int kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case ROP_SigortaSureleri.oniki: result = "12"; break;
                case ROP_SigortaSureleri.onbes: result = "15"; break;
                case ROP_SigortaSureleri.onsekiz: result = "18"; break;
                case ROP_SigortaSureleri.yirmidort: result = "24"; break;
                case ROP_SigortaSureleri.otuz: result = "30"; break;

            }

            return result;
        }
    }
}
