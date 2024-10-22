using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class AracListe : DataTableParameters<X_AracDegerListesi>
    {
        public AracListe(HttpRequestBase httpRequest, Expression<Func<X_AracDegerListesi, object>>[] selectColumns)
            :base(httpRequest, selectColumns)
        {

        }

        public AracListe(HttpRequestBase httpRequest,
                                   Expression<Func<X_AracDegerListesi, object>>[] selectColumns,
                                   Expression<Func<X_AracDegerListesi, object>> rowIdColumn,
                                   Expression<Func<X_AracDegerListesi, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            :base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> MarkaKodu { get; set; }
        public Nullable<int> TipKodu { get; set; }
        public Nullable<int> Model { get; set; }

    }
}
