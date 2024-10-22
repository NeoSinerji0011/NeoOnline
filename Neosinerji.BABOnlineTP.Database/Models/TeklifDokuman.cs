using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifDokuman
    {
        public int Id { get; set; }
        public int TeklifId { get; set; }
        public string DokumanAdi { get; set; }
        public byte DokumanTipi { get; set; }
        public string DokumanURL { get; set; }
        public DateTime KayitTarihi { get; set; }
        public virtual TeklifGenel TeklifGenel  { get; set; }

    }
}
