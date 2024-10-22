using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TVMListe : DataTableParameters<TVMDetay>
    {
        public TVMListe(HttpRequestBase httpRequest, Expression<Func<TVMDetay, object>>[] selectColumns)
            :base(httpRequest, selectColumns)
        {

        }

        public TVMListe(HttpRequestBase httpRequest, 
                                   Expression<Func<TVMDetay, object>>[] selectColumns,
                                   Expression<Func<TVMDetay, object>> rowIdColumn,
                                   Expression<Func<TVMDetay, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            :base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> Kodu { get; set; }
        public string Unvani { get; set; }
        public Nullable<short> Tipi { get; set; }
        public Nullable<int> BagliOlduguTVMKodu { get; set; }
    }
}
