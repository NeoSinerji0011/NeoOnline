using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifNot
    {
        public int TeklifId { get; set; }
        public string Aciklama { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
}
