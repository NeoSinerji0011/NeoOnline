using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class KimlikSorguResponse : MapfreSorguResponse
    {
        public string vrgAd { get; set; }
        public string vrgSoyAd { get; set; }
        public string vrgBabaAdi { get; set; }
        public string vrgAnaAdi { get; set; }
        public string vrgDogumTarihi { get; set; }
        public string vrgDogumYeri { get; set; }
        public string vrgTCKimlikNo { get; set; }
        public string vrgVergiNo { get; set; }
        public string vrgnvan { get; set; }
        public string vrgCinsiyet { get; set; }
        public string hata { get; set; }

        public DateTime DogumTarihiAsDate()
        {
            if(!String.IsNullOrEmpty(vrgDogumTarihi) && vrgDogumTarihi.Length == 10)
            {
                string[] parts = vrgDogumTarihi.Split('-');
                if (parts.Length == 3)
                {
                    int yil = 1;
                    int ay = 1;
                    int gun = 1;

                    int.TryParse(parts[0], out yil);
                    int.TryParse(parts[1], out ay);
                    int.TryParse(parts[2], out gun);

                    return new DateTime(yil, ay, gun);
                }
            }

            return DateTime.MinValue;
        }
    }
}
