using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class HazineYururlulukResponse : MapfreSorguResponse
    {
        public string hata  { get; set; }
        public string mesajtipi { get; set; }
        public string mesaj_gecmis { get; set; }
        public string mesaj_yururluk { get; set; }
    }
}
