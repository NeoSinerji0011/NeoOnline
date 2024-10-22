using Neosinerji.BABOnlineTP.Business.ANADOLU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.MODEL_YILI = String.Empty;
            this.TRAMER_TARIFE_KODU = String.Empty;
            this.MARKA_KODU = String.Empty;
            this.KULLANIM_TIPI_KODU = string.Empty;
            this.GRUP_URUN_ADI = string.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("MODEL_YILI", this.MODEL_YILI));
            sb.Append(MsgParameter("TRAMER_TARIFE_KODU", this.TRAMER_TARIFE_KODU));
            sb.Append(MsgParameter("TRAMER_SORGU_OK", String.Empty));
            sb.Append(MsgParameter("MARKA_KODU", this.MARKA_KODU));
            sb.Append(MsgParameter("KULLANIM_TIPI_KODU", this.KULLANIM_TIPI_KODU));
            sb.Append(MsgParameter("GRUP_URUN_ADI", this.GRUP_URUN_ADI));
            sb.Append(MsgParameter("SESSION_ID", String.Empty));
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string TRAMER_TARIFE_KODU { get; set; }
        public string MODEL_YILI { get; set; }
        public string MARKA_KODU { get; set; }
        public string KULLANIM_TIPI_KODU { get; set; }
        public string GRUP_URUN_ADI { get; set; }

    }
}
