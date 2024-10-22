using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class YZ_BelediyeNufus
    {
        public int Plaka { get; set; }
        public int IlceKodu { get; set; }
        public int BeldeKoyKodu { get; set; }
        public int BelediyeKodu { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string BelediyeAdi { get; set; }
        public string Nitelik { get; set; }
        public decimal ToplamNufus { get; set; }
        public decimal ErkekNufus { get; set; }
        public decimal KadinNufus { get; set; }
    }
}
