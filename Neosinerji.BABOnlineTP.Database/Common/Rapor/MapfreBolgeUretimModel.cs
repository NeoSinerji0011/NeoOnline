using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public class MapfreBolgeUretimModel
    {
        public int? BolgeKodu { get; set; }
        public string BolgeAdi { get; set; }
        public int? TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int KaskoTeklif { get; set; }
        public int KaskoPolice { get; set; }
        public decimal KaskoBrutPrim { get; set; }
        public decimal KaskoKomisyon { get; set; }
        public decimal KaskoPoliceOran { get; set; }
        public int TrafikTeklif { get; set; }
        public int TrafikPolice { get; set; }
        public decimal TrafikBrutPrim { get; set; }
        public decimal TrafikKomisyon { get; set; }
        public decimal TrafikPoliceOran { get; set; }
    }
}
