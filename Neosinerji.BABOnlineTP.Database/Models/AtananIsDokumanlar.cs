using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public class AtananIsDokumanlar
    {
        public int IsId { get; set; }
        public int SiraNo { get; set; }
        public string DokumanAdi { get; set; }
        public DateTime EklemeTarihi { get; set; }
        public int EkleyenPersonelKodu { get; set; }
        public string DokumanURL { get; set; }
    }
}
