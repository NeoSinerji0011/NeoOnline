using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AnaMenu
    {
        public int AnaMenuKodu { get; set; }
        public string Aciklama { get; set; }
        public string YardimAciklama { get; set; }
        public short SiraNumarasi { get; set; }
        public int IslemKodu { get; set; }
        public byte Durum { get; set; }
        public byte UrunYetki { get; set; }
    }
}
