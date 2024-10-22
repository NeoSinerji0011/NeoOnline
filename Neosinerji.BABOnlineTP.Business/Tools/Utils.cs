using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Neosinerji.BABOnlineTP.Business.Tools
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
        }

    }
    public class ReaderColumn
    {
        public string ColumnName { get; set; }
    }
}
