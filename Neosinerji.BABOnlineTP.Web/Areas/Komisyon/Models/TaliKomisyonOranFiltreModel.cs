using Neosinerji.BABOnlineTP.Business.Komisyon;
using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Models
{
    public class TaliKomisyonOranFiltreModel
    {
        public TaliKomisyonOranFiltreModel()
        {
            this.TVMListe = new List<TVMModel>();
            this.DisKaynakList = new List<TVMModel>();
            //this.TaliDisKaynakList = new List<TVMModel>();
            this.BransListe = new List<BransModel>();
            this.SigortaSirketiListe = new List<SigortaSirketiModel>();
            this.KademeListe = new List<KomisyonKademeModel>();
        }
        public TaliKomisyonOranFiltreIslem Islem { get; set; }
        public SatisKanaliDisKaynakFiltre KaynakSecim { get; set; }
        public List<TVMModel> TVMListe { get; set; }
        public List<TVMModel> DisKaynakList { get; set; }
        public int TaliDisKaynakKodu { get; set; }
        public List<BransModel> BransListe { get; set; }
        public List<SigortaSirketiModel> SigortaSirketiListe { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public decimal Oran { get; set; }
        public List<KomisyonKademeModel> KademeListe { get; set; }
        public int GecerliYil { get; set; }
        public bool Kademeli { get; set; }
        public int Sayfa { get; set; }
        public int Adet { get; set; }
    }
}