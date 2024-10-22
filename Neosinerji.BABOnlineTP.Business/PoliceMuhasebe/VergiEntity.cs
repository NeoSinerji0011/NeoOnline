using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceMuhasebe
{
    public class VergiEntity
    {
        public Decimal ? GiderVergisi { get; set; } // Gider Vergisi
        public Decimal ? YanginSigortaVergisi { get; set; } // Yangin Sigorta Vergisi
        public Decimal ? TrafikHizmetleriGelistirmeFonu { get; set; } // Trafik Hizmetleri Gelistirme Fonu
        public Decimal ? GarantiFonu { get; set; } // Garanti Fonu

        public VergiEntity()
        {

        }

        public VergiEntity(PoliceGenel policeGenel)
        {
            
        }
    }

   
}
