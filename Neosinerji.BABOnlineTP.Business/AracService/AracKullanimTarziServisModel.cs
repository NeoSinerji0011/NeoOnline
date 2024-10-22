using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business
{
    public class AracKullanimTarziServisModel
    {
        public string Kod { get; set; }
        public string KullanimTarzi { get; set; }
    }

    public class MapfreToHdiPlakaSorgu
    {
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public string AracKullanimSekli { get; set; }
        public string AracKullanimTarzi { get; set; }
        public string AracMarkaKodu { get; set; }
        public string AracTipKodu { get; set; }
        public string AracModelYili { get; set; }
        public string AracMotorNo { get; set; }
        public string AracSasiNo { get; set; }
        public string AracTescilTarih { get; set; }
        public string AracKoltukSayisi { get; set; }
        public string AracDegeri { get; set; }
        public string AracSilindir { get; set; }
        public string YeniPoliceBaslangicTarih { get; set; }
        public string EskiPoliceSigortaSirkedKodu { get; set; }
        public string EskiPoliceAcenteKod { get; set; }
        public string EskiPoliceNo { get; set; }
        public string EskiPoliceYenilemeNo { get; set; }
        public string TasiyiciSigSirkerKod { get; set; }
        public string TasiyiciSigAcenteNo { get; set; }
        public string TasiyiciSigPoliceNo { get; set; }
        public string TasiyiciSigYenilemeNo { get; set; }

        public string PoliceBitisTarihi { get; set; }

        public string HasarsizlikInd { get; set; }
        public string HasarsizlikSur { get; set; }
        public string HasarsizlikKademe { get; set; }
        public string UygulanmisHasarsizlikKademe { get; set; }

        public string TescilSeri { get; set; }
        public string TescilSeriNo { get; set; }

        public string motorGucu { get; set; }
        public string silindirHacmi { get; set; }
        public string belgeNo { get; set; }
        public string belgeTarihi { get; set; }

        public List<AracKullanimTarziServisModel> TarzList { get; set; }
        public List<AracMarka> Markalar { get; set; }
        public List<AracTip> Tipler { get; set; }

        public string ProjeKodu { get; set; }
        public List<SelectListItem> Ams { get; set; }
        public List<SelectListItem> IkameTurleri { get; set; }
        public string IkameTuru { get; set; }
    }
}
