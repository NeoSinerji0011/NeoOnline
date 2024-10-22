using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class UnicoIlIlce
    {
        public int Id { get; set; }
        public string IlKodu { get; set; }
        public string IlAdi { get; set; }
        public string IlceKodu { get; set; }
        public string IlceAdi { get; set; }
    }
}
