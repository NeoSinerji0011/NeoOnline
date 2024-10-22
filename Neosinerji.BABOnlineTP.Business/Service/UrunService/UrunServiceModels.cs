using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class UrunServiceModel
    {
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public int BransKodu { get; set; }
        public int Durum { get; set; }
        public string BransAdi { get; set; }
    }

    public class UrunTeminatServiceModel
    {
        public int UrunKodu { get; set; }
        public int TeminatKodu { get; set; }
        public string TeminatAdi { get; set; }
        public int SiraNo { get; set; }
    }

    public class UrunVergiServiceModel
    {
        public int UrunKodu { get; set; }
        public int VergiKodu { get; set; }
        public string VergiAdi { get; set; }
        public int SiraNo { get; set; }
    }

    public class UrunSoruServiceModel
    {
        public int UrunKodu { get; set; }
        public int SoruKodu { get; set; }
        public string SoruAdi { get; set; }
        public short SoruCevapTipi { get; set; }
        public int SiraNo { get; set; }
    }
}
