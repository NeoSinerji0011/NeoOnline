using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET_Response");

            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.MUSTERI_NO = XmlValueForKey("MUSTERI_NO");
        }

        public string SESSION_ID { get; set; }
        public string HATA_KODU { get; set; }
        public string HATA_TEXT { get; set; }
        public string MUSTERI_NO { get; set; }
    }
}
