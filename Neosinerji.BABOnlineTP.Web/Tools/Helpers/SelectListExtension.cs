using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public static class SelectListExtension
    {
        public static List<SelectListItem> ListWithOptionLabel(this SelectList selectList) 
        {
            List<SelectListItem> list = selectList.ToList();
            list.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });

            return list;
        }


        public static List<SelectListItem> ListWithOptionLabelIller(this SelectList selectList)
        {
            List<SelectListItem> list = selectList.ToList();

            List<SelectListItem> newList = new List<SelectListItem>();

            newList.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });


            SelectListItem istanbul = list.Where(s => s.Value == "34").FirstOrDefault();
            SelectListItem izmir = list.Where(s => s.Value == "35").FirstOrDefault();
            SelectListItem ankara = list.Where(s => s.Value == "06" || s.Value == "6").FirstOrDefault();

            if (istanbul != null)
            {
                list.Remove(istanbul);
                newList.Insert(1, istanbul);
            }
            if (izmir != null)
            {
                list.Remove(izmir);
                newList.Insert(2, izmir);
            }

            if (ankara != null)
            {
                list.Remove(ankara);
                newList.Insert(3, ankara);
            }

            newList.AddRange(list);

            return newList;
        }

        public static List<SelectListItem> ListWithOptionLabel(this SelectList selectList, bool selected)
        {
            List<SelectListItem> list = selectList.ToList();
            list.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = selected });

            return list;
        }

        public static List<SelectListItem> ListWithOptionLabelIller(this SelectList selectList, bool selected)
        {
            List<SelectListItem> list = selectList.ToList();

            List<SelectListItem> newList = new List<SelectListItem>();

            newList.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = selected });


            SelectListItem istanbul = list.Where(s => s.Value == "34").FirstOrDefault();
            SelectListItem izmir = list.Where(s => s.Value == "35").FirstOrDefault();
            SelectListItem ankara = list.Where(s => s.Value == "06").FirstOrDefault();

            if (istanbul != null)
            {
                list.Remove(istanbul);
                newList.Insert(1, istanbul);
            }
            if (izmir != null)
            {
                list.Remove(izmir);
                newList.Insert(2, izmir);
            }

            if (ankara != null)
            {
                list.Remove(ankara);
                newList.Insert(3, ankara);
            }

            newList.AddRange(list);

            return newList;
        }

        public static List<SelectListItem> ListWithOptionLabel(this SelectList selectList, string optionLabel)
        {
            List<SelectListItem> list = selectList.ToList();
            list.Insert(0, new SelectListItem() { Value = "", Text = optionLabel, Selected = true });

            return list;
        }
    }
}