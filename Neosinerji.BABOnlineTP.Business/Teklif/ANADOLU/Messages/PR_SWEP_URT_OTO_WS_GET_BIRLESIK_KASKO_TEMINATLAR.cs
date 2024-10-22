using Neosinerji.BABOnlineTP.Business.ANADOLU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR()
        {

            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.GRUP_URUN_ADI = String.Empty; 
            this.CORE_CLIENT = "ERE";
            this.KULLANIM_TIPI_KODU = String.Empty;
            this.KULLANIM_SEKLI_KODU = String.Empty;
            this.ARAC_KODU = String.Empty;
            this.WEB_KULLANICI_ADI = String.Empty;
            this.WEB_SIFRE = String.Empty;
            this.SESSION_ID = String.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR>");
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("GRUP_URUN_ADI", this.GRUP_URUN_ADI));
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("KULLANIM_TIPI_KODU", this.KULLANIM_TIPI_KODU));
            sb.Append(MsgParameter("KULLANIM_SEKLI_KODU", this.KULLANIM_SEKLI_KODU));
            sb.Append(MsgParameter("ARAC_KODU", this.ARAC_KODU));
            sb.Append(MsgParameter("WEB_KULLANICI_ADI", this.WEB_KULLANICI_ADI));
            sb.Append(MsgParameter("WEB_SIFRE", this.WEB_SIFRE));
            sb.Append(MsgParameter("SESSION_ID", "test"));
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR>");
            return sb.ToString();
        }

        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string GRUP_URUN_ADI { get; set; }
        public string CORE_CLIENT { get; set; }
        public string KULLANIM_TIPI_KODU { get; set; }
        public string KULLANIM_SEKLI_KODU { get; set; }
        public string ARAC_KODU { get; set; }
        public string WEB_KULLANICI_ADI { get; set; }
        public string WEB_SIFRE { get; set; }
        public string SESSION_ID { get; set; }
    }
}
