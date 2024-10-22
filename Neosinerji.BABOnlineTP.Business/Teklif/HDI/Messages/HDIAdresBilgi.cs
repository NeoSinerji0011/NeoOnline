using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    public class HDIAdresBilgi
    {
        public string Adres { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Semt { get; set; }
        public string KoyMahalle { get; set; }
        public string BinaNo { get; set; }
        public string HanApartmanAd { get; set; }
        public string Daire { get; set; }
        public string Kat { get; set; }
        public string PostaKod { get; set; }
        public string Ilce { get; set; }
        public string IlKod { get; set; }
        public string IKIlKd { get; set; }
        public string IKIlc { get; set; }
        public string TSIlKd { get; set; }
        public string TSIlc { get; set; }
    }
}
