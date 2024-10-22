using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class EgmSorguResponse : MapfreSorguResponse
    {
        public EgmSorguAracBilgi aracBilgi { get; set; }
        public EgmSorguAracSahipBilgileri aracSahipBilgileri { get; set; }
        public EgmSorguAracTescilBilgileri aracTescilBilgileri { get; set; }
        public MapfreTarih expireDate { get; set; }
        public MapfreTarih fesihTarihi { get; set; }
        public string sakincaDurumu { get; set; }
        public string sorguYeri { get; set; }
        public string hata { get; set; }
    }
}
