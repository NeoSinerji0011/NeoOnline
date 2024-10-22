using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DokumanTurleri
    {
        public int DokumanTurKodu { get; set; }
        public string DokumanTurAciklama { get; set; }
        public bool ZorunlulukTipi { get; set; }
    }
}
