using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public class Underwriters
    {
        public int Id { get; set; }
        public Nullable<int> PoliceId { get; set; }
        public Nullable<int> TeklifId { get; set; }
        public string UnderwriterKodu { get; set; }//sigortasirket kodu
        public string UnderwriterAdi { get; set; }
        public Nullable<decimal> UnderwriterPayOrani { get; set; }
        public Nullable<decimal> UnderwriterPrim { get; set; }
        public Nullable<decimal> UnderwriterKomisyonOrani { get; set; }
        public Nullable<decimal> UnderwriterKomisyon { get; set; }

    }
}
