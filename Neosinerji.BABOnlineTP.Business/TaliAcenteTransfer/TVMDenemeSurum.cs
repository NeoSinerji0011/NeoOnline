using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer
{
   public class TVMDenemeSurum
    {

       public TVMDenemeSurum()
        { }
        public TVMDetay TVMDetay { get; set; }
        public TVMYetkiGruplari DenemeSurumYetkiGrup { get; set; }
        public TVMYetkiGrupYetkileri DenemeSurumYetkiGrupYetkileri { get; set; }
        public TVMWebServisKullanicilari DenemeSurumServisKullanici { get; set; }
        public TVMDepartmanlar taliDepartman { get; set; }
        public TVMUrunYetkileri DenemeSurumUrunYetki { get; set; }
        public TVMKullanicilar tvmKullanici { get; set; }
        public List<TVMDepartmanlar> tvmDepartmanlar { get; set; }
        public List<TVMWebServisKullanicilari> tvmWebServisKullanicilari { get; set; }
        public List<TVMUrunYetkileri> tvmUrunYetkileri { get; set; }
    }
}
