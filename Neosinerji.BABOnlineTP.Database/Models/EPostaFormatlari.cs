using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class EPostaFormatlari
    {
        public int FormatId { get; set; }
        public string FormatAdi { get; set; }
        public string Konu { get; set; }
        public string Icerik { get; set; }
    }
}
