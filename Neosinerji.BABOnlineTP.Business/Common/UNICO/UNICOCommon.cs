using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class UNICO_TeminatKodlari
    {
        //ZORUNLULAR
        public const string GLKHHKNH_TEROR = "107";
        public const string SELSU_BASKINI = "120";
        public const string IKAME_ARAC = "428";
        public const string HUKUKSAL_KORUMA = "45";
        public const string KILIT_SISTEMI = "627";
        public const string KASKO = "630";

        public const string DEPREM = "691";
        public const string SELVE_SU_BASKINI = "692";
        public const string KASKOLAY_YARDIM = "695";
        public const string MINI_ONARIM = "696";
        public const string IHTMALI_SORUMLULUK = "730";
        public const string OLUMMALUL_SB = "731";
        public const string OLUMMALUL_KB = "732";
        public const string MADDI_HASAR = "740";
        public const string KOLTUK_FERDI_KAZA = "750";

        //SEÇİMLİLER
        public const string AKSESUAR_TEMINATI = "ASU";
        public const string EK_HUKUKSAL_KORUMA = "48";
        public const string POLICE_PRIMI_KORUMA = "625";
        public const string ULASIM_MASRAFLARI_KARSILAMA = "626";
        public const string MEKANIK_ARIZA = "632";
        public const string GENISLETILMIS_BAKIM = "639";
        public const string SES_SISTEMI = "641";
        public const string GORUNTU_SISTEMI = "642";
        public const string KOLON_SISTEMI = "643";
        public const string TELSIZ_TELEFON = "650";
        public const string ACIL_YARDIM_AMBULANS = "654";
        public const string AIRCONDITION = "660";
        public const string YURT_DISI = "665";
        public const string TASINAN_YUK = "670";
        public const string KULL_GELIR_KAYBI = "680";
        public const string FERDI_KAZA = "751";
        public const string TEDAVI_MASRAFLARI = "760";
        public const string ANAHTARLA_CALINMA = "950";
        public const string CAM_KIRILMASI = "651";
        public const string YANLIS_AKARYAKIT_DOLUMU = "667";


    }
    public class UNICO_Matbular
    {
        public const int PLAKA = 5;
        public const int MOTORNO = 6;
        public const int SASINO = 7;
        public const int TESCILTARIHI = 18;
        public const int HASINDRMORANI = 11;
        public const int POLICESURESI = 34;
    }
    public class UNICO_Istatistikler
    {
        //ZORUNLULAR
        public const string HASARSIZLIK_KORUMA_KLOZU = "HSZ";
        public const string IMM_BEDENI_MADDI = "IMM";
        public const string YURT_ICI_YURT_DISI = "KIB";
        public const string KILIT_BEDELI = "KLT";
        public const string KIRALIK_ARAC_MI = "KRA";
        public const string ARACIN_TIPI = "MAR";
        public const string DEP_SEL_MUAFIYETI = "MFD";
        public const string MANEVI_TAZMINAT = "MNT";
        public const string ARACIN_MARKASI = "MRG";
        public const string SURUCU_KURSU_ARACI_MI = "SRK";
        public const string SURUCU_SAYISI = "SSA";
        public const string IKAME_ARAC_SECIMI = "UST";
        public const string YENI_DEGER_KLOZU = "YDE";
        public const string SERVIS_SARTI = "YET";
        public const string MODEL_YILI = "YIL";
        public const string TEB_UYESIMI = "TBU";


        public const string SIGORTALI_YASI = "SYA";
        public const string POLICE_SURESI_GUN = "YYY";
        public const string ARAC_YASI_411 = "ARG";
        public const string TOPLAM_HASAR_ADEDI = "THA";
        public const string YETKI = "YTK";

        public const string ONCEKI_SIRKET_KODU = "OSK";
        public const string OZEL_TICARI = "KUS";
        public const string HASARSIZLIK_INDIRIM_ORANI = "KKI";
        public const string HASARLILIK_SURPRIMI = "KK1";


        public const string KULLANIM_SEKLI = "AKL";
        public const string SES_SISTEMI_OPSIYONEL_MI = "SST";
        public const string GORUNTU_SISTEMI_OPSIYONEL_MI = "GST";


        public const string ARAC_BEDELI_ANAHTAR_TESLIM = "ANA";
        public const string ENGELLI_ARACIMI = "ENG";
        public const string TEK_SURUCU_MU = "TEK";
        public const string ARAC_BEDELI = "ABD";
        public const string MAVI_PLAKA_EKPRIMI = "MPE";
        public const string DAINI_MURTEHIN_VAR_MI = "DAM";
        public const string ZEYIL_HASAR_PRIMI = "SHZ";

        public const string ACENTE_ISTATISTIGI = "AC1";
        public const string KADEME_SON = "KKZ";

        public const string YURT_DISI_TEM_VARMI = "YTT";

        public const string ONCEKI_POLICE_BASAMAK_KODU = "OPM";


        public const string IL_KODU = "ILK";
        public const string TESCIL_GUN_FARKI = "TGF";
        public const string HASAR_ADEDI = "HSA";
        public const string GDV_VAR_MI = "VRG";
        public const string TOPLAM_KOLTUK_SAYISI = "TKS";
        public const string YOLCU_SAYISI = "YKS";
        public const string SURUCU_GOREVLI = "SUS";
        public const string SIGORTALI_CINSIYETI = "SCI";

        public const string TAHSILAT_TIPI = "TT1";
        public const string TRAFIK_GECMISI_VAR_MI = "TRV";
        public const string TOPLAM_POLICE_ADEDI = "TPA";
        public const string MOTOR_GUCU = "MTR";
        public const string HIZLI_TEKLIF_INDIRIM_GRUPLARI = "HZ1";
        public const string RELATIVITY_SURP = "RLS";
        public const string OZEL_INDIRIM = "İND";
        public const string ESNEKLIK_KATSAYISI = "ELS";
        public const string SCORE = "SCR";
        public const string OZEL_PLAKA = "OPL";
        public const string ESDEGER_VAR_MI = "ESG";
        public const string MESLEK_INDIRIMI = "MEİ";
        public const string YENILEME_NO = "YEI";
        public const string RELATIVITY_IND = "RLI";
        public const string AKSESUAR_TURU = "AKU";
        public const string OZEL_TUZEL_CARPANI = "OTC";
        public const string ILCE_CARPANI_KASKO = "ICM";
        public const string ACN_ILCE_DUZELTME = "YRL";
        public const string CHAMPION_CHALLENGER = "CHC";
        //public const string HASARLILIK_SURPRIMI = "KK5";
        public const string KASKO_MUAFIYETI = "KM1";
        public const string KEMIRGEN_HASARI = "KMR";
        public const string HASARSIZLIK_INDIRIM_CARPANI = "HZC";
        public const string TC_VERGI = "TVZ";
        public const string SADECE_ORJ_PARCA_MI = "ORJ";
        public const string RTSS_ON_OFF = "RTS";
        public const string KASKO_ARAC_DEG_CARPANI = "KAD";
        public const string HIZLI_TEKLIF_INDIRIM_ORANLARI = "HZ2";

        public const string TEKLIF_KAYIT = "HKY";
        public const string YENILEME_KONTROLU_CALISSIN_MI = "YKN";
        public const string KASKO_MUAFIYET_INDIRIMI_453 = "KM2";
        public const string CAPRAZ_SATIS_INDIRIMI = "CSI";
        public const string CINSIYET_KASKO = "CNN";
        public const string HASARSIZLIK_KOR_KLOZ_YENI_IS = "HS2";
        public const string HASARSIZLIK_IND_CRP_YENI_IS = "HC2";
        public const string MARKA_VE_ARACYASI_CARPANI = "MAY";
        public const string SURUCU_YASI_SURPRIMI = "SRS";
        public const string MODEL_IL_CARPANI = "MOI";
        public const string FREKANS_BILGI = "FR1";
        public const string TOPLAM_KARDES_SAYISI = "KDS";
        public const string MODEL_KISA_KASKO = "MDL";
        public const string ARAB_BEDELI_KASKO = "ARB";
        public const string MOTOR_HACMI_ARAC_YASI_CARPANI = "YMH";
        public const string AVIVA_PERSONEL_INDIRIM_LIMITI = "AVI";
        public const string GENISLETILMIS_CAM_YENI_IS = "GC2";
        public const string T_IPTALSIZ_PLC_ADEDI_YENI_IS = "TP2";
        public const string HUKUKSAL_KORUMA_LIMITI = "HKR";
        public const string IMM_HASARSIZLIK_IND_CARPANI = "IHC";

        public const string SEGMENT_KASKO = "SGM";
        public const string ONCEKI_SIRKET_YENI_IS = "OS2";
        public const string CINSIYET_MEDENI_HAL = "CMH";
        public const string SON_5_SENE_AIT_TOPLAM_HSR_AD = "TPH";
        public const string SON_5_YILA_AIT_T_KSR_HSR_ADEDI = "TKH";
        public const string MUSTERI_ACENTE_UYUMU_BILGI = "MAU";
        public const string MOTOR_HACMI_YAKIT_TIPI = "DBM";
        public const string HSZ_IND_VE_YENI_DEGER_CARPANI = "YEH";
        public const string SIGORTALI_YASI_CINSIYET = "CSG";
        public const string ARAC_BEDEL_ARALIK_YENI_IS = "AL2";
        public const string YENI_DEGER_YENIIS_ = "YD2";
        public const string CINSIYET_ARAC_YAS = "CAR";
        public const string EGM_YAPILDI_MI = "EGM";
        public const string HASARSIZLIK_KORUMA_CARPANI = "HZK";
        public const string YENILEME_GUN_FARKI_CARPANI = "YGG";
        public const string TARIFE = "TRR";
        public const string MUSTERI_ACN_IL_UYUM_YENI_IS = "MC2";
        public const string AKSESUAR_SON_DURUM_YENI_IS = "AS2";
        public const string YAKIT_TIPI_KASKO = "YKT";
        //  public const string ARAC_BEDELI = "ABE";
        public const string MESLEK_INDIRIMI_YENI_IS = "MS2";
        public const string MEDENI_HAL_SIGORTALI_YAS_BILGI = "MSY";
        public const string SIGORTALI_MEDENI_HAL_YENI_IS = "SM2";
        public const string YENILEME_CARPANI = "YEN";
        public const string SON_5_YILAN_AIT_T_IPTSZ_PLC_AD = "TPP";
        public const string SON_5_T_HASAR_IPTLSZ_PLC_AD = "FRK";
        public const string ONCEKI_BASAMAK_YENI_IS = "BK2";
        public const string IL_PLAKA_UYUMU_YENI_IS = "IP2";
        public const string IL_PLAKA_UYUMU = "IPU";
        public const string SEGMENT_CINSIYET = "CSE";
        public const string YAKIT_TIPI_YENI_IS = "YK2";
        public const string SERVIS_SARTI_YENI_IS = "YE2";
        public const string SON_5_T_HASAR_YENI_IS = "FR2";
        public const string HASARSIZLIK_INDIRIMI = "K07";
        public const string AVIVA_YENILEME_INDIRIMI = "YNL";
        public const string CINSIYET_YENI_IS_ = "CN2";
        public const string YAS_COCUK_SAYISI_YENI_IS_1828 = "CS2";
        public const string MEDENI_HAL = "MD2";
        public const string CINSIYET_SIGORTALI_YAS_YENI_IS = "CC2";
        public const string CINSIYET_ARAC_YAS_YENI_IS = "CA2";
        public const string BM_CARPANI = "BMF";
        public const string ILCE_CARPANI_YENI_IS = "IC2";
        public const string MOTOR_HACMI_KASKO = "MTO";
        public const string TOPLAM_COCUK_SAYISI = "TCO";
        public const string FREKANS_TOP_POL_ADEDI_BILGI = "FTP";
        public const string YAS_KARDES_SAY_YENI_IS1828 = "KD2";
        public const string YAS_KARDES_SAY1828 = "GKR";
        public const string ARAC_YASI_YENI_IS = "AY2";
        public const string SURUCU_YASI_YENI_IS = "SY2";
        public const string YAS_COCUK_SAYI1828 = "GCO";
        public const string CINSIYET_MEDENI_HAL_YENI_IS = "CY2";
        public const string CINSIYET_SIGORTA_YAS = "CIY";
        public const string SIGORTALI_YAS_ARA = "SGA";
        public const string PLAKA_CARPANI = "PND";
        public const string ESDEGER_PARCA_CARPANI = "EPC";
        public const string ARAC_YAS_SURPRIMI = "ACS";
        public const string HASAR_ADEDI_SURPRIMI = "HDS";
        public const string GENISLETILMIS_CAM = "GCS";


        public const string BASAMAK_KODU_CARPANI = "BKC";
        public const string SERVIS_ISTEGI = "ONR";
        public const string PARCA_TURU = "TDR";
        public const string ARAC_BEDEL_ARALIK_BILGI = "ADL";
        public const string FREKANS_TOP_POL_AD_YENI_IS = "FP2";
        public const string ARACYASI_CINSIYET = "CAY";
        // public const string MEDENI_HAL = "MDH";
        public const string MODEL_GRUP_KASKO = "MDD";
        public const string MARKA_MODEL_YENI_IS = "ML2";
    }

    public class UnicoUrunKodlari
    {
        public const string HususiKasko = "423";
        public const string TicariKasko = "421";
    }
    public class UnicoKimlikTipleri
    {
        public const string Sahis = "0";
        public const string Tuzel = "1";
    }
    public class UnicoTeklifTipleri
    {
        public const decimal Teklif = 0;
        public const decimal Police = 1;
    }
    public class UnicoOdemeTipleri
    {
        public const string Nakit = "N";
        public const string KrediKarti = "K";
        public const string BankaKarti = "B";
    }

    public class UnicoAksesuarTurleri
    {
        public const string Yok = "0";
        public const string ECihazDahil = "3";
        public const string ECihazHaric = "2";
        public const string LPG = "1";
    }
    public class DepSelMuafiyeti
    {
        public const string Muafiyetli510 = "3";
        public const string Muafiyetsiz = "2";
        public const string Muafiyetli525 = "1";
    }
    public class GenisletilmisCam
    {
        public const string Hayir = "3";
        public const string Evet = "2";
    }

    public class IkameLimitleri
    {
        public const string Yok = "0";
        public const string Standart15 = "3";//2X15 GÜN STANDART
        public const string UstSegment15 = "2";//2X15 GÜN ÜST SEGMENT
        public const string Standart7 = "1";//2X7 GÜN STANDART
    }

    public class KaskoMuafiyetLimitleri
    {
        public const string Yok = "2";
        public const string KM500 = "3";
        public const string KM1000 = "4";
        public const string KM1500 = "5";
        public const string OzelTL = "6";
    }
    public class KilitBedeli
    {
        public const string KB5000 = "7";
        public const string KB1500 = "5";
        public const string KB1000 = "4";
        public const string KB800 = "3";
        public const string KB3000 = "6";
    }
    public class ServisSartlari
    {
        public const string AnlasmaliOzelServis = "4";
        public const string AvantajServisAgi = "3";
        public const string Serbest = "1";
        public const string Tamirhane = "0";
        public const string AnlasmaliServis = "2";
    }
    public static class UNICOCommon
    {
        public static List<SelectListItem> KaskoMuafiyetList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "2", Text = "YOK" },
                new SelectListItem() { Value = "3", Text = "500" },
                new SelectListItem() { Value = "4", Text = "1,000" },
                new SelectListItem() { Value = "5", Text = "1,500" },
                new SelectListItem() { Value = "6", Text = "ÖZEL(TL)" }
            });

            return list;
        }
        public static List<SelectListItem> KilitBedelleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "7", Text = "5,000" },
                new SelectListItem() { Value = "5", Text = "1,500" },
                new SelectListItem() { Value = "4", Text = "1,000" },
                new SelectListItem() { Value = "3", Text = "800" },
                new SelectListItem() { Value = "6", Text = "3,000" }
            });

            return list;
        }
        public static List<SelectListItem> IkameSecenekleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "YOK" },
                new SelectListItem() { Value = "3", Text = "2X15 GÜN STANDART" },
                new SelectListItem() { Value = "2", Text = "2X15 GÜN ÜST SEGMENT" },
                new SelectListItem() { Value = "1", Text = "2X7 GÜN STANDART" }
            });

            return list;
        }
        public static List<SelectListItem> DepremSelMuafiyetleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "3", Text = "DEPREM%5 SEL%10 MUAFİYETLİ" },
                new SelectListItem() { Value = "2", Text = "DEPREM SEL MUAFİYETSİZ" },
                new SelectListItem() { Value = "1", Text = "DEPREM%5 SEL%25 MUAFİYETLİ" }
            });

            return list;
        }

        public static List<SelectListItem> AksesuarTurleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "3", Text = "DİĞER (ELEKTRONİK CİHAZLAR DAHİL)" },
                new SelectListItem() { Value = "2", Text = "DİĞER (ELEKTRONİK CİHAZLAR HARİÇ)" },
                new SelectListItem() { Value = "1", Text = " LPG" }
            });

            return list;
        }
        public static List<SelectListItem> UnicoMeslekList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "YOK" },
                new SelectListItem() { Value = "1", Text = "KAMU PERSONELİ" },
                new SelectListItem() { Value = "4", Text = "ÖĞRETMEN / ÖĞRT.GÖRV." },
                new SelectListItem() { Value = "11", Text = "TOYOTA/BOSHOKU PERSONELİ" },
                new SelectListItem() { Value = "12", Text = "TÜRK EĞİTİM-SEN MENSUBU" },
                new SelectListItem() { Value = "16", Text = "GRUP MARMARA ÖĞR./ÖĞRT. GÖRV." },
                new SelectListItem() { Value = "17", Text = "İLKSAN ÜYELERİNE ÖZEL İND." },
            });

            return list;
        }
    }
  
  
}
