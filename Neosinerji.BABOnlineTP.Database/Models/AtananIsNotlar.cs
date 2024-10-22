using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AtananIsNotlar
    {
        public int NotId { get; set; }
        public int IsId { get; set; }
        public string Konu { get; set; }
        public string NotAciklamasi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int TVMKodu { get; set; }
        public int TVMPersonelKodu { get; set; }
        public virtual AtananIsler AtananIsler { get; set; }
    }
}
