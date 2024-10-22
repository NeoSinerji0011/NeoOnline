using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI_Response");

            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.BELDE = XmlValueForKey("BELDE");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.ILCE_KODU = XmlValueForKey("ILCE_KODU");
            this.IL_KODU = XmlValueForKey("IL_KODU");
            this.SESSION_ID = XmlValueForKey("SESSION_ID");
        }

        public string HATA_TEXT { get; set; }
        public string BELDE { get; set; }
        public string HATA_KODU { get; set; }
        public string ILCE_KODU { get; set; }
        public string IL_KODU { get; set; }
        public string SESSION_ID { get; set; }
    }
}
