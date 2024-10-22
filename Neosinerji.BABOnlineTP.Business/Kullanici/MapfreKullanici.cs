using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;


namespace Neosinerji.BABOnlineTP.Business
{
    [Serializable]
    public class MapfreKullanicilar
    {
        public MapfreKullanicilar()
        {

        }
        public string Bolge { get; set; }
        public string TVMUnvan{ get; set; }
        public string AnaPartaj { get; set; }
        public string TaliPartaj { get; set; }
        public string KullaniciAdi { get; set; }
        public string Email { get; set; }
        
    }

    public class MapfreKullaniciArama : DataTableParameters<MapfreKullaniciListeModel>
    {
        public MapfreKullaniciArama(HttpRequestBase httpRequest, Expression<Func<MapfreKullaniciListeModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public MapfreKullaniciArama(HttpRequestBase httpRequest,
                                   Expression<Func<MapfreKullaniciListeModel, object>>[] selectColumns,
                                   Expression<Func<MapfreKullaniciListeModel, object>> rowIdColumn,
                                   Expression<Func<MapfreKullaniciListeModel, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        
        public string KullaniciAdi { get; set; }
        public string Partaj { get; set; }
    }

    public class MapfreKullaniciListeModel
    {
        public int KullaniciId { get; set; }
        public string Bolge { get; set; }
        public string TVMUnvan { get; set; }
        public string AnaPartaj { get; set; }
        public string TaliPartaj { get; set; }
        public string KullaniciAdi { get; set; }
        public string Email { get; set; }
        public bool Olusturuldu { get; set; }
        public string OlusturulduText { get; set; }
    }
}
