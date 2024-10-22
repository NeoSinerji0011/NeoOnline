using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public static class FerdiKazaPlus
    {
        public static List<SelectListItem> LehtarTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = "Kanuni Varisler" },
                new SelectListItem() { Value = "0", Text = "Diğer" }
            });
            return list;
        }

        public static List<SelectListItem> FerdiKazaPlusTeminatTutarlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
            new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                    
                new SelectListItem(){ Value="1", Text="75.000"},
                new SelectListItem(){ Value="2", Text="100.000"},
                new SelectListItem(){ Value="3", Text="150.000"},
              
            });

            return list;
        }

        public static List<SelectListItem> FerdiKazaPlusOdemeSecenekleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
            new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                    
                new SelectListItem(){ Value="2", Text="Taksitli"},
                new SelectListItem(){ Value="1", Text="Pesin"},
              
            });

            return list;
        }

        public static List<SelectListItem> CinsiyetTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "E", Text = babonline.Man},
                new SelectListItem() { Value = "K", Text = babonline.Women }
            });

            return list;
        }

        public static List<SelectListItem> MeslekTipleriMetlife()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Fikren ya da elle çalışanlar hiç bir mesleki faliyette bulunmayanlar"},
                new SelectListItem(){ Value="2", Text="Elle ve aynı zamanda bedenen çalışanlar"},
                new SelectListItem(){ Value="3", Text="Devamlı olarka makine ile bedenen çalışanlar"},
                
            });

            return list;
        }

        public static List<SelectListItem> UrunTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Ferdi Kaza Plus"},
                new SelectListItem(){ Value="2", Text="Yıllık Hayat Sigortası"},
               
                
            });

            return list;
        }

        public static List<SelectListItem> SigortaSureleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="1"},
                
            });

            return list;
        }

        public static List<SelectListItem> AdresTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
            new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                    
                new SelectListItem(){ Value="8", Text="Ev"},
                new SelectListItem(){ Value="9", Text="iş"},
              
            });

            return list;
        }

        public static List<SelectListItem> HareketTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
            new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                    
                new SelectListItem(){ Value="0", Text="Başlangıç"},
                new SelectListItem(){ Value="1", Text="İleri"},
                new SelectListItem(){ Value="2", Text="Beklet"},
                new SelectListItem(){ Value="3", Text="İade"},
                new SelectListItem(){ Value="4", Text="Sonlandır"},
            });

            return list;
        }

        public static List<SelectListItem> IsTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
            new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                    
                new SelectListItem(){ Value="1", Text="İş Oluşturuldu"},
                new SelectListItem(){ Value="5", Text="Süreç Sonu"},
              
            });

            return list;
        }
    }


}