using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsTakipDokuman
    {
        public int IsTakipDokumanId { get; set; }
        public int IsTakipId { get; set; }
        public int DokumanTipi { get; set; }
        public string DokumanURL { get; set; }
        public int KaydedenKullanici { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public virtual IsTakip IsTakip { get; set; }
    }
}
