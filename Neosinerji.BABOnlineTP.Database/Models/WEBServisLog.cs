using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class WEBServisLog
    {
        public int LogId { get; set; }
        public Nullable<int> TeklifId { get; set; }
        public byte IstekTipi { get; set; }
        public System.DateTime IstekTarihi { get; set; }
        public System.DateTime CevapTarihi { get; set; }
        public byte BasariliBasarisiz { get; set; }
        public string IstekUrl { get; set; }
        public string CevapUrl { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
}
