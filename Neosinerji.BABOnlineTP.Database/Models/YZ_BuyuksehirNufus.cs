using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class YZ_BuyuksehirNufus
    {
        public int Plaka { get; set; }
        public string Buyuksehir { get; set; }
        public decimal ToplamNufus { get; set; }
        public decimal ErkekNufus { get; set; }
        public decimal KadinNufus { get; set; }
    }
}
