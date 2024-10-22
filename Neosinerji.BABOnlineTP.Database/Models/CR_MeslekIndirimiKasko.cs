using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_MeslekIndirimiKasko
    {
        public int TUMKodu { get; set; }
        public string MeslekKodu { get; set; }
        public string CR_MeslekKodu { get; set; }
        public string CR_Aciklama { get; set; }
    }
}
