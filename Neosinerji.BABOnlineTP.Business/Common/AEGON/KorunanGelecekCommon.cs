using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class KorunanGelecekSabitleri
    {
        //Giriş Yası limitlri
        public const int VefatMaxSigGirisYasi = 65;
        public const int MaluliyetMaxGirisYasi = 55;
       

        //Sigortalanabilir Yas limitlri
        public const int VefatMaxSigortalanabilirYas = 75;
        public const int MaluliyetMaxSigortalanabilirYas = 65;
       
    }

    public class KorunanGelecekParaBirimi
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

    public class KorunanGelecekSorular
    {
        //Genel Sorular
        //Genel sorular
        public const int SigortaBaslangicTarihi = 212;
        public const int ParaBirimi = 213;
        public const int SigortaSuresi = 214;
        public const int PrimOdemeDonemi = 215;

        //Teminat Seçimiyle İlgili Sorular
        //Ana Teminat
        public const int VefatTeminati = 216;
        //Ek Teminat
        public const int MaluliyetYillikDestek = 222;

    }

    public class KorunanGelecekTeminatlar
    {
        //Ana Teminat
        public const int Vefat = 132;
        //Ek Teminat
        public const int MaluliyetYillikDestek = 141;
    }
}
