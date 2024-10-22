using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI : ANADOLUMessage
    {
        public PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.IL = String.Empty;
            this.ILCE = String.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("IL", this.IL));
            sb.Append(MsgParameter("ILCE", this.ILCE));
            sb.Append("</cms:PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string IL { get; set; }
        public string ILCE { get; set; }
    }
}
