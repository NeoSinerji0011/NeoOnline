using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common.AEGON
{

    public class OdulluBirikimSorular
    {
        //Genel Sorular
        public const int SigortaBaslangicTarihi = 212;
        public const int ParaBirimi = 213;
        public const int SigortaSuresi = 214;
        public const int PrimOdemeDonemi = 215;

        public const int HesaplamaSecenegi = 224;
        public const int YillikPrimTutari = 225;
        public const int SureSonuPrimIadesiTeminati = 226;
    }

    public class OdulluBirikimTeminatlar
    {
        //Ana Teminat 
        public const int Vefat = 132;
        public const int SureSonuPrimIadesiTeminati = 139;

        //Ek Teminat
        public const int EkOdemeTeminati = 140;
    }

    public class ROL_HesaplamaSecenekleri
    {
        public const int YillikPrim = 1;
        public const int SureSonuBirikim = 2;

        public static string GetHesaplamaSecenegi(int kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case ROL_HesaplamaSecenekleri.YillikPrim: result = "Yıllık Prim"; break;
                case ROL_HesaplamaSecenekleri.SureSonuBirikim: result = "Süre Sonu Birikim"; break;
            }

            return result;
        }
    }
}
