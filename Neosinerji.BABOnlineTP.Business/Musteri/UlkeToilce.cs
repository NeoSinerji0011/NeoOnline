using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Service
{
    public class UlkeToilceListModel
    {
        public List<UlkeToilceModel> Items { get; set; }
    }

    public class UlkeToilceModel
    {
        public int MusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public int ArdesTipi { get; set; }

        public string SayfaAdi { get; set; }
        public string AdresTipiText { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string Adres { get; set; }
        public bool Varsayilan { get; set; }

    }

  
}