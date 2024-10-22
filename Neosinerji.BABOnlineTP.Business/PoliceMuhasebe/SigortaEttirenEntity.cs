using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceMuhasebe
{
    public class SigortaEttirenEntity
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

        public SigortaEttirenEntity()
        {

        }

        public SigortaEttirenEntity(PoliceSigortaEttiren PoliceSigortaEttiren)
        {
            if (PoliceSigortaEttiren != null)
            {
                this.Adres = PoliceSigortaEttiren.Adres;
                this.AdresIl = PoliceSigortaEttiren.IlAdi;
                this.AdresIlce = PoliceSigortaEttiren.IlceAdi;
                this.AdSoyadUnvani = PoliceSigortaEttiren.AdiUnvan + " " + PoliceSigortaEttiren.SoyadiUnvan;
                //this.Email 
                this.MuhasebeHesapNumarasi = PoliceSigortaEttiren.MuhasebeEntegrasyonHesapNo;
                this.MusteriGrupKodu = PoliceSigortaEttiren.MusteriGrupKodu;
                //this.MusteriTipi
                this.TelefonNo = PoliceSigortaEttiren.TelefonNo;
                this.MobilTelefonNo = PoliceSigortaEttiren.MobilTelefonNo;

                if (!String.IsNullOrEmpty(PoliceSigortaEttiren.KimlikNo))
                {
                    this.VergiNoTCKimlikNo = PoliceSigortaEttiren.KimlikNo.Trim();
                    this.MusteriTipi = "1"; //TC
                }
                else if (!String.IsNullOrEmpty(PoliceSigortaEttiren.VergiKimlikNo))
                {
                    this.VergiNoTCKimlikNo = PoliceSigortaEttiren.VergiKimlikNo.Trim();
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
