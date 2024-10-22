using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class DuyuruOzetModel
    {
        public int DuyuruId { get; set; }
        public string Konu { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public List<DuyuruDownloadBilgi> Dosyalar { get; set; }
    }

    public class DuyuruDownloadBilgi
    {
        public string URL { get; set; }
        public string DosyaAdi { get; set; }
    }
}
