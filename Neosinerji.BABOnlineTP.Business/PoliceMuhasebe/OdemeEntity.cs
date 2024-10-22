using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceMuhasebe
{
    public class OdemeEntity
    {
        public Int32 TaksitNo { get; set; }
        public DateTime VadeTarihi { get; set; }
        public Decimal TaksitTutari { get; set; }
        public Int32 OdemeTipi { get; set; }

        public OdemeEntity()
        {

        }

        public OdemeEntity(PoliceOdemePlani odemePlani){
            if (odemePlani != null)
            {
                this.TaksitNo = odemePlani.TaksitNo;
                if (odemePlani.VadeTarihi != null)
                {
                    this.VadeTarihi = (DateTime)odemePlani.VadeTarihi;
                }
                if (odemePlani.OdemeTipi != null) { this.OdemeTipi = (Int32)odemePlani.OdemeTipi; }
                if (odemePlani.TaksitTutari != null)
                {
                    this.TaksitTutari = (Decimal)odemePlani.TaksitTutari;
                }
            }
            
        }
    }
}
