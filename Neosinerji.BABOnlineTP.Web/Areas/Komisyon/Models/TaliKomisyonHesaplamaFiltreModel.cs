using Neosinerji.BABOnlineTP.Business.Komisyon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Models
{
    public class TaliKomisyonHesaplamaFiltreModel
    {
        public TaliKomisyonHesaplamaFiltreModel()
        {
            this.TVMListe = new List<TVMModel>();
            this.DisKaynakListe = new List<TVMModel>();
            this.BransListe = new List<BransModel>();
            this.SigortaSirketiListe = new List<SigortaSirketiModel>();
        }
        public TaliKomisyonHesaplamaPoliceDurumu PoliceDurumu { get; set; }
        public List<TVMModel> TVMListe { get; set; }
        public List<TVMModel> DisKaynakListe { get; set; }
        public List<BransModel> BransListe { get; set; }
        public List<SigortaSirketiModel> SigortaSirketiListe { get; set; }
        public DateTime TarihBaslangic { get; set; }
        public DateTime TarihBitis { get; set; }
        public PrimTipleri IptalZeylTahakkuk { get; set; }
    }
}