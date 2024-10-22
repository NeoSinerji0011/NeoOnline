using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AltMenu
    {
        public int AnaMenuKodu { get; set; }
        public int AltMenuKodu { get; set; }
        public string Aciklama { get; set; }
        public string YardimAciklama { get; set; }
        public short SiraNumarasi { get; set; }
        public int IslemKodu { get; set; }
        public byte Durum { get; set; }
    }
}
