using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class NeoConnectSifreTanimlamaModel : DataTableParameters<OtoLoginSigortaSirketKullanicilar>
    {
        public NeoConnectSifreTanimlamaModel(HttpRequestBase httpRequest, Expression<Func<OtoLoginSigortaSirketKullanicilar, object>>[] selectColumns)
            :base(httpRequest, selectColumns)
        {

        }

        public NeoConnectSifreTanimlamaModel(HttpRequestBase httpRequest,
                                   Expression<Func<OtoLoginSigortaSirketKullanicilar, object>>[] selectColumns,
                                   Expression<Func<OtoLoginSigortaSirketKullanicilar, object>> rowIdColumn,
                                   Expression<Func<OtoLoginSigortaSirketKullanicilar, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            :base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }
        public Nullable<int> TVMKodu { get; set; }
        public string Unvani { get; set; }
        public string SigortaSirketAdi { get; set; }
        public Nullable<int> GrupKodu { get; set; }
        public string KullaniciAdi { get; set; }
        public string AcenteKodu { get; set; }
        public string Sifre { get; set; }
        public string ProxyIpPort { get; set; }
    }
}
