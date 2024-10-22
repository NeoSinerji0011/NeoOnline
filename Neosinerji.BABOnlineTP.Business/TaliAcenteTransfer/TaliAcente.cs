using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer
{
    public class TaliAcente : ITaliAcente
    {
        public TaliAcente()
        { }
        public TVMDetay TVMDetay { get; set; }
        public TaliAcenteExcel taliAcenteExcelModel { get; set; }
        public TVMYetkiGruplari taliYetkiGruplari { get; set; }
        public TVMYetkiGrupYetkileri taliYetkiGrupYetkileri { get; set; }
        public TVMWebServisKullanicilari servisKullanici { get; set; }
        public TVMDepartmanlar taliDepartman { get; set; }
        public TVMBolgeleri taliBolge { get; set; }
        public TVMUrunYetkileri tvmUrunYetki { get; set; }
        public TVMKullanicilar tvmKullanici { get; set; }
        public List<TVMYetkiGruplari> yetkiGruplari { get; set; }
        public List<TVMYetkiGrupYetkileri> yetkiGrupYetkileri { get; set; }
        public List<TVMDepartmanlar> tvmDepartmanlar { get; set; }
        public List<TVMBolgeleri> tvmBolgeleri { get; set; }
        public List<TVMWebServisKullanicilari> tvmWebServisKullanicilari { get; set; }
        public List<TVMUrunYetkileri> tvmUrunYetkileri { get; set; }
        public List<TVMYetkiGruplari> yetkiGruplariYeni { get; set; }
    }


    public class TaliAcenteExcel
    {
        public string Unvan { get; set; }
        public string AcikAdres { get; set; }
        public string Telefon { get; set; }
        public string Faks { get; set; }
        public string Email { get; set; }
    }

}



