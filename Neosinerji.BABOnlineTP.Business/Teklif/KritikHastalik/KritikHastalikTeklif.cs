using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.METLIFE;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KritikHastalikTeklif : TeklifBase, IKritikHastalikTeklif
    {
        public KritikHastalikTeklif()
            : base()
        {

        }

        public KritikHastalikTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool chartis = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.METLIFE) == 1;

            if (chartis)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IMETLIFEKritikHastalik kritikHastalik = DependencyResolver.Current.GetService<IMETLIFEKritikHastalik>();
                    kritikHastalik.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(kritikHastalik);
                }
            }

            return base.Hesapla(teklif);
        }
    }
}
