using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class NeoConnectLogRaporlamaModels
    {
        public int LogId { get; set; }
        public Nullable<int> TvmKodu { get; set; }
        public Nullable<int> KullaniciKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string Kullanici { get; set; }
        public string SigortaSirketKodu { get; set; }
        public string IPAdresi { get; set; }
        public string MACAdresi { get; set; }
        public Nullable<System.DateTime> KullaniciGirisTarihi { get; set; }
        public Nullable<System.DateTime> KullaniciCikisTarihi { get; set; }
    }
}