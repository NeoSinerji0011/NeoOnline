using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models
{
    public class TVMProjeKoduGenelModel
    {
        //public int AktifAcenteSayisi { get; set; }
        //public int PasifAcenteSayisi { get; set; }
        public int ToplamAcenteSayisi { get; set; }
        //public int AktifKullaniciSayisi { get; set; }
        //public int PasifKullaniciSayisi { get; set; }
        public int ToplamKullaniciSayisi { get; set; }
        //public int LoginKullaniciBugun { get; set; }

        public GenelRaporProcedureModel RaporSonuc { get; set; }

        public DuzenlenenTeklifSayisi TeklifSayisi { get; set; }
        public DuzelenenPoliceSayisi PoliceSayisi { get; set; }
    }

    public class DuzenlenenTeklifSayisi
    {
        public string kasko { get; set; }
        public string trafik { get; set; }

    }
    public class DuzelenenPoliceSayisi
    {
        public string kasko { get; set; }
        public string trafik { get; set; }

    }
}