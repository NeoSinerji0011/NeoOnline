using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public class TVMUrunYetkileriProcedureModel
    {
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public string UrunURL { get; set; }
        public string Aciklama { get; set; }
        public bool Active { get; set; }
    }
}
