using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class WebServiceLogListe : DataTableParameters<WebServiceLogModelOzel>
    {
        public WebServiceLogListe(HttpRequestBase httpRequest, Expression<Func<WebServiceLogModelOzel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public WebServiceLogListe(HttpRequestBase httpRequest,
                                   Expression<Func<WebServiceLogModelOzel, object>>[] selectColumns,
                                   Expression<Func<WebServiceLogModelOzel, object>> rowIdColumn,
                                   Expression<Func<WebServiceLogModelOzel, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> TeklifId { get; set; }
        public Nullable<byte> IstekTipi { get; set; }
        public Nullable<DateTime> BaslangisTarihi { get; set; }
        public Nullable<DateTime> BitisTarihi { get; set; }
        public Nullable<int> TvmHQKodu { get; set; }
        public Nullable<int> TvmKodu { get; set; }
    }

    public class WebServiceLogModelOzel : WEBServisLog
    {
        public string Unvani { get; set; }
        public string UrunAdi { get; set; }
        public int? Sure { get; set; }
        
    }
}
