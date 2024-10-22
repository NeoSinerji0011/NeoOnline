using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Web.Areas.TVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class YonetimModel
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public List<KullaniciNotListeleModel> KullaniciNotlari { get; set; }
        public List<DuyuruProcedureModel> Duyurular { get; set; }
        public string JScript { get; set; }
        public int DuyuruId { get; set; }

        public string ToplamMusteri { get; set; }
        public string ToplamTeklif { get; set; }
        public string ToplamPolice { get; set; }
        public string PolicelesmeOrani { get; set; }
        public bool NeoOnlineKokpitMenuYekiliMi { get; set; }
    }
}