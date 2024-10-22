using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class OncekiTescilResponse : MapfreSorguResponse
    {
        public string hata { get; set; }
        public string COD_ARAC_RUHSAT_SERI { get; set; }
        public string COD_ARAC_RUHSAT_SERI_NO { get; set; }
    }
}
