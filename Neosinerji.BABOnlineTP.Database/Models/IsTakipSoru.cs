using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsTakipSoru
    {
        public int IsTakipSoruId { get; set; }
        public int IsTakipDetayId { get; set; }
        public int TeklifId { get; set; }
        public int SoruKodu { get; set; }
        public int CevapTipi { get; set; }
        public string Cevap { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public virtual IsTakipDetay IsTakipDetay { get; set; }
    }
}
