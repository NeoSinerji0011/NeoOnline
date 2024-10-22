using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracMarka
    {
        public AracMarka()
        {
            this.AracModels = new List<AracModel>();
            this.AracTips = new List<AracTip>();
        }

        public string MarkaKodu { get; set; }
        public string MarkaAdi { get; set; }
        public virtual ICollection<AracModel> AracModels { get; set; }
        public virtual ICollection<AracTip> AracTips { get; set; }
    }
}
