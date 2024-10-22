using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CariOdemeTipleri
    {
        public int Kodu { get; set; }
        public string Aciklama { get; set; }
        public byte Durum { get; set; }
    }
}
