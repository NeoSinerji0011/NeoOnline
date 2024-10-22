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
    public class FerdiKazaPlusTeklif : TeklifBase, IFerdiKazaPlusTeklif
    {  public FerdiKazaPlusTeklif()
            : base()
        {

        }

        public FerdiKazaPlusTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            IMETLIFEFerdiKazaPlus ferdiKazaPlus = DependencyResolver.Current.GetService<IMETLIFEFerdiKazaPlus>();
            this.AddTeklif(ferdiKazaPlus);

            teklif = _TeklifService.Create(teklif);
            this.Teklif = teklif;

            IsDurum isDurum = base.IsDurumBaslat(this.Teklif);
            ferdiKazaPlus.Hesapla(this.Teklif);
            this.Create(ferdiKazaPlus);
            this.IsDurumTeklifTamamlandi(this.Teklif.GenelBilgiler.TeklifId, ferdiKazaPlus);
            this.IsDurumTamamlandi();

            return isDurum;

        }
    }
}
