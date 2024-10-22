using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class KisiAdresBilgisiType
    {
        public string kimlikNo { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string ilKodu { get; set; }
        public string ilAd { get; set; }
        public string ilceKodu { get; set; }
        public string ilceAd { get; set; }
        public string veriKonum { get; set; }
        public string acikAdres { get; set; }
        public string adresNo { get; set; }
        public string binaPafta { get; set; }
        public string binaParsel { get; set; }
        public string csbm { get; set; }
        public string csbmKodu { get; set; }
        public string disKapiNo { get; set; }
        public string icKapiNo { get; set; }
        public string mahalle { get; set; }
        public string mahalleKodu { get; set; }
        public string mernisIlKodu { get; set; }
        public string mernisIlceKodu { get; set; }

        public MapfreTarih kpsdenSorgulanmaTarihi { get; set; }
        public MapfreTarih beyanTarihi { get; set; }
        public MapfreTarih tasinmaTarihi { get; set; }
        public MapfreTarih tescilTarihi { get; set; }
        public MapfreAdresTipi adresTipi { get; set; }
    }

    public class MapfreAdresTipi
    {
        public string __value__ { get; set; }
    }
}
