using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class UrunTeminat
    {
        public int UrunKodu { get; set; }
        public int TeminatKodu { get; set; }
        public int SiraNo { get; set; }
        public virtual Teminat Teminat { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
