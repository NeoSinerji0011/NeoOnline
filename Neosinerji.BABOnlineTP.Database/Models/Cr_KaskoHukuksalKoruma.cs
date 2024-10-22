using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Cr_KaskoHukuksalKoruma
    {
        public int Id { get; set; }
        public int TUMKodu { get; set; }
        public string DegerKodu { get; set; }
        public Decimal Bedel  { get; set; }

    }
}
