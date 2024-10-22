using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class TrafikHasarBilgi
    {
        public string sirketKodu { get; set; }
        public string acenteKod { get; set; }
        public string policeNo { get; set; }
        public string yenilemeNo { get; set; }
        public string polBaslangicTarihi { get; set; }
        public string polBitisTarihi { get; set; }
        public string plakaNo { get; set; }
        public string sasiNo { get; set; }
        public string motorNo { get; set; }
        public string tahakkukIptal { get; set; }
        public string hasarAdet { get; set; }
        public string policeKey { get; set; }
        public string policeTip { get; set; }
        public string modelYili { get; set; }
        public string adUnvan { get; set; }
        public string soyad { get; set; }
        public string tipDocum { get; set; }
        public string codDocum { get; set; }
        public string marka { get; set; }
        public string model { get; set; }
        public string tescilTarihi { get; set; }
        public string trafigeCikisTarihi { get; set; }
        public string sbmTramerNo { get; set; }
        public string aracTarifeGrupKod { get; set; }
        public string yolcuKapasitesi { get; set; }
        public string motorGucu { get; set; }
        public string silindirHacmi { get; set; }
        public byte imalatYeri { get; set; }
        public string belgeNo { get; set; }
        public string belgeTarihi { get; set; }
        public string renk { get; set; }
        public string uygulanmasiGerekenBasamakKodu { get; set; }
        public string uygulanmisBasamakKodu { get; set; }

        [XmlIgnore]
        public DateTime PoliceBitisTarihi
        {
            get
            {
                return MapfreSorguResponse.ToDateTime(polBitisTarihi);
            }
        }
    }
}
