using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [Serializable, XmlRoot("INFO")]
    public class RAY_Musteri_DatabaseSorguHata_Response
    {
        [XmlElement(ElementName = "INFO_DESC")]
        public string INFO_DESC { get; set; }

         [XmlElement(ElementName = "CORRELATION_ID")]
        public string CORRELATION_ID { get; set; }
    }

}