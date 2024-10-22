using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifWebServisCevap
    {
        public int TeklifId { get; set; }
        public int CevapKodu { get; set; }
        public byte CevapTipi { get; set; }
        public string Cevap { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
}
