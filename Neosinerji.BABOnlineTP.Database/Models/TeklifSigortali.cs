using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifSigortali
    {
        public int SigortaEttirenId { get; set; }
        public int TeklifId { get; set; }
        public int SiraNo { get; set; }
        public int MusteriKodu { get; set; }
        public virtual MusteriGenelBilgiler MusteriGenelBilgiler { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
}
