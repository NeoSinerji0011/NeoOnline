using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class YZ_IlNufus
    {
        public int Plaka { get; set; }
        public string IlAdi { get; set; }
        public decimal ToplamNufus { get; set; }
        public decimal ErkekNufus { get; set; }
        public decimal KadinNufus { get; set; }
        public decimal Enlem { get; set; }
        public decimal Boylam { get; set; }
        public int Yuzolcum { get; set; }
        public int NufusYogunluk { get; set; }
    }
}
