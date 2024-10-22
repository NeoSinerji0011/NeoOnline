using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
   public partial class MuhasebeAktarimKonfigurasyon
    {
        public int Id { get; set; }
        public int SiraNo { get; set; }
        public int TvmKodu { get; set; }
        public string TvmUnvani { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string SirketKodu { get; set; }
        public Nullable<int> BransKodu { get; set; }        
        public byte AktarimTamamlandi { get; set; }        
        public Nullable<double> AktarimYuzdesi{ get; set; }        
    }
}
