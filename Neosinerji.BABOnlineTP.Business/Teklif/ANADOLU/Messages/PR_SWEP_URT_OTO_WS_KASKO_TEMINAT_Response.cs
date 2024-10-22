using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_KASKO_TEMINAT_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_KASKO_TEMINAT_Response");

            this.IMM_TEMINAT_YAPISI = XmlValueForKey("IMM_TEMINAT_YAPISI");
            this.IMM_SECILI_KADEME = XmlValueForKey("IMM_SECILI_KADEME");
            this.HASARSIZLIK_KORUMA_SZLM_SECIM = XmlValueForKey("HASARSIZLIK_KORUMA_SZLM_SECIM");
            this.IKAME_ARAC_BILGILERI = XmlValueForKey("IKAME_ARAC_BILGILERI");
            this.KISISEL_ESYA = XmlValueForKey("KISISEL_ESYA");
            this.SIGARA_VE_BENZERI_MADDE_ZARARLARI = XmlValueForKey("SIGARA_VE_BENZERI_MADDE_ZARARLARI");
            this.YETKILI_OLMAYAN_KISILERCE_CEKILME = XmlValueForKey("YETKILI_OLMAYAN_KISILERCE_CEKILME");
            this.ANAHTARIN_ELE_GECIRILMESI_NEDENIYLE_CALINMA = XmlValueForKey("ANAHTARIN_ELE_GECIRILMESI_NEDENIYLE_CALINMA");
            this.KILIT_MEKANIZMASININ_DEGISTIRILMESI = XmlValueForKey("KILIT_MEKANIZMASININ_DEGISTIRILMESI");
            this.KIYMET_KAZANMA_TENZILI_ISTISNASI = XmlValueForKey("KIYMET_KAZANMA_TENZILI_ISTISNASI");
            this.SEL_SU_BASKINI = XmlValueForKey("SEL_SU_BASKINI");
            this.DEPREM = XmlValueForKey("DEPREM");
            this.GREV_L_HH_VE_TEROR = XmlValueForKey("GREV_L_HH_VE_TEROR");
            this.KULLANIM_GELIR_KAYBI = XmlValueForKey("KULLANIM_GELIR_KAYBI");
            this.KASKO_MUAFIYETI = XmlValueForKey("KASKO_MUAFIYETI");
            this.HUKUKSAL_KORUMA = XmlValueForKey("HUKUKSAL_KORUMA");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.HATA_MESAJI = XmlValueForKey("HATA_MESAJI");
            this.SESSION_ID = XmlValueForKey("SESSION_ID");
        }

        public string IMM_TEMINAT_YAPISI { get; set; }
        public string IMM_SECILI_KADEME { get; set; }
        public string HASARSIZLIK_KORUMA_SZLM_SECIM { get; set; }
        public string IKAME_ARAC_BILGILERI { get; set; }
        public string KISISEL_ESYA { get; set; }
        public string SIGARA_VE_BENZERI_MADDE_ZARARLARI { get; set; }
        public string YETKILI_OLMAYAN_KISILERCE_CEKILME { get; set; }
        public string ANAHTARIN_ELE_GECIRILMESI_NEDENIYLE_CALINMA { get; set; }
        public string KILIT_MEKANIZMASININ_DEGISTIRILMESI { get; set; }
        public string KIYMET_KAZANMA_TENZILI_ISTISNASI { get; set; }
        public string SEL_SU_BASKINI { get; set; }
        public string DEPREM { get; set; }
        public string GREV_L_HH_VE_TEROR { get; set; }
        public string KULLANIM_GELIR_KAYBI { get; set; }
        public string KASKO_MUAFIYETI { get; set; }
        public string HUKUKSAL_KORUMA { get; set; }
        public string HATA_KODU { get; set; }
        public string HATA_MESAJI { get; set; }
        public string SESSION_ID { get; set; }
    }
}
