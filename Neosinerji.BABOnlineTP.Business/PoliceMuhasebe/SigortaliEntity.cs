using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceMuhasebe
{
    public class SigortaliEntity
    {
        public String MusteriGrupKodu { get; set; }
        public String MuhasebeHesapNumarasi { get; set; }
        public String MusteriTipi { get; set; }
        public String VergiNoTCKimlikNo { get; set; }
        public String AdSoyadUnvani { get; set; }
        public String TelefonNo { get; set; }
        public String MobilTelefonNo { get; set; }
        public String Email { get; set; }
        public String AdresIl { get; set; }
        public String AdresIlce { get; set; }
        public String Adres { get; set; }

        public SigortaliEntity()
        {

        }

        public SigortaliEntity(PoliceSigortali PoliceSigortali)
        {
            if (PoliceSigortali != null)
            {
                this.Adres = PoliceSigortali.Adres;
                this.AdresIl = PoliceSigortali.IlAdi;
                if (PoliceSigortali.IlceAdi != null)
                {
                    this.AdresIlce = PoliceSigortali.IlceAdi;
                }

                this.AdSoyadUnvani = PoliceSigortali.AdiUnvan + " " + PoliceSigortali.SoyadiUnvan;
                //this.Email
                this.MuhasebeHesapNumarasi = PoliceSigortali.MuhasebeEntegrasyonHesapNo;
                this.MusteriGrupKodu = PoliceSigortali.MusteriGrupKodu;
                //this.MusteriTipi
                this.TelefonNo = PoliceSigortali.TelefonNo;
                this.MobilTelefonNo = PoliceSigortali.MobilTelefonNo;

                if (!String.IsNullOrEmpty(PoliceSigortali.KimlikNo))
                {
                    this.VergiNoTCKimlikNo = PoliceSigortali.KimlikNo.Trim();
                    this.MusteriTipi = "1"; //TC
                }
                else if (!String.IsNullOrEmpty(PoliceSigortali.VergiKimlikNo))
                {
                    this.VergiNoTCKimlikNo = PoliceSigortali.VergiKimlikNo.Trim();
                    this.MusteriTipi = "2"; //VKN
                }
                else
                {
                    this.VergiNoTCKimlikNo = "";
                    this.MusteriTipi = ""; 
                }
            }
        }
    }
}
