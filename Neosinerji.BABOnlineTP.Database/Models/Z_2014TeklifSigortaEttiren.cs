using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_2014TeklifSigortaEttiren
    {
        public int TableSigortaEttirenId { get; set; }
        public int SigortaEttirenId { get; set; }
        public int TeklifId { get; set; }
        public int SiraNo { get; set; }
        public int MusteriKodu { get; set; }
    }
}
