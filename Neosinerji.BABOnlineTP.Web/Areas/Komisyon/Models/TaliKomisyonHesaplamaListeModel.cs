using Neosinerji.BABOnlineTP.Business.Komisyon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Models
{
    public class TaliKomisyonHesaplamaListeModel
    {
        public int Id { get; set; }
        public TVMModel Tali { get; set; }
        public string DisKaynakUnvani { get; set; }
        public SigortaSirketiModel SigortaSirketi { get; set; }
        public BransModel Brans { get; set; }
        public decimal Prim { get; set; }
        public decimal AlinanKomisyon { get; set; }
        public string PoliceTanzimTarihi { get; set; }
        public string PoliceNo { get; set; }
        public string SigortaliUnvani { get; set; }
        public string SigortaEttirenUnvani { get; set; }
        public decimal? TaliKomisyonOrani { get; set; }
        public decimal TaliKomisyon { get; set; }
    }
}