﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class DataTableList
    {
        public DataTableList()
        {
        }

        public int sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<List<string>> aaData { get; set; }
        public string sColumns { get; set; }

        public void Import(string[] properties)
        {
            sColumns = string.Empty;
            for (int i = 0; i < properties.Length; i++)
            {
                sColumns += properties[i];
                if (i < properties.Length - 1)
                    sColumns += ",";
            }
        }
    }
}