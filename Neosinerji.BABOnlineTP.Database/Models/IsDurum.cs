using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsDurum
    {
        public IsDurum()
        {
            this.IsDurumDetays = new List<IsDurumDetay>();
        }

        public int IsId { get; set; }
        public string Guid { get; set; }
        public byte IsTipi { get; set; }
        public int ReferansId { get; set; }
        public byte IsSayi { get; set; }
        public byte Tamamlanan { get; set; }
        public byte Durumu { get; set; }
        public Nullable<System.DateTime> Baslangic { get; set; }
        public Nullable<System.DateTime> Bitis { get; set; }
        public virtual ICollection<IsDurumDetay> IsDurumDetays { get; set; }
    }
}
