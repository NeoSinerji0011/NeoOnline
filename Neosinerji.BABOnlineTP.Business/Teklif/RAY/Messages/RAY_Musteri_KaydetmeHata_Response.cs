using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("ERROR", Namespace = "", IsNullable = false)]
    public class RAY_Musteri_KaydetmeHata_Response
    {
        [XmlElement]
        public string ERROR_DESC { get; set; }
        [XmlElement]
        public string CORRELATION_ID { get; set; }

      
    }
}
