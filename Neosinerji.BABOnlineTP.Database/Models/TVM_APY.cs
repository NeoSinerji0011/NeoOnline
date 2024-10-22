using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVM_APY
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public string APYKodu { get; set; }
        public virtual TVMDetay TVMDetay { get; set; }
    }
}
