using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class OtorizasyonResponse
    {
        public bool Basarili { get; set; }
        public bool Hata { get; set; }
        public bool Otorizasyon { get; set; }
        public List<string> Hatalar { get; set; }
        public List<string> OtorizasyonMesajlari { get; set; }
        public int MusteriKodu { get; set; }
        public int TeklifId { get; set; }
    }
}
