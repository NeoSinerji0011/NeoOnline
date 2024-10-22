using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TUMListe : DataTableParameters<TUMDetay>
    {
        public TUMListe(HttpRequestBase httpRequest, Expression<Func<TUMDetay, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public TUMListe(HttpRequestBase httpRequest, 
                                   Expression<Func<TUMDetay, object>>[] selectColumns,
                                   Expression<Func<TUMDetay, object>> rowIdColumn,
                                   Expression<Func<TUMDetay, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            :base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
    {
    
    }
        public Nullable<int> Kodu { get; set; }
        public string Unvani { get; set; }
        public string BirlikKodu { get; set; }
    }
}