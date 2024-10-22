using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class EUREKO_TrafikSoruTipNo
    {
        //public const string MarkaModel = "127";
        //public const string KullanimTarzi = "115";
        //public const string PlakaIl = "123";
        //public const string PlakaDetay = "128";
        //public const string KoltukAdedi = "139";
        //public const string TrafikTescilTarihi = "638";
        //public const string TrafikCikisTarihi = "624";
        //public const string RuhsatTescilKodu = "1459";
        //public const string RuhsatTescilRefNo = "1460";
        //public const string SasiNo = "122";
        //public const string MotorNo = "121";

        public const string MarkaModel = "127";
        public const string KullanimTarzi = "115";
        public const string PlakaIl = "123";
        public const string PlakaDetay = "128";
        public const string KoltukAdedi = "139";
        public const string TrafikTescilTarihi = "638";
        public const string TrafikCikisTarihi = "624";
        public const string RuhsatTescilKodu = "1459";
        public const string RuhsatTescilRefNo = "1460";
        public const string SasiNo = "122";
        public const string MotorNo = "121";
        public const string TarifBasamakKodu = "411";

        public const string IlkPoliceMi = "647";
        public const string OPB_SigortaSirketi = "124";
        public const string OPB_PoliceNo = "124";
        public const string OPB_AcenteNo = "627";
        public const string OPB_YenilemeNo = "628";

 
    }

    public class EUREKO_TrafikSoruTipDetayNo
    {
        public const string KTAracKodu = "0"; // 01
        public const string KTKullanimSekli = "1"; //A
        public const string MMAracDegerkodu = "0"; // Marka+tipkodu
        public const string MMAracUretimYili = "3"; //2015       
        public const string PlakaIl = "0";
        public const string PlakaDetay = "0";
        public const string KoltukAdedi = "0";
        public const string TrafikTescilTarihi = "0";
        public const string TrafikCikisTarihi = "0";
        public const string RuhsatTescilKodu = "0";
        public const string RuhsatTescilRefNo = "0";
        public const string SasiNo = "0";
        public const string MotorNo = "0";
        public const string TarifBasamakKodu = "0";

        public const string IlkPoliceMi_Detay = "0";
        public const string OPB_SigortaSirketi_Detay = "0";
        public const string OPB_PoliceNo_Detay = "1";
        public const string OPB_AcenteNo_Detay = "0";
        public const string OPB_YenilemeNo_Detay = "0";
    }

    public class EUREKO_TrafikTeminatlar
    {
        public const string UcuncuSahisMaddiZararlar = "158";
        public const string UcuncuSahisBedeniZararlar = "157";
        public const string UcuncuSahisTedaviMasraflari = "159";
        public const string SGK = "328";
    }

    public class EUREKO_Urunkodlari
    {
        public const string KASKO = "O30";
        public const string TRAFIK = "T10";
    }

    public class EUREKO_KaskoTeminatlar
    {
        public const string KaskoYangin = "92";
        public const string KaskoHirsizlik = "98";
        public const string GLHHareketleri = "128";
        public const string Terorizm = "134";
        public const string Olum = "32";
        public const string SurekliSakatlik = "108";
        public const string SahisBedeniZararlar = "157";
        public const string SahisMaddiZararlar = "158";
        public const string ESigortaAsistans = "171";
        public const string Deprem = "40";
        public const string Seylap = "54";
        public const string HukuksalKoruma = "166";
        public const string HasarsizlikKoruma = "320";
        public const string KemirgenHayvanTeminati = "475";
    }


    public class URL
    {
        public string pdfURL;
        public string bilgilendirmeURL;
    }
}
