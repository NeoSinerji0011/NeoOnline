using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Message
{
    public class HDI_PoliceTransfer_Request
    {
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string TanzimBaslangicTarihi{ get; set; }
        public string TanzimBitisTarihi { get; set; }
    }
}
