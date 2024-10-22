using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial  class ErgoIlIlce
    {
        public int Id { get; set; }
        public int IlKodu { get; set; }
        public string IlAdi { get; set; }
        public int IlceKodu { get; set; }
        public string IlceAdi { get; set; }
        public int TramerDistrictId { get; set; }
    }
}
