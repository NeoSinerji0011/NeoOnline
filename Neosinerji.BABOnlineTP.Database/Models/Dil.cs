using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Dil
    {
        public Dil()
        {
            this.DilAciklamas = new List<DilAciklama>();
        }

        public int Id { get; set; }
        public string DilAdi { get; set; }
        public string DilKodu { get; set; }
        public virtual ICollection<DilAciklama> DilAciklamas { get; set; }
    }
}
