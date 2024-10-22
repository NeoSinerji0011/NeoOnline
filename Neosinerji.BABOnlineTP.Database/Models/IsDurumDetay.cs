using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsDurumDetay
    {
        public int IsDetayId { get; set; }
        public int IsId { get; set; }
        public int TUMKodu { get; set; }
        public int OdemePlaniAlternatifKodu { get; set; }
        public byte Durumu { get; set; }
        public int ReferansId { get; set; }
        public string HataMesaji { get; set; }
        public string BilgiMesaji { get; set; }
        public Nullable<System.DateTime> Baslangic { get; set; }
        public Nullable<System.DateTime> Bitis { get; set; }
        public virtual IsDurum IsDurum { get; set; }
    }
}
