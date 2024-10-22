using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.SOMPOJAPAN
{
    public class SompoDekontRequest
    {
        public string kullaniciAdi { get; set; }
        public string sifre { get; set; }
        public long sessionNo { get; set; }
        public long policeNo { get; set; }
    }
}
