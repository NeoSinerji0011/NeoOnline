using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarAnlasmaliServisler
    {
        public int Kodu { get; set; }
        public string Unvani { get; set; }
        public Nullable<short> Durumu { get; set; }
    }
}
