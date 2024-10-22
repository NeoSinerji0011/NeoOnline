using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class Odeme
    {
        public Odeme(byte odemeSekli, byte taksitSayisi)
        {
            this.OdemeSekli = odemeSekli;
            this.OdemeTipi = OdemeTipleri.Nakit;
            this.TaksitSayisi = taksitSayisi;
        }

        public Odeme(string kartSahibi, string kartNo, string ccv, string skay, string skyil)
        {
            this.OdemeTipi = OdemeTipleri.KrediKarti;
            this.KrediKarti = new KrediKarti();
            this.KrediKarti.KartSahibi = kartSahibi;
            this.KrediKarti.KartNo = kartNo;
            this.KrediKarti.CVC = ccv;
            this.KrediKarti.SKA = skay;
            this.KrediKarti.SKY = skyil;
        }
        public Odeme(string kartSahibi, string kartNo, string ccv, string skay, string skyil, byte odemeSekli)
        {
            this.OdemeTipi = OdemeTipleri.KrediKarti;
            this.KrediKarti = new KrediKarti();
            this.KrediKarti.KartSahibi = kartSahibi;
            this.KrediKarti.KartNo = kartNo;
            this.KrediKarti.CVC = ccv;
            this.KrediKarti.SKA = skay;
            this.KrediKarti.SKY = skyil;
            this.OdemeSekli = odemeSekli;
        }
        public byte OdemeSekli { get; set; }
        public byte TaksitSayisi { get; set; }
        public byte OdemeTipi { get; set; }
        public KrediKarti KrediKarti { get; set; }
    }
}
