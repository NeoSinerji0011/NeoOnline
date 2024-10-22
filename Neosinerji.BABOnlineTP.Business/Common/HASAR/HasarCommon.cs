using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
  public class HasarCommon
    {
      public static List<SelectListItem> HasarDurumlari()
      {
          List<SelectListItem> list = new List<SelectListItem>();

          list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = "Açık" },
                new SelectListItem() { Value = "2", Text = "Kapalı" },
                new SelectListItem() { Value = "3", Text = "Red" },
            });

          return list;
      }
      public static List<SelectListItem> HasarParaBirimleri()
      {
          List<SelectListItem> list = new List<SelectListItem>();

          list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz..." },
                new SelectListItem() { Value = "1", Text = "TL" },
                new SelectListItem() { Value = "2", Text = "Dolar" },
                new SelectListItem() { Value = "3", Text = "Euro" },
            });

          return list;
      }

      public static string ParaBirimiText(short paraBirimiKodu)
      {
          string ParaBirimi="";
          switch (paraBirimiKodu)
          {
              case 1: ParaBirimi = "TL"; break;
              case 2: ParaBirimi = "Dolar"; break;
              case 3: ParaBirimi = "Euro"; break;

              default: ParaBirimi = ""; break;
                
          }
          return ParaBirimi;
      }
    }
}
