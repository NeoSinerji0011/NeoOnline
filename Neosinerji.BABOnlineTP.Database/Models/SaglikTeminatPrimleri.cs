using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class SaglikTeminatPrimleri
    {
        public int Id { get; set; }
        public int BransKodu { get; set; }
        public int UrunKodu { get; set; }
        public string TeminatTipKodu { get; set; }
        public string BolgeKodu { get; set; }
        public int PlanKodu { get; set; }
        public string YasAraligi { get; set; }
        public decimal Prim { get; set; }
        public System.DateTime PrimGecerlilikBaslangicTarihi { get; set; }
        public System.DateTime PrimGecerlilikBitisTarihi { get; set; }
        public virtual SaglikBolgeleri SaglikBolgeleri { get; set; }
        public virtual SaglikTeminatTipleri SaglikTeminatTipleri { get; set; }
    }
}
