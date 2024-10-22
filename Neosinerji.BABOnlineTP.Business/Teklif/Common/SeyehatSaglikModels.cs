using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.CHARTIS
{
    public class SeyehatSaglikSigortalilar
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public DateTime DogumTarihi { get; set; }
        public bool Uyruk { get; set; }
        public bool KimlikTipi { get; set; }
        public string KimlikNo { get; set; }
        public byte BireyTipi { get; set; }
    }
}
