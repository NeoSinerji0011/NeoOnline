using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class YZ_IlceNufus
    {
        public int Plaka { get; set; }
        public int IlceKodu { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public decimal ToplamNufus { get; set; }
        public decimal ErkekNufus { get; set; }
        public decimal KadinNufus { get; set; }
        public int Yuzolcumu { get; set; }
        public int NufusYogunluk { get; set; }
        public decimal IlceEnlem { get; set; }
        public decimal IlceBoylam { get; set; }
        public decimal IlEnlem { get; set; }
        public decimal IlBoylam { get; set; }
        public decimal IleUzaklik { get; set; }
    }
}
