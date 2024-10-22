using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Musteri
{
    [XmlRootAttribute("REQUEST")]
    public class GULF_Musteri_DatabaseSorgu_Request
    {
        [XmlElement("AUTH")]
        public AUTH AUTH { get; set; }

        [XmlElement("ENTITY")]
        public ENTITY ENTITY { get; set; }
    }
    //public class AUTH
    //{
    //    public string USER_NAME;
    //    public string PASSWORD;
    //    public string EXT_CLIENT_IP;
    //    public string CALLER_GUID;
    //}
    public class ENTITY
    {
        public string FIRM_CODE;
        public string ID_TYPE;
        public string ID_NUMBER;
    }
}
