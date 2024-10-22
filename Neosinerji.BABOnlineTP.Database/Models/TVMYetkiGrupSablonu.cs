using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMYetkiGrupSablonu
    {
        public int YetkiSablonId { get; set; }
        public int YetkiGrupKodu { get; set; }
        public string YetkiGrupAdi { get; set; }
       // public int TVMKodu { get; set; }
        public int AnaMenuKodu { get; set; }
        public string AnaMenuAciklama { get; set; }
        public Nullable<int> AltMenuKodu { get; set; }
        public string AltMenuAciklama { get; set; }
        public Nullable<int> SekmeKodu { get; set; }
        public string SekmeAciklama { get; set; }
        public byte Gorme { get; set; }
        public byte YeniKayit { get; set; }
        public byte Degistirme { get; set; }
        public byte Silme { get; set; }
    }
}
