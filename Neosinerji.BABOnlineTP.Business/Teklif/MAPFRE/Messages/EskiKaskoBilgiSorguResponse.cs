using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class EskiKaskoBilgiSorguResponse : MapfreSorguResponse
    {
        public string hata { get; set; }

        public EskiKaskoBilgiSorguPolice police { get; set; }
    }

    public class EskiKaskoBilgiSorguPolice
    {
        public string acenteNo { get; set; }
        public string policeNo { get; set; }
        public string sirketKodu { get; set; }
        public string yenilemeNo { get; set; }
        public string zeylTuru { get; set; }

        public string policeEkiNo { get; set; }
        public string sbmTramerNo { get; set; }

        public string dainiMurtehinKimlikNo { get; set; }


        public string oncekiAcenteNo { get; set; }
        public string oncekiPoliceNo { get; set; }
        public string oncekiSirketKodu { get; set; }
        public string oncekiYenilemeNo { get; set; }

        public string uygulananKademe { get; set; }

        public MapfreTarih policeBaslamaTarihi { get; set; }
        public MapfreTarih policeBitisTarihi { get; set; }
        public MapfreTarih policeEkiBaslamaTarihi { get; set; }
        public MapfreTarih tanzimTarihi { get; set; }

        public EskiKaskoBilgiArac arac { get; set; }
    }

    public class EskiKaskoBilgiArac
    {
        public string aracMarkaKodu { get; set; }
        public string aracTarifeGrupKodu { get; set; }
        public string aracTipKodu { get; set; }
        public string kullanimSekli { get; set; }
        public string modelYili { get; set; }
        public string motorNo { get; set; }
        public string plakaIlKodu { get; set; }
        public string plakaNo { get; set; }
        public string sasiNo { get; set; }
        public string yolcuKapasitesi { get; set; }
        public string yukKapasitesi { get; set; }
    }
}
