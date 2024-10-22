using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarNotlari
    {
        public int NotId { get; set; }
        public int HasarId { get; set; }
        public string NotKonu { get; set; }
        public string NotAciklama { get; set; }
        public System.DateTime NotKayitTarihi { get; set; }
        public virtual HasarGenelBilgiler HasarGenelBilgiler { get; set; }
    }
}
