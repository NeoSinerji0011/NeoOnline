using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class UrunVergi
    {
        public int UrunKodu { get; set; }
        public int VergiKodu { get; set; }
        public int SiraNo { get; set; }
        public virtual Urun Urun { get; set; }
        public virtual Vergi Vergi { get; set; }
    }
}
