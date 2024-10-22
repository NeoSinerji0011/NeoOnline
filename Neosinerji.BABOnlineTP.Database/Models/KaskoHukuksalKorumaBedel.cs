using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class KaskoHukuksalKorumaBedel
    {
        public int Id { get; set; }
        public Decimal Bedel { get; set; }
        public string Text { get; set; }        
    }
}
