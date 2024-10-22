using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif
{
    [XmlRoot(ElementName = "RESPONSE")]
    public class GULF_KaskoTeklifKayitHata_Response
    {
            public string OPERATION_ID { get; set; }
            public string RESULT { get; set; }
            public string ERROR { get; set; }           
     
    }
}
