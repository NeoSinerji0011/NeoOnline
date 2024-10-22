using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class YZ_MahalleNufus
    {
        public int Plaka { get; set; }
        public int IlceKodu { get; set; }
        public int BeldeKoyKodu { get; set; }
        public int MahalleKodu { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string BeldeKoyAdi { get; set; }
        public string BelediyeAdi { get; set; }
        public string MahalleAdi { get; set; }
        public string Nitelik { get; set; }
        public decimal ToplamNufus { get; set; }
        public decimal ErkekNufus { get; set; }
        public decimal KadinNufus { get; set; }
        public string IlAdi2 { get; set; }
        public string IlceAdi2 { get; set; }
        public string MahalleAdi2 { get; set; }
        public decimal MahEnlem { get; set; }
        public decimal MahBoylam { get; set; }
        public decimal IlceEnlem { get; set; }
        public decimal IlceBoylam { get; set; }
        public decimal IlEnlem { get; set; }
        public decimal IlBoylam { get; set; }
        public decimal IleUzaklik { get; set; }
        public decimal IlceyeUzaklik { get; set; }

    }
}
