using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("INET_ERROR", Namespace = "", IsNullable = false)]
    public class RAY_TrafikPolicelestirmeHata_Response
    {
        [XmlElement]
        public string DESCRIPTION { get; set; }

        [XmlElement]
        public string CORRELATION_ID { get; set; }

        [XmlElement]
        public string ERROR_MESSAGE { get; set; }

        
    }
    
}
