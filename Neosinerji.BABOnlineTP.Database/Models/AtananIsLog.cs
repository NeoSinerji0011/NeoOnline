using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AtananIsLog
    {
        public int LogId { get; set; }        
        public string LogDosyasiAdi { get; set; }
        public string LogDosyasiURL { get; set; }
        public int TVMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public DateTime KayitTarihi { get; set; }
        
    }
}
