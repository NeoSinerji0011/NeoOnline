using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TalepKanallari
    {
        public int Kodu { get; set; }
        public string Adi { get; set; }
        public byte Durum { get; set; }
    }
}
