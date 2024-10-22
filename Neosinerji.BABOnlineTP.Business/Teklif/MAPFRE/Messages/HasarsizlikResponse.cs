using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class HasarsizlikResponse : MapfreSorguResponse
    {
        public string hata { get; set; }
        public string hasarsizlik_ind { get; set; }
        public string hasar_srp { get; set; }
        public string uygulanacak_kademe { get; set; }
    }
}
