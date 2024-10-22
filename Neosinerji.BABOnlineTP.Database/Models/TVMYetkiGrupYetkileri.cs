using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMYetkiGrupYetkileri
    {
        public int YetkiId { get; set; }
        public int YetkiGrupKodu { get; set; }
        public int AnaMenuKodu { get; set; }
        public int AltMenuKodu { get; set; }
        public int SekmeKodu { get; set; }
        public byte Gorme { get; set; }
        public byte YeniKayit { get; set; }
        public byte Degistirme { get; set; }
        public byte Silme { get; set; }
        public virtual TVMYetkiGruplari TVMYetkiGruplari { get; set; }
    }
}
