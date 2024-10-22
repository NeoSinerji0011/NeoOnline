using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.AEGON
{
    // TURUNCU ELMA REQUEST 
    public class AegonTERequest
    {
        public int TeklifNo { get; set; }
        public string cinsiyet { get; set; }
        public string dogTar { get; set; }
        public int yas { get; set; }
        public string sigBasTar { get; set; }
        public int sigortaSure { get; set; }
        public string odeDonem { get; set; }
        public string parabirimi { get; set; }
        public string anaTeminat { get; set; }
        public double anaTeminatTutar { get; set; }
        public string ekTeminatKodlari { get; set; }
        public string satisKanali { get; set; }
        public double pGVO { get; set; }
        public string HesaplamaSecenegi { get; set; }

        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
    }



    // PRIM IADELİ REQUEST - RESPONSE
    public class AegonPIRequest
    {
        public int TeklifNo { get; set; }
        public string cinsiyet { get; set; }
        public string dogTar { get; set; }
        public int yas { get; set; }
        public int gvo { get; set; }
        public string sigBasTar { get; set; }
        public int sigortaSure { get; set; }
        public string odeDonem { get; set; }
        public string parabirimi { get; set; }
        public int hesapYontem { get; set; }
        public double tutar { get; set; }
        public int suprimOran { get; set; }

        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
    }

    public class AegonPIResponse
    {
        public string HATA { get; set; }
        public string SURUM_BILGI { get; set; }

        public string B_ANA_TEM_ADI { get; set; }
        public string B_TEMINAT_TUTARI { get; set; }
        public string B_SS_PRIM_IADE_TUTAR { get; set; }
        public string B_YILLIK_PRIM { get; set; }
        public string B_DONEMLIK_PRIM { get; set; }
        public string D_POLICE_YILI { get; set; }
        public string D_KUMULATIF_YILLIK_PRIM { get; set; }
        public string D_MATEMATIK_KARSILIK { get; set; }
        public string D_ISTIRA_KESINTI_ORAN { get; set; }
        public string D_ISTIRA_KESINTI_TUTAR { get; set; }
        public string D_ISTIRA_BEDEL { get; set; }
        public string TIBBI_TETKIK_SONUCU { get; set; }

        public string PRIMDEN_VERGI_AVANTAJI { get; set; }
        public string VAS_PRIM_MALIYETI { get; set; }
        public string B_SS_TOPLAM_PRIM { get; set; }
    }



    // EGITIM REQUEST - RESSPONSE
    public class AegonEgitimRequest
    {
        public int TeklifNo { get; set; }
        public string cinsiyet { get; set; }
        public string dogTar { get; set; }
        public int yas { get; set; }
        public string parabirimi { get; set; }
        public string sigBasTar { get; set; }
        public int sigortaSure { get; set; }
        public int pGeriOdeSure { get; set; }
        public double pGOYillikTutar { get; set; }
        public string odeDonem { get; set; }
        public double pGVO { get; set; }
        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
    }

    public class AegonEgitimResponse
    {
        public string HATA { get; set; }
        public string SURUM_BILGI { get; set; }
        public string IB_AYLIK_PRIM { get; set; }
        public string IB_EGITIM_ONCESI_SURE { get; set; }
        public string IB_EGT_SURE_YILLIK_ODE_MAX { get; set; }
        public string IB_EGT_SURE_YILLIK_ODE_MIN { get; set; }
        public string IB_EGT_SURE_YILLIK_ODE_ORT { get; set; }
        public string IB_KAR_PAYI_DAG_ORAN { get; set; }
        public string IB_MAX_GETIRI_ORAN { get; set; }
        public string IB_MIN_GETIRI_ORAN { get; set; }
        public string IB_NET_MAX { get; set; }
        public string IB_NET_MIN { get; set; }
        public string IB_NET_ORT { get; set; }
        public string IB_ORT_GETIRI_ORAN { get; set; }
        public string IB_TOPLAM_PRIM { get; set; }
        public string IB_VEFAT_TEMINAT_TUTARI { get; set; }
        public string IB_VERGI_AVANTAJI_TUTARI { get; set; }
        public string IB_VER_AV_S_MALIYET { get; set; }
        public string IB_VO_MAX_GETIRI { get; set; }
        public string IB_VO_MIN_GETIRI { get; set; }
        public string IB_VO_ORT_GETIRI { get; set; }
        public string IB_YILLIK_ORT_RISK_PRM_KES { get; set; }
        public string IB_YILLIK_PRIM { get; set; }
        public string ID_GE_VER_ONC_BIRIKIM { get; set; }
        public string ID_GE_YIL_GER_ODE_IKR_TUT { get; set; }
        public string ID_KUM_YILLIK_PRIM { get; set; }
        public string ID_POLICE_YILI { get; set; }
        public string ID_V1_VER_ONC_BIRIKIM { get; set; }
        public string ID_V1_YIL_GER_ODE_IKR_TUT { get; set; }
        public string ID_V2_GER_ODE_IKR_TUT { get; set; }
        public string ID_V2_VER_ONC_BIRIKIM { get; set; }
        public string ID_YAS { get; set; }
        public string ID_YILLIK_RISK_KESINTI { get; set; }
        public string TIBBI_TETKIK_SONUCU { get; set; }
    }



    // ODULLU BIRIKIM REQUEST - RESSPONSE
    public class AegonOBRequest
    {
        public int TeklifNo { get; set; }
        public string cinsiyet { get; set; }
        public string dogTar { get; set; }
        public int yas { get; set; }
        public string sigBasTar { get; set; }
        public string odeDonem { get; set; }
        public int sigortaSure { get; set; }
        public int pHesapSecenek { get; set; }
        public double tutar { get; set; }
        public int pGVO { get; set; }
        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
    }

    public class AegonOBResponse
    {
        public string HATA { get; set; }
        public string SURUM_BILGI { get; set; }
        public string TIBBI_TETKIK_SONUCU { get; set; }
        public string IB_VEFAT_TEMINATI { get; set; }
        public string IB_PRIM_IADE_ODULU { get; set; }
        public string IB_SURE_SONU_PRIM_IADE_ORANI { get; set; }
        public string IB_ARA_DONEM_PRIM_IADE_ORANI { get; set; }
        public string IB_RISK_PRIM_KESINTISI { get; set; }
        public string IB_KAR_PAYI_DAG_ORANI { get; set; }
        public string IB_YILLIK_PRIM { get; set; }
        public string IB_TOPLAM_PRIM { get; set; }
        public string IB_GELIR_VERGISI_ORANI { get; set; }
        public string IB_VERGI_AVANTAJI { get; set; }
        public string IB_VER_SON_PRIM_MALIYET { get; set; }
        public string IB_GARANTI_EDILEN { get; set; }
        public string IB_GAR_EDILEN_ORAN { get; set; }
        public string IB_GAR_EDILEN_VERGI_ONCESI { get; set; }
        public string IB_GAR_EDILEN_NET { get; set; }
        public string IB_GAR_EDILEN_ODUL_DAHIL { get; set; }
        public string IB_VARSAYIM_1_METIN { get; set; }
        public string IB_VARSAYIM_1_ORAN { get; set; }
        public string IB_VARSAYIM_1_VERGI_ONCESI { get; set; }
        public string IB_VARSAYIM_1_NET { get; set; }
        public string IB_VARSAYIM_1_ODUL_DAHIL { get; set; }
        public string IB_VARSAYIM_2_METIN { get; set; }
        public string IB_VARSAYIM_2_ORAN { get; set; }
        public string IB_VARSAYIM_2_VERGI_ONCESI { get; set; }
        public string IB_VARSAYIM_2_NET { get; set; }
        public string IB_VARSAYIM_2_ODUL_DAHIL { get; set; }
        public string ID_YIL { get; set; }
        public string ID_SIGORTALI_YASI { get; set; }
        public string ID_KUMULATIF_PRIM { get; set; }
        public string ID_VARSAYIM_1_VO_MUH_BIRIKIM { get; set; }
        public string ID_VARSAYIM_1_NET_MUH_BIRIKIM { get; set; }
        public string ID_VARSAYIM_1_NET_ISTIRA_FESIH { get; set; }
        public string ID_VARSAYIM_2_VO_MUH_BIRIKIM { get; set; }
        public string ID_VARSAYIM_2_NET_MUH_BIRIKIM { get; set; }
        public string ID_VARSAYIM_2_NET_ISTIRA_FESIH { get; set; }
        public string ID_PRIM_IADELI_ODUL_TUTARI { get; set; }
    }



    // ODEME GÜVENCE REQUEST - RESSPONSE
    public class AegonOGRequest
    {
        public int TeklifNo { get; set; }
        public string cinsiyet { get; set; }
        public string dogTar { get; set; }
        public string ogsBastar { get; set; }
        public string odeDonem { get; set; }
        public string paraBirimi { get; set; }
        public string kapBastar { get; set; }
        public int sigSure { get; set; }
        public double kapAyPrim { get; set; }
        public int gvo { get; set; }

        public string KHPM_varmi { get; set; }
        public string MDPM_varmi { get; set; }
        public string IDPM_varmi { get; set; }
        public string KHYPM_varmi { get; set; }
        public int surprim_ana { get; set; }
        public int surprim_khpm { get; set; }
        public int surprim_mdpm { get; set; }
        public int surprim_idpm { get; set; }
        public int surprim_khypm { get; set; }

        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
        public int pKapTeklifNo { get; set; }
    }

    public class AegonOGResponse
    {
        public string HATA { get; set; }
        public string SURUM_BILGI { get; set; }
        public string TIBBI_TETKIK_SONUCU { get; set; }
        public string UYARI { get; set; }
        public string KUME_ID { get; set; }
        public string OGS_SURE { get; set; }
        public string OGS_SURE_AY { get; set; }
        public string VFPM_TEM_TUTAR { get; set; }
        public string KHPM_TEM_TUTAR { get; set; }
        public string MDPM_TEM_TUTAR { get; set; }
        public string IDPM_TEM_TUTAR { get; set; }
        public string KHYPM_TEM_TUTAR { get; set; }
        public string POLICE_DONEMI { get; set; }
        public string VEF_TEM { get; set; }
        public string KH_TEM_DPMT { get; set; }
        public string TDM_TEM_DPMT { get; set; }
        public string I_TEM_DPMT { get; set; }
        public string KHYT_TEM_DPMT { get; set; }
        public string PRIM { get; set; }
        public string AYLIK_ESDEGER_PRIM { get; set; }

        public string SS_TOPLAM_PRIM { get; set; }
        public string VAS_PRIM_MALIYETI { get; set; }
        public string PRIMDEN_VERGI_AVANTAJI { get; set; }

        public string YILDNM_KADAR_TOP_PRIM { get; set; }
        public string YILDNM_SONRA_TOP_PRIM { get; set; }
        public string YILDNM_KADAR_VER_AVANTAJI { get; set; }
        public string YILDNM_SONRA_VER_AVANTAJI { get; set; }
    }



    //KORUNAN GELECEK REQUEST-RESSPONSE
    public class AegonKGRequest
    {
        public int pTeklifNo { get; set; }
        public string cinsiyet { get; set; }
        public string dogTar { get; set; }
        public int yas { get; set; }
        public string sigBasTar { get; set; }
        public int sigortaSure { get; set; }
        public int gvo { get; set; }
        public int primOdemeSuresi { get; set; }
        public string odeDonem { get; set; }
        public string parabirimi { get; set; }
        public string anaTeminat { get; set; }
        public double anaTeminatTutar { get; set; }
        public string ekTeminat { get; set; }

        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
    }

    public class AegonKGResponse
    {
        public string HATA { get; set; }
        public string SURUM_BILGI { get; set; }
        public string TIBBI_TETKIK_SONUCU { get; set; }
        public string YIL { get; set; }
        public string YAS { get; set; }
        public string PRIM_ODEME_SURESI { get; set; }
        public string AT_SIGORTA_TUTARI { get; set; }
        public string TOPLAM_YILLIK_PRIM { get; set; }
        public string DONEM_PRIMI { get; set; }
        public string SURE_SONU_TOP_PRIM { get; set; }
        public string MALYD_TEM_TUTAR { get; set; }
        public string PRIMDEN_VERGI_AVANTAJI { get; set; }
        public string VAS_PRIM_MALIYETI { get; set; }
    }

    public class AegonKGTable
    {
        public string SOL_YIL { get; set; }
        public string SOL_YAS { get; set; }
        public string SOL_AT_SIGORTA_TUTARI { get; set; }
        public string SOL_TOPLAM_YILLIK_PRIM { get; set; }
        public string SOL_DONEM_PRIMI { get; set; }

        public string SAG_YIL { get; set; }
        public string SAG_YAS { get; set; }
        public string SAG_AT_SIGORTA_TUTARI { get; set; }
        public string SAG_TOPLAM_YILLIK_PRIM { get; set; }
        public string SAG_DONEM_PRIMI { get; set; }
    }



    //Prim İadeli2 REQUEST-RESSPONSE
    public class AegonPI2Request
    {
        public int pTeklifNo { get; set; }
        public string cinsiyet { get; set; }
        public string dogTar { get; set; }
        public int yas { get; set; }
        public string sigBasTar { get; set; }
        public int sigortaSure { get; set; }
        public string odeDonem { get; set; }
        public string parabirimi { get; set; }
        public int hesapYontem { get; set; }
        public int gvo { get; set; }
        public double tutar { get; set; }
        public int surprimOran { get; set; }

        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
    }

    public class AegonPI2Response
    {
        public string HATA { get; set; }
        public string SURUM_BILGI { get; set; }
        public string B_ANA_TEM_ADI { get; set; }
        public string B_TEMINAT_TUTARI { get; set; }
        public string B_SS_PRIM_IADE_TUTAR { get; set; }
        public string B_YILLIK_PRIM { get; set; }
        public string B_DONEMLIK_PRIM { get; set; }
        public string D_POLICE_YILI { get; set; }
        public string D_KUMULATIF_YILLIK_PRIM { get; set; }
        public string D_MATEMATIK_KARSILIK { get; set; }
        public string D_ISTIRA_KESINTI_ORAN { get; set; }
        public string D_ISTIRA_KESINTI_TUTAR { get; set; }
        public string D_ISTIRA_BEDEL { get; set; }
        public string TIBBI_TETKIK_SONUCU { get; set; }

        public string PRIMDEN_VERGI_AVANTAJI { get; set; }
        public string VAS_PRIM_MALIYETI { get; set; }
        public string B_SS_TOPLAM_PRIM { get; set; }
    }


    //Ön provizyon REQUEST-RESSPONSE
    public class AegonOnProvizyonRequest
    {
        public int gercekTeklifId { get; set; }

        public string pUrunHaymerKodux { get; set; }
        public int pTeklifNox { get; set; }
        public string pPartajNox { get; set; }
        public string pBasvuruNox { get; set; }
        public int pOdemeTurux { get; set; }
        public string pDovKodx { get; set; }
        public string pTCKx { get; set; }
        public string pKKNox { get; set; }
        public string pSKT_Ayx { get; set; }
        public string pSKT_Yilx { get; set; }
        public string pCVVx { get; set; }
        public string pTutarx { get; set; }
        public decimal tutarDEC { get; set; }
        public string pParaBirimix { get; set; }
        public string pProvTarx { get; set; }

        public string teklifTarihi { get; set; }
        public string musteriAdSoyad { get; set; }
    }

    public class AegonOnProvizyonResponse
    {
        public string errormsg { get; set; }
        public string response { get; set; }
        public string resultCode { get; set; }
    }
}
