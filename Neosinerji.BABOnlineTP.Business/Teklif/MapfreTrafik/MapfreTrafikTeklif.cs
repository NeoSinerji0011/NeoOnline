using System;
using System.Collections.Generic;
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
using Neosinerji.BABOnlineTP.Business.MAPFRE;

namespace Neosinerji.BABOnlineTP.Business
{
    public class MapfreTrafikTeklif : TeklifBase, IMapfreTrafikTeklif
    {
        public MapfreTrafikTeklif()
            : base()
        {

        }

        public MapfreTrafikTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            IMAPFREProjeTrafik trafik = DependencyResolver.Current.GetService<IMAPFREProjeTrafik>();
            this.AddTeklif(trafik);

            teklif = _TeklifService.Create(teklif);
            this.Teklif = teklif;

            IsDurum isDurum = base.IsDurumBaslat(this.Teklif);
            trafik.Hesapla(this.Teklif);
            this.Create(trafik);
            this.IsDurumTeklifTamamlandi(this.Teklif.GenelBilgiler.TeklifId, trafik);
            this.IsDurumTamamlandi();

            return isDurum;
        }

        public override void Policelestir(ITeklif teklif, Odeme odeme)
        {
            teklif.Policelestir(odeme);
        }

        public override void CreatePDF()
        {
            try
            {
                ITeklif teklif = this.TUMTeklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE);

                if (teklif != null)
                {
                    teklif = _TeklifService.GetTeklif(teklif.GenelBilgiler.TeklifId);
                    IMAPFREProjeTrafik mapfreTrafik = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFREProjeTrafik;
                    string teklifUrl = mapfreTrafik.TeklifPDF();
                    this.Teklif.GenelBilgiler.PDFDosyasi = teklifUrl;
                    _TeklifService.UpdateGenelBilgiler(this.Teklif.GenelBilgiler);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("PDF Oluşturlamadı.");
                _LogService.Error(ex);
                throw;
            }
        }

        public override void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();
            if (!String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFPolice))
                email.GenelUrunMailGonder(this, DigerAdSoyad, DigerEmail);
        }
    }
}
