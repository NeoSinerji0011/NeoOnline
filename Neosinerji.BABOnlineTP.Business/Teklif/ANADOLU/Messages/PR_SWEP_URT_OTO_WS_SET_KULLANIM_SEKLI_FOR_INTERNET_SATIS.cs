using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.ARAC_KODU = String.Empty;
            this.MODEL_YILI = String.Empty;
            this.TRAMER_TARIFE_KODU = String.Empty;
            this.KULLANIM_TIPI_KODU = String.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("ARAC_KODU", this.ARAC_KODU));
            sb.Append(MsgParameter("MODEL_YILI", this.MODEL_YILI));
            sb.Append(MsgParameter("TRAMER_TARIFE_KODU", this.TRAMER_TARIFE_KODU));
            sb.Append(MsgParameter("KULLANIM_TIPI_KODU", this.KULLANIM_TIPI_KODU));
            sb.Append(MsgParameter("SESSION_ID", String.Empty));
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string TRAMER_TARIFE_KODU { get; set; }
        public string ARAC_KODU { get; set; }
        public string MODEL_YILI { get; set; }
        public string KULLANIM_TIPI_KODU { get; set; }
    }
}
