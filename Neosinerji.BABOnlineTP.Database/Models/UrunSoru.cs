using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class UrunSoru
    {
        public int UrunKodu { get; set; }
        public int SoruKodu { get; set; }
        public int SiraNo { get; set; }
        public virtual Soru Soru { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
