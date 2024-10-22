using Neosinerji.BABOnlineTP.Business.ANADOLU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_GET_WEB_KULLANIM_SEKLI_LISTESI_Response");

            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.KULLANIM_SEKLI = XmlValueForKey("KULLANIM_SEKLI");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.HATA_MESAJI = XmlValueForKey("HATA_MESAJI");
        }

        public string SESSION_ID { get; set; }
        public string KULLANIM_SEKLI { get; set; }

        public string HATA_KODU { get; set; }
        public string HATA_MESAJI { get; set; }
    }
}
