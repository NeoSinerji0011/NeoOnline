using Neosinerji.BABOnlineTP.Business.ANADOLU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI_Response");

            this.SUBE_ADI = XmlValueForKey("SUBE_ADI");
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.VKN = XmlValueForKey("VKN");
            this.UNVAN = XmlValueForKey("UNVAN");
            this.SUBE_KODU = XmlValueForKey("SUBE_KODU");
            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.SUBE_ILI = XmlValueForKey("SUBE_ILI");
            this.KURUM_KODU = XmlValueForKey("KURUM_KODU");
            this.BANKA_KODU = XmlValueForKey("BANKA_KODU");

        }
        public string SUBE_ADI { get; set; }
        public string HATA_TEXT { get; set; }
        public string HATA_KODU { get; set; }
        public string VKN { get; set; }
        public string UNVAN { get; set; }
        public string SUBE_KODU { get; set; }
        public string SESSION_ID { get; set; }
        public string SUBE_ILI { get; set; }
        public string KURUM_KODU { get; set; }
        public string BANKA_KODU { get; set; }
    }
}
