using DocumentFormat.OpenXml.ExtendedProperties;
using Neosinerji.BABOnlineTP.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static System.Net.WebRequestMethods;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class Utils
    {
        public static decimal[] TaksitDuzenle(decimal brutPrim, int taksitSayisi)
        {
            if (brutPrim != 0 && taksitSayisi > 0)
            {
                var genelTaksitTutari = decimal.Parse((brutPrim / taksitSayisi).ToString("#.##"));

                decimal taksittutari = genelTaksitTutari;
                decimal sontaksittutari = genelTaksitTutari;
                for (int i = 0; i < taksitSayisi; i++)
                {
                    if ((i + 1) == taksitSayisi)
                    {
                        if (brutPrim != genelTaksitTutari * taksitSayisi)
                        {
                            sontaksittutari = genelTaksitTutari + (brutPrim - genelTaksitTutari * taksitSayisi);
                        }
                    }
                }

                return new decimal[] { taksittutari, sontaksittutari };
            }
            else
                return new decimal[] { 0, 0 };

        }
        public static decimal decimalDuzenle(string value)
        {
            decimal res = 0;
            value = value.Replace(",", ".");
            var sonNoktaIndis = value.LastIndexOf(".");

            try
            {
                var ondalik = value.Substring(sonNoktaIndis + 1);
                var tamkisim = value.Substring(0, sonNoktaIndis).Replace(".", "");
                var bolum = "1".PadRight(ondalik.Length + 1, '0');
                var decimalondalik = Convert.ToDecimal(ondalik) / Convert.ToDecimal(bolum);
                res = Convert.ToDecimal(tamkisim) + decimalondalik;
                return res;
            }
            catch (Exception)
            {
            }
            return Convert.ToDecimal(value);

            //if (value.Contains("."))
            //{
            //    var ondalik = value.Substring(value.LastIndexOf(".")+1);
            //    var tamkisim = value.Substring(0,value.LastIndexOf("."));
            //    var bolum = "1".PadRight(ondalik.Length+1,'0');
            //    var decimalondalik = Convert.ToDecimal(ondalik) / Convert.ToDecimal(bolum);
            //    res = Convert.ToDecimal(tamkisim) + decimalondalik;
            //}
            //else if (value.Contains(","))
            //{
            //    var ondalik = value.Substring(value.LastIndexOf(",")+1);
            //    var tamkisim = value.Substring(0, value.LastIndexOf(","));
            //    var bolum = "1".PadRight(ondalik.Length + 1, '0');
            //    var decimalondalik = Convert.ToDecimal(ondalik) / Convert.ToDecimal(bolum);
            //    res = Convert.ToDecimal(tamkisim) + decimalondalik;
            //} 

            //return res;
        }
        public static string isNullorEmpty(string res)
        {
            if (!string.IsNullOrEmpty(res))
            {
                return res;
            }
            return "";
        }
        public static string[] GetColumnNameList(string birlikkodu)
        {
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Files/Reader"), birlikkodu + ".json");
            string[] list = new string[0];
            var temp = System.IO.File.ReadAllText(path);
            try
            {
                var templist = JsonConvert.DeserializeObject<List<ReaderColumn>>(temp);
                list = new string[templist.Count];
                for (int i = 0; i < templist.Count; i++)
                {
                    list[i] = templist[i].ColumnName;
                }
            }
            catch (Exception)
            { }

            return list;
        }
    }
    public class UnderWriterItem
    {
        public CariHesaplari CariHesaplari { get; set; }
        public string UnderWriterKodu { get; set; }
        public decimal UWPrimTaksitTutar { get; set; }
        public decimal UWPrimTaksitTutarTL { get; set; }
        public decimal UWPrimSonTaksitTutar { get; set; }
        public decimal UWPrimSonTaksitTutarTL { get; set; }
        public decimal UWKomisyonTaksitTutar { get; set; }
        public decimal UWKomisyonTaksitTutarTL { get; set; }
        public decimal UWKomisyonSonTaksitTutar { get; set; }
        public decimal UWKomisyonSonTaksitTutarTL { get; set; }
    }
    public class ReaderColumn
    {
        public string ColumnName { get; set; }
    }
}
