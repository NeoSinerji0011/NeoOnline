using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Web.Mvc;
using System.Data;
using Neosinerji.BABOnlineTP.Database;
using Microsoft.Practices.Unity;
using AutoMapper;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class IslerimListe : DataTableParameters<IslerimProcedureModel>
    {
        public IslerimListe(HttpRequestBase httpRequest, Expression<Func<IslerimProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public IslerimListe(HttpRequestBase httpRequest,
                                  Expression<Func<IslerimProcedureModel, object>>[] selectColumns,
                                  Expression<Func<IslerimProcedureModel, object>> rowIdColumn,
                                  Expression<Func<IslerimProcedureModel, object>> linkColumn1,
                                  string linkColumn1Url,
                                  string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public int TvmKodu { get; set; }
        public int KullaniciKodu { get; set; }
        public string DurumListe { get; set; }

    }
}
