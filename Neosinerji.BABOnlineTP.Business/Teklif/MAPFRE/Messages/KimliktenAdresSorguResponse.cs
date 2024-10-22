using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class KimliktenAdresSorguResponse : MapfreSorguResponse
    {
        public KisiAdresBilgisiType KisiAdresBilgisiType { get; set; }
        public KisiAdresBilgisiType KisiAcikAdresBilgisiType { get; set; }
        public string hata { get; set; }
    }
}
