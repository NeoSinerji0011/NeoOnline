using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarZorunluEvraklari
    {
        public int EvrakId { get; set; }
        public int HasarId { get; set; }
        public int EvrakKodu { get; set; }
        public System.DateTime EvrakKayitTarihi { get; set; }
        public string EvrakURL { get; set; }
        public virtual HasarGenelBilgiler HasarGenelBilgiler { get; set; }
    }
}
