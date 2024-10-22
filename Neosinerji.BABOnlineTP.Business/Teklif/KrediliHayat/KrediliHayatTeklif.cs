using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Pdf;
using Neosinerji.BABOnlineTP.Business.DEMIR;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KrediliHayatTeklif : TeklifBase, IKrediliHayatTeklif
    {
        public KrediliHayatTeklif()
            : base()
        {

        }

        public KrediliHayatTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool demir = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.DEMIR) == 1;

            if (demir)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IDEMIRKrediliHayat hayat = DependencyResolver.Current.GetService<IDEMIRKrediliHayat>();
                    hayat.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(hayat);
                }
            }

            return base.Hesapla(teklif);
        }

        public override void Policelestir(ITeklif teklif, Odeme odeme)
        {
            teklif.Policelestir(odeme);
        }

        public override void CreatePDF()
        {

        }

        public override void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();
            email.SendKrediHayat(this, DigerAdSoyad, DigerEmail);
        }
    }
}
