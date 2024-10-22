using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Komisyon;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Models
{
    public class KullaniciKokpitViewModel
    {
        public List<TVMKullanicilar> kullanicilar { get; set; }
        public OfflineUretimPerformansKullanici performansList { get; set; }
        public int donemYil { get; set; }
     
    }
}