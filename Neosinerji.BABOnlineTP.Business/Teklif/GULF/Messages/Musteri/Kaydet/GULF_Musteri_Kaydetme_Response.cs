using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Musteri
{
    [XmlRootAttribute("RESPONSE", Namespace = "", IsNullable = false)]
    public class GULF_Musteri_Kaydetme_Response
    {
       public string OPERATION_ID { get; set; }
       public string RESULT { get; set; }
       public string ERROR { get; set; }
       public DATA DATA { get; set; }

    }
    public class DATA {
        public ENTITYY ENTITY { get; set; }      

    }
    public class ENTITYY
    {
        public string FIRM_CODE { get; set; }
        public string UNIT_TYPE { get; set; }
        public string UNIT_NO { get; set; }
    }
}
