using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class EgmSorguAracTescilBilgileri
    {
        public string kullanimSekli { get; set; }
        public string plakaIlKodu { get; set; }
        public string plakaNo { get; set; }
        public string tescilSeriNo { get; set; }
        public MapfreTarih tescilTarihi { get; set; }
        public string trafiktenCekildi { get; set; }
    }
}
