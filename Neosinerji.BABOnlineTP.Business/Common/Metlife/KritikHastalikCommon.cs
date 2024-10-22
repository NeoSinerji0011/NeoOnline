using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KritikHastalikParaBirimi
    {
        public const string Tl = "TL";
        public const string Avro = "EUR";
        public const string ABD_Dolar = "USD";

        public static string ParaBirimiText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "TL"; break;
                case "2": result = "EUR"; break;
                case "3": result = "USD"; break;
            }

            return result;
        }
    }  
    public class KritikHastalikSorular
    {
        //Genel sorular
       
        public const int ParaBirimi = 213;
        public const int SigortaSuresi = 214;
        public const int Meslek = 1;
        public const int Kisi_Sayisi = 101;

       
        //Teminat Sorular
        public const int VefatTeminati = 1;
        public const int KazaSonucuMaluliyet = 1;
        public const int HastalikSonucuMaluliyet = 1;
        public const int TehlikeliHastalik = 1;
        public const int TeminatTutari = 1;
        public const int TeminatTutariDiger = 1;

        //Lehtar
        public  const int adi =1;
        public  const int soyadi=1;
        public  const int dogumTarihi=1;
        public const int oran = 1;
    }

    public class KritikHastalikTeminatlari
    {
        //Ana teminatlar
      
        public const int VefatTeminati = 132;
        public const int KazaSonucuMaluliyet = 1;
        public const int HastalikSonucuMaluliyet = 1;
        public const int TehlikeliHastalik = 1;
    }
}
