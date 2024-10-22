using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database
{
     [Serializable]
    public class OfflineUretimPerformans
    {
        public List<OfflineUretimPerformansProcedureModel> performansList { get; set; }
    }
    public class OfflineUretimPerformansProcedureModel
    {
        public int donem { get; set; }
        public int tvmkodu { get; set; }
        public int branskodu { get; set; }
        public int? tvmkodutali { get; set; }

        public decimal? OcakPoliceSayisi { get; set; }
        public decimal? OcakPolicePrimi { get; set; }
        public decimal? OcakPoliceKomisyon { get; set; }
        public decimal? OcakPoliceVerilenKomisyon { get; set; }
            
        public decimal? SubatPoliceSayisi { get; set; }
        public decimal? SubatPolicePrimi { get; set; }
        public decimal? SubatPoliceKomisyon { get; set; }
        public decimal? SubatPoliceVerilenKomisyon { get; set; }
             
        public decimal? MartPoliceSayisi { get; set; }
        public decimal? MartPolicePrimi { get; set; }
        public decimal? MartPoliceKomisyon { get; set; }
        public decimal? MartPoliceVerilenKomisyon { get; set; }
            
        public decimal? NisanPoliceSayisi { get; set; }
        public decimal? NisanPolicePrimi { get; set; }
        public decimal? NisanPoliceKomisyon { get; set; }
        public decimal? NisanPoliceVerilenKomisyon { get; set; }
           
        public decimal? MayisPoliceSayisi { get; set; }
        public decimal? MayisPolicePrimi { get; set; }
        public decimal? MayisPoliceKomisyon { get; set; }
        public decimal? MayisPoliceVerilenKomisyon { get; set; }
             
        public decimal? HaziranPoliceSayisi { get; set; }
        public decimal? HaziranPolicePrimi { get; set; }
        public decimal? HaziranPoliceKomisyon { get; set; }
        public decimal? HaziranPoliceVerilenKomisyon { get; set; }
         
        public decimal? TemmuzPoliceSayisi { get; set; }
        public decimal? TemmuzPolicePrimi { get; set; }
        public decimal? TemmuzPoliceKomisyon { get; set; }
        public decimal? TemmuzPoliceVerilenKomisyon { get; set; }
             
        public decimal? AgustosPoliceSayisi { get; set; }
        public decimal? AgustosPolicePrimi { get; set; }
        public decimal? AgustosPoliceKomisyon { get; set; }
        public decimal? AgustosPoliceVerilenKomisyon { get; set; }
              
        public decimal? EylulPoliceSayisi { get; set; }
        public decimal? EylulPolicePrimi { get; set; }
        public decimal? EylulPoliceKomisyon { get; set; }
        public decimal? EylulPoliceVerilenKomisyon { get; set; }
              
        public decimal? EkimPoliceSayisi { get; set; }
        public decimal? EkimPolicePrimi { get; set; }
        public decimal? EkimPoliceKomisyon { get; set; }
        public decimal? EkimPoliceVerilenKomisyon { get; set; }
              
        public decimal? KasimPoliceSayisi { get; set; }
        public decimal? KasimPolicePrimi { get; set; }
        public decimal? KasimPoliceKomisyon { get; set; }
        public decimal? KasimPoliceVerilenKomisyon { get; set; }
           
        public decimal? AralikPoliceSayisi { get; set; }
        public decimal? AralikPolicePrimi { get; set; }
        public decimal? AralikPoliceKomisyon { get; set; }
        public decimal? AralikPoliceVerilenKomisyon { get; set; }

    }
}
