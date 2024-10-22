
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Musteri.Kaydet
{
    [XmlRootAttribute("RESPONSE", Namespace = "", IsNullable = false)]
    public class GULF_Musteri_KaydetmeHata_Response
    {
        public string OPERATION_ID { get; set; }
        public string RESULT { get; set; }
        public string ERROR { get; set; }
        public string DATA { get; set; }
    }
}
