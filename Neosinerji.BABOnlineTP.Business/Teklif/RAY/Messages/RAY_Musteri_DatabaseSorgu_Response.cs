using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Neosinerji.BABOnlineTP.Business.RAY.Message
{
    [XmlRootAttribute("RESPONSE", Namespace = "", IsNullable = false)]
    public class RAY_Musteri_DatabaseSorgu_Response
    {
        public UNSMAS UNSMAS { get; set; }

        [XmlElement]
        public string CORRELATION_ID { get; set; }

    }

    public class UNSMAS 
    {
        [XmlElement]
        public string UNIT_NO { get; set; }
        [XmlElement]
        public string DESIGN_NO { get; set; }
        [XmlElement]
        public string PERSONAL_COMMERCIAL { get; set; }
        [XmlElement]
        public string CHANNEL_NO { get; set; }
        [XmlElement]
        public string NATIONALITY { get; set; }
        [XmlElement]
        public string CITIZENSHIP_NUMBER { get; set; }
        [XmlElement]
        public string GSM_COUNTRY_CODE1 { get; set; }
        [XmlElement]
        public string GSM_CODE1 { get; set; }
        [XmlElement]
        public string GSM_NUMBER1 { get; set; }
        [XmlElement]
        public string NAME { get; set; }
        [XmlElement]
        public string SURNAME { get; set; }
        [XmlElement]
        public string FIRM_NAME { get; set; }
        [XmlElement]
        public string FATHER_NAME { get; set; }
        [XmlElement]
        public string GENDER { get; set; }
        [XmlElement]
        public string BIRTH_DATE { get; set; }
        [XmlElement]
        public string BIRTH_PLACE { get; set; }
        [XmlElement]
        public string ADDRESS1 { get; set; }
        [XmlElement]
        public string EMAIL_ADDRESS1 { get; set; }
        [XmlElement]
        public string MARITAL_STATUS { get; set; }
        [XmlElement]
        public string OCCUPATION { get; set; }
        [XmlElement]
        public string EDUC_STAT { get; set; }
        [XmlElement]
        public string WORK_STATUS { get; set; }


        public List<UNSADR> UNSADR = new List<UNSADR>();
    }

    public class UNSADR
    {
        [XmlElement]
        public string ADR_TYPE { get; set; }

        [XmlElement]
        public string ADR_DATA { get; set; }

        [XmlElement]
        public string WHICH_ADRESS { get; set; }

    }
}
