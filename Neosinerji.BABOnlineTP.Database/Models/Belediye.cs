using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Belediye
    {
        public int IlKodu { get; set; }
        public int BelediyeKodu { get; set; }
        public string BelediyeAdi { get; set; }
        public virtual BelediyeIl BelediyeIl { get; set; }
    }
}
