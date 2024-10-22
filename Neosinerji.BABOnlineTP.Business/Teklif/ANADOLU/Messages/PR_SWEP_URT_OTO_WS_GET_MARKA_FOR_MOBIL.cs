using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.KANAL = "WEB";
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("KANAL", this.KANAL));

            sb.Append("</cms:PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL>");
            return sb.ToString();
        }

        public string CORE_DOMAIN { get; set; }
        public string CORE_CLIENT { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string KANAL { get; set; }
    }
}
