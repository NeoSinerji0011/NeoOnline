using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models
{
    public class MapfreBolgeUretimRaporModel
    {
        public DateTime BaslangicTarih { get; set; }
        public DateTime BitisTarih { get; set; }
        public int? BolgeKodu { get; set; }
        public bool Acenteler { get; set; }
        public SelectList Bolgeler { get; set; }

        public bool BolgeSecebilir { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ErrorReport { get; set; }
        public List<MapfreBolgeUretimModel> RaporBolgeler { get; set; }
        public List<MapfreBolgeUretimModel> Rapor { get; set; }

        public string IntToString(int value)
        {
            if (value == 0)
                return String.Empty;

            return value.ToString();
        }

        public string DecimalToString(decimal value)
        {
            if (value == 0)
                return String.Empty;

            return value.ToString("N2");
        }
    }
}