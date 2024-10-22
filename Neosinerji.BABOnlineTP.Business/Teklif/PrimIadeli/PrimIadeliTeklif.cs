﻿using System;
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
using Neosinerji.BABOnlineTP.Business.AEGON;
using Newtonsoft.Json;
using Neosinerji.BABOnlineTP.Business.CHARTIS;
using System.IO;
using Neosinerji.BABOnlineTP.Business.Common.AEGON;

namespace Neosinerji.BABOnlineTP.Business
{
    public class PrimIadeliTeklif : TeklifBase, IPrimIadeliTeklif
    {

        public PrimIadeliTeklif()
            : base()
        {

        }

        public PrimIadeliTeklif(int teklifId)
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
                    IAEGONPrimIadeli PrimIadeli = DependencyResolver.Current.GetService<IAEGONPrimIadeli>();
                    PrimIadeli.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(PrimIadeli);
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
