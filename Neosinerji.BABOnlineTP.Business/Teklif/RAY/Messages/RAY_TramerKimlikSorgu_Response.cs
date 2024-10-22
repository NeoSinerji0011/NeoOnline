using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("RESPONSE", Namespace = "", IsNullable = false)]
    public class RAY_TramerKimlikSorgu_Response
    {
        [XmlElement]
        public string TC_KIMLIK_NO { get; set; }

        [XmlElement]
        public string ADI { get; set; }

        [XmlElement]
        public string SOYADI { get; set; }

        [XmlElement]
        public string CINSIYETI { get; set; }

        [XmlElement]
        public string MEDENI_HALI { get; set; }

        [XmlElement]
        public string DOGUM_TARIHI { get; set; }

        [XmlElement]
        public string DOGUM_YERI { get; set; }

        [XmlElement]
        public string IL { get; set; }

        [XmlElement]
        public string ILCE { get; set; }

        [XmlElement]
        public string ANNE_ADI { get; set; }

        [XmlElement]
        public string BABA_ADI { get; set; }

        [XmlElement]
        public string CORRELATION_ID { get; set; }

        
    }
    [XmlRootAttribute("ERROR", Namespace = "", IsNullable = false)]
    public class HATA
    {
        [XmlElement]
        public string ERROR_DESC { get; set; }

    }
}
