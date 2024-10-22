using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarAsistansFirmalari
    {
        public int AsistansKodu { get; set; }
        public string AsistansAdUnvan { get; set; }
        public string AsistansSoyadUnvan { get; set; }
        public short Durumu { get; set; }
    }
}
