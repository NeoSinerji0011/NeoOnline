using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsTipleri
    {
        public int TipKodu { get; set; }

        public string TipAciklama { get; set; }

        public byte Durum { get; set; }
    }
}
