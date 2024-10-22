using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
   
    public partial class LilyumKartTeminatlar
    {
        public LilyumKartTeminatlar()
        {

        }

        public int TeminatId { get; set; }
        public string GrupAdi { get; set; }
        public string TeminatAdi { get; set; }
        public Nullable<decimal> Limit { get; set; }
        public string Aciklama { get; set; }
        public byte KullanimHakki { get; set; }
        
            
    }
}
