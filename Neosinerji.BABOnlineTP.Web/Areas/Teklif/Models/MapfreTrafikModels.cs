using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class MapfreTrafikModel : TrafikModel
    {
        public new MapfreHazirlayanModel Hazirlayan { get; set; }
        public new MapfreOdemeModel KrediKarti { get; set; }
        public List<KeyValuePair<string,string>> AracGrupTarzListesi { get; set; }
    }

    public class MapfreDetayTrafikModel : DetayTrafikModel
    {
        public new MapfreOdemeModel KrediKarti { get; set; }
        public bool Satinalinabilir { get; set; }
    }
}