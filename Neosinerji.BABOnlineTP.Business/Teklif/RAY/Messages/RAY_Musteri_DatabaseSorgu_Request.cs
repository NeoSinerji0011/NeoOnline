using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("REQUEST")]
    public class RAY_Musteri_DatabaseSorgu_Request
    {
        public string PROCESS_ID;
        public string USER_NAME;
        public string PASSWORD;
        public string CHANNEL;
        public string CITIZENSHIP_NUMBER;
        public string TAX_NUMBER;
        public string FOREIGN_CITIZENSHIP_NUMBER;     
    }
}
