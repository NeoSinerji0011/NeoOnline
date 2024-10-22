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
using Neosinerji.BABOnlineTP.Business.CHARTIS;
using Newtonsoft.Json;
using Neosinerji.BABOnlineTP.Business.AEGON;
using System.IO;
using Neosinerji.BABOnlineTP.Business.Common.AEGON;
using System.Globalization;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KorunanGelecekTeklif : TeklifBase, IKorunanGelecekTeklif
    {
        public KorunanGelecekTeklif()
            : base()
        {

        }

        public KorunanGelecekTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool aegon = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.AEGON) == 1;

            if (aegon)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IAEGONKorunanGelecek KorunanGelecek = DependencyResolver.Current.GetService<IAEGONKorunanGelecek>();
                    KorunanGelecek.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(KorunanGelecek);
                }
            }

            return base.Hesapla(teklif);
        }

        public override void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();

            if (!String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFDosyasi))
            {
                email.SendAegonEMailTeklif(this.Teklif, DigerAdSoyad, DigerEmail, false);
            }
        }
       
    }
}
