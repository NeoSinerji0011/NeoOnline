using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class OtorizasyonMesajResponse : MapfreSorguResponse
    {
        public string hata { get; set; }
        public string row { get; set; }
    }
}
