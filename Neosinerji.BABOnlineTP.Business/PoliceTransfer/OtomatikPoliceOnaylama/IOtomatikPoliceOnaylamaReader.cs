using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public interface IOtomatikPoliceOnaylamaReader
    {
        List<PoliceOnaySonucModel> PoliceleriOnayla();
    }
    public class PoliceOnaySonucModel
    {
        public string SigortaSirketKodu { get; set; }
        public string SigortaSirketUnvani { get; set; }
        public string PoliceNumarasi { get; set; }
        public int? EkNumarasi { get; set; }
        public int? YenilemeNumarasi { get; set; }
        public int TaliAcenteKodu { get; set; }
        public string TaliAcenteUnvani { get; set; }
        public bool GuncellemeBasarili { get; set; }
        public string BilgiMesaji { get; set; }
        public string GenelHataMesaji { get; set; }
    }
}
