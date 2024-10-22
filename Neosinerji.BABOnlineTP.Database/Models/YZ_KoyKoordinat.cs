using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class YZ_KoyKoordinat
    {
        public string KoyAdi { get; set; }
        public string MahalleAdi { get; set; }
        public string IlceAdi { get; set; }
        public string IlAdi { get; set; }
        public string Tip { get; set; }
        public decimal Enlem { get; set; }
        public decimal Boylam { get; set; }
    }
}
