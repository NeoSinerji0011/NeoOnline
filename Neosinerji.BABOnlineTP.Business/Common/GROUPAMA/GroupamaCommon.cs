using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common.GROUPAMA
{
    public class GroupamaUrunKodlari
    {
        public const int Trafik = 3102;
        public const int HususiOtoKasko = 3700;
        public const int ElitKasko = 3701;
        public const int KlasikKasko = 3660;
    }


    public class Groupama_TrafikSorular
    {
        public const int KullanimTarzi = 1;
        public const int MarkaTip = 3;
        public const int Model = 20;
        public const int PlakaIlKodu = 9;
        public const int PlakaNumarasi = 43;
        public const int MotorNo = 41;
        public const int SasiNo = 42;
        public const int YerAdedi = 39;
        public const int TescilSeriKodu = 2687;
        public const int TescilSeriNo = 2587;
        public const int RuhsatDuzeltmeTarihi = 408;
        public const int TrafigeCikisTarihi = 1408;
        public const int SurucuAdedi = 51;
        public const int SurucuYrdAdedi = 52;
        public const int TrafikPulNo = 840;
        public const int TramerBelgeNo = 1409;
        public const int TramerBelgeTarihi = 1410;
        public const int TarifeBasamagi = 13;
        public const int OncekiSirketKodu = 1400;
        public const int OncekiAcenteKodu = 1456;
        public const int OncekiPoliceNo = 1457;
        public const int OncekiYenilemeNo = 1458;
        public const int OncekiTarifeBasamagi = 1404;
        public const int ZorunluKarayoluTasimacilikMSSVar = 1319;
        public const int ZKYTMSSirketkodu = 1411;
        public const int ZKYTMSAcenteNo = 2234;
        public const int ZKYTMSPoliceNo = 2235;
        public const int ZKYTMSYenilemeNo = 2236;
        public const int KamuKurumIndirimi = 2189;


    }
    //3660 kamyonet
    public class Groupoma_KlasikKaskoSorular_SigortaKonusuListesi
    {
        //public const int ARAC = 1;
        //public const int YHIMS_SBBEDENI = 19;
        //public const int YHIMS_KBBEDENİ = 20;
        //public const int YHIMS_MADDİ = 21;
        public const int YHIMS_MADDİ_BEDENI = 43;
        public const int RADYO_TEYP_CD_PLAYER = 2;
        public const int KOLONLAR = 3;
        public const int EMTEA = 17;
        public const int DIGER_EK_CİHAZLAR = 33;
        public const int KASA_TANKER = 35;
        public const int TRANSMIKSER_BEDELI = 37;
        public const int ARAC_HUKUKSAL_KORUMA = 38;
        public const int SURUCU_HUKUKSAL_KOR = 39;
        public const int TMM = 40;
        public const int YHİMS_KB_MANEVİ_TAZ = 42;
        public const int PRIM_KORUMA = 51;

    }
    public class Groupoma_KlasikKaskoSorular_TeminatListesi
    {
        public const int AMORTISMAN = 12;
        public const int ANAHTAR_KAYBI_ZARARLARI = 419;
        public const int GREV_LOKAVT_KARGASALIK_HALK_HAREKETLERI = 4;
        public const int TEROR_SABOTAJ = 5;
        public const int SEL_SU = 6;
        public const int DEPREM = 11;
        public const int KULLANIM_VE_GELIR_KAYBI_EK_TEMINATI = 8;
        public const int YURT_DISI_KASKO = 282;
        public const int PATLAYICI_MADDDELER = 14;
        public const int PRIM_KORUMA = 473;
    }
    public class Groupoma_KlasikKaskoSorular_SoruListesi
    {
        public const int MARKA_TIP = 3;//
        public const int KULLANIM_TARZI = 1;//
        public const int MODEL = 20;//
        public const int YER_ADEDİ = 39;//
        public const int MOTOR_NUMARASI = 41;//
        public const int SASI_NUMARASI = 42;//
        public const int PLAKA_IL_KODU = 9;//
        public const int PLAKA_NUMARASI = 43;//
        public const int PESIN_INDIRIMI = 38;
        public const int RADYO_TEYP_MARKASI = 265;
        public const int KOLONLAR_MARKA = 266;
        public const int DIGER_EK_CIHAZLAR = 852;
        public const int DIS_KASKO_BITIS_TARIHI = 877;
        public const int ONCEKI_SIRKET_KODU = 1400;//
        public const int ONCEKI_ACENTE = 1456;//
        public const int ONCEKI_POLICE = 1457;//
        public const int ONCEKI_YENILEME_NO = 1458;//
        public const int TRAMER_BELGE_NO = 1409;//
        public const int TRAMER_BELGE_TARIHI = 1410;//
        public const int TRAMER_BELGE_SIRA_NO = 1455;
        public const int HASARSIZLIK_VARMI = 33;
        public const int HAS_TEN_KOR_VAR_MI = 251;
        public const int YAKINLIK_DERECESI = 2614;
        public const int YHIMS_BASAMAK_NO = 47;
        public const int KOLTUK_SAHIS_BASINA_OLUM_TAZMINATI = 53;//
        public const int KOLTUK_SBDS_TAZ = 58;//
        public const int KOLTUK_SAHIS_BASINA_TEDAVI_TAZMINATI = 54;//
        public const int MAL_CINSI = 333;
        public const int KASANIN_OZELLIGI = 1134;
        public const int ACIK_KASA_MI = 26;
        public const int EMTEA_CINSI = 25;
        public const int TASINAN_EMTEA_CESIDI = 457;
        public const int RIZIKO_FIYATI = 2214;
        public const int SIGORTALI_DOGUM_TARIHI = 1067;//
        public const int YHIMS_VARMI = 75;//
        public const int KOLTUK_FK_VARMI = 76;//
        public const int ACENTE_OZEL_INDIRIMI = 2329;
        public const int KAZA_DESTEK_VARMI = 2343;
        public const int TEMINAT_LIMITLERI = 151;//
        public const int MENFAATTAR = 816;
    }
    public class Groupoma_KlasikKaskoSorular_IMMTipi
    {
        public const int YHIMS_MADDI_BEDENI_AYRIMLI = 1;
        public const int YHIMS_YOK = 2;
        public const int YHIMS_MADDI_BEDENI_TEK_LIMIT = 3;
    }
    //3700 hususi oto
    public class Groupoma_HususiOtoKaskoSigortaKonusuListesi
    {
        public const int ARAC = 1;
        public const int RADYO_TEYP_CD_PLAYER = 2;
        public const int KOLONLAR = 3;
        public const int ALARM = 8;
        public const int CELIK_JANT = 9;
        public const int LPG = 16;
        public const int DIGER_EK_CIHAZLAR = 33;
        public const int PRIM_KORUMA = 50;

        public const int GroupamaAracHukuksalKoruma = 38;
        public const int GroupamaSurucuHukuksalKoruma = 39;
        public const int YHİMS_KB_MANEVİ_TAZ = 42;
        public const int YHIMS_MADDİ_BEDENI = 43;

    }
    public class Groupoma_HususiOtoKaskoSorular_TeminatListesi
    {
        public const int YURT_DISI_KASKO = 282;
        public const int PRIM_KORUMA = 473;
    }
    public class Groupoma_HususiOtoKaskoSorular_SoruListesi
    {
        public const int MARKA_TIP = 3; //
        public const int KULLANIM_TARZI = 1; //
        public const int MODEL = 20;  //
        public const int YER_ADEDI = 39; //
        public const int MOTOR_NUMARASI = 41; //
        public const int SASI_NUMARASI = 42;  //
        public const int PLAKA_IL_KODU = 9;   //
        public const int PLAKA_NUMARASI = 43; //
        public const int PESIN_INDIRIMI = 38;
        public const int ASISTANS_PLUS_PAKETI = 2986;
        public const int ALARM_VAR_MI = 2949;  //
        public const int ARAC_SURUCU_BILGISI = 2952;
        public const int ARAC_GECE_PARK_YERI = 2953;
        public const int EHLIYET_TARIHI = 2951;
        public const int RADYO_TEYP_MARKASI = 265;
        public const int KOLONLAR_MARKA = 266;
        public const int ALARM_MARKA = 267;
        public const int DIGER_EK_CIHAZLAR = 852;
        public const int KOLTUK_SAHIS_BASINA_OLUM_TAZMINATI = 53;//
        public const int KOLTUK_SBDS_TAZ = 58;//
        public const int KOLTUK_SAHIS_BASINA_TEDAVI_TAZMINATI = 54;//
        public const int TRAMER_BELGE_NO = 1409;//
        public const int TRAMER_BELGE_TARIHI = 1410;//
        public const int TRAMER_BELGE_SIRA_NO = 1455;
        public const int ONCEKI_SIRKET_KODU = 1400;  //
        public const int ONCEKI_ACENTE = 1456;  //
        public const int ONCEKI_POLICE = 1457;   //
        public const int ONCEKI_YENILEME_NO = 1458;     //
        public const int HASARSIZLIK_VARMI = 33;
        public const int HAS_TEN_KOR_VAR_MI = 251;
        public const int YAKINLIK_DERECESI = 2614;
        public const int RIZIKO_FIYATI = 2214;
        public const int SIGORTALI_DOGUM_TARIHI = 1067;  //
        public const int SIGORTALI_CINSIYETI = 1068;
        public const int OZEL_TUZEL_SIGORTALI = 1415;
        public const int ACENTE_OZEL_INDIRIMI = 2329;
        public const int TESCILNO_ASBISNO = 2587;  // TESCİL NO/ASBİS NO
        public const int KONTORL_PARAMETRE4 = 2257;
        public const int KAZA_DESTEK_VARMI = 2343;
        public const int TEMINAT_LIMITLERI = 151; //
        public const int MESLEK_INDIRIMI = 2130; //
    }
    public class Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular
    {
        public const int OTOMOBİL = 1;
        public const int TAKSİ = 2;
        public const int RENT_A_CAR_10 = 3;
        public const int MİNİBÜS_HATLI_DOLMUS_10_17_YOLCU_dankucuk = 5;
        public const int MİNİBÜS_ÖZEL_SERVİS_ŞEHİRİÇİ_10_17_YOLCU = 6;
        public const int MİDİBÜS_HATLI_ŞEHİRLER_ARASI_18_30_YOLCU = 7;
        public const int MİDİBÜS_ÖZEL_SERVİS_ŞEHİR_İÇİ_18_30_YOLCU = 8;
        public const int OTOBÜS_TİCARİ_ŞEHİRLER_ARASI_31_VE_ÜST = 10;
        public const int OTOBÜS_TİCARİ_ŞEHİR_İÇİ_ÖZEL_31_VE_ÜST = 11;
        public const int KAMYONET = 12;
        public const int KAMYON = 13;
        public const int ÇÖP_İTFAİYE_KAMYONU = 18;
        public const int MİKSER_BETON_POMPALI_KAYA_KAMYONU = 20;
        public const int SİLOLU_KAMYON = 19;
        public const int ÇEKİCİ_TIR = 14;
        public const int TANKER = 15;
        public const int TANKER_ASİT_TAŞIYAN = 16;
        public const int TANKER_ASİT_PETROL_VB_TAŞIMAYAN = 15;
        public const int ZIRHLI_ARAÇ = 21;
        public const int İŞ_MAKİNASI_HAREKETLİ = 22;
        public const int İŞ_MAKİNASI_SABİT = 23;
        public const int MOTOSİKLET_TRİPORTÖR = 26;
        public const int BİÇER_DÖVER = 25;
        public const int ZIRAİ_TRAKTÖR = 24;
        public const int RÖMORK = 27;
        public const int AMBULANS = 30;
        public const int ARAZİ_TAŞITI_JEEP = 28;
        public const int LOKOMOTİF_VAGON = 31;

    }

    //3701
    public class Groupoma_ElitKaskoSorular_SigortaKonusuListesi
    {
        public const int RADYO_TEYP_CD_PLAYER = 2;
        public const int KOLONLAR = 3;
        public const int ALARM = 8;
        public const int CELIK_JANT = 9;
        public const int LPG = 16;
        public const int DIGER_EK_CIHAZLAR = 33;
        public const int PRIM_KORUMA = 50;

    }
    public class Groupoma_ElitKaskoSorular_TeminatListesi
    {
        public const int YURT_DISI_KASKO = 282;
        public const int PRIM_KORUMA = 473;
    }
    public class Groupoma_ElitKaskoSorular_SoruListesi
    {
        public const int MARKA_TIP = 3;
        public const int KULLANIM_TARZI = 1;
        public const int MODEL = 20;
        public const int YER_ADEDI = 39;
        public const int MOTOR_NUMARASI = 41;
        public const int SASI_NUMARASI = 42;
        public const int PLAKA_IL_KODU = 9;
        public const int PLAKA_NUMARASI = 43;
        public const int PESİN_İNDİRİMİ = 38;
        public const int ASISTANS_PLUS_PAKETI = 2986;
        public const int ALARM_VAR_MI = 2949;
        public const int ARAC_SURUCU_BILGISI = 2952;
        public const int ARAC_GECE_PARK_YERI = 2953;
        public const int EHLIYET_TARIHI = 2951;
        public const int dıs_kasko_bıtıs_tarıhı = 877;
        public const int RADYO_TEYP_MARKASI = 265;
        public const int KOLONLAR_MARKA = 266;
        public const int ALARM_MARKA = 267;
        public const int DIGER_EK_CİHAZLAR = 852;
        public const int KOLTUK_SAHIS_BASINA_OLUM_TAZMINATI = 53;
        public const int KOLTUK_SBDS_TAZ = 58;
        public const int KOLTUK_SAHIS_BASINA_TEDAVI_TAZMINATI = 54;
        public const int TRAMER_BELGE_NO = 1409;
        public const int TRAMER_BELGE_TARIHI = 1410;
        public const int TRAMER_BELGE_SIRA_NO = 1455;
        public const int ONCEKI_SIRKET_KODU = 1400;
        public const int ONCEKI_ACENTE = 1456;
        public const int ONCEKI_POLICE = 1457;
        public const int ONCEKI_YENILEME_NO = 1458;
        public const int HASARSIZLIK_VARMI = 33;
        public const int HAS_TEN_KOR_VAR_MI = 251;
        public const int YAKINLIK_DERECESI = 2614;
        public const int RIZIKO_FIYATI = 2214;
        public const int SIGORTALI_DOGUM_TARIHI = 1067;
        public const int SIGORTALI_CINSIYETI = 1068;
        public const int OZEL_TUZEL_SIGORTALI = 1415;
        public const int ACENTE_OZEL_INDIRIMI = 2329;
        public const int TESCILNO_ASBISNO = 2587;
        public const int KONTORL_PARAMETRE4 = 2257;
        public const int KAZA_DESTEK_VARMI = 2343;
        public const int TEMINAT_LIMITLERI = 151;

    }

    public class Groupoma_Odeme_Plani
    {
        public const int KrediKartiBlokeli = 86;
    }

    public class Groupoma_CevapTipi
    {
        public const double Evet = 1;
        public const double Hayir = 2;
    }


    public class GroupamaYHIMSecimleri
    {
        public const int Ayrimli = 1;
        public const int Yok = 2;
        public const int TekLimit = 3;
    }

    
}
