using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class WebServiceLogListService : IWebServiceLogListService
    {
        ITVMContext _TVMContext;
        ITUMContext _TUMContext;
        ITeklifContext _TeklifContext;
        IParametreContext _UnitOfWork;
       
        public WebServiceLogListService(ITVMContext tvmContext, ITUMContext tumContext, ITeklifContext teklifContext, IParametreContext unitOfWork)
        {
            _TVMContext = tvmContext;
            _TUMContext = tumContext;
            _TeklifContext = teklifContext;
            _UnitOfWork = unitOfWork;
            
        }

        #region WebServiceLogService
        public List<WebServiceLogModelOzel> PagedList(WebServiceLogListe wbsrvclgListe)
        {
            byte istekTip = wbsrvclgListe.IstekTipi.Value;
            //int TeklifNo = wbsrvclgListe.TeklifId.Value;

            if (wbsrvclgListe.BitisTarihi.HasValue)
                wbsrvclgListe.BitisTarihi = wbsrvclgListe.BitisTarihi.Value.AddDays(1);

            List<WebServiceLogModelOzel> ServiceList = new List<WebServiceLogModelOzel>();

            if (istekTip == WebServisIstekTipleri.Teklif || 
                istekTip == WebServisIstekTipleri.Police || 
                istekTip == WebServisIstekTipleri.Muhasebe)
            {
                IQueryable<TeklifGenel> teklifquery = _TeklifContext.TeklifGenelRepository.Filter(m => m.TanzimTarihi > wbsrvclgListe.BaslangisTarihi &
                                                                                                       m.TanzimTarihi < wbsrvclgListe.BitisTarihi);

                if (wbsrvclgListe.TvmKodu.HasValue)
                {
                    teklifquery = teklifquery.Where(s => s.TVMKodu == wbsrvclgListe.TvmKodu);
                }

                if (wbsrvclgListe.TeklifId.HasValue)
                    teklifquery = teklifquery.Where(s => s.TeklifNo == wbsrvclgListe.TeklifId);

                IQueryable<WEBServisLog> servisquery = _TVMContext.WEBServisLogRepository.All();
                IQueryable<TUMDetay> tumquery = _TUMContext.TUMDetayRepository.All();          
            
                var resultval = servisquery
                                .Join(teklifquery, s => s.TeklifId, t => t.TeklifId, (s, t) => new { s, t })
                                .Join(tumquery, x => x.t.TUMKodu, tu => tu.Kodu, (x, tu) => new { x, tu })
                                    .Where(y => y.x.s.IstekTipi == istekTip)
                                .Select(m => new
                                {
                                    m.x.s.LogId,
                                    m.x.s.TeklifId,
                                    m.x.s.IstekTipi,
                                    m.x.s.IstekTarihi,
                                    m.x.s.CevapTarihi,
                                    m.x.s.BasariliBasarisiz,
                                    m.x.s.IstekUrl,
                                    m.x.s.CevapUrl,
                                    m.x.t.UrunKodu,
                                    m.tu.Unvani,
                                });

                foreach (var r in resultval)
                {
                    WebServiceLogModelOzel model = new WebServiceLogModelOzel();
                    model.LogId = r.LogId;
                    model.TeklifId = r.TeklifId;
                    model.IstekTipi = r.IstekTipi;
                    model.IstekTarihi = r.IstekTarihi;
                    model.CevapTarihi = r.CevapTarihi;
                    model.BasariliBasarisiz = r.BasariliBasarisiz;
                    model.IstekUrl = r.IstekUrl;
                    model.CevapUrl = r.CevapUrl;
                    model.Sure = Convert.ToInt32(r.CevapTarihi.Subtract(r.IstekTarihi).TotalSeconds);
                    model.UrunAdi = UrunKodlari.GetUrunAdi(r.UrunKodu);
                    model.Unvani = r.Unvani;


                    ServiceList.Add(model);
                }
            }
            else
            {
                List<WEBServisLog> servisquery = _TVMContext.WEBServisLogRepository.All()
                                                                  .Where(w => w.IstekTarihi >= wbsrvclgListe.BaslangisTarihi && 
                                                                              w.IstekTarihi <= wbsrvclgListe.BitisTarihi &&
                                                                              w.IstekTipi == istekTip)
                                                                  .ToList<WEBServisLog>();

                foreach (var r in servisquery)
                {
                    WebServiceLogModelOzel model = new WebServiceLogModelOzel();
                    model.LogId = r.LogId;
                    model.TeklifId = 0;
                    model.IstekTipi = r.IstekTipi;
                    model.IstekTarihi = r.IstekTarihi;
                    model.CevapTarihi = r.CevapTarihi;
                    model.BasariliBasarisiz = r.BasariliBasarisiz;
                    model.IstekUrl = r.IstekUrl;
                    model.CevapUrl = r.CevapUrl;
                    model.Sure = Convert.ToInt32(r.CevapTarihi.Subtract(r.IstekTarihi).TotalSeconds);
                    model.UrunAdi = String.Empty;
                    model.Unvani = String.Empty;

                
                ServiceList.Add(model);
            }

            }

            return ServiceList;
        }

       
        #endregion

    }
}
