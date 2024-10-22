using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DilAciklama
    {
        public int DilId { get; set; }
        public string TabloAdi { get; set; }
        public int TabloId { get; set; }
        public int TabloId_2 { get; set; }
        public int AnaMenuKodu { get; set; }
        public string DilAciklama_1 { get; set; }
        public string DilAciklama_2 { get; set; }
        public virtual Dil Dil { get; set; }
    }
}
