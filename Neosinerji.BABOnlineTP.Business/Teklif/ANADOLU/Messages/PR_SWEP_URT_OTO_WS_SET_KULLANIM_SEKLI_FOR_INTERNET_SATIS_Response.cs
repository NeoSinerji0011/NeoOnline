using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_SET_KULLANIM_SEKLI_FOR_INTERNET_SATIS_Response");

            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.KULLANIM_SEKLI = XmlValueForKey("KULLANIM_SEKLI");
        }

        public string SESSION_ID { get; set; }
        public string KULLANIM_SEKLI { get; set; }
    }
}
