using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class UrunSorguResponse : MapfreSorguResponse
    {
        public string hata { get; set; }
        public string onarimYeri { get; set; }
        public string urunkodu { get; set; }
        public string modelYili { get; set; }
        public string yerAdedi { get; set; }
    }
}
