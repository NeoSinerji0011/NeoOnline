using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class OdemeGUvenceSabitleri
    {
        //Giriş Yası limitlri
        public const int KHPMMaxGirisYasi = 60;
        public const int MDPMMaxGirisYasi = 60;
        public const int KHYPMMaxGirisYasi = 60;
        public const int IDPMMaxGirisYasi = 55;
        public const int VEFPMMaxGirisYasi = 70;

        //Sigortalanabilir Yas limitlri
        public const int KHPMMaxSigortalanabilirYas = 65;
        public const int MDPMMaxSigortalanabilirYas = 65;
        public const int KHYPMMaxSigortalanabilirYas = 65;
        public const int IDPMMaxSigortalanabilirYas = 65;
        public const int VEFPMMaxSigortalanabilirYas = 75;

    }

    public class OdemeGuvenceParaBirimi
    {
        public const string Avro = "EUR";
        public const string ABD_Dolar = "USD";
        public const string TL = "TL";

        public static string ParaBirimiText(string kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case "1": result = "EUR"; break;
                case "2": result = "USD"; break;
                case "3": result = "TL"; break;
            }

            return result;
        }
    }

    public class OdemeGuvenceSorular
    {
        //Genel Sorular
        public const int SigortaBaslangicTarihi = 212;
        public const int PrimOdemeDonemi = 215;

        //Ek Teminat Seçenekleri
        public const int KritikHastalikDPM = 218;
        public const int TamVeTaimiMaluliyetDPM = 219;
        public const int IssizlikDPM = 244;
        public const int KazaSonucuHastanedeyatarakTDPM = 220;

        //Odeme Güvence KAP Sorular

        public const int PoliceBaslangicTarihi = 11;
        public const int PoliceNumarasi = 112;
        public const int ParaBirimi = 213;
        public const int SigortaSuresi = 214;
        public const int aylikPrimTutari = 225;
        public const int KapYildonumuKalanSure = 250;

        public const int SS_TOPLAM_PRIM = 251;
        public const int VAS_PRIM_MALIYETI = 252;
        public const int PRIMDEN_VERGI_AVANTAJI = 253;
        public const int YILDNM_KADAR_TOP_PRIM = 254;
        public const int YILDNM_SONRA_TOP_PRIM = 255;
        public const int YILDNM_KADAR_VER_AVANTAJI = 256;
        public const int YILDNM_SONRA_VER_AVANTAJI = 257;

        public const int IssizlikDPM_Faydalanabilirmi = 292;
    }

    public class OdemeGuvenceTeminatlar
    {
        public const int VefatDurumundaPMT = 142;
        public const int KritikHastalikDPM = 143;
        public const int TamVeTaimiMaluliyetDPM = 144;
        public const int IssizlikDPM = 145;
        public const int KazaSonucuHastanedeyatarakTDPM = 146;
    }
}
