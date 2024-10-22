using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceMuhasebe
{
    public class AracEntity
    {
        public String PlakaKodu { get; set; }
        public String PlakaNo { get; set; }

        public AracEntity()
        {

        }
        public AracEntity(PoliceArac PoliceArac)
        {
            if (PoliceArac!=null)
            {
                this.PlakaKodu = PoliceArac.PlakaKodu;
                this.PlakaNo = PoliceArac.PlakaNo;
            }
        }
    }
}
