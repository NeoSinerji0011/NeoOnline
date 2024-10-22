using Neosinerji.BABOnlineTP.Business.ANADOLU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI : ANADOLUMessage
    {
        public PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
        }
        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI";
        }
        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("MENFAAT_TIPI", this.MENFAAT_TIPI));
            sb.Append(MsgParameter("KURUM_TIPI", this.KURUM_TIPI));
            sb.Append(MsgParameter("MENFAAT_TURU", this.MENFAAT_TURU));
            sb.Append(MsgParameter("SUBE_KODU", this.SUBE_KODU));
            sb.Append(MsgParameter("SESSION_ID", this.SESSION_ID));
            sb.Append(MsgParameter("SUBE_ILI", this.SUBE_ILI));
            sb.Append(MsgParameter("KURUM_KODU", this.KURUM_KODU));
            sb.Append(MsgParameter("BANKA_KODU", this.BANKA_KODU));
           
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET>");
            return sb.ToString();
        }
        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string MENFAAT_TIPI { get; set; }
        public string KURUM_TIPI { get; set; }
        public string MENFAAT_TURU { get; set; }
        public string SUBE_KODU { get; set; }
        public string SESSION_ID { get; set; }
        public string SUBE_ILI { get; set; }
        public string KURUM_KODU { get; set; }
        public string BANKA_KODU { get; set; }
    }
}
