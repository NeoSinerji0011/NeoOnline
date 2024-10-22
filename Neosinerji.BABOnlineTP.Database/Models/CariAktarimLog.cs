using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CariAktarimLog
    {
        public int Id { get; set; }
        public int? TvmKodu { get; set; }
        public string TvmUnvan { get; set; }
        public int? Donem { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string EkNo { get; set; }
        public string CariHesapKodu { get; set; }
        public string CariHesapUnvani { get; set; }
        public byte Basarili { get; set; }
        public string Mesaj { get; set; }
        public DateTime? KayitTarihi{ get; set; }
    }
}
