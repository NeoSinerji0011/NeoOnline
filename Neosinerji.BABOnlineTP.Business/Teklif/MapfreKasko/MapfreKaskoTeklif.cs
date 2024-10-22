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
    public class MapfreKaskoTeklif : TeklifBase, IMapfreKaskoTeklif
    {
        public MapfreKaskoTeklif()
            : base()
        {

        }

        public MapfreKaskoTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            IMAPFREProjeKasko kasko = DependencyResolver.Current.GetService<IMAPFREProjeKasko>();
            this.AddTeklif(kasko);

            teklif = _TeklifService.Create(teklif);
            this.Teklif = teklif;

            IsDurum isDurum = base.IsDurumBaslat(this.Teklif);
            kasko.Hesapla(this.Teklif);
            this.Create(kasko);
            this.IsDurumTeklifTamamlandi(this.Teklif.GenelBilgiler.TeklifId, kasko);
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
                    MAPFREProjeKasko mapfreKasko = TeklifUrunFactory.AsUrunClass(teklif) as MAPFREProjeKasko;
                    string teklifUrl = mapfreKasko.TeklifPDF();
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
            bool pdfDosyasiVar = !String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFDosyasi);

            if (!pdfDosyasiVar)
            {
                try
                {
                    if (this.Teklif.GenelBilgiler.TUMKodu == 0)
                        this.CreatePDF();

                }
                catch (Exception ex)
                {
                    _LogService.Error(ex);
                }
            }
            if (pdfDosyasiVar)
                email.GenelUrunMailGonder(this, DigerAdSoyad, DigerEmail);
        }
    }
}
