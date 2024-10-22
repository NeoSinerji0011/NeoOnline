using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMYetkiGruplari
    {
        public TVMYetkiGruplari()
        {
            this.TVMYetkiGrupYetkileris = new List<TVMYetkiGrupYetkileri>();
        }

        public int YetkiGrupKodu { get; set; }
        public int TVMKodu { get; set; }
        public string YetkiGrupAdi { get; set; }
        public Nullable<bool> YetkiSeviyesi { get; set; }
        public virtual TVMDetay TVMDetay { get; set; }
        public virtual ICollection<TVMYetkiGrupYetkileri> TVMYetkiGrupYetkileris { get; set; }
    }
}
