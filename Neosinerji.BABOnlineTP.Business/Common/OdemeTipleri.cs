using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class OdemeTipleri
    {
        public const byte Yok = 0;
        public const byte Nakit = 1;
        public const byte KrediKarti = 2;
        public const byte Havale = 3;
        public const byte CekSenet = 4;
        public const byte AcenteKrediKarti = 5;
        public const byte BlokeliKrediKarti = 6;
        public const byte BankaHesabi = 7;

        public static List<SelectListItem> OdemeTipleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                    new SelectListItem(){ Text=ResourceHelper.GetString("Cash"), Value="1"},
                    new SelectListItem(){ Text=ResourceHelper.GetString("Credit_Card"), Value="2"},
                    new SelectListItem(){ Text=ResourceHelper.GetString("Transfer"), Value="3"},
                    new SelectListItem(){ Text=ResourceHelper.GetString("CekSenet"),Value="4"}
            });

            return list;
        }

        public static string OdemeTipi(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    //case Nakit: result = ResourceHelper.GetString("Cash"); break;
                    //case KrediKarti: result = ResourceHelper.GetString("Credit_Card"); break;
                    //case Havale: result = ResourceHelper.GetString("Transfer"); break;
                    //case CekSenet: result = ResourceHelper.GetString("CekSenet"); break;
                    case Nakit: result = "Nakit"; break;
                    case KrediKarti: result ="Kredi Kartı"; break;
                    case Havale: result = "Havale"; break;
                    case CekSenet: result = "Çek/Senet"; break;
                    default: result = ""; break;
                }

            return result;
        }

    }

    public class TahsilatOdemeTipleri
    {
        public const byte Yok = 0;
        public const byte Nakit = 1;
        public const byte KrediKarti = 2;
        public const byte Havale = 3;
        public const byte Cek = 4;
        public const byte AcenteKrediKarti = 5;
        public const byte AcentePosHesabi = 6;
        public const byte Senet = 7;
        public const byte AcenteBireyselKrediKarti = 9;
      //public const byte BlokeliKrediKarti = 6;
      //public const byte BankaHesabi = 7;
    }


}
